using System;
using FluentBehaviourTree;
using SOO2.Game.Server.AI.Contracts;

namespace SOO2.Game.Server.Extension
{
    public static class BehaviourTreeBuilderExtensions
    {
        public static BehaviourTreeBuilder Do<T>(this BehaviourTreeBuilder builder, params object[] args)
            where T: IAIComponent
        {
            var component = App.ResolveByInterface<IAIComponent>(typeof(T).ToString());
            return component.Build(builder, args);
        }
    }
}
