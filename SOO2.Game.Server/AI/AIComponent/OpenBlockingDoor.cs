using FluentBehaviourTree;
using NWN;
using SOO2.Game.Server.AI.Contracts;
using SOO2.Game.Server.GameObject;
using static NWN.NWScript;

namespace SOO2.Game.Server.AI.AIComponent
{
    public class OpenBlockingDoor: IAIComponent
    {
        private readonly INWScript _;

        public OpenBlockingDoor(INWScript script)
        {
            _ = script;
        }

        public BehaviourTreeBuilder Build(BehaviourTreeBuilder builder, params object[] args)
        {
            return builder.Do("OpenBlockingDoor", t =>
            {
                NWObject door = NWObject.Wrap(_.GetBlockingDoor());
                if (!door.IsValid) return BehaviourTreeStatus.Failure;
                NWCreature self = (NWCreature)args[0];
                self.AssignCommand(() =>
                {
                    if (_.GetIsDoorActionPossible(door.Object, DOOR_ACTION_OPEN) == TRUE)
                    {
                        _.DoDoorAction(door.Object, DOOR_ACTION_OPEN);
                    }
                });
                
                return BehaviourTreeStatus.Running;
            });
        }
    }
}
