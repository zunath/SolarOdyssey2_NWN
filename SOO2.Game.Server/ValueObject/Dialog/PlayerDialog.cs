﻿using System.Collections.Generic;
using Freescape.Game.Server.GameObject;

namespace Freescape.Game.Server.ValueObject.Dialog
{
    public class PlayerDialog
    {
        private Dictionary<string, DialogPage> Pages { get; set; }
        public string CurrentPageName { get; set; }
        public int PageOffset { get; set; }
        public string ActiveDialogName { get; set; }
        public NWObject DialogTarget { get; set; }
        public CustomData CustomData { get; set; }
        public bool IsEnding { get; set; }
        public string DefaultPageName { get; set; }
        public int DialogNumber { get; set; }

        public PlayerDialog(string defaultPageName)
        {
            Pages = new Dictionary<string, DialogPage>();
            CurrentPageName = string.Empty;
            PageOffset = 0;
            ActiveDialogName = string.Empty;
            DefaultPageName = defaultPageName;
            CustomData = new CustomData();
        }

        public void AddPage(string pageName, DialogPage page)
        {
            Pages.Add(pageName, page);
            if (Pages.Count == 1)
            {
                CurrentPageName = pageName;
            }
        }

        public DialogPage CurrentPage => Pages[CurrentPageName];

        public DialogPage GetPageByName(string pageName)
        {
            return Pages[pageName];
        }

        public void ResetPage()
        {
            CurrentPageName = DefaultPageName;
        }

    }
}
