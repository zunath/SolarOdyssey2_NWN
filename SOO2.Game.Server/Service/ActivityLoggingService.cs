﻿using System;
using System.Linq;
using Freescape.Game.Server.Data.Contracts;
using Freescape.Game.Server.Data.Entities;
using Freescape.Game.Server.Enumeration;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.NWNX.Contracts;
using Freescape.Game.Server.Service.Contracts;
using NWN;

namespace Freescape.Game.Server.Service
{
    public class ActivityLoggingService: IActivityLoggingService
    {
        private readonly INWScript _;
        private readonly IDataContext _db;
        private readonly INWNXChat _nwnxChat;

        public ActivityLoggingService(INWScript script, IDataContext db, INWNXChat nwnxChat)
        {
            _ = script;
            _db = db;
            _nwnxChat = nwnxChat;
        }

        public void OnModuleClientEnter()
        {
            NWPlayer oPC = NWPlayer.Wrap(_.GetEnteringObject());
            string name = oPC.Name;
            string cdKey = _.GetPCPublicCDKey(oPC.Object);
            string account = _.GetPCPlayerName(oPC.Object);
            DateTime now = DateTime.UtcNow;
            string nowString = now.ToString("yyyy-MM-dd hh:mm:ss");

            // CD Key and accounts are stored as local strings on the PC
            // because they cannot be retrieved using NWScript functions
            // on the module OnClientLeave event.

            oPC.SetLocalString("PC_CD_KEY", cdKey);
            oPC.SetLocalString("PC_ACCOUNT", account);

            Console.WriteLine(nowString + ": " + name + " (" + account + "/" + cdKey + ") connected to the server.");

            ClientLogEvent entity = new ClientLogEvent
            {
                AccountName = account,
                CDKey = cdKey,
                ClientLogEventTypeID = 1,
                PlayerID = oPC.IsDM ? null : oPC.GlobalID,
                DateOfEvent = now
            };
            
            _db.ClientLogEvents.Add(entity);
            _db.SaveChanges();
        }

        public void OnModuleClientLeave()
        {
            NWPlayer oPC = NWPlayer.Wrap(_.GetExitingObject());
            string name = oPC.Name;
            string cdKey = oPC.GetLocalString("PC_CD_KEY");
            string account = oPC.GetLocalString("PC_ACCOUNT");
            DateTime now = DateTime.UtcNow;
            string nowString = now.ToString("yyyy-MM-dd hh:mm:ss");

            Console.WriteLine(nowString + ": " + name + " (" + account + "/" + cdKey + ") left the server.");

            ClientLogEvent entity = new ClientLogEvent
            {
                AccountName = account,
                CDKey = cdKey,
                ClientLogEventTypeID = 2,
                PlayerID = oPC.IsDM ? null : oPC.GlobalID,
                DateOfEvent = now
            };

            _db.ClientLogEvents.Add(entity);
            _db.SaveChanges();
        }


        private static int ConvertNWNXChatChannelIDToDatabaseID(int nwnxChatChannelID)
        {
            switch (nwnxChatChannelID)
            {
                case (int)ChatChannel.PlayerTalk:
                case (int)ChatChannel.DMTalk:
                    return 3;
                case (int)ChatChannel.PlayerShout:
                case (int)ChatChannel.DMShout:
                    return 1;
                case (int)ChatChannel.PlayerWhisper:
                case (int)ChatChannel.DMWhisper:
                    return 2;
                case (int)ChatChannel.PlayerTell:
                case (int)ChatChannel.DMTell:
                    return 6;
                case (int)ChatChannel.ServerMessage:
                    return 7;
                case (int)ChatChannel.PlayerParty:
                case (int)ChatChannel.DMParty:
                    return 4;
                default:
                    return 5;
            }
        }
        
        public void OnModuleNWNXChat(NWPlayer sender)
        {
            if (!sender.IsPlayer && !sender.IsDM) return;
            string text = _nwnxChat.GetMessage();
            int mode = _nwnxChat.GetChannel();
            int channel = ConvertNWNXChatChannelIDToDatabaseID(mode);
            NWObject recipient = _nwnxChat.GetTarget();
            ChatChannelsDomain channelEntity = _db.ChatChannelsDomains.Single(x => x.ChatChannelID == channel);

            // Sender - should always have this data.
            string senderCDKey = _.GetPCPublicCDKey(sender.Object);
            string senderAccountName = sender.Name;
            string senderPlayerID = null;
            string senderDMName = null;

            // DMs do not have PlayerIDs so store their name in another field.
            if (sender.IsDM)
                senderDMName = "[DM: " + sender.Name + " (" + senderCDKey + ")]";
            else
                senderPlayerID = sender.GlobalID;

            // Receiver - may or may not have the data.

            string receiverCDKey = null;
            string receiverAccountName = null;
            string receiverPlayerID = null;
            string receiverDMName = null;

            if (recipient.IsValid)
            {
                receiverCDKey =  _.GetPCPublicCDKey(recipient.Object);
                receiverAccountName = recipient.Name;

                // DMs do not have PlayerIDs so store their name in another field.
                if (recipient.IsDM)
                    receiverDMName = "[DM: " + recipient.Name + " (" + senderCDKey + ")]";
                else
                    receiverPlayerID = recipient.GlobalID;
            }

            ChatLog entity = new ChatLog
            {
                Message = text,
                SenderCDKey = senderCDKey,
                SenderAccountName = senderAccountName,
                SenderPlayerID = senderPlayerID,
                SenderDMName = senderDMName,
                ReceiverCDKey = receiverCDKey,
                ReceiverAccountName = receiverAccountName,
                ReceiverPlayerID = receiverPlayerID,
                ReceiverDMName = receiverDMName,
                ChatChannelID = channelEntity.ChatChannelID
            };
            
            _db.ChatLogs.Add(entity);
            _db.SaveChanges();

        }

    }
}
