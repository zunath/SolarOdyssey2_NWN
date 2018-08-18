using System;
using FluentBehaviourTree;
using SOO2.Game.Server.BehaviourComponent.Contracts;

namespace SOO2.Game.Server.Extension
{
    public static class BehaviourTreeBuilderExtensions
    {
        public static BehaviourTreeBuilder Do<T>(this BehaviourTreeBuilder builder, params object[] args)
            where T: IBehaviourComponent
        {
            Console.WriteLine(typeof(T).ToString());
            var component = App.ResolveByInterface<IBehaviourComponent>(typeof(T).ToString());
            return component.Build(builder, args);
        }
    }
}
