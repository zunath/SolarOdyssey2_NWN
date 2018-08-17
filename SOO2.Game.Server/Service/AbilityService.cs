using System;
using System.Linq;
using Freescape.Game.Server.Bioware.Contracts;
using Freescape.Game.Server.Data.Contracts;
using Freescape.Game.Server.Data.Entities;
using Freescape.Game.Server.Enumeration;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.NWNX.Contracts;
using Freescape.Game.Server.Perk;
using Freescape.Game.Server.Service.Contracts;
using NWN;
using static NWN.NWScript;
using PerkExecutionType = Freescape.Game.Server.Enumeration.PerkExecutionType;

namespace Freescape.Game.Server.Service
{
    public class AbilityService : IAbilityService
    {
        private readonly INWScript _;
        private readonly IDataContext _db;
        private readonly IPerkService _perk;
        private readonly IPVPSanctuaryService _pvpSanctuary;
        private readonly ITimeService _time;
        private readonly IBiowarePosition _biowarePosition;
        private readonly INWNXPlayer _nwnxPlayer;
        private readonly IColorTokenService _color;
        private readonly IRandomService _random;
        private readonly IFoodService _food;

        public AbilityService(INWScript script, 
            IDataContext db,
            IPerkService perk,
            IPVPSanctuaryService pvpSanctuary,
            ITimeService time,
            IBiowarePosition biowarePosition,
            INWNXPlayer nwnxPlayer,
            IColorTokenService color,
            IRandomService random,
            IFoodService food)
        {
            _ = script;
            _db = db;
            _perk = perk;
            _pvpSanctuary = pvpSanctuary;
            _time = time;
            _biowarePosition = biowarePosition;
            _nwnxPlayer = nwnxPlayer;
            _color = color;
            _random = random;
            _food = food;
        }

        private const int SPELL_STATUS_STARTED = 1;
        private const int SPELL_STATUS_INTERRUPTED = 2;


        public void OnModuleItemActivated()
        {
            NWPlayer pc = NWPlayer.Wrap(_.GetItemActivator());
            NWItem item = NWItem.Wrap(_.GetItemActivated());
            NWPlayer target = NWPlayer.Wrap(_.GetItemActivatedTarget());
            int perkID = item.GetLocalInt("ACTIVATION_PERK_ID");
            if (perkID <= 0) return;

            Data.Entities.Perk perk = _db.Perks.SingleOrDefault(x => x.PerkID == perkID);

            if (perk == null) return;

            IPerk perkAction = App.ResolveByInterface<IPerk>("Perk." + perk.JavaScriptName);
            if (perkAction == null) return;

            PlayerCharacter playerEntity = _db.PlayerCharacters.Single(x => x.PlayerID == pc.GlobalID);

            if (_perk.GetPCPerkLevel(pc, perk.PerkID) <= 0)
            {
                pc.SendMessage("You do not meet the prerequisites to use this ability.");
                return;
            }

            if (perkAction.IsHostile() && target.IsPlayer)
            {
                if (!_pvpSanctuary.IsPVPAttackAllowed(pc, target)) return;
            }

            if (pc.Area.Resref != target.Area.Resref ||
                    _.LineOfSightObject(pc.Object, target.Object) == 0)
            {
                pc.SendMessage("You cannot see your target.");
                return;
            }

            if (!perkAction.CanCastSpell(pc, target))
            {
                pc.SendMessage(perkAction.CannotCastSpellMessage() ?? "That ability cannot be used at this time.");
                return;
            }

            int manaCost = perkAction.ManaCost(pc, perkAction.ManaCost(pc, perk.BaseManaCost));
            if (playerEntity.CurrentMana < manaCost)
            {
                pc.SendMessage("You do not have enough mana. (Required: " + manaCost + ". You have: " + playerEntity.CurrentMana + ")");
                return;
            }

            if (pc.IsBusy || pc.CurrentHP <= 0)
            {
                pc.SendMessage("You are too busy to activate that ability.");
                return;
            }

            // Check cooldown
            PCCooldown pcCooldown = _db.PCCooldowns.SingleOrDefault(x => x.PlayerID == pc.GlobalID && x.CooldownCategoryID == perk.CooldownCategoryID);
            if (pcCooldown == null)
            {
                pcCooldown = new PCCooldown
                {
                    CooldownCategoryID = Convert.ToInt32(perk.CooldownCategoryID),
                    DateUnlocked = DateTime.UtcNow.AddSeconds(-1),
                    PlayerID = pc.GlobalID
                };

                _db.PCCooldowns.Add(pcCooldown);
                _db.SaveChanges();
            }

            DateTime unlockDateTime = pcCooldown.DateUnlocked;
            DateTime now = DateTime.UtcNow;

            if (unlockDateTime > now)
            {
                string timeToWait = _time.GetTimeToWaitLongIntervals(now, unlockDateTime, false);
                pc.SendMessage("That ability can be used again in " + timeToWait + ".");
                return;
            }

            // Spells w/ casting time
            if (perk.PerkExecutionType.PerkExecutionTypeID == (int)PerkExecutionType.Spell)
            {
                CastSpell(pc, target, perk, perkAction, perk.CooldownCategory);
            }
            // Combat Abilities w/o casting time
            else if (perk.PerkExecutionType.PerkExecutionTypeID == (int)PerkExecutionType.CombatAbility)
            {
                perkAction.OnImpact(pc, target);

                if (manaCost > 0)
                {
                    playerEntity.CurrentMana = playerEntity.CurrentMana - manaCost;
                    _db.SaveChanges();
                }
                ApplyCooldown(pc, perk.CooldownCategory, perkAction);
            }
            // Queued Weapon Skills
            else if (perk.PerkExecutionType.PerkExecutionTypeID == (int)PerkExecutionType.QueuedWeaponSkill)
            {
                HandleQueueWeaponSkill(pc, perk, perkAction);
            }
        }


        private void CastSpell(NWPlayer pc,
                                      NWObject target,
                                      Data.Entities.Perk entity,
                                      IPerk perk,
                                      CooldownCategory cooldown)
        {
            string spellUUID = Guid.NewGuid().ToString();
            int itemBonus = pc.CastingSpeed;
            float baseCastingTime = perk.CastingTime(pc, (float)entity.BaseCastingTime);
            float castingTime = baseCastingTime;

            // Casting Bonus % - Shorten casting time.
            if (itemBonus < 0)
            {
                float castingPercentageBonus = Math.Abs(itemBonus) * 0.01f;
                castingTime = castingTime - (castingTime * castingPercentageBonus);
            }
            // Casting Penalty % - Increase casting time.
            else if (itemBonus > 0)
            {
                float castingPercentageBonus = Math.Abs(itemBonus) * 0.01f;
                castingTime = castingTime + (castingTime * castingPercentageBonus);
            }

            if (castingTime < 0.5f)
                castingTime = 0.5f;

            // Heavy armor increases casting time by 2x the base casting time
            if (pc.Chest.CustomItemType == CustomItemType.HeavyArmor)
            {
                castingTime = baseCastingTime * 2;
            }

            if (_.GetActionMode(pc.Object, ACTION_MODE_STEALTH) == 1)
                _.SetActionMode(pc.Object, ACTION_MODE_STEALTH, 0);

            _.ClearAllActions();
            _biowarePosition.TurnToFaceObject(target, pc);
            _.ApplyEffectToObject(DURATION_TYPE_TEMPORARY,
                    _.EffectVisualEffect(VFX_DUR_ELEMENTAL_SHIELD),
                    pc.Object,
                    castingTime + 0.2f);

            float animationTime = castingTime;
            pc.AssignCommand(() => _.ActionPlayAnimation(ANIMATION_LOOPING_CONJURE1, 1.0f, animationTime - 0.1f));

            pc.IsBusy = true;
            CheckForSpellInterruption(pc, spellUUID, pc.Position);
            pc.SetLocalInt(spellUUID, SPELL_STATUS_STARTED);

            _nwnxPlayer.StartGuiTimingBar(pc, (int)castingTime, "");

            pc.DelayCommand(() =>
            {
                if (pc.GetLocalInt(spellUUID) == SPELL_STATUS_INTERRUPTED || // Moved during casting
                    pc.CurrentHP < 0 || pc.IsDead) // Or is dead/dying
                {
                    pc.DeleteLocalInt(spellUUID);
                    pc.SendMessage("Your spell has been interrupted.");
                    return;
                }

                pc.DeleteLocalInt(spellUUID);

                if ((PerkExecutionType)entity.ExecutionTypeID == PerkExecutionType.Spell ||
                    (PerkExecutionType)entity.ExecutionTypeID == PerkExecutionType.CombatAbility)
                {
                    perk.OnImpact(pc, target);
                }
                else
                {
                    HandleQueueWeaponSkill(pc, entity, perk);
                }


                // Adjust mana only if spell cost > 0
                PlayerCharacter pcEntity = _db.PlayerCharacters.Single(x => x.PlayerID == pc.GlobalID);
                if (perk.ManaCost(pc, entity.BaseManaCost) > 0)
                {
                    pcEntity.CurrentMana = pcEntity.CurrentMana - perk.ManaCost(pc, entity.BaseManaCost);
                    _db.SaveChanges();
                    pc.SendMessage(_color.Custom("Mana: " + pcEntity.CurrentMana + " / " + pcEntity.MaxMana, 32, 223, 219));

                }

                if (_random.Random(100) + 1 <= 3)
                {
                    _food.DecreaseHungerLevel(pc, 1);
                }
                // Mark cooldown on category
                ApplyCooldown(pc, cooldown, perk);
                pc.IsBusy = false;

            }, castingTime + 0.5f);
        }

        private void ApplyCooldown(NWPlayer pc, CooldownCategory cooldown, IPerk ability)
        {
            float finalCooldown = ability.CooldownTime(pc, (float)cooldown.BaseCooldownTime);
            int cooldownSeconds = (int)finalCooldown;
            int cooldownMillis = (int)((finalCooldown - cooldownSeconds) * 100);

            PCCooldown pcCooldown = _db.PCCooldowns.Single(x => x.PlayerID == pc.GlobalID && x.CooldownCategoryID == cooldown.CooldownCategoryID);
            pcCooldown.DateUnlocked = DateTime.UtcNow.AddSeconds(cooldownSeconds).AddMilliseconds(cooldownMillis);
            _db.SaveChanges();
        }

        private void CheckForSpellInterruption(NWPlayer pc, string spellUUID, Vector position)
        {
            Vector currentPosition = pc.Position;

            if (currentPosition.m_X != position.m_X ||
                currentPosition.m_Y != position.m_Y ||
                currentPosition.m_Z != position.m_Z)
            {
                _nwnxPlayer.StopGuiTimingBar(pc, "", -1);
                pc.IsBusy = false;
                pc.SetLocalInt(spellUUID, SPELL_STATUS_INTERRUPTED);
                return;
            }
            
            pc.DelayCommand(() => CheckForSpellInterruption(pc, spellUUID, position), 1.0f);
        }

        private void HandleQueueWeaponSkill(NWPlayer pc, Data.Entities.Perk entity, IPerk ability)
        {
            string queueUUID = Guid.NewGuid().ToString();
            pc.SetLocalInt("ACTIVE_WEAPON_SKILL", entity.PerkID);
            pc.SetLocalString("ACTIVE_WEAPON_SKILL_UUID", queueUUID);
            pc.SendMessage("Weapon skill '" + entity.Name + "' queued for next attack.");

            ApplyCooldown(pc, entity.CooldownCategory, ability);

            // Player must attack within 30 seconds after queueing or else it wears off.
            pc.DelayCommand(() =>
            {
                if (pc.GetLocalString("ACTIVE_WEAPON_SKILL_UUID") == queueUUID)
                {
                    pc.DeleteLocalInt("ACTIVE_WEAPON_SKILL");
                    pc.DeleteLocalString("ACTIVE_WEAPON_SKILL_UUID");
                    pc.SendMessage("Your weapon skill '" + entity.Name + "' is no longer queued.");
                }
            }, 30.0f);
        }

        public PlayerCharacter RestoreMana(NWPlayer oPC, int amount, PlayerCharacter entity)
        {
            entity.CurrentMana = entity.CurrentMana + amount;
            if (entity.CurrentMana > entity.MaxMana)
                entity.CurrentMana = entity.MaxMana;

            oPC.SendMessage(_color.Custom("Mana: " + entity.CurrentMana + " / " + entity.MaxMana, 32, 223, 219));

            return entity;
        }

        public void RestoreMana(NWPlayer oPC, int amount)
        {
            PlayerCharacter entity = _db.PlayerCharacters.Single(x => x.PlayerID == oPC.GlobalID);
            RestoreMana(oPC, amount, entity);
            _db.SaveChanges();
        }

        public void OnHitCastSpell(NWPlayer oPC)
        {
            NWObject oTarget = NWObject.Wrap(_.GetSpellTargetObject());
            int activeWeaponSkillID = oPC.GetLocalInt("ACTIVE_WEAPON_SKILL");
            if (activeWeaponSkillID <= 0) return;

            Data.Entities.Perk entity = _db.Perks.Single(x => x.PerkID == activeWeaponSkillID);

            IPerk perk = App.ResolveByInterface<IPerk>("Perk." + entity.JavaScriptName);

            perk?.OnImpact(oPC, oTarget);

            oPC.DeleteLocalString("ACTIVE_WEAPON_SKILL_UUID");
            oPC.DeleteLocalInt("ACTIVE_WEAPON_SKILL");
        }

    }
}
