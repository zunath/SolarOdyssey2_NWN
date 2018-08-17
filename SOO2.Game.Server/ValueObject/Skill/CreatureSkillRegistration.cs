using System.Collections.Generic;
using System.Linq;
using Freescape.Game.Server.GameObject;

namespace Freescape.Game.Server.ValueObject.Skill
{
    public class CreatureSkillRegistration
    {
        public string CreatureID { get; set; }
        public Dictionary<string, PlayerSkillRegistration> Registrations { get; set; }

        public CreatureSkillRegistration(string creatureID)
        {
            CreatureID = creatureID;
            Registrations = new Dictionary<string, PlayerSkillRegistration>();
        }

        private PlayerSkillRegistration GetRegistration(NWPlayer oPC)
        {
            if (Registrations.ContainsKey(oPC.GlobalID))
            {
                return Registrations[oPC.GlobalID];
            }
            else
            {
                PlayerSkillRegistration reg = new PlayerSkillRegistration(oPC);
                Registrations[oPC.GlobalID] = reg;
                return reg;
            }
        }

        public void AddSkillRegistrationPoint(NWPlayer oPC, int skillID, int level)
        {
            PlayerSkillRegistration reg = GetRegistration(oPC);
            reg.AddSkillPointRegistration(skillID, level);
        }

        public List<PlayerSkillRegistration> GetAllRegistrations()
        {
            return Registrations.Values.ToList();
        }

        public bool IsRegistrationEmpty()
        {
            return Registrations.Count <= 0;
        }

        public void RemovePlayerRegistration(NWPlayer oPC)
        {
            if (Registrations.ContainsKey(oPC.GlobalID))
            {
                Registrations.Remove(oPC.GlobalID);
            }
            
        }
    }
}
