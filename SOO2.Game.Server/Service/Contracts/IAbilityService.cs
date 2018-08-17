using Freescape.Game.Server.Data.Entities;
using Freescape.Game.Server.GameObject;

namespace Freescape.Game.Server.Service.Contracts
{
    public interface IAbilityService
    {
        void OnHitCastSpell(NWPlayer oPC);
        void OnModuleItemActivated();
        void RestoreMana(NWPlayer oPC, int amount);
        PlayerCharacter RestoreMana(NWPlayer oPC, int amount, PlayerCharacter entity);
    }
}
