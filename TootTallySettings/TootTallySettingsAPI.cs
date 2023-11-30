using System.Collections.Generic;
using TootTallyCore.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace TootTallySettings
{
    public class TootTallySettingsAPI : ITootTallySettingsAPI
    {
        public List<ITootTallySettingPage> SettingPages { get; set; } = new List<ITootTallySettingPage>();

        public ITootTallySettingPage AddNewPage(string pageName, string headerText, float elementSpacing, Color bgColor) =>
        AddNewPage(pageName, headerText, elementSpacing, bgColor, GetDefaultColorBlock);

        public ITootTallySettingPage AddNewPage(string pageName, string headerText, float elementSpacing, Color bgColor, ColorBlock btnColor)
        {
            var page = GetSettingPageByName(pageName);
            if (page != null)
            {
                Plugin.LogInfo($"Page {pageName} already exists.");
                return page;
            }

            page = new TootTallySettingPage(pageName, headerText, elementSpacing, bgColor, btnColor);
            page.OnPageAdd();
            SettingPages.Add(page);

            return page;
        }

        public ITootTallySettingPage AddNewPage(ITootTallySettingPage settingPage)
        {
            var page = GetSettingPageByName(settingPage.Name);
            if (page != null)
            {
                Plugin.LogInfo($"Page {page.Name} already exists.");
                return page;
            }

            page = settingPage;
            page.OnPageAdd();
            SettingPages.Add(page);

            return page;
        }

        public void RemovePage(ITootTallySettingPage page)
        {
            page.OnPageRemove();
            if (SettingPages.Contains(page))
                SettingPages.Remove(page);
            else
                Plugin.LogInfo($"Page {page.Name} couldn't be found.");
        }

        private ITootTallySettingPage GetSettingPageByName(string name) => SettingPages?.Find(page => page.Name == name);

        private ColorBlock GetDefaultColorBlock =>
            new ColorBlock()
            {
                normalColor = new Color(.95f, .21f, .35f),
                highlightedColor = new Color(.77f, .17f, .28f),
                pressedColor = new Color(1, 1, 0),
                selectedColor = new Color(.95f, .21f, .35f),
                fadeDuration = .08f,
                colorMultiplier = 1
            };
    }
}
