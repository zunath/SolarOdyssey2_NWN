using Freescape.Game.Server.Data.Entities;
using Freescape.Game.Server.Enumeration;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Item.Contracts;
using Freescape.Game.Server.Service.Contracts;
using Freescape.Game.Server.ValueObject;
using NWN;
using static NWN.NWScript;

namespace Freescape.Game.Server.Item.FirstAid
{
    public class Bandages: IActionItem
    {
        private readonly INWScript _;
        private readonly ISkillService _skill;
        private readonly ICustomEffectService _customEffect;
        private readonly IRandomService _random;
        private readonly IPerkService _perk;

        public Bandages(INWScript script,
            ISkillService skill,
            ICustomEffectService customEffect,
            IRandomService random,
            IPerkService perk)
        {
            _ = script;
            _skill = skill;
            _customEffect = customEffect;
            _random = random;
            _perk = perk;
        }

        public CustomData StartUseItem(NWCreature user, NWItem item, NWObject target, Location targetLocation)
        {
            user.SendMessage("You begin treating " + target.Name + "'s wounds...");
            return null;
        }

        public void ApplyEffects(NWCreature user, NWItem item, NWObject target, Location targetLocation, CustomData customData)
        {
            _customEffect.RemovePCCustomEffect((NWPlayer)target, CustomEffectType.Bleeding);
            _.ApplyEffectToObject(DURATION_TYPE_INSTANT, _.EffectHeal(2), target.Object);
            user.SendMessage("You finish bandaging " + target.Name + "'s wounds.");

            PCSkill skill = _skill.GetPCSkill((NWPlayer)user, SkillType.FirstAid);
            int xp = (int)_skill.CalculateSkillAdjustedXP(100, item.RecommendedLevel, skill.Rank);
            _skill.GiveSkillXP((NWPlayer)user, SkillType.FirstAid, xp);
        }

        public float Seconds(NWCreature user, NWItem item, NWObject target, Location targetLocation, CustomData customData)
        {
            if (_random.Random(100) + 1 <= _perk.GetPCPerkLevel((NWPlayer)user, PerkType.SpeedyMedic) * 10)
            {
                return 0.1f;
            }

            PCSkill skill = _skill.GetPCSkill((NWPlayer)user, SkillType.FirstAid);
            float seconds = 6.0f - (skill.Rank * 0.2f);
            if (seconds < 1.0f) seconds = 1.0f;
            return seconds;
        }

        public bool FaceTarget()
        {
            return true;
        }

        public int AnimationID()
        {
            return ANIMATION_LOOPING_GET_MID;
        }

        public float MaxDistance()
        {
            return 3.5f;
        }

        public bool ReducesItemCharge(NWCreature user, NWItem item, NWObject target, Location targetLocation, CustomData customData)
        {
            return true;
        }

        public string IsValidTarget(NWCreature user, NWItem item, NWObject target, Location targetLocation)
        {
            if (_.GetIsPC(target.Object) == FALSE || _.GetIsDM(target.Object) == TRUE)
            {
                return "Only players may be targeted with this item.";
            }

            if (!_customEffect.DoesPCHaveCustomEffect((NWPlayer)target, CustomEffectType.Bleeding))
            {
                return "Your target is not bleeding.";
            }

            return null;
        }

        public bool AllowLocationTarget()
        {
            return false;
        }
    }
}
