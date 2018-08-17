using System;
using System.Collections.Generic;
using Freescape.Game.Server.ValueObject;
using Freescape.Game.Server.ValueObject.Dialog;
using Freescape.Game.Server.ValueObject.Skill;

namespace Freescape.Game.Server
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

        public AppState()
        {
            PlayerDialogs = new Dictionary<string, PlayerDialog>();
            DialogFilesInUse = new Dictionary<int, bool>();
            EffectTicks = new Dictionary<string, int>();
            CreatureSkillRegistrations = new Dictionary<string, CreatureSkillRegistration>();
            NPCEffects = new Dictionary<CasterSpellVO, int>();
            EffectsToRemove = new List<CasterSpellVO>();
            ProcessingEvents = new Dictionary<string, Action>();

            for (int x = 1; x <= Constants.NumberOfDialogs; x++)
            {
                DialogFilesInUse.Add(x, false);
            }
        }

    }
}
