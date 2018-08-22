using System;
using FluentBehaviourTree;
using NWN;
using SOO2.Game.Server.BehaviourComponent.Contracts;
using SOO2.Game.Server.Bioware.Contracts;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Service.Contracts;
using static NWN.NWScript;

namespace SOO2.Game.Server.BehaviourComponent
{
    public class AggroTargetBySight: IBehaviourComponent
    {
        private readonly INWScript _;
        private readonly IEnmityService _enmity;
        private readonly IBiowarePosition _biowarePos;

        public AggroTargetBySight(INWScript script,
            IEnmityService enmity,
            IBiowarePosition biowarePos)
        {
            _ = script;
            _enmity = enmity;
            _biowarePos = biowarePos;
        }

        public BehaviourTreeBuilder Build(BehaviourTreeBuilder builder, params object[] args)
        {
            return builder.Do("AggroTargetBySight", t =>
            {
                NWCreature self = (NWCreature)args[0];

                if (self.IsInCombat) return BehaviourTreeStatus.Failure;

                float aggroRange = self.GetLocalFloat("AGGRO_RANGE");
                if (aggroRange <= 0.0f) aggroRange = 10.0f;
                Location targetLocation = _.Location(
                    self.Area.Object,
                    _biowarePos.GetChangedPosition(self.Position, aggroRange, self.Facing),
                    self.Facing + 180.0f);
                
                NWCreature creature = NWCreature.Wrap(_.GetFirstObjectInShape(SHAPE_SPELLCYLINDER, aggroRange, targetLocation, TRUE, OBJECT_TYPE_CREATURE, self.Position));
                while (creature.IsValid)
                {
                    if (_.GetIsEnemy(creature.Object, self.Object) == TRUE &&
                        !_enmity.IsOnEnmityTable(self, creature) &&
                        _.GetDistanceBetween(self.Object, creature.Object) <= aggroRange &&
                        !creature.HasAnyEffect(EFFECT_TYPE_INVISIBILITY, EFFECT_TYPE_SANCTUARY))
                    {
                        _enmity.AdjustEnmity(self, creature, 0, 1);
                    }
                    
                    creature = NWCreature.Wrap(_.GetNextObjectInShape(SHAPE_SPELLCYLINDER, aggroRange, targetLocation, TRUE, OBJECT_TYPE_CREATURE, self.Position));
                }

                return BehaviourTreeStatus.Running;
            });
        }
    }
}
