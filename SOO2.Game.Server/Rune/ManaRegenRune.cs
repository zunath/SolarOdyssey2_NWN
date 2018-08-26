﻿using System;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Rune.Contracts;

namespace SOO2.Game.Server.Rune
{
    public class ManaRegenRune : IRune
    {
        public string CanApply(NWPlayer player, NWItem target, params string[] args)
        {
            if (target.ManaRegenBonus >= 20)
                return "You cannot improve that item's mana regen bonus any further.";

            return null;
        }

        public void Apply(NWPlayer player, NWItem target, params string[] args)
        {
            int value = Convert.ToInt32(args[0]);
            int newValue = target.ManaRegenBonus + value;
            if (newValue > 20) newValue = 20;
            target.ManaRegenBonus = newValue;
        }

        public string Description(NWPlayer player, NWItem target, params string[] args)
        {
            int value = Convert.ToInt32(args[0]);
            return "Mana Regen +" + value;
        }
    }
}
