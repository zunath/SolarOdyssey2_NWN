using Freescape.Game.Server.Enumeration;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using NWN;
using static NWN.NWScript;

namespace Freescape.Game.Server.Perk.Evocation
{
    public class LightningShock : IPerk
    {
        private readonly INWScript _;
        private readonly IRandomService _random;
        private readonly IPerkService _perk;
        private readonly ISkillService _skill;

        public LightningShock(INWScript script,
            IRandomService random,
            IPerkService perk,
            ISkillService skill)
        {
            _ = script;
            _random = random;
            _perk = perk;
            _skill = skill;
        }

        public bool CanCastSpell(NWPlayer oPC, NWObject oTarget)
        {
            return true;
        }

        public string CannotCastSpellMessage()
        {
            return null;
        }

        public int ManaCost(NWPlayer oPC, int baseManaCost)
        {
            return baseManaCost;
        }

        public float CastingTime(NWPlayer oPC, float baseCastingTime)
        {
            return baseCastingTime;
        }

        public float CooldownTime(NWPlayer oPC, float baseCooldownTime)
        {
            return baseCooldownTime;
        }

        public void OnImpact(NWPlayer oPC, NWObject oTarget)
        {
            int level = _perk.GetPCPerkLevel(oPC, PerkType.LightningShock);
            int damage;

            switch (level)
            {
                case 1:
                    damage = _random.Random(8) + 1;
                    break;
                case 2:
                    damage = _random.Random(6) + 1;
                    damage += _random.Random(6) + 1;
                    break;
                case 3:
                    damage = _random.Random(6) + 1;
                    damage += _random.Random(6) + 1;
                    break;
                case 4:
                    damage = _random.Random(4) + 1;
                    damage += _random.Random(4) + 1;
                    damage += _random.Random(4) + 1;
                    damage += _random.Random(4) + 1;
                    break;
                case 5:
                    damage = _random.Random(4) + 1;
                    damage += _random.Random(4) + 1;
                    damage += _random.Random(4) + 1;
                    damage += _random.Random(4) + 1;
                    damage += _random.Random(4) + 1;
                    break;
                default:
                    return;
            }

            int wisdom = oPC.WisdomModifier;
            int intelligence = oPC.IntelligenceModifier;

            float damageMultiplier = 1.0f + (intelligence * 0.2f) + (wisdom * 0.1f);
            damage = (int)(damage * damageMultiplier);

            _skill.RegisterPCToNPCForSkill(oPC, (NWCreature)oTarget, SkillType.EvocationMagic);
            _.ApplyEffectToObject(DURATION_TYPE_INSTANT, _.EffectVisualEffect(VFX_IMP_DOOM), oTarget.Object);
            _.ApplyEffectToObject(DURATION_TYPE_INSTANT, _.EffectDamage(damage), oTarget.Object);
        }

        public void OnPurchased(NWPlayer oPC, int newLevel)
        {
        }

        public void OnRemoved(NWPlayer oPC)
        {
        }

        public void OnItemEquipped(NWPlayer oPC, NWItem oItem)
        {
        }

        public void OnItemUnequipped(NWPlayer oPC, NWItem oItem)
        {
        }

        public bool IsHostile()
        {
            return true;
        }
    }
}
