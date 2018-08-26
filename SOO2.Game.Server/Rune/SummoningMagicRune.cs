using System;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Rune.Contracts;

namespace SOO2.Game.Server.Rune
{
    public class SummoningMagicRune : IRune
    {
        public string CanApply(NWPlayer player, NWItem target, params string[] args)
        {
            if (target.SummoningBonus >= 50)
                return "You cannot improve that item's summoning magic bonus any further.";

            return null;
        }

        public void Apply(NWPlayer player, NWItem target, params string[] args)
        {
            int value = Convert.ToInt32(args[0]);
            int newValue = target.SummoningBonus + value;
            if (newValue > 50) newValue = 50;
            target.SummoningBonus = newValue;
        }

        public string Description(NWPlayer player, NWItem target, params string[] args)
        {
            int value = Convert.ToInt32(args[0]);
            return "Summoning Magic +" + value;
        }
    }
}
