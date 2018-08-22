using FluentBehaviourTree;

namespace SOO2.Game.Server.AI.Contracts
{
    public interface IAIComponent
    {
        BehaviourTreeBuilder Build(BehaviourTreeBuilder builder, params object[] args);
    }
}
