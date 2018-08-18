using System;
using System.Linq;
using FluentBehaviourTree;
using SOO2.Game.Server.BehaviourComponent.Contracts;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Service.Contracts;

namespace SOO2.Game.Server.BehaviourComponent
{
    public class ClearInvalidEnmityTargets: IBehaviourComponent
    {
        private readonly IEnmityService _enmity;

        public ClearInvalidEnmityTargets(IEnmityService enmity)
        {
            _enmity = enmity;
        }

        public BehaviourTreeBuilder Build(BehaviourTreeBuilder builder, params object[] args)
        {
            NWCreature self = (NWCreature) args[0];

            return builder.Do("ClearInvalidEnmityTargets", t =>
            {
                var enmityList = _enmity.GetEnmityTable(self)
                    .Where(x => x.Value.TargetObject == null ||
                                !x.Value.TargetObject.Area.Equals(self.Area) ||
                                !x.Value.TargetObject.IsValid);
                foreach (var obj in enmityList.ToArray())
                {
                    _enmity.GetEnmityTable(self).Remove(obj.Key);
                }

                return BehaviourTreeStatus.Running;
            });
        }
    }
}
