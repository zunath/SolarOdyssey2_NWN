using SOO2.Game.Server.GameObject;

namespace SOO2.Game.Server.ValueObject
{
    public class Enmity
    {
        public NWCreature TargetObject { get; }
        public int Amount { get; set; }

        public Enmity(NWCreature targetObject)
        {
            TargetObject = targetObject;
        }
    }
}
