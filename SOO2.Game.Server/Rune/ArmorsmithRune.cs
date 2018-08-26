﻿using System;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Rune.Contracts;

namespace SOO2.Game.Server.Rune
{
    public class ArmorsmithRune: IRune
    {
        public string CanApply(NWPlayer player, NWItem target, params string[] args)
        {
            if (target.CraftBonusArmorsmith >= 30)
                return "You cannot improve that item's armorsmith bonus any further.";

            return null;
        }

        public void Apply(NWPlayer player, NWItem target, params string[] args)
        {
            int value = Convert.ToInt32(args[0]);
            int newValue = target.CraftBonusArmorsmith + value;
            if (newValue > 30) newValue = 30;
            target.CraftBonusArmorsmith = newValue;
        }

        public string Description(NWPlayer player, NWItem target, params string[] args)
        {
            int value = Convert.ToInt32(args[0]);
            return "Armorsmith +" + value;
        }
    }
}
