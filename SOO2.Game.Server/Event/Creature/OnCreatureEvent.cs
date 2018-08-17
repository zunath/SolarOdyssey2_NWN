using System;
using Freescape.Game.Server.Creature;
using Freescape.Game.Server.Creature.Contracts;
using Freescape.Game.Server.Enumeration;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using Object = NWN.Object;

namespace Freescape.Game.Server.Event.Creature
{
    public class OnCreatureEvent : IRegisteredEvent
    {
        private NWCreature Self { get; }
        private readonly ISkillService _skill;
        private readonly ILootService _loot;
        private readonly IBehaviourService _behaviour;

        public OnCreatureEvent(ISkillService skill,
            ILootService loot,
            IBehaviourService behaviour)
        {
            Self = NWCreature.Wrap(Object.OBJECT_SELF);
            _skill = skill;
            _loot = loot;
            _behaviour = behaviour;
        }

        public bool Run(params object[] args)
        {
            string creatureScript = Self.GetLocalString("SCRIPT");
            ICreature creature = null;

            if (!string.IsNullOrWhiteSpace(creatureScript) && 
                App.IsKeyRegistered<ICreature>("Creature." + creatureScript))
            {
                creature = App.ResolveByInterface<ICreature>("Creature." + creatureScript);
                if (creature.IgnoreNWNEvents) Self.SetLocalInt("IGNORE_NWN_EVENTS", 1);
                if (creature.IgnoreOnBlocked) Self.SetLocalInt("IGNORE_NWN_ON_BLOCKED_EVENT", 1);
                if (creature.IgnoreOnCombatRoundEnd) Self.SetLocalInt("IGNORE_NWN_ON_COMBAT_ROUND_END_EVENT", 1);
                if (creature.IgnoreOnConversation) Self.SetLocalInt("IGNORE_NWN_ON_CONVERSATION_EVENT", 1);
                if (creature.IgnoreOnDamaged) Self.SetLocalInt("IGNORE_NWN_ON_DAMAGED_EVENT", 1);
                if (creature.IgnoreOnDeath) Self.SetLocalInt("IGNORE_NWN_ON_DEATH_EVENT", 1);
                if (creature.IgnoreOnDisturbed) Self.SetLocalInt("IGNORE_NWN_ON_DISTURBED_EVENT", 1);
                if (creature.IgnoreOnHeartbeat) Self.SetLocalInt("IGNORE_NWN_ON_HEARTBEAT_EVENT", 1);
                if (creature.IgnoreOnPerception) Self.SetLocalInt("IGNORE_NWN_ON_PERCEPTION_EVENT", 1);
                if (creature.IgnoreOnPhysicalAttacked) Self.SetLocalInt("IGNORE_NWN_ON_PHYSICAL_ATTACKED_EVENT", 1);
                if (creature.IgnoreOnRested) Self.SetLocalInt("IGNORE_NWN_ON_RESTED_EVENT", 1);
                if (creature.IgnoreOnSpawn) Self.SetLocalInt("IGNORE_NWN_ON_SPAWN_EVENT", 1);
                if (creature.IgnoreOnSpellCastAt) Self.SetLocalInt("IGNORE_NWN_ON_SPELL_CAST_AT_EVENT", 1);
                if (creature.IgnoreOnUserDefined) Self.SetLocalInt("IGNORE_NWN_ON_USER_DEFINED_EVENT", 1);
            }

            CreatureEventType type = (CreatureEventType)args[0];
            switch (type)
            {
                case CreatureEventType.OnPhysicalAttacked:
                    creature?.OnPhysicalAttacked();
                    break;
                case CreatureEventType.OnBlocked:
                    creature?.OnBlocked();
                    break;
                case CreatureEventType.OnConversation:
                    creature?.OnConversation();
                    break;
                case CreatureEventType.OnDamaged:
                    creature?.OnDamaged();
                    break;
                case CreatureEventType.OnDeath:
                    _skill.OnCreatureDeath(Self);
                    _loot.OnCreatureDeath(Self);

                    if (creature != null)
                    {
                        string behaviourID = Self.GetLocalString("REGISTERED_BEHAVIOUR_ID");
                        if (!string.IsNullOrWhiteSpace(behaviourID))
                        {
                            _behaviour.UnregisterBehaviour(behaviourID);
                            Self.DeleteLocalString("REGISTERED_BEHAVIOUR_ID");
                        }

                        creature.OnDeath();

                    }
                    break;
                case CreatureEventType.OnDisturbed:
                    creature?.OnDisturbed();
                    break;
                case CreatureEventType.OnHeartbeat:
                    creature?.OnHeartbeat();
                    break;
                case CreatureEventType.OnPerception:
                    creature?.OnPerception();
                    break;
                case CreatureEventType.OnRested:
                    creature?.OnRested();
                    break;
                case CreatureEventType.OnCombatRoundEnd:
                    creature?.OnCombatRoundEnd();
                    break;
                case CreatureEventType.OnSpawn:
                    if (creature != null)
                    {
                        if (creature.Behaviour != null)
                        {
                            string behaviourID = _behaviour.RegisterBehaviour(creature.Behaviour);
                            Self.SetLocalString("REGISTERED_BEHAVIOUR_ID", behaviourID);
                        }

                        creature.OnSpawn();

                    }
                    break;
                case CreatureEventType.OnSpellCastAt:
                    creature?.OnSpellCastAt();
                    break;
                case CreatureEventType.OnUserDefined:
                    creature?.OnUserDefined();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return true;
        }
    }
}
