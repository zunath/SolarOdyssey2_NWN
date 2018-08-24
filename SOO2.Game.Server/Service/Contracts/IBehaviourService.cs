using FluentBehaviourTree;
using SOO2.Game.Server.GameObject;

namespace SOO2.Game.Server.Service.Contracts
{
    public interface IBehaviourService
    {
        void RegisterBehaviour(IBehaviourTreeNode node, NWCreature creature);
    }
}