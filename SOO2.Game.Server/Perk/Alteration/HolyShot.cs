using NWN;
using SOO2.Game.Server.Enumeration;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Service.Contracts;
using static NWN.NWScript;

namespace SOO2.Game.Server.Perk.Alteration
{
    public class HolyShot: IPerk
    {
        private readonly INWScript _;
        private readonly IPerkService _perk;
        private readonly IRandomService _random;
        private readonly ISkillService _skill;

        public HolyShot(
            INWScript script,
            IPerkService perk,
            IRandomService random,
            ISkillService skill)
        {
            _ = script;
            _perk = perk;
            _random = random;
            _skill = skill;
        }

        public bool CanCastSpell(NWPlayer oPC, NWObject oTarget)
        {
            return true;
        }

        public string CannotCastSpellMessage(NWPlayer oPC, NWObject oTarget)
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
            int level = _perk.GetPCPerkLevel(oPC, PerkType.HolyShot);
            int damage;
            int alterationBonus = oPC.AlterationBonus;

            switch (level)
            {
                case 1:
                    damage = _random.Random(8 + alterationBonus) + 1;
                    break;
                case 2:
                    damage = _random.Random(6 + alterationBonus) + 1;
                    damage += _random.Random(6 + alterationBonus) + 1;
                    break;
                case 3:
                    damage = _random.Random(6 + alterationBonus) + 1;
                    damage += _random.Random(6 + alterationBonus) + 1;
                    break;
                case 4:
                    damage = _random.Random(4 + alterationBonus) + 1;
                    damage += _random.Random(4 + alterationBonus) + 1;
                    damage += _random.Random(4 + alterationBonus) + 1;
                    damage += _random.Random(4 + alterationBonus) + 1;
                    break;
                case 5:
                    damage = _random.Random(4 + alterationBonus) + 1;
                    damage += _random.Random(4 + alterationBonus) + 1;
                    damage += _random.Random(4 + alterationBonus) + 1;
                    damage += _random.Random(4 + alterationBonus) + 1;
                    damage += _random.Random(4 + alterationBonus) + 1;
                    break;
                default:
                    return;
            }

            int wisdom = oPC.WisdomModifier;
            int intelligence = oPC.IntelligenceModifier;

            float damageMultiplier = 1.0f + (intelligence * 0.4f) + (wisdom * 0.2f);
            damage = (int)(damage * damageMultiplier);

            Effect vfx = _.EffectBeam(VFX_BEAM_SILENT_HOLY, oPC.Object, BODY_NODE_CHEST);
            _.ApplyEffectToObject(DURATION_TYPE_TEMPORARY, vfx, oTarget.Object, 1.5f);

            _skill.RegisterPCToNPCForSkill(oPC, NWCreature.Wrap(oTarget.Object), SkillType.AlterationMagic);

            oPC.AssignCommand(() =>
            {
                _.ApplyEffectToObject(DURATION_TYPE_INSTANT, _.EffectDamage(damage), oTarget.Object);
            });
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

        public void OnCustomEnmityRule(NWPlayer oPC, int amount)
        {
        }

        public bool IsHostile()
        {
            return true;
        }
    }
}
