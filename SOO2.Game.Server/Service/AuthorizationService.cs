using System.Linq;
using SOO2.Game.Server.Data.Contracts;
using SOO2.Game.Server.Data.Entities;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Service.Contracts;
using NWN;

namespace SOO2.Game.Server.Service
{
    public class AuthorizationService: IAuthorizationService
    {
        private readonly INWScript _;
        private readonly IDataContext _db;

        public AuthorizationService(INWScript script, IDataContext db)
        {
            _ = script;
            _db = db;
        }

        public bool IsPCRegisteredAsDM(NWPlayer player)
        {
            if (player.IsDM) return true;
            if (!player.IsPlayer) return false;

            string cdKey = _.GetPCPublicCDKey(player.Object);

            AuthorizedDM entity = _db.AuthorizedDMs.SingleOrDefault(x => x.CDKey == cdKey && x.IsActive);
            return entity != null;
        }
    }
}
