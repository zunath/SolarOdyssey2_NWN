using FluentBehaviourTree;

namespace SOO2.Game.Server.BehaviourComponent.Contracts
{
    public interface IBehaviourComponent
    {
        BehaviourTreeBuilder Build(BehaviourTreeBuilder builder, params object[] args);
    }
}
