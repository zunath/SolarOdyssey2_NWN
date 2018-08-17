using Freescape.Game.Server.Enumeration;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using NWN;
using static NWN.NWScript;

namespace Freescape.Game.Server.Perk.Armor
{
    public class DeflectDamage: IPerk
    {
        private readonly INWScript _;
        private readonly IPerkService _perk;

        public DeflectDamage(INWScript script,
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
            return "You must be equipped with heavy armor to use that combat ability.";
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
            int damageBase;
            float length = 12.0f;
            int randomDamage;

            switch (perkLevel)
            {
                case 1:
                    damageBase = 1;
                    randomDamage = 6; // 6 = DAMAGE_BONUS_1d4 constant
                    break;
                case 2:
                    damageBase = 1;
                    randomDamage = 8; // 8 = DAMAGE_BONUS_1d8 constant
                    break;
                case 3:
                    damageBase = 2;
                    randomDamage = 10; // 10 = DAMAGE_BONUS_2d6 constant
                    break;
                case 4:
                    damageBase = 2;
                    randomDamage = 11; // 11 = DAMAGE_BONUS_2d8 constant
                    break;
                case 5:
                    damageBase = 3;
                    randomDamage = 15; // 15 = DAMAGE_BONUS_2d12 constant
                    break;
                default:
                    return;
            }

            Effect effect = _.EffectDamageShield(damageBase, randomDamage, DAMAGE_TYPE_MAGICAL);
            _.ApplyEffectToObject(DURATION_TYPE_TEMPORARY, effect, oPC.Object, length);

            effect = _.EffectVisualEffect(VFX_DUR_AURA_ORANGE);
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
