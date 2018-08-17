﻿using System;
using System.Linq;
using Freescape.Game.Server.Data.Contracts;
using Freescape.Game.Server.Data.Entities;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using Freescape.Game.Server.ValueObject.Dialog;
using NWN;
using static NWN.NWScript;
using Object = NWN.Object;

namespace Freescape.Game.Server.Conversation
{
    public class Outfit: ConversationBase
    {
        private readonly IColorTokenService _color;
        private readonly ISerializationService _serialization;
        private readonly IDataContext _db;

        public Outfit(
            INWScript script, 
            IDialogService dialog,
            IColorTokenService color,
            ISerializationService serialization,
            IDataContext db) 
            : base(script, dialog)
        {
            _color = color;
            _db = db;
            _serialization = serialization;
        }

        public override PlayerDialog SetUp(NWPlayer player)
        {
            PlayerDialog dialog = new PlayerDialog("MainPage");

            DialogPage mainPage = new DialogPage(
                "Please select an option.",
                "Save Outfit",
                "Load Outfit",
                "Back"
            );

            DialogPage saveOutfitPage = new DialogPage(
                "Please select a slot to save the outfit in.\n\nRed slots are unused. Green slots contain stored data. Selecting a green slot will overwrite whatever is in that slot."
            );

            DialogPage loadOutfitPage = new DialogPage(
                "Please select an outfit to load."
            );

            dialog.AddPage("MainPage", mainPage);
            dialog.AddPage("SaveOutfitPage", saveOutfitPage);
            dialog.AddPage("LoadOutfitPage", loadOutfitPage);
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
                {
                    switch (responseID)
                    {
                        case 1: // Save Outfit
                            ShowSaveOutfitOptions();
                            ChangePage("SaveOutfitPage");
                            break;
                        case 2: // Load Outfit
                            ShowLoadOutfitOptions();
                            ChangePage("LoadOutfitPage");
                            break;
                        case 3: // Back
                            GetPC().AssignCommand(() => _.ActionStartConversation(GetPC().Object, "x0_skill_ctrap", TRUE, FALSE));
                            break;
                    }
                    break;
                }
                case "SaveOutfitPage":
                {
                    HandleSaveOutfit(responseID);
                    break;
                }
                case "LoadOutfitPage":
                {
                    HandleLoadOutfit(responseID);
                    break;
                }
            }
        }

        private PCOutfit GetPlayerOutfits(NWPlayer oPC)
        {
            return _db.PCOutfits.SingleOrDefault(x => x.PlayerID == oPC.GlobalID);
        }

        private void HandleSaveOutfit(int responseID)
        {
            DialogResponse response = GetResponseByID("SaveOutfitPage", responseID);
            if (response.Text == "Back")
            {
                ChangePage("MainPage");
                return;
            }


            NWPlayer oPC = GetPC();
            NWItem oClothes = NWItem.Wrap(_.GetItemInSlot(INVENTORY_SLOT_CHEST, oPC.Object));
            PCOutfit entity = GetPlayerOutfits(oPC);

            if (entity == null)
            {
                entity = new PCOutfit
                {
                    PlayerID = oPC.GlobalID
                };
            }

            if (!oClothes.IsValid)
            {
                oPC.FloatingText(_color.Red("You do not have clothes equipped"));
                return;
            }

            string clothesData = _serialization.Serialize(oClothes);
            if (responseID == 1) entity.Outfit1 = clothesData;
            else if (responseID == 2) entity.Outfit2 = clothesData;
            else if (responseID == 3) entity.Outfit3 = clothesData;
            else if (responseID == 4) entity.Outfit4 = clothesData;
            else if (responseID == 5) entity.Outfit5 = clothesData;
            else if (responseID == 6) entity.Outfit6 = clothesData;
            else if (responseID == 7) entity.Outfit7 = clothesData;
            else if (responseID == 8) entity.Outfit8 = clothesData;
            else if (responseID == 9) entity.Outfit9 = clothesData;
            else if (responseID == 10) entity.Outfit10 = clothesData;

            _db.SaveChanges();

            ShowSaveOutfitOptions();
        }

        private void HandleLoadOutfit(int responseID)
        {
            DialogResponse response = GetResponseByID("LoadOutfitPage", responseID);

            if (!response.HasCustomData)
            {
                ChangePage("MainPage");
            }
            else
            {
                NWPlayer oPC = GetPC();
                int outfitID = (int)response.CustomData[string.Empty];
                PCOutfit entity = GetPlayerOutfits(GetPC());

                NWPlaceable oTempStorage = NWPlaceable.Wrap(_.GetObjectByTag("OUTFIT_BARREL"));
                NWItem oClothes = oPC.Chest;
                NWItem storedClothes = null;
                oClothes.SetLocalString("TEMP_OUTFIT_UUID", oPC.GlobalID);

                if (outfitID == 1) storedClothes      =  _serialization.DeserializeItem(entity.Outfit1, oTempStorage); 
                else if (outfitID == 2) storedClothes =  _serialization.DeserializeItem(entity.Outfit2, oTempStorage); 
                else if (outfitID == 3) storedClothes =  _serialization.DeserializeItem(entity.Outfit3, oTempStorage); 
                else if (outfitID == 4) storedClothes =  _serialization.DeserializeItem(entity.Outfit4, oTempStorage); 
                else if (outfitID == 5) storedClothes =  _serialization.DeserializeItem(entity.Outfit5, oTempStorage); 
                else if (outfitID == 6) storedClothes =  _serialization.DeserializeItem(entity.Outfit6, oTempStorage); 
                else if (outfitID == 7) storedClothes =  _serialization.DeserializeItem(entity.Outfit7, oTempStorage); 
                else if (outfitID == 8) storedClothes =  _serialization.DeserializeItem(entity.Outfit8, oTempStorage); 
                else if (outfitID == 9) storedClothes =  _serialization.DeserializeItem(entity.Outfit9, oTempStorage); 
                else if (outfitID == 10) storedClothes = _serialization.DeserializeItem(entity.Outfit10, oTempStorage);

                if (storedClothes == null) throw new Exception("Unable to locate stored clothes.");

                Object oCopy = _.CopyItem(oClothes.Object, oTempStorage.Object, TRUE);
                oCopy = _.CopyItemAndModify(oCopy, ITEM_APPR_TYPE_ARMOR_MODEL, ITEM_APPR_ARMOR_MODEL_LBICEP, _.GetItemAppearance(storedClothes.Object, ITEM_APPR_TYPE_ARMOR_MODEL, ITEM_APPR_ARMOR_MODEL_LBICEP), TRUE);
                oCopy = _.CopyItemAndModify(oCopy, ITEM_APPR_TYPE_ARMOR_COLOR, ITEM_APPR_ARMOR_MODEL_LBICEP, _.GetItemAppearance(storedClothes.Object, ITEM_APPR_TYPE_ARMOR_COLOR, ITEM_APPR_ARMOR_MODEL_LBICEP), TRUE);

                oCopy = _.CopyItemAndModify(oCopy, ITEM_APPR_TYPE_ARMOR_MODEL, ITEM_APPR_ARMOR_MODEL_BELT, _.GetItemAppearance(storedClothes.Object, ITEM_APPR_TYPE_ARMOR_MODEL, ITEM_APPR_ARMOR_MODEL_BELT), TRUE);
                oCopy = _.CopyItemAndModify(oCopy, ITEM_APPR_TYPE_ARMOR_COLOR, ITEM_APPR_ARMOR_MODEL_BELT, _.GetItemAppearance(storedClothes.Object, ITEM_APPR_TYPE_ARMOR_COLOR, ITEM_APPR_ARMOR_MODEL_BELT), TRUE);

                oCopy = _.CopyItemAndModify(oCopy, ITEM_APPR_TYPE_ARMOR_MODEL, ITEM_APPR_ARMOR_MODEL_LFOOT, _.GetItemAppearance(storedClothes.Object, ITEM_APPR_TYPE_ARMOR_MODEL, ITEM_APPR_ARMOR_MODEL_LFOOT), TRUE);
                oCopy = _.CopyItemAndModify(oCopy, ITEM_APPR_TYPE_ARMOR_COLOR, ITEM_APPR_ARMOR_MODEL_LFOOT, _.GetItemAppearance(storedClothes.Object, ITEM_APPR_TYPE_ARMOR_COLOR, ITEM_APPR_ARMOR_MODEL_LFOOT), TRUE);

                oCopy = _.CopyItemAndModify(oCopy, ITEM_APPR_TYPE_ARMOR_MODEL, ITEM_APPR_ARMOR_MODEL_LFOREARM, _.GetItemAppearance(storedClothes.Object, ITEM_APPR_TYPE_ARMOR_MODEL, ITEM_APPR_ARMOR_MODEL_LFOREARM), TRUE);
                oCopy = _.CopyItemAndModify(oCopy, ITEM_APPR_TYPE_ARMOR_COLOR, ITEM_APPR_ARMOR_MODEL_LFOREARM, _.GetItemAppearance(storedClothes.Object, ITEM_APPR_TYPE_ARMOR_COLOR, ITEM_APPR_ARMOR_MODEL_LFOREARM), TRUE);

                oCopy = _.CopyItemAndModify(oCopy, ITEM_APPR_TYPE_ARMOR_MODEL, ITEM_APPR_ARMOR_MODEL_LHAND, _.GetItemAppearance(storedClothes.Object, ITEM_APPR_TYPE_ARMOR_MODEL, ITEM_APPR_ARMOR_MODEL_LHAND), TRUE);
                oCopy = _.CopyItemAndModify(oCopy, ITEM_APPR_TYPE_ARMOR_COLOR, ITEM_APPR_ARMOR_MODEL_LHAND, _.GetItemAppearance(storedClothes.Object, ITEM_APPR_TYPE_ARMOR_COLOR, ITEM_APPR_ARMOR_MODEL_LHAND), TRUE);

                oCopy = _.CopyItemAndModify(oCopy, ITEM_APPR_TYPE_ARMOR_MODEL, ITEM_APPR_ARMOR_MODEL_LSHIN, _.GetItemAppearance(storedClothes.Object, ITEM_APPR_TYPE_ARMOR_MODEL, ITEM_APPR_ARMOR_MODEL_LSHIN), TRUE);
                oCopy = _.CopyItemAndModify(oCopy, ITEM_APPR_TYPE_ARMOR_COLOR, ITEM_APPR_ARMOR_MODEL_LSHIN, _.GetItemAppearance(storedClothes.Object, ITEM_APPR_TYPE_ARMOR_COLOR, ITEM_APPR_ARMOR_MODEL_LSHIN), TRUE);

                oCopy = _.CopyItemAndModify(oCopy, ITEM_APPR_TYPE_ARMOR_MODEL, ITEM_APPR_ARMOR_MODEL_LSHOULDER, _.GetItemAppearance(storedClothes.Object, ITEM_APPR_TYPE_ARMOR_MODEL, ITEM_APPR_ARMOR_MODEL_LSHOULDER), TRUE);
                oCopy = _.CopyItemAndModify(oCopy, ITEM_APPR_TYPE_ARMOR_COLOR, ITEM_APPR_ARMOR_MODEL_LSHOULDER, _.GetItemAppearance(storedClothes.Object, ITEM_APPR_TYPE_ARMOR_COLOR, ITEM_APPR_ARMOR_MODEL_LSHOULDER), TRUE);

                oCopy = _.CopyItemAndModify(oCopy, ITEM_APPR_TYPE_ARMOR_MODEL, ITEM_APPR_ARMOR_MODEL_LTHIGH, _.GetItemAppearance(storedClothes.Object, ITEM_APPR_TYPE_ARMOR_MODEL, ITEM_APPR_ARMOR_MODEL_LTHIGH), TRUE);
                oCopy = _.CopyItemAndModify(oCopy, ITEM_APPR_TYPE_ARMOR_COLOR, ITEM_APPR_ARMOR_MODEL_LTHIGH, _.GetItemAppearance(storedClothes.Object, ITEM_APPR_TYPE_ARMOR_COLOR, ITEM_APPR_ARMOR_MODEL_LTHIGH), TRUE);

                oCopy = _.CopyItemAndModify(oCopy, ITEM_APPR_TYPE_ARMOR_MODEL, ITEM_APPR_ARMOR_MODEL_NECK, _.GetItemAppearance(storedClothes.Object, ITEM_APPR_TYPE_ARMOR_MODEL, ITEM_APPR_ARMOR_MODEL_NECK), TRUE);
                oCopy = _.CopyItemAndModify(oCopy, ITEM_APPR_TYPE_ARMOR_COLOR, ITEM_APPR_ARMOR_MODEL_NECK, _.GetItemAppearance(storedClothes.Object, ITEM_APPR_TYPE_ARMOR_COLOR, ITEM_APPR_ARMOR_MODEL_NECK), TRUE);

                oCopy = _.CopyItemAndModify(oCopy, ITEM_APPR_TYPE_ARMOR_MODEL, ITEM_APPR_ARMOR_MODEL_PELVIS, _.GetItemAppearance(storedClothes.Object, ITEM_APPR_TYPE_ARMOR_MODEL, ITEM_APPR_ARMOR_MODEL_PELVIS), TRUE);
                oCopy = _.CopyItemAndModify(oCopy, ITEM_APPR_TYPE_ARMOR_COLOR, ITEM_APPR_ARMOR_MODEL_PELVIS, _.GetItemAppearance(storedClothes.Object, ITEM_APPR_TYPE_ARMOR_COLOR, ITEM_APPR_ARMOR_MODEL_PELVIS), TRUE);

                oCopy = _.CopyItemAndModify(oCopy, ITEM_APPR_TYPE_ARMOR_MODEL, ITEM_APPR_ARMOR_MODEL_RBICEP, _.GetItemAppearance(storedClothes.Object, ITEM_APPR_TYPE_ARMOR_MODEL, ITEM_APPR_ARMOR_MODEL_RBICEP), TRUE);
                oCopy = _.CopyItemAndModify(oCopy, ITEM_APPR_TYPE_ARMOR_COLOR, ITEM_APPR_ARMOR_MODEL_RBICEP, _.GetItemAppearance(storedClothes.Object, ITEM_APPR_TYPE_ARMOR_COLOR, ITEM_APPR_ARMOR_MODEL_RBICEP), TRUE);

                oCopy = _.CopyItemAndModify(oCopy, ITEM_APPR_TYPE_ARMOR_MODEL, ITEM_APPR_ARMOR_MODEL_RFOOT, _.GetItemAppearance(storedClothes.Object, ITEM_APPR_TYPE_ARMOR_MODEL, ITEM_APPR_ARMOR_MODEL_RFOOT), TRUE);
                oCopy = _.CopyItemAndModify(oCopy, ITEM_APPR_TYPE_ARMOR_COLOR, ITEM_APPR_ARMOR_MODEL_RFOOT, _.GetItemAppearance(storedClothes.Object, ITEM_APPR_TYPE_ARMOR_COLOR, ITEM_APPR_ARMOR_MODEL_RFOOT), TRUE);

                oCopy = _.CopyItemAndModify(oCopy, ITEM_APPR_TYPE_ARMOR_MODEL, ITEM_APPR_ARMOR_MODEL_RFOREARM, _.GetItemAppearance(storedClothes.Object, ITEM_APPR_TYPE_ARMOR_MODEL, ITEM_APPR_ARMOR_MODEL_RFOREARM), TRUE);
                oCopy = _.CopyItemAndModify(oCopy, ITEM_APPR_TYPE_ARMOR_COLOR, ITEM_APPR_ARMOR_MODEL_RFOREARM, _.GetItemAppearance(storedClothes.Object, ITEM_APPR_TYPE_ARMOR_COLOR, ITEM_APPR_ARMOR_MODEL_RFOREARM), TRUE);

                oCopy = _.CopyItemAndModify(oCopy, ITEM_APPR_TYPE_ARMOR_MODEL, ITEM_APPR_ARMOR_MODEL_RHAND, _.GetItemAppearance(storedClothes.Object, ITEM_APPR_TYPE_ARMOR_MODEL, ITEM_APPR_ARMOR_MODEL_RHAND), TRUE);
                oCopy = _.CopyItemAndModify(oCopy, ITEM_APPR_TYPE_ARMOR_COLOR, ITEM_APPR_ARMOR_MODEL_RHAND, _.GetItemAppearance(storedClothes.Object, ITEM_APPR_TYPE_ARMOR_COLOR, ITEM_APPR_ARMOR_MODEL_RHAND), TRUE);

                oCopy = _.CopyItemAndModify(oCopy, ITEM_APPR_TYPE_ARMOR_MODEL, ITEM_APPR_ARMOR_MODEL_ROBE, _.GetItemAppearance(storedClothes.Object, ITEM_APPR_TYPE_ARMOR_MODEL, ITEM_APPR_ARMOR_MODEL_ROBE), TRUE);
                oCopy = _.CopyItemAndModify(oCopy, ITEM_APPR_TYPE_ARMOR_COLOR, ITEM_APPR_ARMOR_MODEL_ROBE, _.GetItemAppearance(storedClothes.Object, ITEM_APPR_TYPE_ARMOR_COLOR, ITEM_APPR_ARMOR_MODEL_ROBE), TRUE);

                oCopy = _.CopyItemAndModify(oCopy, ITEM_APPR_TYPE_ARMOR_MODEL, ITEM_APPR_ARMOR_MODEL_RSHIN, _.GetItemAppearance(storedClothes.Object, ITEM_APPR_TYPE_ARMOR_MODEL, ITEM_APPR_ARMOR_MODEL_RSHIN), TRUE);
                oCopy = _.CopyItemAndModify(oCopy, ITEM_APPR_TYPE_ARMOR_COLOR, ITEM_APPR_ARMOR_MODEL_RSHIN, _.GetItemAppearance(storedClothes.Object, ITEM_APPR_TYPE_ARMOR_COLOR, ITEM_APPR_ARMOR_MODEL_RSHIN), TRUE);

                oCopy = _.CopyItemAndModify(oCopy, ITEM_APPR_TYPE_ARMOR_MODEL, ITEM_APPR_ARMOR_MODEL_RSHOULDER, _.GetItemAppearance(storedClothes.Object, ITEM_APPR_TYPE_ARMOR_MODEL, ITEM_APPR_ARMOR_MODEL_RSHOULDER), TRUE);
                oCopy = _.CopyItemAndModify(oCopy, ITEM_APPR_TYPE_ARMOR_COLOR, ITEM_APPR_ARMOR_MODEL_RSHOULDER, _.GetItemAppearance(storedClothes.Object, ITEM_APPR_TYPE_ARMOR_COLOR, ITEM_APPR_ARMOR_MODEL_RSHOULDER), TRUE);

                oCopy = _.CopyItemAndModify(oCopy, ITEM_APPR_TYPE_ARMOR_MODEL, ITEM_APPR_ARMOR_MODEL_RTHIGH, _.GetItemAppearance(storedClothes.Object, ITEM_APPR_TYPE_ARMOR_MODEL, ITEM_APPR_ARMOR_MODEL_RTHIGH), TRUE);
                oCopy = _.CopyItemAndModify(oCopy, ITEM_APPR_TYPE_ARMOR_COLOR, ITEM_APPR_ARMOR_MODEL_RTHIGH, _.GetItemAppearance(storedClothes.Object, ITEM_APPR_TYPE_ARMOR_COLOR, ITEM_APPR_ARMOR_MODEL_RTHIGH), TRUE);

                oCopy = _.CopyItemAndModify(oCopy, ITEM_APPR_TYPE_ARMOR_MODEL, ITEM_APPR_ARMOR_MODEL_TORSO, _.GetItemAppearance(storedClothes.Object, ITEM_APPR_TYPE_ARMOR_MODEL, ITEM_APPR_ARMOR_MODEL_TORSO), TRUE);
                oCopy = _.CopyItemAndModify(oCopy, ITEM_APPR_TYPE_ARMOR_COLOR, ITEM_APPR_ARMOR_MODEL_TORSO, _.GetItemAppearance(storedClothes.Object, ITEM_APPR_TYPE_ARMOR_COLOR, ITEM_APPR_ARMOR_MODEL_TORSO), TRUE);

                NWItem oFinal = NWItem.Wrap(_.CopyItem(oCopy, oPC.Object, TRUE));
                oFinal.DeleteLocalString("TEMP_OUTFIT_UUID");
                _.DestroyObject(oCopy);
                oClothes.Destroy();
                storedClothes.Destroy();

                oPC.AssignCommand(() => _.ActionEquipItem(oFinal.Object, INVENTORY_SLOT_CHEST));
                
                foreach (NWItem item in oTempStorage.InventoryItems)
                {
                    if (item.GetLocalString("TEMP_OUTFIT_UUID") == oPC.GlobalID)
                    {
                        item.Destroy();
                    }
                }

                ShowLoadOutfitOptions();
            }
        }

        private void ShowSaveOutfitOptions()
        {
            PCOutfit entity = GetPlayerOutfits(GetPC()) ?? new PCOutfit();

            ClearPageResponses("SaveOutfitPage");

            string responseText = entity.Outfit1 == null ? _color.Red("Save in Slot 1") : _color.Green("Save in Slot 1");
            AddResponseToPage("SaveOutfitPage", responseText);

            responseText = entity.Outfit2 == null ? _color.Red("Save in Slot 2") : _color.Green("Save in Slot 2");
            AddResponseToPage("SaveOutfitPage", responseText);

            responseText = entity.Outfit3 == null ? _color.Red("Save in Slot 3") : _color.Green("Save in Slot 3");
            AddResponseToPage("SaveOutfitPage", responseText);

            responseText = entity.Outfit4 == null ? _color.Red("Save in Slot 4") : _color.Green("Save in Slot 4");
            AddResponseToPage("SaveOutfitPage", responseText);

            responseText = entity.Outfit5 == null ? _color.Red("Save in Slot 5") : _color.Green("Save in Slot 5");
            AddResponseToPage("SaveOutfitPage", responseText);

            responseText = entity.Outfit6 == null ? _color.Red("Save in Slot 6") : _color.Green("Save in Slot 6");
            AddResponseToPage("SaveOutfitPage", responseText);

            responseText = entity.Outfit7 == null ? _color.Red("Save in Slot 7") : _color.Green("Save in Slot 7");
            AddResponseToPage("SaveOutfitPage", responseText);

            responseText = entity.Outfit8 == null ? _color.Red("Save in Slot 8") : _color.Green("Save in Slot 8");
            AddResponseToPage("SaveOutfitPage", responseText);

            responseText = entity.Outfit9 == null ? _color.Red("Save in Slot 9") : _color.Green("Save in Slot 9");
            AddResponseToPage("SaveOutfitPage", responseText);

            responseText = entity.Outfit10 == null ? _color.Red("Save in Slot 10") : _color.Green("Save in Slot 10");
            AddResponseToPage("SaveOutfitPage", responseText);

            AddResponseToPage("SaveOutfitPage", "Back");

        }

        private void ShowLoadOutfitOptions()
        {
            PCOutfit entity = GetPlayerOutfits(GetPC()) ?? new PCOutfit();
            ClearPageResponses("LoadOutfitsPage");

            if (entity.Outfit1 != null)
                AddResponseToPage("LoadOutfitsPage", "Load from Slot 1", true, new Tuple<string, dynamic>(string.Empty, 1));
            if (entity.Outfit2 != null)
                AddResponseToPage("LoadOutfitsPage", "Load from Slot 2", true, new Tuple<string, dynamic>(string.Empty, 2));
            if (entity.Outfit3 != null)
                AddResponseToPage("LoadOutfitsPage", "Load from Slot 3", true, new Tuple<string, dynamic>(string.Empty, 3));
            if (entity.Outfit4 != null)
                AddResponseToPage("LoadOutfitsPage", "Load from Slot 4", true, new Tuple<string, dynamic>(string.Empty, 4));
            if (entity.Outfit5 != null)
                AddResponseToPage("LoadOutfitsPage", "Load from Slot 5", true, new Tuple<string, dynamic>(string.Empty, 5));
            if (entity.Outfit6 != null)
                AddResponseToPage("LoadOutfitsPage", "Load from Slot 6", true, new Tuple<string, dynamic>(string.Empty, 6));
            if (entity.Outfit7 != null)
                AddResponseToPage("LoadOutfitsPage", "Load from Slot 7", true, new Tuple<string, dynamic>(string.Empty, 7));
            if (entity.Outfit8 != null)
                AddResponseToPage("LoadOutfitsPage", "Load from Slot 8", true, new Tuple<string, dynamic>(string.Empty, 8));
            if (entity.Outfit9 != null)
                AddResponseToPage("LoadOutfitsPage", "Load from Slot 9", true, new Tuple<string, dynamic>(string.Empty, 9));
            if (entity.Outfit10 != null)
                AddResponseToPage("LoadOutfitsPage", "Load from Slot 10", true, new Tuple<string, dynamic>(string.Empty, 10));

            AddResponseToPage("LoadOutfitsPage", "Back");
        }

        public override void EndDialog()
        {
        }
    }
}
