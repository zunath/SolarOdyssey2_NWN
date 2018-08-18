using System;
using System.Collections.Generic;
using SOO2.Game.Server.ValueObject;
using SOO2.Game.Server.ValueObject.Dialog;
using SOO2.Game.Server.ValueObject.Skill;

namespace SOO2.Game.Server
{
    public class AppState
    {
        public Dictionary<string, PlayerDialog> PlayerDialogs { get; }
        public Dictionary<int, bool> DialogFilesInUse { get; }
        public Dictionary<string, int> EffectTicks { get; }
        public Dictionary<string, CreatureSkillRegistration> CreatureSkillRegistrations;
        public Dictionary<CasterSpellVO, int> NPCEffects { get; }
        public List<CasterSpellVO> EffectsToRemove { get; }
        public Dictionary<string, Action> ProcessingEvents { get; set; }
        public Dictionary<string, EnmityTable> NPCEnmityTables { get; set; }
        public Dictionary<string, CustomData> CustomObjectData { get; set; } 

        public AppState()
        {
            PlayerDialogs = new Dictionary<string, PlayerDialog>();
            DialogFilesInUse = new Dictionary<int, bool>();
            EffectTicks = new Dictionary<string, int>();
            CreatureSkillRegistrations = new Dictionary<string, CreatureSkillRegistration>();
            NPCEffects = new Dictionary<CasterSpellVO, int>();
            EffectsToRemove = new List<CasterSpellVO>();
            ProcessingEvents = new Dictionary<string, Action>();
            NPCEnmityTables = new Dictionary<string, EnmityTable>();
            CustomObjectData = new Dictionary<string, CustomData>();

            for (int x = 1; x <= Constants.NumberOfDialogs; x++)
            {
                DialogFilesInUse.Add(x, false);
            }
        }

    }
}
