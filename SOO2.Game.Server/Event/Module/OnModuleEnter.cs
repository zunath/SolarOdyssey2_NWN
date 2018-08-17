﻿using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using NWN;
using static NWN.NWScript;
using Object = NWN.Object;

namespace Freescape.Game.Server.Event.Module
{
    internal class OnModuleEnter : IRegisteredEvent
    {
        private readonly INWScript _;
        private readonly IPlayerService _player;
        private readonly ISkillService _skill;
        private readonly IQuestService _quest;
        private readonly IActivityLoggingService _activityLogging;
        private readonly IMapPinService _mapPin;
        private readonly IBackgroundService _background;

        public OnModuleEnter(INWScript script,
            IPlayerService player,
            ISkillService skill,
            IQuestService quest,
            IActivityLoggingService activityLogging,
            IMapPinService mapPin,
            IBackgroundService background)
        {
            _ = script;
            _player = player;
            _skill = skill;
            _quest = quest;
            _activityLogging = activityLogging;
            _mapPin = mapPin;
            _background = background;
        }

        public bool Run(params object[] args)
        {
            NWPlayer player = GetEnteringPlayer();

            _.ExecuteScript("x3_mod_def_enter", Object.OBJECT_SELF);
            _player.InitializePlayer(player);
            _skill.OnModuleEnter();
            _player.LoadCharacter(player);
            _player.ShowMOTD(player);
            ApplyGhostwalk();
            _quest.OnClientEnter();
            _activityLogging.OnModuleClientEnter();
            ApplyScriptEvents(player);
            _mapPin.OnModuleClientEnter();
            _background.OnModuleClientEnter();

            return true;
        }

        private NWPlayer GetEnteringPlayer()
        {
            return NWPlayer.Wrap(_.GetEnteringObject());
        }

        private void ApplyGhostwalk()
        {
            NWPlayer oPC = GetEnteringPlayer();

            if (!oPC.IsPlayer) return;

            Effect eGhostWalk = _.EffectCutsceneGhost();
            eGhostWalk = _.TagEffect(eGhostWalk, "GHOST_WALK");
            _.ApplyEffectToObject(DURATION_TYPE_PERMANENT, eGhostWalk, oPC.Object);

        }
        
        private void ApplyScriptEvents(NWObject oPC)
        {
            // As of 2018-03-28 only the OnDialogue, OnHeartbeat, and OnUserDefined events fire for PCs.
            // The rest are included here for completeness sake.

            //_.SetEventScript(oPC.Object, EVENT_SCRIPT_CREATURE_ON_BLOCKED_BY_DOOR, "pc_on_blocked");
            //_.SetEventScript(oPC.Object, EVENT_SCRIPT_CREATURE_ON_DAMAGED, "pc_on_damaged");
            //_.SetEventScript(oPC.Object, EVENT_SCRIPT_CREATURE_ON_DEATH, "pc_on_death");
            _.SetEventScript(oPC.Object, EVENT_SCRIPT_CREATURE_ON_DIALOGUE, "default");
            //_.SetEventScript(oPC.Object, EVENT_SCRIPT_CREATURE_ON_DISTURBED, "pc_on_disturb");
            //_.SetEventScript(oPC.Object, EVENT_SCRIPT_CREATURE_ON_END_COMBATROUND, "pc_on_endround");
            _.SetEventScript(oPC.Object, EVENT_SCRIPT_CREATURE_ON_HEARTBEAT, "pc_on_heartbeat");
            //_.SetEventScript(oPC.Object, EVENT_SCRIPT_CREATURE_ON_MELEE_ATTACKED, "pc_on_attacked");
            //_.SetEventScript(oPC.Object, EVENT_SCRIPT_CREATURE_ON_NOTICE, "pc_on_notice");
            //_.SetEventScript(oPC.Object, EVENT_SCRIPT_CREATURE_ON_RESTED, "pc_on_rested");
            //_.SetEventScript(oPC.Object, EVENT_SCRIPT_CREATURE_ON_SPAWN_IN, "pc_on_spawn");
            //_.SetEventScript(oPC.Object, EVENT_SCRIPT_CREATURE_ON_SPELLCASTAT, "pc_on_spellcast");
            _.SetEventScript(oPC.Object, EVENT_SCRIPT_CREATURE_ON_USER_DEFINED_EVENT, "pc_on_user");
        }
    }
}
