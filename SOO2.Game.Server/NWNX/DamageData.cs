﻿using SOO2.Game.Server.GameObject;

namespace SOO2.Game.Server.NWNX
{
    public class DamageData
    {
        public NWObject Damager { get; set; }
        public int Bludgeoning { get; set; }
        public int Pierce { get; set; }
        public int Slash { get; set; }
        public int Magical { get; set; }
        public int Acid { get; set; }
        public int Cold { get; set; }
        public int Divine { get; set; }
        public int Electrical { get; set; }
        public int Fire { get; set; }
        public int Negative { get; set; }
        public int Positive { get; set; }
        public int Sonic { get; set; }
        public int Base { get; set; }
    }
}
