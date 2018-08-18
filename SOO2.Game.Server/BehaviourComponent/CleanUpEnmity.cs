using System;
using System.Linq;
using FluentBehaviourTree;
using SOO2.Game.Server.BehaviourComponent.Contracts;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Service.Contracts;

namespace SOO2.Game.Server.BehaviourComponent
{
    public class CleanUpEnmity: IBehaviourComponent
    {
        private readonly IEnmityService _enmity;

        public CleanUpEnmity(IEnmityService enmity)
        {
            _enmity = enmity;
        }

        public BehaviourTreeBuilder Build(BehaviourTreeBuilder builder, params object[] args)
        {
            NWCreature self = (NWCreature) args[0];

            return builder.Do("CleanUpEnmity", t =>
            {
                foreach (var enmity in _enmity.GetEnmityTable(self).ToArray())
                {
                    var val = enmity.Value;
                    var target = val.TargetObject;

                    // Remove invalid objects from the enmity table
                    if (target == null ||
                        !val.TargetObject.IsValid ||
                        !target.Area.Equals(self.Area) ||
                        target.IsDead)
                    {
                        _enmity.GetEnmityTable(self).Remove(enmity.Key);
                        continue;
                    }

                    // Reduce volatile enmity every tick
                    _enmity.GetEnmityTable(self)[target.GlobalID].VolatileAmount--;
                }

                return BehaviourTreeStatus.Running;
            });
        }
    }
}
