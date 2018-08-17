using System;
using Freescape.Game.Server.Event.Legacy;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using NWN;
using Object = NWN.Object;

namespace Freescape.Game.Server.Event.Feat
{
    public class OnHitCastSpell: IRegisteredEvent
    {
        private readonly INWScript _;
        private readonly IDurabilityService _durability;
        private readonly IAbilityService _ability;
        private readonly ISkillService _skill;
        private readonly IPerkService _perk;

        public OnHitCastSpell(
            INWScript script,
            IDurabilityService durability,
            IAbilityService ability,
            ISkillService skill,
            IPerkService perk)
        {
            _ = script;
            _durability = durability;
            _ability = ability;
            _skill = skill;
            _perk = perk;
        }

        public bool Run(params object[] args)
        {
            NWPlayer player = NWPlayer.Wrap(Object.OBJECT_SELF);

            if (player.IsValid)
            {
                _durability.OnHitCastSpell(player);
                _ability.OnHitCastSpell(player);
                _skill.OnHitCastSpell(player);
                _perk.OnHitCastSpell(player);
                HandleItemSpecificCastSpell();
            }
            return true;
        }
        
        private void HandleItemSpecificCastSpell()
        {
            NWObject oSpellOrigin = NWObject.Wrap(_.GetSpellCastItem());
            // Item specific
            string script = oSpellOrigin.GetLocalString("JAVA_SCRIPT");

            if (!string.IsNullOrWhiteSpace(script))
            {
                // Remove "Item." prefix if it exists.
                if (script.StartsWith("Item."))
                    script = script.Substring(5);

                App.RunEvent<LegacyJVMEvent>("Item." + script);
            }
        }
    }
}
