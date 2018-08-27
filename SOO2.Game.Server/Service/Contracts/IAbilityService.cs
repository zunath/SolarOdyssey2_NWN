using SOO2.Game.Server.Data.Entities;
using SOO2.Game.Server.GameObject;

namespace SOO2.Game.Server.Service.Contracts
{
    public interface IAbilityService
    {
        void OnHitCastSpell(NWPlayer oPC);
        void OnModuleUseFeat();
        void RestoreMana(NWPlayer oPC, int amount);
        PlayerCharacter RestoreMana(NWPlayer oPC, int amount, PlayerCharacter entity);
        void OnModuleApplyDamage();
    }
}
