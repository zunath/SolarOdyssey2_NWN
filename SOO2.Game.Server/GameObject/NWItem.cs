using System.Collections.Generic;
using NWN;
using SOO2.Game.Server.Enumeration;
using SOO2.Game.Server.GameObject.Contracts;
using SOO2.Game.Server.Service.Contracts;
using static NWN.NWScript;
using Object = NWN.Object;

namespace SOO2.Game.Server.GameObject
{
    public class NWItem : NWObject, INWItem
    {
        private readonly IDurabilityService _durability;

        public NWItem(INWScript script,
            IDurabilityService durability,
            AppState state)
            : base(script, state)
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
            get
            {
                int armorClass = GetItemPropertyValue((int)CustomItemPropertyType.ArmorClass);
                return armorClass > 0 ? armorClass :
                    _.GetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_AC");
            }
            set => _.SetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_AC", value);
        }
        public virtual CustomItemType CustomItemType
        {
            get
            {
                int itemType = GetItemPropertyValue((int)CustomItemPropertyType.ItemType);
                return itemType > 0 ? (CustomItemType)itemType : 
                    (CustomItemType)_.GetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_TYPE");
            }
            set => _.SetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_TYPE", (int)value);
        }

        public virtual int RecommendedLevel
        {
            get
            {
                int craftBonus = GetItemPropertyValue((int)CustomItemPropertyType.RecommendedLevel);
                return craftBonus > 0 ? craftBonus :
                    _.GetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_TYPE_RECOMMENDED_LEVEL");
            }
            set => _.SetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_TYPE_RECOMMENDED_LEVEL", value);
        }

        public virtual int LoggingBonus
        {
            get
            {
                int craftBonus = GetItemPropertyValue((int)CustomItemPropertyType.LoggingBonus);
                return craftBonus > 0 ? craftBonus :
                    _.GetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_LOGGING_BONUS");
            }
            set => _.SetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_LOGGING_BONUS", value);
        }

        public virtual int MiningBonus
        {
            get
            {
                int craftBonus = GetItemPropertyValue((int)CustomItemPropertyType.MiningBonus);
                return craftBonus > 0 ? craftBonus :
                    _.GetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_MINING_BONUS");
            }
            set => _.SetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_MINING_BONUS", value);
        }

        public virtual int CastingSpeed
        {
            get
            {
                int castingSpeed = GetItemPropertyValue((int)CustomItemPropertyType.CastingSpeed);
                if (castingSpeed <= 0) return _.GetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_CASTING_SPEED");

                if (castingSpeed <= 99) return -castingSpeed;
                return castingSpeed - 99;
            }
            set => _.SetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_CASTING_SPEED", value);
        }

        public virtual int CraftBonusMetalworking
        {
            get
            {
                int craftBonus = GetItemPropertyValue((int)CustomItemPropertyType.CraftBonusMetalworking);
                return craftBonus > 0 ? craftBonus :
                    _.GetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_CRAFT_BONUS_METALWORKING");
            }
            set => _.SetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_CRAFT_BONUS_METALWORKING", value);
        }
        public virtual int CraftBonusArmorsmith
        {
            get
            {
                int craftBonus = GetItemPropertyValue((int)CustomItemPropertyType.CraftBonusArmorsmith);
                return craftBonus > 0 ? craftBonus :
                    _.GetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_CRAFT_BONUS_ARMORSMITH");
            }
            set => _.SetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_CRAFT_BONUS_ARMORSMITH", value);
        }
        public virtual int CraftBonusWeaponsmith
        {
            get
            {
                int craftBonus = GetItemPropertyValue((int)CustomItemPropertyType.CraftBonusWeaponsmith);
                return craftBonus > 0 ? craftBonus :
                    _.GetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_CRAFT_BONUS_WEAPONSMITH");
            }
            set => _.SetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_CRAFT_BONUS_WEAPONSMITH", value);
        }
        public virtual int CraftBonusCooking
        {
            get
            {
                int craftBonus = GetItemPropertyValue((int)CustomItemPropertyType.CraftBonusCooking);
                return craftBonus > 0 ? craftBonus :
                    _.GetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_CRAFT_BONUS_COOKING");
            }
            set => _.SetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_CRAFT_BONUS_COOKING", value);
        }
        public virtual int CraftBonusEngineering
        {
            get
            {
                int craftBonus = GetItemPropertyValue((int)CustomItemPropertyType.CraftBonusEngineering);
                return craftBonus > 0 ? craftBonus :
                    _.GetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_CRAFT_BONUS_ENGINEERING");
            }
            set => _.SetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_CRAFT_BONUS_ENGINEERING", value);
        }
        public virtual SkillType AssociatedSkillType
        {
            get
            {
                int skillType = GetItemPropertyValue((int)CustomItemPropertyType.AssociatedSkill);
                return skillType > 0 ? (SkillType)skillType :
                    (SkillType)_.GetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_ASSOCIATED_SKILL_ID");
            }
            set => _.SetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_ASSOCIATED_SKILL_ID", (int)value);
        }
        public virtual int CraftTierLevel
        {
            get
            {
                int craftTier = GetItemPropertyValue((int)CustomItemPropertyType.CraftTierLevel);
                return craftTier > 0 ? craftTier :
                    _.GetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_CRAFT_TIER_LEVEL");
            }
            set => _.SetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_CRAFT_TIER_LEVEL", value);
        }
        public virtual int HPBonus
        {
            get
            {
                int hpBonus = GetItemPropertyValue((int) CustomItemPropertyType.HPBonus);
                return hpBonus > 0 ? hpBonus :
                    _.GetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_HP_BONUS");
            }
            set => _.SetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_HP_BONUS", value);
        }
        public virtual int ManaBonus
        {
            get
            {
                int manaBonus = GetItemPropertyValue((int)CustomItemPropertyType.ManaBonus);
                return manaBonus > 0 ? manaBonus :
                    _.GetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_MANA_BONUS");
            }
            set => _.SetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_MANA_BONUS", value);
        }

        public virtual int EnmityRate
        {
            get
            {
                int enmityRate = GetItemPropertyValue((int)CustomItemPropertyType.EnmityRate);
                if (enmityRate <= 0) return _.GetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_ENMITY_RATE");

                if (enmityRate <= 50) return -enmityRate;
                return enmityRate - 50;
            }
            set => _.SetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_ENMITY_RATE", value);
        }

        public virtual int EvocationBonus
        {
            get
            {
                int evocationBonus = GetItemPropertyValue((int)CustomItemPropertyType.EvocationBonus);
                return evocationBonus > 0 ? evocationBonus :
                    _.GetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_EVOCATION_BONUS");
            }
            set => _.SetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_EVOCATION_BONUS", value);
        }
        public virtual int AlterationBonus
        {
            get
            {
                int alterationBonus = GetItemPropertyValue((int)CustomItemPropertyType.AlterationBonus);
                return alterationBonus > 0 ? alterationBonus :
                    _.GetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_ALTERATION_BONUS");
            }
            set => _.SetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_ALTERATION_BONUS", value);
        }
        public virtual int SummoningBonus
        {
            get
            {
                int summoningBonus = GetItemPropertyValue((int)CustomItemPropertyType.SummoningBonus);
                return summoningBonus > 0 ? summoningBonus :
                    _.GetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_SUMMONING_BONUS");
            }
            set => _.SetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_SUMMONING_BONUS", value);
        }
        public virtual int LuckBonus
        {
            get
            {
                int luckBonus = GetItemPropertyValue((int)CustomItemPropertyType.LuckBonus);
                return luckBonus > 0 ? luckBonus :
                    _.GetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_LUCK_BONUS");
            }
            set => _.SetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_LUCK_BONUS", value);
        }
        public virtual int MeditateBonus
        {
            get
            {
                int meditateBonus = GetItemPropertyValue((int)CustomItemPropertyType.MeditateBonus);
                return meditateBonus > 0 ? meditateBonus :
                    _.GetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_MEDITATE_BONUS");
            }
            set => _.SetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_MEDITATE_BONUS", value);
        }
        public virtual int FirstAidBonus
        {
            get
            {
                int firstAidBonus = GetItemPropertyValue((int)CustomItemPropertyType.FirstAidBonus);
                return firstAidBonus > 0 ? firstAidBonus :
                    _.GetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_FIRST_AID_BONUS");
            }
            set => _.SetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_FIRST_AID_BONUS", value);
        }
        public virtual int HPRegenBonus
        {
            get
            {
                int hpRegenBonus = GetItemPropertyValue((int)CustomItemPropertyType.HPRegenBonus);
                return hpRegenBonus > 0 ? hpRegenBonus :
                    _.GetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_HP_REGEN_BONUS");
            }
            set => _.SetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_HP_REGEN_BONUS", value);
        }
        public virtual int ManaRegenBonus
        {
            get
            {
                int manaRegenBonus = GetItemPropertyValue((int)CustomItemPropertyType.ManaRegenBonus);
                return manaRegenBonus > 0 ? manaRegenBonus :
                    _.GetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_MANA_REGEN_BONUS");
            }
            set => _.SetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_MANA_REGEN_BONUS", value);
        }
        public virtual int BaseAttackBonus
        {
            get
            {
                int baseAttackBonus = GetItemPropertyValue((int)CustomItemPropertyType.BaseAttackBonus);
                return baseAttackBonus > 0 ? baseAttackBonus :
                    _.GetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_BASE_ATTACK_BONUS");
            }
            set => _.SetLocalInt(Object, "CUSTOM_ITEM_PROPERTY_BASE_ATTACK_BONUS", value);
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

        public virtual bool IsRanged => _.GetWeaponRanged(Object) == 1;


        private int GetItemPropertyValue(int itemPropertyID)
        {
            ItemProperty ip = _.GetFirstItemProperty(Object);
            while (_.GetIsItemPropertyValid(ip) == TRUE)
            {
                if (_.GetItemPropertyType(ip) == itemPropertyID)
                {
                    return _.GetItemPropertyCostTableValue(ip); 
                }

                ip = _.GetNextItemProperty(Object);
            }

            return -1;
        }

    }
}
