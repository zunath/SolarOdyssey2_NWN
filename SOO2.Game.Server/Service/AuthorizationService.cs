using System.Linq;
using Freescape.Game.Server.Data.Contracts;
using Freescape.Game.Server.Data.Entities;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using NWN;

namespace Freescape.Game.Server.Service
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
