using Freescape.Game.Server.Enumeration;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using NWN;
using static NWN.NWScript;

namespace Freescape.Game.Server.Perk.Armor
{
    public class Evasiveness : IPerk
    {
        private readonly INWScript _;
        private readonly IPerkService _perk;

        public Evasiveness(INWScript script,
            IPerkService perk)
        {
            _ = script;
            _perk = perk;
        }

        public bool CanCastSpell(NWPlayer oPC, NWObject oTarget)
        {
            NWItem armor = oPC.Chest;
            return armor.CustomItemType == CustomItemType.HeavyArmor;
        }

        public string CannotCastSpellMessage()
        {
            return "You must be equipped with light armor to use that combat ability.";
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
            int perkLevel = _perk.GetPCPerkLevel(oPC, PerkType.Evasiveness);
            int concealment;
            float length;

            switch (perkLevel)
            {
                case 1:
                    concealment = 10;
                    length = 12.0f;
                    break;
                case 2:
                    concealment = 15;
                    length = 12.0f;
                    break;
                case 3:
                    concealment = 20;
                    length = 12.0f;
                    break;
                case 4:
                    concealment = 25;
                    length = 12.0f;
                    break;
                case 5:
                    concealment = 30;
                    length = 18.0f;
                    break;
                default:
                    return;
            }

            Effect effect = _.EffectConcealment(concealment);
            _.ApplyEffectToObject(DURATION_TYPE_TEMPORARY, effect, oPC.Object, length);

            effect = _.EffectVisualEffect(VFX_DUR_AURA_CYAN);
            _.ApplyEffectToObject(DURATION_TYPE_TEMPORARY, effect, oPC.Object, length);

            effect = _.EffectVisualEffect(VFX_IMP_AC_BONUS);
            _.ApplyEffectToObject(DURATION_TYPE_INSTANT, effect, oPC.Object);
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
