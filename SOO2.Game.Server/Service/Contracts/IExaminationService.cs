using SOO2.Game.Server.GameObject;

namespace SOO2.Game.Server.Service.Contracts
{
    public interface IExaminationService
    {
        bool OnModuleExamine(NWPlayer examiner, NWObject target);
    }
}
