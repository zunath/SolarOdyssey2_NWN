using System;
using System.Collections.Generic;
using Freescape.Game.Server.Data.Entities;
using Freescape.Game.Server.GameObject.Contracts;
using Freescape.Game.Server.NWNX.Contracts;
using NWN;
using static NWN.NWScript;
using Object = NWN.Object;

namespace Freescape.Game.Server.GameObject
{
    public class NWPlayer : NWCreature, INWPlayer
    {
        public NWPlayer(INWScript script, INWNXCreature nwnxCreature)
            : base(script, nwnxCreature)
        {
        }

        public new static NWPlayer Wrap(Object @object)
        {
            var obj = (NWPlayer)App.Resolve<INWPlayer>();
            obj.Object = @object;

            return obj;
        }
        
        public virtual bool IsBusy
        {
            get => GetLocalInt("IS_BUSY") == 1;
            set => SetLocalInt("IS_BUSY", value ? 1 : 0);
        }


        public virtual List<NWPlayer> GetPartyMembers()
        {
            List<NWPlayer> partyMembers = new List<NWPlayer>();
            Object member = _.GetFirstFactionMember(Object);
            while (_.GetIsObjectValid(member) == TRUE)
            {
                partyMembers.Add(Wrap(member));
                member = _.GetNextFactionMember(Object);
            }

            return partyMembers;
        }

        public virtual PlayerCharacter ToEntity()
        {
            PlayerCharacter entity = new PlayerCharacter
            {
                PlayerID = GlobalID,
                CharacterName = Name,
                HitPoints = CurrentHP,
                LocationAreaTag = _.GetTag(_.GetAreaFromLocation(Location)),
                LocationX = Position.m_X,
                LocationY = Position.m_Y,
                LocationZ = Position.m_Z,
                LocationOrientation = Facing,
                CreateTimestamp = DateTime.UtcNow,
                MaxHunger = 150,
                CurrentHunger = 150,
                CurrentHungerTick = 300,
                UnallocatedSP = 5,
                NextSPResetDate = null,
                ResetTokens = 3,
                NextResetTokenReceiveDate = null,
                HPRegenerationAmount = 1,
                RegenerationTick = 20,
                RegenerationRate = 0,
                VersionNumber = 1,
                MaxMana = 0,
                CurrentMana = 0,
                CurrentManaTick = 20,
                RevivalStoneCount = 3,
                RespawnAreaTag = string.Empty,
                RespawnLocationX = 0.0f,
                RespawnLocationY = 0.0f,
                RespawnLocationZ = 0.0f,
                RespawnLocationOrientation = 0.0f,
                DateLastForcedSPReset = null,
                DateSanctuaryEnds = DateTime.UtcNow + TimeSpan.FromDays(3),
                IsSanctuaryOverrideEnabled = false,
                STRBase = RawStrength, 
                DEXBase = RawDexterity, 
                CONBase = RawConstitution, 
                INTBase = RawIntelligence, 
                WISBase = RawWisdom, 
                CHABase = RawCharisma, 
                TotalSPAcquired = 0,
                DisplayHelmet = true
            };
            
            return entity;
        }
    }
}
