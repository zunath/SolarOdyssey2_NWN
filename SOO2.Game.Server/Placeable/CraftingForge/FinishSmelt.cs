﻿using Freescape.Game.Server.Data.Entities;
using Freescape.Game.Server.Enumeration;
using Freescape.Game.Server.Event;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using NWN;
using Object = NWN.Object;

namespace Freescape.Game.Server.Placeable.CraftingForge
{
    public class FinishSmelt: IRegisteredEvent
    {
        private readonly INWScript _;
        private readonly ISkillService _skill;
        private readonly ICraftService _craft;
        private readonly IRandomService _random;
        private readonly IPerkService _perk;
        private readonly IFoodService _food;

        public FinishSmelt(INWScript script,
            ISkillService skill,
            ICraftService craft,
            IRandomService random,
            IPerkService perk,
            IFoodService food)
        {
            _ = script;
            _skill = skill;
            _craft = craft;
            _random = random;
            _perk = perk;
            _food = food;
        }

        public bool Run(params object[] args)
        {
            NWPlayer player = NWPlayer.Wrap(Object.OBJECT_SELF);
            NWPlaceable forge = NWPlaceable.Wrap(player.GetLocalObject("FORGE"));
            string resref = forge.GetLocalString("FORGE_ORE");

            forge.DeleteLocalObject("FORGE_USER");
            player.DeleteLocalObject("FORGE");
            forge.DeleteLocalString("FORGE_ORE");
            player.IsBusy = false;

            PCSkill pcSkill = _skill.GetPCSkill(player, SkillType.Metalworking);
            int level = _craft.GetIngotLevel(resref);
            string ingotResref = _craft.GetIngotResref(resref);
            if (pcSkill == null || level < 0 || string.IsNullOrWhiteSpace(ingotResref)) return false;

            int delta = pcSkill.Rank - level;
            int count = 2;

            if (delta > 2) count = delta;
            if (count > 6) count = 6;

            if (_random.Random(100) + 1 <= _perk.GetPCPerkLevel(player, PerkType.Lucky))
            {
                count++;
            }

            if (_random.Random(100) + 1 <= _perk.GetPCPerkLevel(player, PerkType.SmeltingMastery) * 10)
            {
                count++;
            }

            for (int x = 1; x <= count; x++)
            {
                _.CreateItemOnObject(ingotResref, player.Object);
            }

            int xp = (int)_skill.CalculateSkillAdjustedXP(100, level, pcSkill.Rank);
            _skill.GiveSkillXP(player, SkillType.Metalworking, xp);

            if (_random.Random(100) + 1 <= 3)
            {
                _food.DecreaseHungerLevel(player, 1);
            }
            return true;
        }
    }
}
