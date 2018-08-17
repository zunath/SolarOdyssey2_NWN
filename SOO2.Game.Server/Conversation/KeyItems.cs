﻿using System;
using System.Collections.Generic;
using System.Linq;
using Freescape.Game.Server.Data.Entities;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using Freescape.Game.Server.ValueObject.Dialog;
using NWN;

namespace Freescape.Game.Server.Conversation
{
    public class KeyItems: ConversationBase
    {
        private readonly IKeyItemService _keyItem;
        private readonly IColorTokenService _color;

        public KeyItems(
            INWScript script, 
            IDialogService dialog,
            IKeyItemService keyItem,
            IColorTokenService color) 
            : base(script, dialog)
        {
            _keyItem = keyItem;
            _color = color;
        }

        public override PlayerDialog SetUp(NWPlayer player)
        {

            PlayerDialog dialog = new PlayerDialog("MainPage");
            DialogPage mainPage = new DialogPage(
                "Select a key item category.",
                "Maps",
                "Quest Items",
                "Documents",
                "Back"
            );

            DialogPage keyItemListPage = new DialogPage(
                "Select a key item."
            );

            dialog.AddPage("MainPage", mainPage);
            dialog.AddPage("KeyItemsListPage", keyItemListPage);
            return dialog;
        }

        public override void Initialize()
        {
        }

        public override void DoAction(NWPlayer player, string pageName, int responseID)
        {
            switch (pageName)
            {
                case "MainPage":
                    switch (responseID)
                    {
                        case 1: // "Maps"
                        case 2: // "Quest Items"
                        case 3: // "Documents"
                            GetPC().SetLocalInt("TEMP_MENU_KEY_ITEM_CATEGORY_ID", responseID);
                            LoadKeyItemsOptions(responseID);
                            break;
                        case 4: // "Back"
                            SwitchConversation("RestMenu");
                            break;
                    }
                    break;
                case "KeyItemsListPage":
                    HandleKeyItemSelection(responseID);
                    break;
            }
        }

        public override void EndDialog()
        {
            ClearTempVariables();
        }

        private void ClearTempVariables()
        {
            GetPC().DeleteLocalString("TEMP_MENU_KEY_ITEM_CATEGORY_ID");
            SetPageHeader("KeyItemsListPage", "Select a key item.");
        }

        private void LoadKeyItemsOptions(int categoryID)
        {
            List<PCKeyItem> items = _keyItem.GetPlayerKeyItemsByCategory(GetPC(), categoryID).ToList();

            ClearPageResponses("KeyItemsListPage");
            foreach (PCKeyItem item in items)
            {
                AddResponseToPage("KeyItemsListPage", item.KeyItem.Name, true,
                    new Tuple<string, dynamic>(string.Empty, item.KeyItemID));
            }
            AddResponseToPage("KeyItemsListPage", "Back");
            ChangePage("KeyItemsListPage");
        }

        private void HandleKeyItemSelection(int responseID)
        {
            DialogResponse response = GetResponseByID(GetCurrentPageName(), responseID);
            if (!response.HasCustomData)
            {
                ClearTempVariables();
                ChangePage("MainPage");
            }
            else
            {
                SetPageHeader("KeyItemsListPage", BuildKeyItemHeader(responseID));
            }
        }

        private string BuildKeyItemHeader(int responseID)
        {
            DialogResponse response = GetResponseByID(GetCurrentPageName(), responseID);
            int keyItemID = (int)response.CustomData[string.Empty];
            KeyItem entity = _keyItem.GetKeyItemByID(keyItemID);

            string header = _color.Green("Key Item: ") + entity.Name + "\n\n";
            header += _color.Green("Description: ") + entity.Description + "\n";

            return header;
        }

    }
}
