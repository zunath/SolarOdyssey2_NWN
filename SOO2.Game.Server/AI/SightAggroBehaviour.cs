using FluentBehaviourTree;
using NWN;
using SOO2.Game.Server.AI.AIComponent;
using SOO2.Game.Server.Extension;
using SOO2.Game.Server.NWNX.Contracts;
using SOO2.Game.Server.Service.Contracts;

namespace SOO2.Game.Server.AI
{
    public class SightAggroBehaviour: StandardBehaviour
    {
        public SightAggroBehaviour(
            BehaviourTreeBuilder builder, 
            INWScript script, 
            IEnmityService enmity, 
            IDialogService dialog,
            INWNXObject nwnxObject) 
            : base(builder, script, enmity, dialog, nwnxObject)
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
