using NWN;
using SOO2.Game.Server.ChatCommand.Contracts;
using SOO2.Game.Server.Enumeration;
using SOO2.Game.Server.GameObject;
using static NWN.NWScript;

namespace SOO2.Game.Server.ChatCommand
{
    [CommandDetails("Revives & heals you to full.", CommandPermissionType.DM)]
    public class Rez: IChatCommand
    {
        private readonly INWScript _;

        public Rez(INWScript script)
        {
            _ = script;
        }

        public void DoAction(NWPlayer user, params string[] args)
        {
            if (user.IsDead)
            {
                _.ApplyEffectToObject(DURATION_TYPE_INSTANT, _.EffectResurrection(), user.Object);
            }

            _.ApplyEffectToObject(DURATION_TYPE_INSTANT, _.EffectHeal(999), user.Object);

        }
    }
}
