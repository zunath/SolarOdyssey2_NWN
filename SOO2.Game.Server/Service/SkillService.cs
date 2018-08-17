using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Freescape.Game.Server.Bioware.Contracts;
using Freescape.Game.Server.Data.Contracts;
using Freescape.Game.Server.Data.Entities;
using Freescape.Game.Server.Data.SqlResults;
using Freescape.Game.Server.Enumeration;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.NWNX.Contracts;
using Freescape.Game.Server.Service.Contracts;
using Freescape.Game.Server.ValueObject.Skill;
using NWN;
using static NWN.NWScript;

namespace Freescape.Game.Server.Service
{
    public class SkillService : ISkillService
    {
        private const float PrimaryIncrease = 0.2f;
        private const float SecondaryIncrease = 0.1f;
        private const float TertiaryIncrease = 0.05f;
        private const int MaxAttributeBonus = 70;
        private const string IPPenaltyTag = "SKILL_PENALTY_ITEM_PROPERTY";


        private readonly IDataContext _db;
        private readonly INWScript _;
        private readonly IRandomService _random;
        private readonly IFoodService _food;
        private readonly INWNXCreature _nwnxCreature;
        private readonly IPerkService _perk;
        private readonly IBiowareXP2 _biowareXP2;
        private readonly ICustomEffectService _customEffect;
        private readonly AppState _state;

        public SkillService(IDataContext db,
            INWScript script,
            IRandomService random,
            IFoodService food,
            INWNXCreature nwnxCreature,
            IPerkService perk,
            IBiowareXP2 biowareXP2,
            ICustomEffectService customEffect,
            AppState state)
        {
            _db = db;
            _ = script;
            _random = random;
            _food = food;
            _nwnxCreature = nwnxCreature;
            _perk = perk;
            _biowareXP2 = biowareXP2;
            _customEffect = customEffect;
            _state = state;
        }

        public int SkillCap => 500;

        public void ApplyStatChanges(NWPlayer player, NWItem ignoreItem)
        {
            if (!player.IsPlayer) return;
            if (!player.IsInitializedAsPlayer) return;

            PlayerCharacter pcEntity = _db.PlayerCharacters.Single(x => x.PlayerID == player.GlobalID);
            List<PCSkill> skills = _db.PCSkills.Where(x => x.PlayerID == player.GlobalID && x.Skill.IsActive).ToList();
            float strBonus = 0.0f;
            float dexBonus = 0.0f;
            float conBonus = 0.0f;
            float intBonus = 0.0f;
            float wisBonus = 0.0f;
            float chaBonus = 0.0f;

            foreach (PCSkill pcSkill in skills)
            {
                Skill skill = pcSkill.Skill;
                CustomAttribute primary = (CustomAttribute)skill.Primary;
                CustomAttribute secondary = (CustomAttribute)skill.Secondary;
                CustomAttribute tertiary = (CustomAttribute)skill.Tertiary;

                // Primary Bonuses
                if (primary == CustomAttribute.STR) strBonus += PrimaryIncrease * pcSkill.Rank;
                else if (primary == CustomAttribute.DEX) dexBonus += PrimaryIncrease * pcSkill.Rank;
                else if (primary == CustomAttribute.CON) conBonus += PrimaryIncrease * pcSkill.Rank;
                else if (primary == CustomAttribute.INT) intBonus += PrimaryIncrease * pcSkill.Rank;
                else if (primary == CustomAttribute.WIS) wisBonus += PrimaryIncrease * pcSkill.Rank;
                else if (primary == CustomAttribute.CHA) chaBonus += PrimaryIncrease * pcSkill.Rank;

                // Secondary Bonuses
                if (secondary == CustomAttribute.STR) strBonus += SecondaryIncrease * pcSkill.Rank;
                else if (secondary == CustomAttribute.DEX) dexBonus += SecondaryIncrease * pcSkill.Rank;
                else if (secondary == CustomAttribute.CON) conBonus += SecondaryIncrease * pcSkill.Rank;
                else if (secondary == CustomAttribute.INT) intBonus += SecondaryIncrease * pcSkill.Rank;
                else if (secondary == CustomAttribute.WIS) wisBonus += SecondaryIncrease * pcSkill.Rank;
                else if (secondary == CustomAttribute.CHA) chaBonus += SecondaryIncrease * pcSkill.Rank;

                // Tertiary Bonuses
                if (tertiary == CustomAttribute.STR) strBonus += TertiaryIncrease * pcSkill.Rank;
                else if (tertiary == CustomAttribute.DEX) dexBonus += TertiaryIncrease * pcSkill.Rank;
                else if (tertiary == CustomAttribute.CON) conBonus += TertiaryIncrease * pcSkill.Rank;
                else if (tertiary == CustomAttribute.INT) intBonus += TertiaryIncrease * pcSkill.Rank;
                else if (tertiary == CustomAttribute.WIS) wisBonus += TertiaryIncrease * pcSkill.Rank;
                else if (tertiary == CustomAttribute.CHA) chaBonus += TertiaryIncrease * pcSkill.Rank;
            }

            // Check caps.
            if (strBonus > MaxAttributeBonus) strBonus = MaxAttributeBonus;
            if (dexBonus > MaxAttributeBonus) dexBonus = MaxAttributeBonus;
            if (conBonus > MaxAttributeBonus) conBonus = MaxAttributeBonus;
            if (intBonus > MaxAttributeBonus) intBonus = MaxAttributeBonus;
            if (wisBonus > MaxAttributeBonus) wisBonus = MaxAttributeBonus;
            if (chaBonus > MaxAttributeBonus) chaBonus = MaxAttributeBonus;

            if (pcEntity.BackgroundID == (int)BackgroundType.Archer || pcEntity.BackgroundID == (int)BackgroundType.Crossbowman)
            {
                dexBonus++;
                wisBonus++;
            }
            if (pcEntity.BackgroundID == (int)BackgroundType.Guard || pcEntity.BackgroundID == (int)BackgroundType.Berserker)
            {
                strBonus++;
                conBonus++;
            }

            if (pcEntity.BackgroundID == (int)BackgroundType.TwinBladeSpecialist)
            {
                dexBonus++;
                conBonus++;
            }

            if (pcEntity.BackgroundID == (int)BackgroundType.MartialArtist)
            {
                strBonus++;
                dexBonus++;
            }

            // Apply attributes
            _nwnxCreature.SetRawAbilityScore(player, ABILITY_STRENGTH, (int)strBonus + pcEntity.STRBase);
            _nwnxCreature.SetRawAbilityScore(player, ABILITY_DEXTERITY, (int)dexBonus + pcEntity.DEXBase);
            _nwnxCreature.SetRawAbilityScore(player, ABILITY_CONSTITUTION, (int)conBonus + pcEntity.CONBase);
            _nwnxCreature.SetRawAbilityScore(player, ABILITY_INTELLIGENCE, (int)intBonus + pcEntity.INTBase);
            _nwnxCreature.SetRawAbilityScore(player, ABILITY_WISDOM, (int)wisBonus + pcEntity.WISBase);
            _nwnxCreature.SetRawAbilityScore(player, ABILITY_CHARISMA, (int)chaBonus + pcEntity.CHABase);

            // Apply AC
            int ac = CalculateItemAC(player, ignoreItem) + _customEffect.CalculateEffectAC(player);
            _nwnxCreature.SetBaseAC(player, ac);

            // Apply BAB
            int bab = CalculateBAB(player, ignoreItem);
            _nwnxCreature.SetBaseAttackBonus(player, bab);


            int equippedItemHPBonus = 0;
            int equippedItemManaBonus = 0;

            for (int slot = 0; slot < NUM_INVENTORY_SLOTS; slot++)
            {
                NWItem item = NWItem.Wrap(_.GetItemInSlot(slot, player.Object));
                if (item.Equals(ignoreItem)) continue;
                
                equippedItemHPBonus += item.HPBonus;
                equippedItemManaBonus += item.ManaBonus;
            }

            // Apply HP
            int hp = 30 + player.ConstitutionModifier * 5;
            hp += _perk.GetPCPerkLevel(player, PerkType.Health) * 5;
            hp += equippedItemHPBonus;
            if (pcEntity.BackgroundID == (int)BackgroundType.Knight)
                hp += 10;

            if (hp > 255) hp = 255;
            if (hp < 20) hp = 20;
            _nwnxCreature.SetMaxHitPointsByLevel(player, 1, hp);
            if (player.CurrentHP > player.MaxHP)
            {
                int amount = player.CurrentHP - player.MaxHP;
                Effect damage = _.EffectDamage(amount);
                _.ApplyEffectToObject(DURATION_TYPE_INSTANT, damage, player.Object);
            }

            // Apply Mana
            int mana = 20;
            mana += (player.IntelligenceModifier + player.WisdomModifier + player.CharismaModifier) * 5;
            mana += _perk.GetPCPerkLevel(player, PerkType.Mana) * 5;
            mana += equippedItemManaBonus;
            if (pcEntity.BackgroundID == (int)BackgroundType.Wizard || pcEntity.BackgroundID == (int)BackgroundType.Cleric)
                mana += 10;

            if (mana < 0) mana = 0;
            pcEntity.MaxMana = mana;

            _db.SaveChanges();
        }

        public void RegisterPCToAllCombatTargetsForSkill(NWPlayer player, SkillType skillType)
        {
            int skillID = (int) skillType;
            if (!player.IsPlayer) return;
            if (skillID <= 0) return;

            List<NWPlayer> members = new List<NWPlayer>();
            NWPlayer member = NWPlayer.Wrap(_.GetFirstFactionMember(player.Object));
            while(member.IsValid)
            {
                members.Add(member);
                member = NWPlayer.Wrap(_.GetNextFactionMember(player.Object));
            }
            
            int nth = 1;
            NWCreature creature = NWCreature.Wrap(_.GetNearestCreature(CREATURE_TYPE_IS_ALIVE, 1, player.Object, nth, CREATURE_TYPE_PLAYER_CHAR, 0));
            while (creature.IsValid)
            {
                if (_.GetDistanceBetween(player.Object, creature.Object) > 20.0f) break;

                NWObject target = NWObject.Wrap(_.GetAttackTarget(creature.Object));
                if (target.IsValid && members.Contains(target))
                {
                    if (target.IsValid && target.Area.Equals(player.Area))
                    {
                        RegisterPCToNPCForSkill(player, creature, skillID);
                    }
                }

                nth++;
                creature = NWCreature.Wrap(_.GetNearestCreature(CREATURE_TYPE_IS_ALIVE, 1, player.Object, nth, CREATURE_TYPE_PLAYER_CHAR, 0));
            }
        }

        public void GiveSkillXP(NWPlayer oPC, SkillType skill, int xp)
        {
            GiveSkillXP(oPC, (int)skill, xp);
        }

        public void GiveSkillXP(NWPlayer oPC, int skillID, int xp)
        {
            if (skillID <= 0 || xp <= 0 || !oPC.IsPlayer) return;

            PlayerCharacter player = _db.PlayerCharacters.Single(x => x.PlayerID == oPC.GlobalID);
            PCSkill skill = GetPCSkillByID(oPC.GlobalID, skillID);
            SkillXPRequirement req = _db.SkillXPRequirements.Single(x => x.SkillID == skillID && x.Rank == skill.Rank);
            int maxRank = _db.SkillXPRequirements.Where(x => x.SkillID == skillID).Max(m => m.Rank);
            int originalRank = skill.Rank;
            xp = CalculateTotalSkillPointsPenalty(player.TotalSPAcquired, xp);

            // Run the skill decay rules.
            // If the method returns false, that means all skills are locked.
            // So we can't give the player any XP.
            if (!ApplySkillDecay(oPC, skill, xp))
            {
                return;
            }

            skill.XP = skill.XP + xp;
            oPC.SendMessage("You earned " + skill.Skill.Name + " skill experience. (" + xp + ")");

            // Skill is at cap and player would level up.
            // Reduce XP to required amount minus 1 XP
            if (skill.Rank >= maxRank && skill.XP > req.XP)
            {
                skill.XP = skill.XP - 1;
            }

            while (skill.XP >= req.XP)
            {
                skill.XP = skill.XP - req.XP;
                
                if (player.TotalSPAcquired < SkillCap)
                {
                    player.UnallocatedSP++;
                    player.TotalSPAcquired++;
                }
                
                skill.Rank++;
                oPC.FloatingText("Your " + skill.Skill.Name + " skill level increased!");
                req = _db.SkillXPRequirements.Single(x => x.SkillID == skillID && x.Rank == skill.Rank);

                // Reapply skill penalties on a skill level up.
                for (int slot = 0; slot < NUM_INVENTORY_SLOTS; slot++)
                {
                    NWItem item = NWItem.Wrap(_.GetItemInSlot(slot, oPC.Object));
                    RemoveWeaponPenalties(item);
                    ApplyWeaponPenalties(oPC, NWItem.Wrap(new NWN.Object()));
                }
            }
            
            _db.SaveChanges();
            
            // Update player and apply stat changes only if a level up occurred.
            if (originalRank != skill.Rank)
            {
                ApplyStatChanges(oPC, null);
            }


        }

        public PCSkill GetPCSkill(NWPlayer player, SkillType skill)
        {
            return GetPCSkill(player, (int) skill);
        }

        public PCSkill GetPCSkill(NWPlayer player, int skillID)
        {
            if (!player.IsPlayer) return null;
            return _db.PCSkills.Single(x => x.PlayerID == player.GlobalID && x.SkillID == skillID);
        }

        public int GetPCTotalSkillCount(string playerID)
        {
            return _db.PCSkills.Where(x => x.PlayerID == playerID).Sum(x => x.Rank);
        }

        public PCSkill GetPCSkillByID(string playerID, int skillID)
        {
            return _db.PCSkills.Single(x => x.PlayerID == playerID && x.SkillID == skillID);
        }

        public List<SkillCategory> GetActiveCategories()
        {
            return _db.SkillCategories.Where(x => x.IsActive).ToList();
        }

        public List<PCSkill> GetPCSkillsForCategory(string playerID, int skillCategoryID)
        {
            return _db.PCSkills.Where(x => x.Skill.IsActive && x.Skill.SkillCategoryID == skillCategoryID && x.PlayerID == playerID).ToList();
        }

        public SkillXPRequirement GetSkillXPRequirementByRank(int skillID, int rank)
        {
            return _db.SkillXPRequirements.Single(x => x.SkillID == skillID && x.Rank == rank);
        }

        public void ToggleSkillLock(string playerID, int skillID)
        {
            PCSkill pcSkill = GetPCSkillByID(playerID, skillID);
            pcSkill.IsLocked = !pcSkill.IsLocked;

            _db.SaveChanges();
        }

        public void OnCreatureDeath(NWCreature creature)
        {
            CreatureSkillRegistration reg = GetCreatureSkillRegistration(creature.GlobalID);
            List<PlayerSkillRegistration> playerRegs = reg.GetAllRegistrations();

            foreach (PlayerSkillRegistration preg in playerRegs)
            {
                // Rules for acquiring skill XP:
                // Player must be valid.
                // Player must be in the same area as the creature that just died.
                // Player must be within 30 meters of the creature that just died.
                if (!preg.Player.IsValid ||
                    preg.Player.Area.Resref != creature.Area.Resref ||    
                        _.GetDistanceBetween(preg.Player.Object, creature.Object) > 30.0f)
                    continue;
                
                float cr = creature.ChallengeRating;
                float baseXP = cr * 400 + _random.Random(20);
                float moduleXPAdjustment = _.GetLocalFloat(_.GetModule(), "SKILL_SYSTEM_MODULE_XP_MODIFIER");
                if (moduleXPAdjustment <= 0.0f) moduleXPAdjustment = 1.0f;
                baseXP = baseXP * moduleXPAdjustment;

                List<Tuple<int, PlayerSkillPointTracker>> skillRegs = preg.GetSkillRegistrationPoints();
                int totalPoints = preg.GetTotalSkillRegistrationPoints();
                bool receivesMartialArtsPenalty = CheckForMartialArtsPenalty(skillRegs);

                // Grant XP based on points acquired during combat.
                foreach (Tuple<int, PlayerSkillPointTracker> skreg in skillRegs)
                {
                    int skillID = skreg.Item1;
                    int skillRank = GetPCSkillByID(preg.Player.GlobalID, skillID).Rank;
                    int points = skreg.Item2.Points;
                    float percentage = points / (float)totalPoints;
                    float adjustedXP = baseXP * percentage;
                    adjustedXP = CalculateSkillAdjustedXP(adjustedXP, skreg.Item2.RegisteredLevel, skillRank);

                    // Penalty to martial arts XP for using a shield.
                    if (skillID == (int)SkillType.MartialArts && receivesMartialArtsPenalty)
                        adjustedXP = adjustedXP * 0.4f;

                    GiveSkillXP(preg.Player, skillID, (int)(adjustedXP));
                }

                float armorXP = baseXP * 0.20f;
                int lightArmorPoints = 0;
                int heavyArmorPoints = 0;

                for (int slot = 0; slot < NUM_INVENTORY_SLOTS; slot++)
                {
                    NWItem item = NWItem.Wrap(_.GetItemInSlot(slot, preg.Player.Object));
                    if (item.CustomItemType == CustomItemType.LightArmor)
                    {
                        lightArmorPoints++;
                    }
                    else if (item.CustomItemType == CustomItemType.HeavyArmor)
                    {
                        heavyArmorPoints++;
                    }
                }
                totalPoints = lightArmorPoints + heavyArmorPoints;
                if (totalPoints <= 0) continue;

                float percent = lightArmorPoints / (float)totalPoints;
                GiveSkillXP(preg.Player, SkillType.LightArmor, (int)(armorXP * percent));

                percent = heavyArmorPoints / (float)totalPoints;
                GiveSkillXP(preg.Player, SkillType.HeavyArmor, (int)(armorXP * percent));
            }

            _state.CreatureSkillRegistrations.Remove(creature.GlobalID);
        }

        private bool CheckForMartialArtsPenalty(List<Tuple<int, PlayerSkillPointTracker>> skillRegs)
        {
            bool usedShield = false;
            bool usedMartialArts = false;
            foreach (Tuple<int, PlayerSkillPointTracker> sreg in skillRegs)
            {
                if (sreg.Item1 == (int)SkillType.Shields) usedShield = true;
                else if (sreg.Item1 == (int)SkillType.MartialArts) usedMartialArts = true;

                if (usedMartialArts && usedShield) return true;
            }

            return false;
        }
        public void OnAreaExit()
        {
            NWPlayer oPC = NWPlayer.Wrap(_.GetExitingObject());
            RemovePlayerFromRegistrations(oPC);
        }

        public void OnModuleEnter()
        {
            NWPlayer oPC = NWPlayer.Wrap(_.GetEnteringObject());

            if (oPC.IsPlayer)
            {
                _db.StoredProcedure("InsertAllPCSkillsByID",
                    new SqlParameter("PlayerID", oPC.GlobalID));
                ForceEquipFistGlove(oPC);
            }
        }

        public void OnModuleClientLeave()
        {
            NWPlayer oPC = NWPlayer.Wrap(_.GetExitingObject());
            RemovePlayerFromRegistrations(oPC);
        }

        public void OnModuleItemEquipped()
        {
            NWPlayer oPC = NWPlayer.Wrap(_.GetPCItemLastEquippedBy());
            if (!oPC.IsInitializedAsPlayer) return; // Players who log in for the first time don't have an ID yet.

            NWItem oItem = NWItem.Wrap(_.GetPCItemLastEquipped());
            ApplyStatChanges(oPC, null);
            ApplyWeaponPenalties(oPC, oItem);
        }

        public void OnModuleItemUnequipped()
        {
            NWPlayer oPC = NWPlayer.Wrap(_.GetPCItemLastUnequippedBy());
            NWItem oItem = NWItem.Wrap(_.GetPCItemLastUnequipped());
            HandleGlovesUnequipEvent();
            ApplyStatChanges(oPC, oItem);
            RemoveWeaponPenalties(oItem);
        }

        public float CalculateSkillAdjustedXP(float xp, int registeredLevel, int skillRank)
        {
            int delta = registeredLevel - skillRank;
            float levelAdjustment = 0.2f * delta;

            if (levelAdjustment > 1.0f) levelAdjustment = 1.0f;
            if (levelAdjustment < -1.0f) levelAdjustment = -1.0f;

            xp = xp + (xp * levelAdjustment);
            return xp;
        }
        
        private void ForceEquipFistGlove(NWPlayer oPC)
        {
            oPC.DelayCommand(() =>
            {
                if (!oPC.Arms.IsValid)
                {
                    oPC.ClearAllActions();
                    NWItem glove = NWItem.Wrap(_.CreateItemOnObject("fist", oPC.Object));
                    glove.SetLocalInt("UNBREAKABLE", 1);

                    oPC.AssignCommand(() => _.ActionEquipItem(glove.Object, INVENTORY_SLOT_ARMS));
                }
            }, 1.0f);
        }

        private void RemovePlayerFromRegistrations(NWPlayer oPC)
        {
            foreach (CreatureSkillRegistration reg in _state.CreatureSkillRegistrations.Values.ToArray())
            {
                reg.RemovePlayerRegistration(oPC);

                if (reg.IsRegistrationEmpty())
                {
                    _state.CreatureSkillRegistrations.Remove(reg.CreatureID);
                }
                else
                {
                    _state.CreatureSkillRegistrations[reg.CreatureID] = reg;
                }
            }
        }

        private void HandleGlovesUnequipEvent()
        {
            NWPlayer oPC = NWPlayer.Wrap(_.GetPCItemLastUnequippedBy());
            NWItem oItem = NWItem.Wrap(_.GetPCItemLastUnequipped());
            int type = oItem.BaseItemType;

            if (!oPC.IsPlayer) return;
            if (type != BASE_ITEM_BRACER && type != BASE_ITEM_GLOVES) return;

            // If fist was unequipped, destroy it.
            if (oItem.Resref == "fist")
            {
                oItem.Destroy();
            }

            // Check in 1 second to see if PC has a glove equipped. If they don't, create a fist glove and equip it.
            ForceEquipFistGlove(oPC);
        }


        private int CalculateTotalSkillPointsPenalty(int totalSkillPoints, int xp)
        {
            if (totalSkillPoints >= 450)
            {
                xp = (int)(xp * 0.70f);
            }
            else if (totalSkillPoints >= 400)
            {
                xp = (int)(xp * 0.80f);
            }
            else if (totalSkillPoints >= 350)
            {
                xp = (int)(xp * 0.85f);
            }
            else if (totalSkillPoints >= 300)
            {
                xp = (int)(xp * 0.90f);
            }
            else if (totalSkillPoints >= 250)
            {
                xp = (int)(xp * 0.95f);
            }

            return xp;
        }

        private bool ApplySkillDecay(NWPlayer oPC, PCSkill levelingSkill, int xp)
        {
            int totalSkillRanks = GetPCTotalSkillCount(oPC.GlobalID);
            if (totalSkillRanks < SkillCap) return true;

            // Find out if we have enough XP to remove. If we don't, make no changes and return false signifying no XP could be removed.
            List<TotalSkillXPResult> skillTotalXP = _db.StoredProcedure<TotalSkillXPResult>("GetTotalXPAmountsForPC",
                new SqlParameter("PlayerID", oPC.GlobalID),
                new SqlParameter("SkillID", levelingSkill.SkillID));
            int aggregateXP = 0;
            foreach (TotalSkillXPResult p in skillTotalXP)
            {
                aggregateXP += p.TotalSkillXP;
            }
            if (aggregateXP < xp) return false;

            // We have enough XP to remove. Reduce XP, picking random skills each time we reduce.
            List<PCSkill> skillsPossibleToDecay = _db.PCSkills
                .Where(x => !x.IsLocked &&
                            x.PlayerID == oPC.GlobalID &&
                            x.SkillID != levelingSkill.SkillID &&
                            (x.XP > 0 || x.Rank > 0)).ToList();
            while (xp > 0)
            {
                int skillIndex = _random.Random(skillsPossibleToDecay.Count);
                PCSkill decaySkill = skillsPossibleToDecay[skillIndex];
                int totalDecaySkillXP = skillTotalXP[skillIndex].TotalSkillXP;

                if (totalDecaySkillXP >= xp)
                {
                    totalDecaySkillXP = totalDecaySkillXP - xp;
                    xp = 0;
                }
                else if (totalDecaySkillXP < xp)
                {
                    totalDecaySkillXP = 0;
                    xp = xp - totalDecaySkillXP;
                }

                // If skill drops to 0 total XP, remove it from the possible list of skills
                if (totalDecaySkillXP <= 0)
                {
                    skillsPossibleToDecay.Remove(decaySkill);
                    decaySkill.XP = 0;
                    decaySkill.Rank = 0;
                }
                // Otherwise calculate what rank and XP value the skill should now be.
                else
                {
                    List<SkillXPRequirement> reqs = _db.SkillXPRequirements.Where(x => x.SkillID == decaySkill.SkillID && x.Rank <= decaySkill.Rank).ToList();
                    int newDecaySkillRank = 0;
                    foreach (SkillXPRequirement req in reqs)
                    {
                        if (totalDecaySkillXP >= req.XP)
                        {
                            totalDecaySkillXP = totalDecaySkillXP - req.XP;
                            newDecaySkillRank++;
                        }
                        else if (totalDecaySkillXP < req.XP)
                        {
                            break;
                        }
                    }

                    decaySkill.Rank = newDecaySkillRank;
                    decaySkill.XP = totalDecaySkillXP;
                }

                _db.SaveChanges();
            }

            ApplyStatChanges(oPC, null);
            return true;
        }

        private int GetWeaponSkillID(NWItem item)
        {
            int skillID = -1;
            int type = item.BaseItemType;
            int[] oneHandedTypes = {
                BASE_ITEM_BASTARDSWORD,
                BASE_ITEM_BATTLEAXE,
                BASE_ITEM_CLUB,
                BASE_ITEM_DAGGER,
                BASE_ITEM_HANDAXE,
                BASE_ITEM_KAMA,
                BASE_ITEM_KATANA,
                BASE_ITEM_KUKRI,
                BASE_ITEM_LIGHTFLAIL,
                BASE_ITEM_LIGHTHAMMER,
                BASE_ITEM_LIGHTMACE,
                BASE_ITEM_LONGSWORD,
                BASE_ITEM_RAPIER,
                BASE_ITEM_SCIMITAR,
                BASE_ITEM_SHORTSPEAR,
                BASE_ITEM_SHORTSWORD,
                BASE_ITEM_SICKLE,
                BASE_ITEM_WHIP
        };

            int[] twoHandedTypes = {
                BASE_ITEM_DIREMACE,
                BASE_ITEM_DWARVENWARAXE,
                BASE_ITEM_GREATAXE,
                BASE_ITEM_GREATSWORD,
                BASE_ITEM_HALBERD,
                BASE_ITEM_HEAVYFLAIL,
                BASE_ITEM_MORNINGSTAR,
                BASE_ITEM_QUARTERSTAFF,
                BASE_ITEM_SCYTHE,
                BASE_ITEM_TRIDENT,
                BASE_ITEM_WARHAMMER
        };

            int[] twinBladeTypes = {
                BASE_ITEM_DOUBLEAXE,
                BASE_ITEM_TWOBLADEDSWORD
        };

            int[] martialArtsTypes = {
                BASE_ITEM_BRACER,
                BASE_ITEM_GLOVES
        };

            int[] archeryTypes = {
                BASE_ITEM_HEAVYCROSSBOW,
                BASE_ITEM_LIGHTCROSSBOW,
                BASE_ITEM_LONGBOW,
                BASE_ITEM_SHORTBOW,
                BASE_ITEM_ARROW,
                BASE_ITEM_BOLT
        };

            int[] throwingTypes = {
                BASE_ITEM_GRENADE,
                BASE_ITEM_SHURIKEN,
                BASE_ITEM_SLING,
                BASE_ITEM_THROWINGAXE,
                BASE_ITEM_BULLET,
                BASE_ITEM_DART
        };

            if (oneHandedTypes.Contains(type)) skillID = (int)SkillType.OneHanded;
            else if (twoHandedTypes.Contains(type)) skillID = (int)SkillType.TwoHanded;
            else if (twinBladeTypes.Contains(type)) skillID = (int)SkillType.TwinBlades;
            else if (martialArtsTypes.Contains(type)) skillID = (int)SkillType.MartialArts;
            else if (archeryTypes.Contains(type)) skillID = (int)SkillType.Archery;
            else if (throwingTypes.Contains(type)) skillID = (int)SkillType.Throwing;

            return skillID;
        }


        private CreatureSkillRegistration GetCreatureSkillRegistration(string creatureUUID)
        {
            if (_state.CreatureSkillRegistrations.ContainsKey(creatureUUID))
            {
                return _state.CreatureSkillRegistrations[creatureUUID];
            }
            else
            {
                var reg = new CreatureSkillRegistration(creatureUUID);
                _state.CreatureSkillRegistrations[creatureUUID] = reg;
                return reg;
            }
        }


        public void OnHitCastSpell(NWPlayer oPC)
        {
            if (!oPC.IsPlayer) return;
            NWItem oSpellOrigin = NWItem.Wrap(_.GetSpellCastItem());
            NWCreature oTarget = NWCreature.Wrap(_.GetSpellTargetObject());

            int skillID = GetWeaponSkillID(oSpellOrigin);
            if (skillID <= -1) return;
            if (oTarget.IsPlayer || oTarget.IsDM) return;
            if (oTarget.ObjectType != OBJECT_TYPE_CREATURE) return;
            
            CreatureSkillRegistration reg = GetCreatureSkillRegistration(oTarget.GlobalID);

            reg.AddSkillRegistrationPoint(oPC, skillID, oSpellOrigin.RecommendedLevel);

            // Add a registration point if a shield is equipped. This is to prevent players from swapping out a weapon for a shield
            // just before they kill an enemy.
            NWItem oShield = oPC.LeftHand;
            if (oShield.BaseItemType == BASE_ITEM_SMALLSHIELD ||
                oShield.BaseItemType == BASE_ITEM_LARGESHIELD ||
                oShield.BaseItemType == BASE_ITEM_TOWERSHIELD)
            {
                reg.AddSkillRegistrationPoint(oPC, (int)SkillType.Shields, oShield.RecommendedLevel);
            }

            if (_random.Random(100) + 1 <= 3)
            {
                _food.DecreaseHungerLevel(oPC, 1);
            }
        }

        public void RegisterPCToNPCForSkill(NWPlayer pc, NWCreature npc, int skillID)
        {
            if (!pc.IsPlayer || !pc.IsValid) return;
            if (npc.IsPlayer || npc.IsDM || !npc.IsValid) return;
            if (skillID <= 0) return;

            PCSkill pcSkill = GetPCSkill(pc, skillID);
            if (pcSkill == null) return;
            
            CreatureSkillRegistration reg = GetCreatureSkillRegistration(npc.GlobalID);
            reg.AddSkillRegistrationPoint(pc, skillID, pcSkill.Rank);
        }

        public void RegisterPCToNPCForSkill(NWPlayer pc, NWCreature npc, SkillType skillType)
        {
            RegisterPCToNPCForSkill(pc, npc, (int) skillType);
        }

        public void RegisterPCToAllCombatTargetsForSkill(NWPlayer pc, int skillID)
        {
            if (!pc.IsPlayer || skillID <= 0) return;

            List<NWCreature> members = new List<NWCreature>();

            NWN.Object member = _.GetFirstFactionMember(pc.Object);
            while (_.GetIsObjectValid(member) == TRUE)
            {
                members.Add(NWCreature.Wrap(member));
                member = _.GetNextFactionMember(pc.Object);
            }

            int nth = 1;
            NWCreature creature = NWCreature.Wrap(_.GetNearestCreature(CREATURE_TYPE_IS_ALIVE, 1, pc.Object, nth, CREATURE_TYPE_PLAYER_CHAR, 0));
            while (creature.IsValid)
            {
                if (_.GetDistanceBetween(pc.Object, creature.Object) > 20.0f) break;

                NWCreature target = NWCreature.Wrap(_.GetAttackTarget(creature.Object));
                if (target.IsValid && members.Contains(target))
                {
                    if (target.IsValid && target.Area.Equals(pc.Area))
                    {
                        RegisterPCToNPCForSkill(pc, creature, skillID);
                    }
                }

                nth++;
                creature = NWCreature.Wrap(_.GetNearestCreature(CREATURE_TYPE_IS_ALIVE, 1, pc.Object, nth, CREATURE_TYPE_PLAYER_CHAR, 0));
            }
        }

        private int CalculateBAB(NWPlayer oPC, NWItem ignoreItem)
        {
            NWItem weapon = oPC.RightHand;

            // The unequip event fires before the item is actually unequipped, so we need
            // to have additional checks to make sure we're not getting the weapon that's about to be
            // unequipped.
            if (weapon.Equals(ignoreItem))
            {
                weapon = null;
                NWItem offHand = oPC.LeftHand;

                if (offHand.CustomItemType == CustomItemType.Blade ||
                   offHand.CustomItemType == CustomItemType.FinesseBlade ||
                   offHand.CustomItemType == CustomItemType.Blunt ||
                   offHand.CustomItemType == CustomItemType.HeavyBlade ||
                   offHand.CustomItemType == CustomItemType.HeavyBlunt ||
                   offHand.CustomItemType == CustomItemType.Polearm ||
                   offHand.CustomItemType == CustomItemType.TwinBlade ||
                   offHand.CustomItemType == CustomItemType.MartialArtWeapon ||
                   offHand.CustomItemType == CustomItemType.Bow ||
                   offHand.CustomItemType == CustomItemType.Crossbow ||
                   offHand.CustomItemType == CustomItemType.Throwing)
                {
                    weapon = offHand;
                }
            }

            if (weapon == null || !weapon.IsValid)
            {
                weapon = oPC.Arms;
            }
            if (!weapon.IsValid) return 0;

            int weaponSkillID = GetWeaponSkillID(weapon);
            PCSkill skill = GetPCSkill(oPC, weaponSkillID);
            if (skill == null) return 0;
            int skillBAB = skill.Rank / 10;
            int perkBAB = 0;

            if (weaponSkillID == (int)SkillType.Throwing)
            {
                perkBAB += _perk.GetPCPerkLevel(oPC, PerkType.TossAccuracy);
            }

            return 1 + skillBAB + perkBAB; // Note: Always add 1 to BAB. 0 will cause a crash in NWNX.
        }

        private int CalculateItemAC(NWPlayer oPC, NWItem ignoreItem)
        {
            int ac = 0;
            for (int slot = 0; slot < NUM_INVENTORY_SLOTS; slot++)
            {
                NWItem oItem = NWItem.Wrap(_.GetItemInSlot(slot, oPC.Object));
                if (oItem.Equals(ignoreItem))
                    continue;

                if (oItem.IsValid)
                {
                    int itemAC = oItem.AC;

                    ac += itemAC;
                }
            }
            return ac;
        }

        private void ApplyWeaponPenalties(NWPlayer oPC, NWItem oItem)
        {
            int skillID = GetWeaponSkillID(oItem);
            if (skillID <= 0) return;

            PCSkill pcSkill = GetPCSkill(oPC, skillID);
            if (pcSkill == null) return;
            int rank = pcSkill.Rank;
            int recommendedRank = oItem.RecommendedLevel;
            if (rank >= recommendedRank) return;

            int delta = rank - recommendedRank;
            int penalty;

            if (delta <= -20)
            {
                penalty = 99;
            }
            else if (delta <= -16)
            {
                penalty = 5;
            }
            else if (delta <= -12)
            {
                penalty = 4;
            }
            else if (delta <= -8)
            {
                penalty = 3;
            }
            else if (delta <= -4)
            {
                penalty = 2;
            }
            else if (delta <= 0)
            {
                penalty = 1;
            }
            else penalty = 99;

            // No combat damage penalty
            if (penalty == 99)
            {
                ItemProperty noDamage = _.ItemPropertyNoDamage();
                noDamage = _.TagItemProperty(noDamage, IPPenaltyTag);
                _biowareXP2.IPSafeAddItemProperty(oItem, noDamage, 0.0f, AddItemPropertyPolicy.ReplaceExisting, false, false);
                penalty = 5; // Reset to 5 so that the following penalties apply.
            }

            // Decreased attack penalty
            ItemProperty ipPenalty = _.ItemPropertyAttackPenalty(penalty);
            ipPenalty = _.TagItemProperty(ipPenalty, IPPenaltyTag);
            _biowareXP2.IPSafeAddItemProperty(oItem, ipPenalty, 0.0f, AddItemPropertyPolicy.ReplaceExisting, false, false);

            // Decreased damage penalty
            ipPenalty = _.ItemPropertyDamagePenalty(penalty);
            ipPenalty = _.TagItemProperty(ipPenalty, IPPenaltyTag);
            _biowareXP2.IPSafeAddItemProperty(oItem, ipPenalty, 0.0f, AddItemPropertyPolicy.ReplaceExisting, false, false);

            // Decreased enhancement bonus penalty
            ipPenalty = _.ItemPropertyEnhancementPenalty(penalty);
            ipPenalty = _.TagItemProperty(ipPenalty, IPPenaltyTag);
            _biowareXP2.IPSafeAddItemProperty(oItem, ipPenalty, 0.0f, AddItemPropertyPolicy.ReplaceExisting, false, false);

            oPC.SendMessage("A penalty has been applied to your weapon '" + oItem.Name + "' due to your skill being under the recommended level.");
        }

        private void RemoveWeaponPenalties(NWItem oItem)
        {
            int skillID = GetWeaponSkillID(oItem);
            if (skillID <= 0) return;
            
            foreach (ItemProperty ip in oItem.ItemProperties)
            {
                string tag = _.GetItemPropertyTag(ip);
                if ( tag == IPPenaltyTag)
                {
                    _.RemoveItemProperty(oItem.Object, ip);
                }
            }
        }
    }
}
