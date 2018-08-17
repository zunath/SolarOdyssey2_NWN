using SOO2.Game.Server.Data.Entities;
using SOO2.Game.Server.GameObject;

namespace SOO2.Game.Server.Service.Contracts
{
    public interface IPlayerService
    {
        PlayerCharacter GetPlayerEntity(NWPlayer player);
        PlayerCharacter GetPlayerEntity(string playerID);
        void InitializePlayer(NWPlayer player);
        void LoadCharacter(NWPlayer player);
        void OnAreaEnter();
        void SaveCharacter(NWPlayer player);
        void SaveLocation(NWPlayer player);
        void ShowMOTD(NWPlayer player);
    }
}
