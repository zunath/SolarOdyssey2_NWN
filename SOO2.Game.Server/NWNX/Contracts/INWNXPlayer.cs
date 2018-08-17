using Freescape.Game.Server.GameObject;

namespace Freescape.Game.Server.NWNX.Contracts
{
    public interface INWNXPlayer
    {
        void ForcePlaceableExamineWindow(NWPlayer player, NWPlaceable placeable);
        void StopGuiTimingBar(NWPlayer player, string script, int id);
        void StartGuiTimingBar(NWPlayer player, float seconds, string script);
        void StopGuiTimingBar(NWPlayer player, string script);
        void SetAlwaysWalk(NWPlayer player, int bWalk);
        QuickBarSlot NWNX_Player_GetQuickBarSlot(NWPlayer player, int slot);
        void SetQuickBarSlot(NWPlayer player, int slot, QuickBarSlot qbs);
    }
}