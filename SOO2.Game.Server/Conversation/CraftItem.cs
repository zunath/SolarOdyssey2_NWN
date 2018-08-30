using NWN;
using SOO2.Game.Server.Enumeration;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Service.Contracts;
using SOO2.Game.Server.ValueObject;
using SOO2.Game.Server.ValueObject.Dialog;
using static NWN.NWScript;

namespace SOO2.Game.Server.Conversation
{
    public class CraftItem: ConversationBase
    {
        private readonly ICraftService _craft;
        private readonly IColorTokenService _color;

        public CraftItem(
            INWScript script, 
            IDialogService dialog,
            ICraftService craft,
            IColorTokenService color) 
            : base(script, dialog)
        {
            _craft = craft;
            _color = color;
        }

        public override PlayerDialog SetUp(NWPlayer player)
        {
            PlayerDialog dialog = new PlayerDialog("MainPage");
            DialogPage mainPage = new DialogPage();
            
            dialog.AddPage("MainPage", mainPage);
            return dialog;
        }

        public override void Initialize()
        {
            var model = _craft.GetPlayerCraftingData(GetPC());
            var device = GetDevice();

            // Entering the conversation for the first time from the blueprint selection menu.
            if (model.BlueprintID <= 0)
            {
                model.BlueprintID = GetPC().GetLocalInt("CRAFT_BLUEPRINT_ID");
                model.Blueprint = _craft.GetBlueprintByID(model.BlueprintID);
                GetDevice().IsLocked = true;
            }
            // Otherwise returning from accessing the device's inventory.
            else
            {
                model.Access = CraftingAccessType.None;

                _.SetEventScript(device.Object, EVENT_SCRIPT_PLACEABLE_ON_USED, "jvm_script_1");
                _.SetEventScript(device.Object, EVENT_SCRIPT_PLACEABLE_ON_OPEN, string.Empty);
                _.SetEventScript(device.Object, EVENT_SCRIPT_PLACEABLE_ON_CLOSED, string.Empty);
                _.SetEventScript(device.Object, EVENT_SCRIPT_PLACEABLE_ON_INVENTORYDISTURBED, string.Empty);
            }

            BuildMainPageHeader();
            BuildMainPageOptions();
        }

        public override void DoAction(NWPlayer player, string pageName, int responseID)
        {
            switch (pageName)
            {
                case "MainPage":
                    HandleMainPageResponses(responseID);
                    break;
            }
            
        }

        public override void EndDialog()
        {
            var model = _craft.GetPlayerCraftingData(GetPC());
            if (model.IsAccessingStorage) return;

            ClearModel();
        }
        
        private NWPlaceable GetDevice()
        {
            return NWPlaceable.Wrap(GetDialogTarget().Object);
        }

        private void ClearModel()
        {
            var model = _craft.GetPlayerCraftingData(GetPC());

            foreach (var item in model.MainComponents)
            {
                _.CopyItem(item.Object, GetPC().Object, TRUE);
                item.Destroy();
            }
            foreach (var item in model.SecondaryComponents)
            {
                _.CopyItem(item.Object, GetPC().Object, TRUE);
                item.Destroy();
            }
            foreach (var item in model.TertiaryComponents)
            {
                _.CopyItem(item.Object, GetPC().Object, TRUE);
                item.Destroy();
            }
            foreach (var item in model.EnhancementComponents)
            {
                _.CopyItem(item.Object, GetPC().Object, TRUE);
                item.Destroy();
            }
            
            GetPC().Data.Remove("CRAFTING_MODEL");
            GetPC().DeleteLocalInt("CRAFT_BLUEPRINT_ID");
        }

        private void BuildMainPageHeader()
        {
            var model = _craft.GetPlayerCraftingData(GetPC());
            var bp = model.Blueprint;
            
            string header = _color.Green("Blueprint: ") + bp.Quantity + "x " + bp.ItemName + "\n";
            header += _color.Green("Required Components: ") + "\n\n";
            header += _color.Green("Main: ") + bp.MainMinimum + "x " + bp.MainComponentType.Name + "\n";

            if (bp.SecondaryMinimum > 0 && bp.SecondaryComponentTypeID > 0)
            {
                header += _color.Green("Secondary: ") + bp.SecondaryMinimum + "x " + bp.SecondaryComponentType.Name + "\n";
            }
            if (bp.TertiaryMinimum > 0 && bp.TertiaryComponentTypeID > 0)
            {
                header += _color.Green("Tertiary: ") + bp.TertiaryMinimum + "x " + bp.TertiaryComponentType.Name + "\n";
            }

            
            header += "\n" + _color.Green("Your components:") + "\n\n";
            if (!model.HasPlayerComponents) header += "No components selected yet!";
            else
            {
                foreach (var item in model.MainComponents)
                {
                    header += item.Name + "\n";
                }
                foreach (var item in model.SecondaryComponents)
                {
                    header += item.Name + "\n";
                }
                foreach (var item in model.TertiaryComponents)
                {
                    header += item.Name + "\n";
                }
                foreach (var item in model.EnhancementComponents)
                {
                    header += item.Name + "\n";
                }
            }

            SetPageHeader("MainPage", header);
        }

        private void BuildMainPageOptions()
        {
            var model = _craft.GetPlayerCraftingData(GetPC());

            AddResponseToPage("MainPage", "Create Item", model.CanBuildItem);
            AddResponseToPage("MainPage", "Select Main Components");
            AddResponseToPage("MainPage", "Select Secondary Components", model.Blueprint.SecondaryMinimum > 0);
            AddResponseToPage("MainPage", "Select Tertiary Components", model.Blueprint.TertiaryMinimum > 0);

            AddResponseToPage("MainPage", "Change Blueprint");
        }

        private void HandleMainPageResponses(int responseID)
        {
            var model = _craft.GetPlayerCraftingData(GetPC());
            NWPlaceable device = GetDevice();

            switch(responseID)
            {
                case 1: // Create item
                    if(!model.CanBuildItem)
                    {
                        GetPC().FloatingText("You are missing some required components.");
                        return;
                    }

                    _craft.CraftItem(GetPC(), device, model.BlueprintID);
                    break;
                case 2: // Select main components
                    model.Access = CraftingAccessType.MainComponent;
                    OpenDeviceInventory();
                    break;
                case 3: // Select secondary components
                    model.Access = CraftingAccessType.SecondaryComponent;
                    OpenDeviceInventory();
                    break;
                case 4: // Select tertiary components
                    model.Access = CraftingAccessType.TertiaryComponent;
                    OpenDeviceInventory();
                    break;
                case 5: // Back (return to blueprint selection)
                    ClearModel();
                    SwitchConversation("CraftingDevice");
                    break;
            }
        }

        private void OpenDeviceInventory()
        {
            var model = _craft.GetPlayerCraftingData(GetPC());
            NWPlaceable device = GetDevice();
            device.IsLocked = false;
            model.IsAccessingStorage = true;

            _.SetEventScript(device.Object, EVENT_SCRIPT_PLACEABLE_ON_USED, string.Empty);
            _.SetEventScript(device.Object, EVENT_SCRIPT_PLACEABLE_ON_OPEN, "jvm_script_2");
            _.SetEventScript(device.Object, EVENT_SCRIPT_PLACEABLE_ON_CLOSED, "jvm_script_3");
            _.SetEventScript(device.Object, EVENT_SCRIPT_PLACEABLE_ON_INVENTORYDISTURBED, "jvm_script_4");

            device.SetLocalString("JAVA_SCRIPT_2", "Placeable.CraftingDevice.OnOpened");
            device.SetLocalString("JAVA_SCRIPT_3", "Placeable.CraftingDevice.OnClosed");
            device.SetLocalString("JAVA_SCRIPT_4", "Placeable.CraftingDevice.OnDisturbed");

            GetPC().AssignCommand(() => _.ActionInteractObject(device.Object));
            EndConversation();
        }

    }
}
