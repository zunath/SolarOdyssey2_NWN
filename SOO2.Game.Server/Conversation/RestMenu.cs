﻿using System.Linq;
using Freescape.Game.Server.Data.Contracts;
using Freescape.Game.Server.Data.Entities;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using Freescape.Game.Server.ValueObject.Dialog;
using NWN;

namespace Freescape.Game.Server.Conversation
{
    internal class RestMenu : ConversationBase
    {
        private readonly IDataContext _db;
        private readonly IColorTokenService _color;
        private readonly ISkillService _skill;
        private readonly IMenuService _menu;

        public RestMenu(INWScript script,
            IDialogService dialog,
            IColorTokenService color,
            IDataContext db,
            ISkillService skill,
            IMenuService menu)
            : base(script, dialog)
        {
            _color = color;
            _db = db;
            _skill = skill;
            _menu = menu;
        }

        public override PlayerDialog SetUp(NWPlayer player)
        {
            PlayerDialog dialog = new PlayerDialog("MainPage");
            DialogPage mainPage = new DialogPage(
                BuildMainPageHeader(player),
                _color.Green("Open Overflow Inventory"),
                "View Skills",
                "View Perks",
                "View Blueprints",
                "Dice Bag",
                "Emote Menu",
                "View Key Items",
                "Modify Clothes",
                "Character Management",
                "Open Trash Can (Destroy Items)");

            dialog.AddPage("MainPage", mainPage);

            return dialog;
        }

        public override void Initialize()
        {
            string playerID = GetPC().GlobalID;
            long overflowCount = _db.PCOverflowItems.Where(x => x.PlayerID == playerID).LongCount();

            if (overflowCount <= 0)
            {
                SetResponseVisible("MainPage", 1, false);
            }

        }

        public override void DoAction(NWPlayer player, string pageName, int responseID)
        {
            switch (pageName)
            {
                case "MainPage":
                    switch (responseID)
                    {
                        // Open Overflow Inventory
                        case 1:
                            NWObject storage = NWObject.Wrap(_.CreateObject(NWScript.OBJECT_TYPE_PLACEABLE, "overflow_storage", player.Location));
                            player.AssignCommand(() => _.ActionInteractObject(storage.Object));
                            break;
                        // View Skills
                        case 2:
                            SwitchConversation("ViewSkills");
                            break;
                        // View Perks
                        case 3:
                            SwitchConversation("ViewPerks");
                            break;
                        // View Blueprints
                        case 4:
                            SwitchConversation("ViewBlueprints");
                            break;
                        // Dice Bag
                        case 5:
                            player.SetLocalObject("dmfi_univ_target", player.Object);
                            player.SetLocalLocation("dmfi_univ_location", player.Location);
                            player.SetLocalString("dmfi_univ_conv", "pc_dicebag");
                            player.AssignCommand(() =>
                            {
                                _.ClearAllActions();
                                _.ActionStartConversation(player.Object, "dmfi_universal", 1, 0);
                            });
                            break;
                        // Emote Menu
                        case 6:
                            player.SetLocalObject("dmfi_univ_target", player.Object);
                            player.SetLocalLocation("dmfi_univ_location", player.Location);
                            player.SetLocalString("dmfi_univ_conv", "pc_emote");
                            player.AssignCommand(() =>
                            {
                                _.ClearAllActions();
                                _.ActionStartConversation(player.Object, "dmfi_universal", 1, 0);
                            });
                            break;
                        // Key Item Categories Page
                        case 7:
                            SwitchConversation("KeyItems");
                            break;
                        // Modify Clothes
                        case 8:
                            player.AssignCommand(() => _.ActionStartConversation(player.Object, "x0_skill_ctrap", 1, 0));
                            break;
                        // Character Management
                        case 9:
                            SwitchConversation("CharacterManagement");
                            break;
                        // Open Trash Can (Destroy Items)
                        case 10:
                            EndConversation();
                            NWPlaceable trashCan = NWPlaceable.Wrap(_.CreateObject(NWScript.OBJECT_TYPE_PLACEABLE, "reo_trash_can", player.Location));

                            player.AssignCommand(() => _.ActionInteractObject(trashCan.Object));
                            _.DelayCommand(0.2f, () => trashCan.IsUseable = false);
                            break;
                    }
                    break;

            }
        }

        public override void EndDialog()
        {
        }

        private string BuildMainPageHeader(NWPlayer player)
        {
            PlayerCharacter playerEntity = _db.PlayerCharacters.Single(x => x.PlayerID == player.GlobalID);
            int totalSkillCount = _db.PCSkills.Where(x => x.PlayerID == player.GlobalID).Sum(s => s.Rank);

            string header = _color.Green("Name: ") + player.Name + "\n\n";
            header += _color.Green("Skill Points: ") + totalSkillCount + " / " + _skill.SkillCap + "\n";
            header += _color.Green("Unallocated SP: ") + playerEntity.UnallocatedSP + "\n";
            header += _color.Green("Hunger: ") + _menu.BuildBar(playerEntity.CurrentHunger, playerEntity.MaxHunger, 100) + "\n";
            header += _color.Green("Mana: ") + _menu.BuildBar(playerEntity.CurrentMana, playerEntity.MaxMana, 100, _color.TokenStart(32, 223, 219)) + "\n";

            return header;
        }


    }
}
