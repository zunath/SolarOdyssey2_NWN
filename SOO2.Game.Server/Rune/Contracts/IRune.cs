using SOO2.Game.Server.GameObject;

namespace SOO2.Game.Server.Rune.Contracts
{
    public interface IRune
    {
        string CanApply(NWPlayer player, NWItem target, params string[] args);
        void Apply(NWPlayer player, NWItem target, params string[] args);
        string Description(NWPlayer player, NWItem target, params string[] args);
    }
}
