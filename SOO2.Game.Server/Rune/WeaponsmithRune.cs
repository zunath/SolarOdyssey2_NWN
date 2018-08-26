﻿using System;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Rune.Contracts;

namespace SOO2.Game.Server.Rune
{
    public class WeaponsmithRune : IRune
    {
        public string CanApply(NWPlayer player, NWItem target, params string[] args)
        {
            if (target.CraftBonusWeaponsmith >= 30)
                return "You cannot improve that item's weaponsmith bonus any further.";

            return null;
        }

        public void Apply(NWPlayer player, NWItem target, params string[] args)
        {
            int value = Convert.ToInt32(args[0]);
            int newValue = target.CraftBonusWeaponsmith + value;
            if (newValue > 30) newValue = 30;
            target.CraftBonusWeaponsmith = newValue;
        }

        public string Description(NWPlayer player, NWItem target, params string[] args)
        {
            int value = Convert.ToInt32(args[0]);
            return "Weaponsmith +" + value;
        }
    }
}
