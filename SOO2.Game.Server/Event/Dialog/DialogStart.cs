﻿using NWN;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Service.Contracts;
using static NWN.NWScript;
using Object = NWN.Object;

namespace SOO2.Game.Server.Event.Dialog
{
    public class DialogStart: IRegisteredEvent
    {
        private readonly INWScript _;
        private readonly IDialogService _dialog;

        public DialogStart(INWScript script, IDialogService dialogService)
        {
            _ = script;
            _dialog = dialogService;
        }

        public bool Run(params object[] args)
        {
            NWPlayer pc = NWPlayer.Wrap(_.GetLastUsedBy());
            if (!pc.IsValid) pc = NWPlayer.Wrap(_.GetPCSpeaker());

            string conversation = _.GetLocalString(Object.OBJECT_SELF, "CONVERSATION");
            
            if (!string.IsNullOrWhiteSpace(conversation))
            {
                int objectType = _.GetObjectType(Object.OBJECT_SELF);
                if (objectType == OBJECT_TYPE_PLACEABLE)
                {
                    NWPlaceable talkTo = NWPlaceable.Wrap(Object.OBJECT_SELF);
                    _dialog.StartConversation(pc, talkTo, conversation);
                }
                else
                {
                    NWCreature talkTo = NWCreature.Wrap(Object.OBJECT_SELF);
                    _dialog.StartConversation(pc, talkTo, conversation);
                }
            }
            else
            {
                _.ActionStartConversation(pc.Object, "", TRUE, FALSE);
            }

            return true;
        }
    }
}
