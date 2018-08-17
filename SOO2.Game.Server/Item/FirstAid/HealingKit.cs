using System.Linq;
using Freescape.Game.Server.Data.Contracts;
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
    public class HealingKit: IActionItem
    {

        private readonly INWScript _;
        private readonly ISkillService _skill;
        private readonly IRandomService _random;
        private readonly IPerkService _perk;
        private readonly IDataContext _db;

        public HealingKit(INWScript script,
            ISkillService skill,
            IRandomService random,
            IPerkService perk,
            IDataContext db)
        {
            _ = script;
            _skill = skill;
            _random = random;
            _perk = perk;
            _db = db;
        }

        public CustomData StartUseItem(NWCreature user, NWItem item, NWObject target, Location targetLocation)
        {
            user.SendMessage("You begin treating " + target.Name + "'s wounds...");
            return null;
        }

        public void ApplyEffects(NWCreature user, NWItem item, NWObject target, Location targetLocation, CustomData customData)
        {
            target.RemoveEffect(EFFECT_TYPE_REGENERATE);
            PCSkill skill = _skill.GetPCSkill((NWPlayer)user, SkillType.FirstAid);
            int luck = _perk.GetPCPerkLevel((NWPlayer)user, PerkType.Lucky);
            int perkDurationBonus = _perk.GetPCPerkLevel((NWPlayer)user, PerkType.HealingKitExpert) * 6 + (luck * 2);
            float duration = 30.0f + (skill.Rank * 0.4f) + perkDurationBonus;
            int restoreAmount = 1 + item.GetLocalInt("HEALING_BONUS");

            int perkBlastBonus = _perk.GetPCPerkLevel((NWPlayer)user, PerkType.ImmediateImprovement);
            if (perkBlastBonus > 0)
            {
                int blastHeal = restoreAmount * perkBlastBonus;
                if (_random.Random(100) + 1 <= luck / 2)
                {
                    blastHeal *= 2;
                }
                _.ApplyEffectToObject(DURATION_TYPE_INSTANT, _.EffectHeal(blastHeal), target.Object);
            }

            Effect regeneration = _.EffectRegenerate(restoreAmount, 6.0f);
            _.ApplyEffectToObject(DURATION_TYPE_TEMPORARY, regeneration, target.Object, duration);
            user.SendMessage("You successfully treat " + target.Name + "'s wounds.");

            int xp = (int)_skill.CalculateSkillAdjustedXP(100, item.RecommendedLevel, skill.Rank);
            _skill.GiveSkillXP((NWPlayer)user, SkillType.FirstAid, xp);
        }

        public float Seconds(NWCreature user, NWItem item, NWObject target, Location targetLocation, CustomData customData)
        {
            if ( _random.Random(100) + 1 <= _perk.GetPCPerkLevel((NWPlayer)user, PerkType.SpeedyMedic) * 10)
            {
                return 0.1f;
            }

            PCSkill skill = _skill.GetPCSkill((NWPlayer)user, SkillType.FirstAid);
            return 12.0f - (skill.Rank * 0.1f);
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
            PlayerCharacter pcEntity = _db.PlayerCharacters.Single(x => x.PlayerID == user.GlobalID);
            int consumeChance = _perk.GetPCPerkLevel((NWPlayer)user, PerkType.FrugalMedic) * 10;

            if (pcEntity.BackgroundID == (int)BackgroundType.Medic)
            {
                consumeChance += 5;
            }


            return _random.Random(100) + 1 > consumeChance;
        }

        public string IsValidTarget(NWCreature user, NWItem item, NWObject target, Location targetLocation)
        {
            if (_.GetIsPC(target.Object) == FALSE || _.GetIsDM(target.Object) == TRUE)
            {
                return "Only players may be targeted with this item.";
            }

            if (target.CurrentHP >= target.MaxHP)
            {
                return "Your target is not hurt.";
            }

            return null;
        }

        public bool AllowLocationTarget()
        {
            return false;
        }
    }
}
