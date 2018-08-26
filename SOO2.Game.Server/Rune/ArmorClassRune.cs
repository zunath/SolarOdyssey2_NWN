using System;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Rune.Contracts;

namespace SOO2.Game.Server.Rune
{
    public class ArmorClassRune: IRune
    {
        public string CanApply(NWPlayer player, NWItem target, params string[] args)
        {
            return null;
        }

        public void Apply(NWPlayer player, NWItem target, params string[] args)
        {
            int amount = Convert.ToInt32(args[0]);
            target.CustomAC += amount;
        }

        public string Description(NWPlayer player, NWItem target, params string[] args)
        {
            int amount = Convert.ToInt32(args[0]);
            return "Armor Class +" + amount;
        }
    }
}
