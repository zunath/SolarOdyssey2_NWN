using Freescape.Game.Server.GameObject;

namespace Freescape.Game.Server.ValueObject
{
    public class CasterSpellVO
    {
        public NWCreature Caster { get; set; }
        public NWObject Target { get; set; }
        public string EffectName { get; set; }
        public int CustomEffectID { get; set; }
    }

}
