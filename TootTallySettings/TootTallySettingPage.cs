using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using TootTallySettings;
using TootTallyCore.Graphics;
using TootTallySettings.TootTallySettingsObjects;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Rewired.Data;
using System.Security.Policy;
using TootTallyCore.Utils.Assets;
using System.ComponentModel;

namespace TootTallySettings
{
    public class TootTallySettingPage
    {
        public static readonly Vector2 DEFAULT_OBJECT_SIZE = new Vector2(350, 50);
        public static readonly float DEFAULT_SLIDER_LENGTH = 250;
        public static readonly float DEFAULT_HEADER_FONTSIZE = 40;
        public static readonly float DEFAULT_FONTSIZE = 20;

        public bool isInitialized;

        public string name, headerName;
        public float elementSpacing;
        protected List<BaseTootTallySettingObject> _settingObjectList;
        private CustomButton _pageButton;
        protected GameObject _fullPanel;
        protected CustomButton _backButton;
        protected Slider _verticalSlider;
        protected ScrollableSliderHandler _scrollableSliderHandler;
        public GameObject gridPanel;
        private Color _bgColor;
        private ColorBlock _btnColors;
        private bool _isInitialized;
        public TootTallySettingPage(string pageName, string headerName, float elementSpacing, Color bgColor, ColorBlock btnColors)
        {
            this.name = pageName;
            this.headerName = headerName;
            this.elementSpacing = elementSpacing;
            _bgColor = bgColor;
            _btnColors = btnColors;
            _settingObjectList = new List<BaseTootTallySettingObject>();
            if (TootTallySettingsManager.isInitialized)
                Initialize();
        }

        public virtual void Initialize()
        {
            _fullPanel = TootTallySettingObjectFactory.CreateSettingPanel(GameObject.Find("MainCanvas").transform, name, headerName, elementSpacing, _bgColor);

            _backButton = GameObjectFactory.CreateCustomButton(_fullPanel.transform, new Vector2(-1570, -66), new Vector2(250, 80), "Return", $"{name}ReturnButton", TootTallySettingsManager.OnBackButtonClick);

            gridPanel = _fullPanel.transform.Find("SettingsPanelGridHolder").gameObject;

            _verticalSlider = TootTallySettingObjectFactory.CreateVerticalSlider(_fullPanel.transform, $"{name}VerticalSlider", new Vector2(1700, -200), new Vector2(-1080, 20));
            _verticalSlider.onValueChanged.AddListener(delegate { OnSliderValueChangeScrollGridPanel(gridPanel, _verticalSlider.value); });
            _scrollableSliderHandler = _verticalSlider.gameObject.AddComponent<ScrollableSliderHandler>();
            _scrollableSliderHandler.enabled = false;

            _pageButton = GameObjectFactory.CreateCustomButton(TootTallySettingsManager.GetSettingPanelGridHolderTransform, Vector2.zero, new Vector2(250, 60), name, $"Open{name}Button", () => TootTallySettingsManager.SwitchActivePage(this));
            if (_btnColors != null)
                _pageButton.button.colors = _btnColors;
            if (_extraImagePath != null)
                AddImageToPageButton(_extraImagePath);
            else if (_extraImage != null)
                AddImageToPageButton(_extraImage);

            _settingObjectList.ForEach(obj =>
            {
                try
                {
                    obj.Initialize();
                }
                catch (Exception e)
                {
                    Plugin.LogError($"Couldn't initialize toottallysetting object {obj.name}");
                    Plugin.LogError(e.Message);
                    Plugin.LogError(e.StackTrace);
                }
            });
            _isInitialized = true;
        }

        private string _extraImagePath;
        private Sprite _extraImage;

        public void AddImageToPageButton(string spriteName)
        {
            if (_pageButton != null)
            {
                var image = GameObjectFactory.CreateImageHolder(_pageButton.transform, Vector2.zero, Vector2.one * 78f, AssetManager.GetSprite(spriteName), $"{name}PageIcon");
                var rect = image.GetComponent<RectTransform>();
                rect.anchorMax = rect.anchorMin = new Vector2(.15f, .5f);
                _pageButton.transform.GetChild(0).GetComponent<RectTransform>().anchorMin = new Vector2(.2f, 0);
            }
            else
                _extraImagePath = spriteName;
        }

        public void AddImageToPageButton(Sprite sprite)
        {
            if (_pageButton != null)
            {
                var image = GameObjectFactory.CreateImageHolder(_pageButton.transform, Vector2.zero, Vector2.one * 78f, sprite, $"{name}PageIcon");
                var rect = image.GetComponent<RectTransform>();
                rect.anchorMax = rect.anchorMin = new Vector2(.15f, .5f);
                _pageButton.transform.GetChild(0).GetComponent<RectTransform>().anchorMin = new Vector2(.2f, 0);
            }
            else
                _extraImage = sprite;
        }

        public virtual void OnPageAdd() { }

        public void OnPageRemove()
        {
            List<BaseTootTallySettingObject> objToDelete = new(_settingObjectList);
            objToDelete.ForEach(obj =>
            {
                obj.Remove();
            });
            GameObject.DestroyImmediate(_fullPanel);
            GameObject.DestroyImmediate(_pageButton.gameObject);
        }

        public void RemoveSettingObjectFromList(string name)
        {
            var settingObject = _settingObjectList.Find(obj => obj.name == name);
            if (settingObject == null)
            {
                Plugin.LogInfo($"{name} object couldn't be found.");
                return;
            }
            RemoveSettingObjectFromList(settingObject);
            UpdateVerticalSlider();
        }
        private static void OnSliderValueChangeScrollGridPanel(GameObject gridPanel, float value)
        {
            var gridPanelRect = gridPanel.GetComponent<RectTransform>();
            gridPanelRect.anchoredPosition = new Vector2(gridPanelRect.anchoredPosition.x, (value * gridPanelRect.sizeDelta.y) - (1 - value) * 150f); //This is so scuffed I fucking love it
        }

        public void RemoveSettingObjectFromList(BaseTootTallySettingObject settingObject)
        {
            if (!settingObject.isDisposed)
                settingObject.Dispose();

            _settingObjectList.Remove(settingObject);

            if (_isInitialized)
                UpdateVerticalSlider();
        }

        public void RemoveAllObjects()
        {
            BaseTootTallySettingObject[] allObjectsList = new BaseTootTallySettingObject[_settingObjectList.Count];
            _settingObjectList.CopyTo(allObjectsList);
            allObjectsList.ToList().ForEach(o => o.Remove());

            if (_isInitialized)
                UpdateVerticalSlider();
        }

        public BaseTootTallySettingObject AddSettingObjectToList(BaseTootTallySettingObject settingObject)
        {
            _settingObjectList.Add(settingObject);

            if (_isInitialized)
                UpdateVerticalSlider();

            return settingObject;
        }

        public void Remove()
        {
            TootTallySettingsManager.RemovePage(this);
        }

        public BaseTootTallySettingObject GetSettingObjectByName(string name) => _settingObjectList.Find(obj => obj.name == name);

        public virtual void OnShow() { }

        public void Show()
        {
            _fullPanel.SetActive(true);
            UpdateVerticalSlider();
            OnShow();
        }

        private void UpdateVerticalSlider()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(gridPanel.GetComponent<RectTransform>());
            _verticalSlider.gameObject.SetActive(gridPanel.GetComponent<RectTransform>().sizeDelta.y > -100f);
            _scrollableSliderHandler.enabled = gridPanel.GetComponent<RectTransform>().sizeDelta.y > -100f;
        }

        public virtual void OnHide() { }

        public void Hide()
        {
            _fullPanel.SetActive(false);
            _scrollableSliderHandler.enabled = false;
            OnHide();
        }

        public TootTallySettingButton AddButton(string name, Vector2 size, string text, string description = "", Action OnClick = null) => AddSettingObjectToList(new TootTallySettingButton(this, name, size, text, description, OnClick)) as TootTallySettingButton;
        public TootTallySettingButton AddButton(string name, Vector2 size, string text, Action OnClick = null) => AddButton(name, size, text, "", OnClick);
        public TootTallySettingButton AddButton(string name, Action OnClick = null) => AddButton(name, DEFAULT_OBJECT_SIZE, name, OnClick);

        public TootTallySettingSlider AddSlider(string name, float min, float max, float length, string text, ConfigEntry<float> config, bool integerOnly) => AddSettingObjectToList(new TootTallySettingSlider(this, name, min, max, length, text, config, integerOnly)) as TootTallySettingSlider;
        public TootTallySettingSlider AddSlider(string name, float min, float max, ConfigEntry<float> config, bool integerOnly) => AddSlider(name, min, max, DEFAULT_SLIDER_LENGTH, name, config, integerOnly);

        public TootTallySettingToggle AddToggle(string name, Vector2 size, string text, ConfigEntry<bool> config, UnityAction<bool> onValueChange = null) => AddSettingObjectToList(new TootTallySettingToggle(this, name, size, text, config, onValueChange)) as TootTallySettingToggle;
        public TootTallySettingToggle AddToggle(string name, ConfigEntry<bool> config, UnityAction<bool> onValueChange = null) => AddToggle(name, DEFAULT_OBJECT_SIZE, name, config, onValueChange);

        public TootTallySettingDropdown AddDropdown(string name, string text, ConfigEntry<string> config, params string[] optionValues) => AddSettingObjectToList(new TootTallySettingDropdown(this, name, text, config, optionValues)) as TootTallySettingDropdown;
        public TootTallySettingDropdown AddDropdown(string name, ConfigEntry<string> config, params string[] optionValues) => AddDropdown(name, name, config, optionValues);
        public TootTallySettingDropdown AddDropdown(string name, ConfigEntryBase config) => AddSettingObjectToList(new TootTallySettingDropdown(this, name, config)) as TootTallySettingDropdown;

        public TootTallySettingTextField AddTextField(string name, Vector2 size, float fontSize, string defaultValue, string description = "", bool isPassword = false, Action<string> onEditEnd = null) => AddSettingObjectToList(new TootTallySettingTextField(this, name, size, fontSize, defaultValue, description, isPassword, onEditEnd)) as TootTallySettingTextField;
        public TootTallySettingTextField AddTextField(string name, Vector2 size, float fontSize, string defaultValue, bool isPassword = false, Action<string> onEditEnd = null) => AddTextField(name, size, fontSize, defaultValue, "", isPassword, onEditEnd);
        public TootTallySettingTextField AddTextField(string name, string defaultValue, bool isPassword = false, Action<string> onEditEnd = null) => AddTextField(name, DEFAULT_OBJECT_SIZE, DEFAULT_FONTSIZE, defaultValue, isPassword, onEditEnd);

        public TootTallySettingColorSliders AddColorSliders(string name, string text, float length, ConfigEntry<Color> config) => AddSettingObjectToList(new TootTallySettingColorSliders(this, name, text, length, config)) as TootTallySettingColorSliders;
        public TootTallySettingColorSliders AddColorSliders(string name, string text, ConfigEntry<Color> config) => AddColorSliders(name, text, DEFAULT_SLIDER_LENGTH, config);


        public TootTallySettingLabel AddLabel(string name, string text, float fontSize, FontStyles fontStyles = FontStyles.Normal, TextAlignmentOptions align = TextAlignmentOptions.MidlineLeft) => AddSettingObjectToList(new TootTallySettingLabel(this, name, text, fontSize, fontStyles, align)) as TootTallySettingLabel;
        public TootTallySettingLabel AddLabel(string name, FontStyles fontStyles = FontStyles.Normal, TextAlignmentOptions align = TextAlignmentOptions.MidlineLeft) => AddLabel(name, name, DEFAULT_FONTSIZE, fontStyles, align);


    }
}
