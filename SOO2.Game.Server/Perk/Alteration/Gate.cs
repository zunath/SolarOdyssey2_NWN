using Freescape.Game.Server.Bioware.Contracts;
using Freescape.Game.Server.Enumeration;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using NWN;
using static NWN.NWScript;

namespace Freescape.Game.Server.Perk.Alteration
{
    public class Gate: IPerk
    {
        private readonly INWScript _;
        private readonly IDeathService _death;
        private readonly ISkillService _skill;
        private readonly IBiowareXP2 _biowareXP2;
        private readonly IItemService _item;

        public Gate(INWScript script,
            IDeathService death,
            ISkillService skill,
            IBiowareXP2 biowareXP2,
            IItemService item)
        {
            _ = script;
            _death = death;
            _skill = skill;
            _biowareXP2 = biowareXP2;
            _item = item;
        }

        public bool CanCastSpell(NWPlayer oPC, NWObject oTarget)
        {
            if (oPC.Equals(oTarget)) return true;
            
            foreach (NWPlayer member in oPC.GetPartyMembers())
            {
                if (oTarget.Equals(member))
                    return true;
            }
            return false;
        }

        public string CannotCastSpellMessage()
        {
            return "Only party members may be targeted with that ability.";
        }

        public int ManaCost(NWPlayer oPC, int baseManaCost)
        {
            return baseManaCost;
        }

        public float CastingTime(NWPlayer oPC, float baseCastingTime)
        {
            int wisdom = oPC.WisdomModifier;
            int intelligence = oPC.IntelligenceModifier;
            float castingTime = baseCastingTime - ((wisdom * 1.5f) + (intelligence));

            if (castingTime <= 2.0f) castingTime = 2.0f;
            return castingTime;
        }

        public float CooldownTime(NWPlayer oPC, float baseCooldownTime)
        {
            return baseCooldownTime;
        }

        public void OnImpact(NWPlayer oPC, NWObject oTarget)
        {
            Location location = oTarget.Location;
            Effect effect = _.EffectVisualEffect(VFX_IMP_UNSUMMON);

            _death.TeleportPlayerToBindPoint((NWPlayer)oTarget);
            _.ApplyEffectAtLocation(DURATION_TYPE_INSTANT, effect, location);

            _skill.GiveSkillXP(oPC, SkillType.AlterationMagic, 100);
        }

        public void OnPurchased(NWPlayer oPC, int newLevel)
        {
            // Party members become targetable at level 2.
            if (newLevel >= 2)
            {
                NWItem item = NWItem.Wrap(_.GetItemPossessedBy(oPC.Object, "perk_gate"));
                _item.StripAllItemProperties(item);
                ItemProperty ip = _.ItemPropertyCastSpell(IP_CONST_CASTSPELL_UNIQUE_POWER, IP_CONST_CASTSPELL_NUMUSES_UNLIMITED_USE);
                _biowareXP2.IPSafeAddItemProperty(item, ip, 0.0f, AddItemPropertyPolicy.ReplaceExisting, false, false);
            }
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
