using FluentBehaviourTree;
using NWN;
using SOO2.Game.Server.AI.AIComponent;
using SOO2.Game.Server.Extension;
using SOO2.Game.Server.Service.Contracts;

namespace SOO2.Game.Server.AI
{
    public class SightAggroBehaviour: StandardBehaviour
    {
        public SightAggroBehaviour(
            BehaviourTreeBuilder builder, 
            INWScript script, 
            IEnmityService enmity, 
            IDialogService dialog) 
            : base(builder, script, enmity, dialog)
        {
        }

        public override BehaviourTreeBuilder Behaviour => 
            base.Behaviour
            .Do<EquipBestMelee>(Self)
            .Do<EquipBestRanged>(Self)
            .Do<AggroTargetBySight>(Self)
            .Do<RandomWalk>(Self);

    }
}
