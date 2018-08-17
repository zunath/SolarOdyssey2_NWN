using System;
using System.Collections.Generic;
using Freescape.Game.Server.GameObject;

namespace Freescape.Game.Server.ValueObject.Skill
{
    public class PlayerSkillRegistration
    {
        public NWPlayer Player { get; private set; }
        private Dictionary<int, PlayerSkillPointTracker> SkillPoints { get; set; }

        public PlayerSkillRegistration(NWPlayer oPC)
        {
            SkillPoints = new Dictionary<int, PlayerSkillPointTracker>();
            Player = oPC;
        }


        public void AddSkillPointRegistration(int skillID, int level)
        {
            PlayerSkillPointTracker tracker;

            if (!SkillPoints.ContainsKey(skillID))
            {
                tracker = new PlayerSkillPointTracker(skillID);
            }
            else tracker = SkillPoints[skillID];

            tracker.Points++;

            // Always take the lowest level.
            if (tracker.RegisteredLevel == -1 || level < tracker.RegisteredLevel)
            {
                tracker.RegisteredLevel = level;
            }

            SkillPoints[skillID] = tracker;
        }

        public List<Tuple<int, PlayerSkillPointTracker>> GetSkillRegistrationPoints()
        {
            List<Tuple<int, PlayerSkillPointTracker>> result = new List<Tuple<int, PlayerSkillPointTracker>>();

            foreach (var sp in SkillPoints)
            {
                Tuple<int, PlayerSkillPointTracker> pair = new Tuple<int, PlayerSkillPointTracker>(sp.Key, sp.Value);
                result.Add(pair);
            }
            
            return result;
        }

        public int GetTotalSkillRegistrationPoints()
        {
            int totalPoints = 0;

            foreach (Tuple<int, PlayerSkillPointTracker> reg in GetSkillRegistrationPoints())
            {
                totalPoints += reg.Item2.Points;
            }

            return totalPoints;
        }
    }
}
