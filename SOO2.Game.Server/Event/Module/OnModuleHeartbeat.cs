﻿using System.Linq;
using NWN;
using SOO2.Game.Server.Data.Contracts;
using SOO2.Game.Server.Data.Entities;
using SOO2.Game.Server.Enumeration;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Service.Contracts;
using static NWN.NWScript;

namespace SOO2.Game.Server.Event.Module
{
    internal class OnModuleHeartbeat : IRegisteredEvent
    {
        private readonly INWScript _;
        private readonly IDataContext _db;
        private readonly IFoodService _food;
        private readonly ICustomEffectService _customEffect;
        private readonly IItemService _item;
        private readonly IEffectTrackerService _effectTracker;
        private readonly IAbilityService _ability;
        private readonly IPerkService _perk;
        
        public OnModuleHeartbeat(INWScript script,
            IDataContext db,
            IFoodService food,
            ICustomEffectService customEffect,
            IItemService item,
            IEffectTrackerService effectTracker,
            IAbilityService ability,
            IPerkService perk)
        {
            _ = script;
            _db = db;
            _food = food;
            _customEffect = customEffect;
            _item = item;
            _effectTracker = effectTracker;
            _ability = ability;
            _perk = perk;
        }

        public bool Run(params object[] args)
        {
            NWPlayer player = NWPlayer.Wrap(_.GetFirstPC());

            while(player.IsValid)
            {
                if (!player.IsDM)
                {
                    _effectTracker.ProcessPCEffects(player);

                    PlayerCharacter entity = _db.PlayerCharacters.SingleOrDefault(x => x.PlayerID == player.GlobalID);

                    if (entity != null)
                    {
                        _food.RunHungerCycle(player, entity);
                        HandleRegenerationTick(player, entity);
                        HandleManaRegenerationTick(player, entity);

                        _db.SaveChanges();
                    }
                }

                player = NWPlayer.Wrap(_.GetNextPC());
            }

            SaveCharacters();
            _customEffect.OnModuleHeartbeat();

            _item.OnModuleHeartbeat();

            return true;
        }
        
        // Export all characters every minute.
        private void SaveCharacters()
        {
            int currentTick = NWModule.Get().GetLocalInt("SAVE_CHARACTERS_TICK") + 1;

            if (currentTick >= 10)
            {
                _.ExportAllCharacters();
                currentTick = 0;
            }

            NWModule.Get().SetLocalInt("SAVE_CHARACTERS_TICK", currentTick);
        }

        private void HandleRegenerationTick(NWPlayer oPC, PlayerCharacter entity)
        {
            entity.RegenerationTick = entity.RegenerationTick - 1;
            int rate = 20;
            int amount = entity.HPRegenerationAmount;

            if (entity.RegenerationTick <= 0)
            {
                if (entity.CurrentHunger <= 20)
                {
                    oPC.SendMessage("You are hungry and not recovering HP naturally. Eat food to start recovering again.");
                }
                else if (oPC.CurrentHP < oPC.MaxHP)
                {
                    // CON bonus
                    int con = oPC.ConstitutionModifier;
                    if (con > 0)
                    {
                        amount += con;
                    }
                    amount += oPC.EffectiveHPRegenBonus;
                    
                    if (oPC.Chest.CustomItemType == CustomItemType.HeavyArmor)
                    {
                        int sturdinessLevel = _perk.GetPCPerkLevel(oPC, PerkType.Sturdiness);
                        if (sturdinessLevel > 0)
                        {
                            amount += sturdinessLevel + 1;
                        }
                    }
                    _.ApplyEffectToObject(DURATION_TYPE_INSTANT, _.EffectHeal(amount), oPC.Object);
                }

                entity.RegenerationTick = rate;
            }
        }

        private void HandleManaRegenerationTick(NWPlayer oPC, PlayerCharacter entity)
        {
            entity.CurrentManaTick = entity.CurrentManaTick - 1;
            int rate = 20;
            int amount = 1;

            if (entity.CurrentManaTick <= 0)
            {
                if (entity.CurrentHunger <= 20)
                {
                    oPC.SendMessage("You are hungry and not recovering mana naturally. Eat food to start recovering again.");
                }
                else if (entity.CurrentMana < entity.MaxMana)
                {
                    // CHA bonus
                    int cha = oPC.CharismaModifier;
                    if (cha > 0)
                    {
                        amount += cha;
                    }
                    amount += oPC.EffectiveManaRegenBonus;

                    if (oPC.Chest.CustomItemType == CustomItemType.MysticArmor)
                    {
                        int clarityLevel = _perk.GetPCPerkLevel(oPC, PerkType.Clarity);
                        if (clarityLevel > 0)
                        {
                            amount += clarityLevel + 1;
                        }
                    }

                    entity = _ability.RestoreMana(oPC, amount, entity);
                }

                entity.CurrentManaTick = rate;
            }
        }

    }
}
