using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.ValueObject;

namespace SOO2.Game.Server.Service.Contracts
{
    public interface ILootService
    {
        ItemVO PickRandomItemFromLootTable(int lootTableID);
        void OnCreatureDeath(NWCreature creature);
    }
}
