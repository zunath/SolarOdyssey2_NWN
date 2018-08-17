using Freescape.Game.Server.Enumeration;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using NWN;
using static NWN.NWScript;

namespace Freescape.Game.Server.Perk.Alteration
{
    public class Aegis: IPerk
    {
        private readonly INWScript _;
        private readonly IPerkService _perk;
        private readonly ICustomEffectService _customEffect;
        private readonly ISkillService _skill;
        private readonly IRandomService _random;

        public Aegis(
            INWScript script, 
            IPerkService perk, 
            ICustomEffectService customEffect,
            ISkillService skill,
            IRandomService random)
        {
            _ = script;
            _perk = perk;
            _customEffect = customEffect;
            _skill = skill;
            _random = random;
        }

        public bool CanCastSpell(NWPlayer oPC, NWObject oTarget)
        {
            int level = _perk.GetPCPerkLevel(oPC, PerkType.Aegis);
            int activeAegisLevel = _customEffect.GetActiveEffectLevel(oTarget, CustomEffectType.Aegis);

            return level >= activeAegisLevel;
        }

        public string CannotCastSpellMessage()
        {
            return "A more powerful effect already exists on your target.";
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
            int level = _perk.GetPCPerkLevel(oPC, PerkType.Aegis);
            int ticks;

            switch (level)
            {
                case 1:
                case 2:
                    ticks = 10;
                    break;
                case 3:
                case 4:
                case 5:
                    ticks = 50;
                    break;
                default: return;
            }
            
            int luck = _perk.GetPCPerkLevel(oPC, PerkType.Lucky);
            if(_random.Random(100) + 1 <= luck)
            {
                ticks = ticks * 2;
            }

            int wisdom = oPC.WisdomModifier;
            int intelligence = oPC.IntelligenceModifier;
            ticks = ticks + intelligence + (wisdom * 2);

            _customEffect.ApplyCustomEffect(oPC, (NWPlayer)oTarget, CustomEffectType.Aegis, ticks, level);
            _skill.ApplyStatChanges((NWPlayer)oTarget, null);
            _skill.RegisterPCToAllCombatTargetsForSkill(oPC, SkillType.AlterationMagic);

            Effect vfx = _.EffectVisualEffect(VFX_IMP_AC_BONUS);
            _.ApplyEffectToObject(DURATION_TYPE_INSTANT, vfx, oTarget.Object);

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
            return false;
        }
    }
}
