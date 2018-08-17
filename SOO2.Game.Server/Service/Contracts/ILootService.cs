using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.ValueObject;

namespace Freescape.Game.Server.Service.Contracts
{
    public interface ILootService
    {
        ItemVO PickRandomItemFromLootTable(int lootTableID);
        void OnCreatureDeath(NWCreature creature);
    }
}
