using System.Collections.Generic;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using NWN;
using static NWN.NWScript;

namespace Freescape.Game.Server.Service
{
    public class EffectTrackerService : IEffectTrackerService
    {
        private readonly INWScript _;
        private readonly AppState _state;

        public EffectTrackerService(INWScript script, AppState state)
        {
            _ = script;
            _state = state;
        }

        public void ProcessPCEffects(NWPlayer oPC)
        {
            HashSet<string> foundIDs = new HashSet<string>();

            foreach (Effect effect in oPC.Effects)
            {
                string pcUUID = oPC.GlobalID;
                string effectKey = pcUUID + "_" + _.GetEffectType(effect);

                if (_.GetEffectDurationType(effect) != DURATION_TYPE_PERMANENT) continue;
                if (!string.IsNullOrWhiteSpace(_.GetEffectTag(effect))) continue;

                int ticks = _state.EffectTicks.ContainsKey(effectKey) ? _state.EffectTicks[effectKey] : 40;
                ticks--;

                if (ticks <= 0)
                {
                    _.RemoveEffect(oPC.Object, effect);
                    _state.EffectTicks.Remove(effectKey);
                }
                else
                {
                    foundIDs.Add(effectKey);
                    _state.EffectTicks[effectKey] = ticks;
                }
            }

            foreach (var effect in _state.EffectTicks)
            {
                if (!foundIDs.Contains(effect.Key))
                {
                    _state.EffectTicks.Remove(effect.Key);
                }
            }
        }
}
}
