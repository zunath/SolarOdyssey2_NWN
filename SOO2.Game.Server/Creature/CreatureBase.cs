using FluentBehaviourTree;
using Freescape.Game.Server.Creature.Contracts;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using NWN;

namespace Freescape.Game.Server.Creature
{
    public abstract class CreatureBase: ICreature
    {
        protected NWCreature Self { get; }

        protected CreatureBase()
        {
            Self = NWCreature.Wrap(Object.OBJECT_SELF);
        }

        public virtual IBehaviourTreeNode Behaviour => null;

        public virtual bool IgnoreNWNEvents => false;
        public virtual bool IgnoreOnBlocked => false;
        public virtual bool IgnoreOnCombatRoundEnd => false;
        public virtual bool IgnoreOnConversation => false;
        public virtual bool IgnoreOnDamaged => false;
        public virtual bool IgnoreOnDeath => false;
        public virtual bool IgnoreOnDisturbed => false;
        public virtual bool IgnoreOnHeartbeat => false;
        public virtual bool IgnoreOnPerception => false;
        public virtual bool IgnoreOnPhysicalAttacked => false;
        public virtual bool IgnoreOnRested => false;
        public virtual bool IgnoreOnSpawn => false;
        public virtual bool IgnoreOnSpellCastAt => false;
        public virtual bool IgnoreOnUserDefined => false;


        public virtual void OnBlocked()
        {
        }

        public virtual void OnCombatRoundEnd()
        {
        }

        public virtual void OnConversation()
        {
        }

        public virtual void OnDamaged()
        {
        }

        public virtual void OnDeath()
        {
        }

        public virtual void OnDisturbed()
        {
        }

        public virtual void OnHeartbeat()
        {
        }

        public virtual void OnPerception()
        {
        }

        public virtual void OnPhysicalAttacked()
        {
        }

        public virtual void OnRested()
        {
        }

        public virtual void OnSpawn()
        {
        }

        public virtual void OnSpellCastAt()
        {
        }

        public virtual void OnUserDefined()
        {
        }

    }
}
