using SOO2.Game.Server.Data.Entities;
using SOO2.Game.Server.GameObject;

namespace SOO2.Game.Server.Service.Contracts
{
    public interface IFoodService
    {
        PlayerCharacter RunHungerCycle(NWPlayer pc, PlayerCharacter entity);
        PlayerCharacter ApplyHungerPenalties(PlayerCharacter entity, NWPlayer pc);
        void IncreaseHungerLevel(NWPlayer oPC, int amount, bool isTainted);
        PlayerCharacter DecreaseHungerLevel(PlayerCharacter entity, NWPlayer oPC, int amount);
        void DecreaseHungerLevel(NWPlayer oPC, int amount);
    }
}
