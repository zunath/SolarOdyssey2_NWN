using SOO2.Game.Server.GameObject;

namespace SOO2.Game.Server.Rune.Contracts
{
    public interface IRune
    {
        string CanApply(NWPlayer player, NWItem target, string value);
        void Apply(NWPlayer player, NWItem target, string value);
        string Description(NWPlayer player, NWItem target, string value);
    }
}
