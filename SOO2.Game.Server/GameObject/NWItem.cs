using System.Collections.Generic;
using Freescape.Game.Server.Enumeration;
using Freescape.Game.Server.GameObject.Contracts;
using Freescape.Game.Server.Service.Contracts;
using NWN;
using static NWN.NWScript;
using Object = NWN.Object;

namespace Freescape.Game.Server.GameObject
{
    public class NWItem : NWObject, INWItem
    {
        private readonly IDurabilityService _durability;

        public NWItem(INWScript script,
            IDurabilityService durability)
            : base(script)
        {
            _durability = durability;
        }

        public new static NWItem Wrap(Object @object)
        {
            var obj = (NWItem)App.Resolve<INWItem>();
            obj.Object = @object;
            
            return obj;
        }

        public virtual NWCreature Possessor => NWCreature.Wrap(_.GetItemPossessor(Object));

        public virtual int BaseItemType => _.GetBaseItemType(Object);

        public virtual bool IsDroppable
        {
            get => _.GetDroppableFlag(Object) == 1;
            set => _.SetDroppableFlag(Object, value ? 1 : 0);
        }

        public virtual bool IsCursed
        {
            get => _.GetItemCursedFlag(Object) == 1;
            set => _.SetItemCursedFlag(Object, value ? 1 : 0);
        }

        public virtual bool IsStolen
        {
            get => _.GetStolenFlag(Object) == 1;
            set => _.SetStolenFlag(Object, value ? 1 : 0);
        }
        
        public virtual bool IsIdentified
        {
            get => _.GetIdentified(Object) == 1;
            set => _.SetIdentified(Object, value ? 1 : 0);
        }
        public virtual int AC => _.GetItemACValue(Object);

        public virtual int Charges
        {
            get => _.GetItemCharges(Object);
            set => _.SetItemCharges(Object, value);
        }

        public virtual int ReduceCharges(int reduceBy = 1)
        {
            Charges = Charges - reduceBy;
            if (Charges < 0) Charges = 0;
            return Charges;
        }

        public virtual int StackSize
        {
            get => _.GetItemStackSize(Object);
            set => _.SetItemStackSize(Object, value);
        }

        public virtual float Weight => _.GetWeight(Object) * 0.1f;

        public virtual List<ItemProperty> ItemProperties
        {
            get
            {
                List<ItemProperty> ips = new List<ItemProperty>();
                ItemProperty ip = _.GetFirstItemProperty(Object);
                while (_.GetIsItemPropertyValid(ip) == TRUE)
                {
                    ips.Add(ip);
                    ip = _.GetNextItemProperty(Object);
                }

                return ips;
            }
        }

        public virtual float MaxDurability
        {
            get => _durability.GetMaxDurability(this);
            set => _durability.SetMaxDurability(this, value);
        }

        public virtual float Durability
        {
            get => _durability.GetDurability(this);
            set => _durability.SetDurability(this, value);
        }

        public virtual int CustomAC
        {
            get => _.GetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_AC");
            set => _.SetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_AC", value);
        }
        public virtual CustomItemType CustomItemType
        {
            get => (CustomItemType)_.GetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_TYPE");
            set => _.SetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_TYPE", (int)value);
        }

        public virtual int RecommendedLevel
        {
            get => _.GetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_TYPE_RECOMMENDED_LEVEL");
            set => _.SetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_TYPE_RECOMMENDED_LEVEL", value);
        }

        public virtual int LoggingBonus
        {
            get => _.GetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_LOGGING_BONUS");
            set => _.SetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_LOGGING_BONUS", value);
        }

        public virtual int MiningBonus
        {
            get => _.GetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_MINING_BONUS");
            set => _.SetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_MINING_BONUS", value);
        }

        public virtual int CastingSpeed
        {
            get => _.GetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_CASTING_SPEED");
            set => _.SetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_CASTING_SPEED", value);
        }

        public virtual int CraftBonusMetalworking
        {
            get => _.GetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_CRAFT_BONUS_METALWORKING");
            set => _.SetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_CRAFT_BONUS_METALWORKING", value);
        }
        public virtual int CraftBonusArmorsmith
        {
            get => _.GetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_CRAFT_BONUS_ARMORSMITH");
            set => _.SetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_CRAFT_BONUS_ARMORSMITH", value);
        }
        public virtual int CraftBonusWeaponsmith
        {
            get => _.GetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_CRAFT_BONUS_WEAPONSMITH");
            set => _.SetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_CRAFT_BONUS_WEAPONSMITH", value);
        }
        public virtual int CraftBonusCooking
        {
            get => _.GetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_CRAFT_BONUS_COOKING");
            set => _.SetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_CRAFT_BONUS_COOKING", value);
        }
        public virtual int CraftBonusWoodworking
        {
            get => _.GetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_CRAFT_BONUS_WOODWORKING");
            set => _.SetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_CRAFT_BONUS_WOODWORKING", value);
        }
        public virtual int AssociatedSkillID
        {
            get => _.GetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_ASSOCIATED_SKILL_ID");
            set => _.SetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_ASSOCIATED_SKILL_ID", value);
        }
        public virtual int CraftTierLevel
        {
            get => _.GetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_CRAFT_TIER_LEVEL");
            set => _.SetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_CRAFT_TIER_LEVEL", value);
        }
        public virtual int HPBonus
        {
            get => _.GetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_HP_BONUS");
            set => _.SetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_HP_BONUS", value);
        }
        public virtual int ManaBonus
        {
            get => _.GetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_MANA_BONUS");
            set => _.SetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_MANA_BONUS", value);
        }


        public virtual void ReduceItemStack()
        {
            int stackSize = _.GetItemStackSize(Object);
            if (stackSize > 1)
            {
                _.SetItemStackSize(Object, stackSize - 1);
            }
            else
            {
                _.DestroyObject(Object);
            }
        }
    }
}
