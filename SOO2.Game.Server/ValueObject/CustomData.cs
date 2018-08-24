using System.Collections.Generic;
using SOO2.Game.Server.GameObject;

namespace SOO2.Game.Server.ValueObject
{
    public class CustomData: Dictionary<string, dynamic>
    {
        public NWObject Owner { get; set; }

        public CustomData()
        {
            
        }

        public CustomData(NWObject owner)
        {
            Owner = owner;
        }
    }
}
