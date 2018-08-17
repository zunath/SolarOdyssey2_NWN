using FluentBehaviourTree;

namespace SOO2.Game.Server.Service.Contracts
{
    public interface IBehaviourService
    {
        string RegisterBehaviour(IBehaviourTreeNode node);
        void UnregisterBehaviour(string behaviourID);
    }
}