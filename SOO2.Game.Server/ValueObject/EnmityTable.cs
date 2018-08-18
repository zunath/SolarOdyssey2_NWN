using System.Collections.Generic;
using SOO2.Game.Server.GameObject;

namespace SOO2.Game.Server.ValueObject
{
    public class EnmityTable: Dictionary<string, Enmity>
    {
        public NWCreature NPCObject { get; }

        public EnmityTable(NWCreature npcObject)
        {
            NPCObject = npcObject;
        }
    }
}
