using System;
using NWN;
using SOO2.Game.Server.Bioware.Contracts;
using SOO2.Game.Server.Enumeration;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Rune.Contracts;
using static NWN.NWScript;

namespace SOO2.Game.Server.Rune
{
    public class AbilityScoreRune: IRune
    {
        private readonly INWScript _;
        private readonly IBiowareXP2 _biowareXP2;

        public AbilityScoreRune(INWScript script,
            IBiowareXP2 biowareXP2)
        {
            _ = script;
            _biowareXP2 = biowareXP2;
        }

        private Tuple<int, int> GetExistingIPInfo(NWItem item, int abilityType)
        {
            Tuple<int, int> result = new Tuple<int, int>(0, 0);
            foreach (var ip in item.ItemProperties)
            {
                int type = _.GetItemPropertyType(ip);
                if (type == ITEM_PROPERTY_ABILITY_BONUS)
                {
                    int currentAbilityType = _.GetItemPropertySubType(ip);
                    if (currentAbilityType == abilityType)
                    {
                        int currentValue = _.GetItemPropertyCostTableValue(ip);
                        result = new Tuple<int, int>(abilityType, currentValue);
                        break;
                    }
                }
            }

            return result;
        }

        private Tuple<int, int, string> ParseData(string data)
        {
            string[] split = data.Split(',');
            string strType = split[0];
            int amount = Convert.ToInt32(split[1]);
            int type = -1;

            switch (strType)
            {
                case "STR":
                    type = ABILITY_STRENGTH;
                    break;
                case "CON":
                    type = ABILITY_CONSTITUTION;
                    break;
                case "DEX":
                    type = ABILITY_DEXTERITY;
                    break;
                case "WIS":
                    type = ABILITY_WISDOM;
                    break;
                case "INT":
                    type = ABILITY_INTELLIGENCE;
                    break;
                case "CHA":
                    type = ABILITY_CHARISMA;
                    break;
            }

            return new Tuple<int, int, string>(type, amount, strType + " +" + amount);
        }

        

        public string CanApply(NWPlayer player, NWItem target, string value)
        {
            var parsed = ParseData(value);
            var info = GetExistingIPInfo(target, parsed.Item1);
            return info.Item2 >= 12 ? "You cannot improve that item's stat any further." : null;
        }

        public void Apply(NWPlayer player, NWItem target, string value)
        {
            var data = ParseData(value);
            if (data.Item1 < 0) return;

            var existingValues = GetExistingIPInfo(target, data.Item1);
            int newValue = data.Item2 + existingValues.Item2;
            if (newValue > 12) newValue = 12;
            
            ItemProperty ip = _.ItemPropertyAbilityBonus(data.Item1, newValue);
            _biowareXP2.IPSafeAddItemProperty(target, ip, 0.0f, AddItemPropertyPolicy.ReplaceExisting, true, false);
        }

        public string Description(NWPlayer player, NWItem target, string value)
        {
            var data = ParseData(value);
            if (data.Item1 < 0) return "Invalid";
            return data.Item3;
        }
    }
}
