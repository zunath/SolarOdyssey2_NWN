using NWN;
using SOO2.Game.Server.Enumeration;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Service.Contracts;
using SOO2.Game.Server.ValueObject;
using static NWN.NWScript;

namespace SOO2.Game.Server.Service
{
    public class RuneService : IRuneService
    {
        private readonly INWScript _;
        private readonly IColorTokenService _color;

        public RuneService(INWScript script,
            IColorTokenService color)
        {
            _ = script;
            _color = color;
        }

        public CustomItemPropertyType GetRuneType(NWItem item)
        {
            CustomItemPropertyType ipType = CustomItemPropertyType.Unknown;
            foreach (var ip in item.ItemProperties)
            {
                int type = _.GetItemPropertyType(ip);
                if (type == (int)CustomItemPropertyType.RedRune ||
                    type == (int)CustomItemPropertyType.BlueRune ||
                    type == (int)CustomItemPropertyType.GreenRune ||
                    type == (int)CustomItemPropertyType.YellowRune)
                {
                    ipType = (CustomItemPropertyType)type;
                    break;
                }
            }

            return ipType;
        }

        public RuneSlots GetRuneSlots(NWItem item)
        {
            RuneSlots runeSlots = new RuneSlots();
            foreach (var ip in item.ItemProperties)
            {
                int type = _.GetItemPropertyType(ip);
                switch (type)
                {
                    case (int)CustomItemPropertyType.RunicSlotRed:
                        runeSlots.RedSlots++;
                        break;
                    case (int)CustomItemPropertyType.RunicSlotBlue:
                        runeSlots.YellowSlots++;
                        break;
                    case (int)CustomItemPropertyType.RunicSlotGreen:
                        runeSlots.GreenSlots++;
                        break;
                    case (int)CustomItemPropertyType.RunicSlotYellow:
                        runeSlots.YellowSlots++;
                        break;
                    case (int)CustomItemPropertyType.RunicSlotPrismatic:
                        runeSlots.PrismaticSlots++;
                        break;
                }
            }

            for (int red = 1; red <= runeSlots.RedSlots; red++)
            {
                int runeID = item.GetLocalInt("RUNIC_SLOT_RED_" + red);
                if (runeID > 0)
                    runeSlots.FilledRedSlots++;
            }
            for (int blue = 1; blue <= runeSlots.BlueSlots; blue++)
            {
                int runeID = item.GetLocalInt("RUNIC_SLOT_BLUE_" + blue);
                if (runeID > 0)
                    runeSlots.FilledBlueSlots++;
            }
            for (int green = 1; green <= runeSlots.GreenSlots; green++)
            {
                int runeID = item.GetLocalInt("RUNIC_SLOT_GREEN_" + green);
                if (runeID > 0)
                    runeSlots.FilledGreenSlots++;
            }
            for (int yellow = 1; yellow <= runeSlots.YellowSlots; yellow++)
            {
                int runeID = item.GetLocalInt("RUNIC_SLOT_YELLOW_" + yellow);
                if (runeID > 0)
                    runeSlots.FilledYellowSlots++;
            }
            for (int prismatic = 1; prismatic <= runeSlots.PrismaticSlots; prismatic++)
            {
                int runeID = item.GetLocalInt("RUNIC_SLOT_PRISMATIC_" + prismatic);
                if (runeID > 0)
                    runeSlots.FilledPrismaticSlots++;
            }

            return runeSlots;
        }

        public bool IsRune(NWItem item)
        {
            return GetRuneType(item) != CustomItemPropertyType.Unknown;
        }

        public string PrismaticString()
        {
            return _color.Red("p") + _color.Orange("r") + _color.Yellow("i") + _color.Green("s") + _color.Blue("m") +
                                   _color.LightPurple("a") + _color.Purple("t") + _color.White("i") + _color.Black("c");
        }

        public string OnModuleExamine(string existingDescription, NWPlayer examiner, NWObject examinedObject)
        {
            if (examinedObject.ObjectType != OBJECT_TYPE_ITEM) return existingDescription;
            NWItem examinedItem = NWItem.Wrap(examinedObject.Object);
            string description = string.Empty;
            RuneSlots slot = GetRuneSlots(examinedItem);
            
            for (int red = 1; red <= slot.FilledRedSlots; red++)
            {
                description += _color.Red("Red Slot #" + red + ": ") + examinedItem.GetLocalString("RUNIC_SLOT_RED_DESC_" + red) + "\n";
            }
            for (int blue = 1; blue <= slot.FilledBlueSlots; blue++)
            {
                description += _color.Red("Blue Slot #" + blue + ": ") + examinedItem.GetLocalString("RUNIC_SLOT_BLUE_DESC_" + blue) + "\n";
            }
            for (int green = 1; green <= slot.FilledGreenSlots; green++)
            {
                description += _color.Red("Green Slot #" + green + ": ") + examinedItem.GetLocalString("RUNIC_SLOT_GREEN_DESC_" + green) + "\n";
            }
            for (int yellow = 1; yellow <= slot.FilledYellowSlots; yellow++)
            {
                description += _color.Red("Yellow Slot #" + yellow + ": ") + examinedItem.GetLocalString("RUNIC_SLOT_YELLOW_DESC_" + yellow) + "\n";
            }
            for (int prismatic = 1; prismatic <= slot.FilledPrismaticSlots; prismatic++)
            {
                description += PrismaticString() + " Slot #" + prismatic + ": " + examinedItem.GetLocalString("RUNIC_SLOT_PRISMATIC_DESC_" + prismatic) + "\n";
            }
            
            return existingDescription + "\n" + description;
        }

    }
}
