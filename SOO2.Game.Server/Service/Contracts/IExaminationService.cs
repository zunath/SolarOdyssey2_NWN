using Freescape.Game.Server.GameObject;

namespace Freescape.Game.Server.Service.Contracts
{
    public interface IExaminationService
    {
        bool OnModuleExamine(NWPlayer examiner, NWObject target);
    }
}
