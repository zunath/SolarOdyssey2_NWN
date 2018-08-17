using System;
using System.Collections.Generic;
using Freescape.Game.Server.Data.Entities;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using Freescape.Game.Server.ValueObject.Dialog;
using NWN;

namespace Freescape.Game.Server.Conversation
{
    public class Crafting: ConversationBase
    {
        private readonly IColorTokenService _color;
        private readonly ICraftService _craft;

        public Crafting(
            INWScript script, 
            IDialogService dialog,
            IColorTokenService color,
            ICraftService craft) 
            : base(script, dialog)
        {
            _color = color;
            _craft = craft;
        }

        public override PlayerDialog SetUp(NWPlayer player)
        {
            PlayerDialog dialog = new PlayerDialog("MainPage");
            DialogPage mainPage = new DialogPage(
                "Please select a blueprint. Only blueprints you've learned will be displayed here. Learn more blueprints by purchasing crafting perks!",
                "Back"
            );
            DialogPage blueprintPage = new DialogPage(
                "<SET LATER>",
                _color.Green("Examine Item"),
                "Select Blueprint",
                "Back"
            );
            DialogPage blueprintListPage = new DialogPage(
                "Please select a blueprint. Only blueprints you've learned will be displayed here. Learn more blueprints by purchasing crafting perks!",
                "Back"
            );

            dialog.AddPage("MainPage", mainPage);
            dialog.AddPage("BlueprintListPage", blueprintListPage);
            dialog.AddPage("BlueprintPage", blueprintPage);
            return dialog;
        }

        public override void Initialize()
        {
            LoadCategoryResponses();
        }

        public override void DoAction(NWPlayer player, string pageName, int responseID)
        {
            switch (pageName)
            {
                case "MainPage":
                    HandleCategoryResponse(responseID);
                    break;
                case "BlueprintListPage":
                    HandleBlueprintListResponse(responseID);
                    break;
                case "BlueprintPage":
                    HandleBlueprintResponse(responseID);
                    break;
            }
        }

        public override void EndDialog()
        {
        }


        private void LoadCategoryResponses()
        {
            NWPlaceable device = (NWPlaceable)GetDialogTarget();
            int deviceID = device.GetLocalInt("CRAFT_DEVICE_ID");
            List<CraftBlueprintCategory> categories = _craft.GetCategoriesAvailableToPCByDeviceID(GetPC().GlobalID, deviceID);

            ClearPageResponses("MainPage");

            foreach (CraftBlueprintCategory category in categories)
            {
                AddResponseToPage("MainPage", category.Name, category.IsActive, new Tuple<string, dynamic>(string.Empty, category.CraftBlueprintCategoryID));
            }
            
            AddResponseToPage("MainPage", "Back");
        }

        private void LoadBlueprintListPage(int categoryID)
        {
            NWObject device = GetDialogTarget();
            int deviceID = device.GetLocalInt("CRAFT_DEVICE_ID");

            List<CraftBlueprint> blueprints = _craft.GetPCBlueprintsByDeviceAndCategoryID(GetPC().GlobalID, deviceID, categoryID);

            ClearPageResponses("BlueprintListPage");
            foreach (CraftBlueprint bp in blueprints)
            {
                AddResponseToPage("BlueprintListPage", bp.ItemName, bp.IsActive, new Tuple<string, dynamic>(string.Empty, bp.CraftBlueprintID));
            }

            AddResponseToPage("BlueprintListPage", "Back");
        }

        private void LoadBlueprintPage(int blueprintID)
        {
            SetPageHeader("BlueprintPage", _craft.BuildBlueprintHeader(GetPC(), blueprintID));
            GetResponseByID("BlueprintPage", 1).CustomData[string.Empty] = blueprintID;
            GetResponseByID("BlueprintPage", 2).CustomData[string.Empty] = blueprintID;
        }

        private void HandleCategoryResponse(int responseID)
        {
            DialogResponse response = GetResponseByID("MainPage", responseID);
            if (!response.HasCustomData)
            {
                NWPlaceable device = (NWPlaceable)GetDialogTarget();
                GetPC().AssignCommand(() => _.ActionInteractObject(device.Object));
                return;
            }

            int categoryID = (int)response.CustomData[string.Empty];
            LoadBlueprintListPage(categoryID);
            ChangePage("BlueprintListPage");
        }

        private void HandleBlueprintListResponse(int responseID)
        {
            DialogResponse response = GetResponseByID("BlueprintListPage", responseID);
            if (!response.HasCustomData)
            {
                ChangePage("MainPage");
                return;
            }

            int blueprintID = (int)response.CustomData[string.Empty];
            LoadBlueprintPage(blueprintID);
            ChangePage("BlueprintPage");
        }

        private void HandleBlueprintResponse(int responseID)
        {
            DialogResponse response = GetResponseByID("BlueprintPage", responseID);
            int blueprintID;

            switch (responseID)
            {
                case 1: // Examine item
                    blueprintID = (int)response.CustomData[string.Empty];
                    CraftBlueprint entity = _craft.GetBlueprintByID(blueprintID);
                    NWPlaceable tempContainer = NWPlaceable.Wrap(_.GetObjectByTag("craft_temp_store"));
                    NWItem examineItem = NWItem.Wrap(_.CreateItemOnObject(entity.ItemResref, tempContainer.Object));
                    examineItem.Destroy();

                    GetPC().AssignCommand(() => _.ActionExamine(examineItem.Object));
                    break;
                case 2: // Select Blueprint
                    blueprintID = (int)response.CustomData[string.Empty];
                    GetPC().SendMessage("Blueprint selected. Add necessary resources to the device and click the 'Craft Item' option.");

                    NWPlaceable device = (NWPlaceable)GetDialogTarget();
                    device.SetLocalInt("CRAFT_BLUEPRINT_ID", blueprintID);
                    GetPC().AssignCommand(() => _.ActionInteractObject(device.Object));
                    EndConversation();
                    break;
                case 3: // Back
                    ChangePage("BlueprintListPage");
                    break;
            }
        }
    }
}
