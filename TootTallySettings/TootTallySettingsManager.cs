using HarmonyLib;
using System.Collections.Generic;
using TootTallyCore.Graphics;
using TootTallyCore.Graphics.Animations;
using TootTallyCore.Utils.Assets;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TootTallyCore;

namespace TootTallySettings
{
    public static class TootTallySettingsManager
    {
        private const string MAIN_MENU_PATH = "MainCanvas/MainMenu";
        public static bool isInitialized;

        private static HomeController _currentInstance;

        private static GameObject _mainMenu, _mainSettingPanel, _settingPanelGridHolder;

        private static CustomButton _backButton;
        public static Transform GetSettingPanelGridHolderTransform { get => _settingPanelGridHolder.transform; }

        private static List<TootTallySettingPage> _settingPageList = new List<TootTallySettingPage>();
        private static TootTallySettingPage _currentActivePage;
        private static TootTallyAnimation _currentAnim;


        [HarmonyPatch(typeof(GameObjectFactory), nameof(GameObjectFactory.OnHomeControllerInitialize))]
        [HarmonyPostfix]
        public static void InitializeTootTallySettingsManager(HomeController homeController)
        {
            _currentInstance = homeController;

            TootTallySettingObjectFactory.Initialize(_currentInstance);

            GameObject mainCanvas = GameObject.Find("MainCanvas");
            _mainMenu = mainCanvas.transform.Find("MainMenu").gameObject;

            var btn = GameObjectFactory.CreateCustomButton(_mainMenu.transform, new Vector2(-1661, -456), new Vector2(164, 164), "Mod\nSettings", "TTSettingsOpenButton", delegate
            {
                _mainSettingPanel.SetActive(true);
                _currentAnim?.Dispose();
                _currentAnim = TootTallyAnimationManager.AddNewPositionAnimation(_mainMenu, new Vector2(1940, 0), 1f, new SecondDegreeDynamicsAnimation(2.1f, .98f, 0f));
            });
            btn.textHolder.color = Color.yellow;
            btn.textHolder.fontSize = 26;
            btn.GetComponent<Image>().sprite = AssetManager.GetSprite("PfpMask.png");
            btn.GetComponent<Image>().color = Color.black;
            btn.button.colors = new ColorBlock()
            {
                normalColor = Color.white,
                pressedColor = Color.gray,
                selectedColor = Color.white,
                highlightedColor = new Color(.3f, .3f, 0),
                colorMultiplier = 1f,
                fadeDuration = 0.2f
            };
            var outline = btn.gameObject.AddComponent<Outline>();
            outline.effectColor = Color.yellow;
            outline.effectDistance = new Vector2(3, -3);
            _mainSettingPanel = TootTallySettingObjectFactory.CreateMainSettingPanel(_mainMenu.transform);
            _backButton = GameObjectFactory.CreateCustomButton(_mainSettingPanel.transform, new Vector2(-50, -66), new Vector2(250, 80), "Back", "TTSettingsBackButton", OnBackButtonClick);
            

            _settingPanelGridHolder = _mainSettingPanel.transform.Find("SettingsPanelGridHolder").gameObject;
            ShowMainSettingPanel();

            _settingPageList.ForEach(page => page.Initialize());
            isInitialized = true;
        }

        [HarmonyPatch(typeof(HomeController), nameof(HomeController.Update))]
        [HarmonyPostfix]
        public static void Update()
        {
            if (!isInitialized) return;
            if (Input.GetKeyDown(KeyCode.Escape)) OnBackButtonClick();
        }

        public static void OnBackButtonClick()
        {
            if (_currentActivePage != null)
            {
                _currentActivePage.Hide();
                _currentActivePage = null;
                ShowMainSettingPanel();
            }
            else if (_mainSettingPanel.activeSelf)
                ReturnToMainMenu();
        }

        public static TootTallySettingPage AddNewPage(string pageName, string headerText, float elementSpacing, Color bgColor) =>
        AddNewPage(pageName, headerText, elementSpacing, bgColor, GetDefaultColorBlock);

        public static TootTallySettingPage AddNewPage(string pageName, string headerText, float elementSpacing, Color bgColor, ColorBlock btnColor)
        {
            var page = GetSettingPageByName(pageName);
            if (page != null)
            {
                Plugin.LogInfo($"Page {pageName} already exist.");
                return page;
            }

            page = new TootTallySettingPage(pageName, headerText, elementSpacing, bgColor, btnColor);
            page.OnPageAdd();
            _settingPageList.Add(page);

            return page;
        }

        public static TootTallySettingPage AddNewPage(TootTallySettingPage settingPage)
        {
            var page = GetSettingPageByName(settingPage.name);
            if (page != null)
            {
                Plugin.LogInfo($"Page {page.name} already exist.");
                return page;
            }

            page = settingPage;
            page.OnPageAdd();
            _settingPageList.Add(page);

            return page;
        }

        public static void SwitchActivePage(TootTallySettingPage page)
        {
            _currentActivePage?.Hide();
            _currentActivePage = page;
            HideMainSettingPanel();
            page.Show();
        }

        public static void RemovePage(TootTallySettingPage page)
        {
            page.OnPageRemove();
            if (_settingPageList.Contains(page))
                _settingPageList.Remove(page);
            else
                Plugin.LogInfo($"Page {page.name} couldn't be found.");
        }

        public static void ShowMainSettingPanel()
        {
            _mainSettingPanel.SetActive(true);
        }

        public static void HideMainSettingPanel()
        {
            _mainSettingPanel.SetActive(false);
        }

        public static void ReturnToMainMenu()
        {
            _currentInstance.tryToSaveSettings();
            _currentAnim?.Dispose(); //Never too sure lmfao
            _currentAnim = TootTallyAnimationManager.AddNewPositionAnimation(_mainMenu, Vector2.zero, 1f, new SecondDegreeDynamicsAnimation(2.1f, .98f, 0f), sender => _mainSettingPanel.SetActive(false));
        }

        public static void OnRefreshTheme()
        {
            if (_settingPageList == null || !isInitialized) return;

            _mainSettingPanel.transform.Find("TootTallySettingsHeader").GetComponent<TMP_Text>().color = Theme.colors.leaderboard.text;
            GameObject.DestroyImmediate(_backButton.gameObject);
            _backButton = GameObjectFactory.CreateCustomButton(_mainSettingPanel.transform, new Vector2(-50, -66), new Vector2(250, 80), "Back", "TTSettingsBackButton", OnBackButtonClick);
            _settingPageList.ForEach(page => page.RefreshTheme());
            _currentActivePage.Show();
        }

        public static TootTallySettingPage GetSettingPageByName(string name) => _settingPageList?.Find(page => page.name == name);

        private static ColorBlock GetDefaultColorBlock =>
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
