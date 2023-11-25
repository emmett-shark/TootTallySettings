using BepInEx.Configuration;
using TMPro;
using TootTallyCore.Graphics;
using UnityEngine;
using UnityEngine.UI;

namespace TootTallySettings
{
    public static class TootTallySettingObjectFactory
    {
        private static GameObject _mainCanvas;
        private static GameObject _panelPrefab;
        private static Slider _sliderPrefab;
        private static Toggle _togglePrefab;
        private static Dropdown _dropdownPrefab;
        private static TMP_InputField _inputFieldPrefab;
        private static bool _isInitialized;

        public static void Initialize(HomeController __instance)
        {
            if (_isInitialized) return;

            _mainCanvas = GameObject.Find("MainCanvas");
            SetPanelPrefab();
            SetSliderPrefab(__instance);
            SetTogglePrefab(__instance);
            SetDropdownPrefab(__instance);
            SetInputFieldPrefab(__instance);

            _isInitialized = true;
        }

        private static void SetPanelPrefab()
        {
            var mainMenu = _mainCanvas.transform.Find("MainMenu").gameObject;

            var mainPanel = Object.Instantiate(_mainCanvas.transform.Find("SettingsPanel").gameObject, mainMenu.transform);
            mainPanel.name = "TootTallySettingsPanelPrefab";
            mainPanel.GetComponent<Image>().color = new Color(0, .2f, 0, 0); //Hide box

            int childCount = mainPanel.transform.childCount - 1;
            for (int i = childCount; i >= 0; i--)
                Object.DestroyImmediate(mainPanel.transform.GetChild(i).gameObject);

            var settingPanelGridHolder = Object.Instantiate(mainPanel, mainPanel.transform);
            settingPanelGridHolder.name = "SettingsPanelGridHolder";
            settingPanelGridHolder.SetActive(true);

            settingPanelGridHolder.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            settingPanelGridHolder.GetComponent<Image>().color = new Color(.2f, 0, 0, 0); //Hide box

            var contentSizeFitter = settingPanelGridHolder.AddComponent<ContentSizeFitter>(); //To set the rectTransform.size dynamically
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var headerText = GameObjectFactory.CreateSingleText(mainPanel.transform, "TootTallySettingsHeader", "TootTally Settings (BETA)", Color.white);
            headerText.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 475);
            headerText.fontSize = 72;
            headerText.fontStyle = FontStyles.Bold | FontStyles.UpperCase;
            headerText.alignment = TextAlignmentOptions.Top;

            _panelPrefab = Object.Instantiate(mainPanel);
            Object.DestroyImmediate(mainPanel);
            _panelPrefab.name = "SettingPanelPrefab";
            _panelPrefab.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            Object.DontDestroyOnLoad(_panelPrefab);
        }

        private static void SetSliderPrefab(HomeController __instance)
        {
            _sliderPrefab = Object.Instantiate(__instance.fullsettingspanel.transform.Find("Settings/AUDIO/master_volume/SET_sld_volume").GetComponent<Slider>());
            _sliderPrefab.name = "TootTallySettingsSliderPrefab";
            var handle = _sliderPrefab.transform.Find("Handle Slide Area/Handle");
            var scrollSpeedSliderText = GameObjectFactory.CreateSingleText(handle, "SliderHandleText", "1", Color.white);
            scrollSpeedSliderText.text = "50";

            _sliderPrefab.onValueChanged = new Slider.SliderEvent();

            _sliderPrefab.minValue = 0;
            _sliderPrefab.maxValue = 1;
            _sliderPrefab.value = .5f;

            Object.DontDestroyOnLoad(_sliderPrefab);
        }

        private static void SetTogglePrefab(HomeController __instance)
        {
            _togglePrefab = Object.Instantiate(__instance.set_tog_accessb_jumpscare);
            _togglePrefab.name = "TootTallySettingsTogglePrefab";
            _togglePrefab.onValueChanged = new Toggle.ToggleEvent();

            Object.DontDestroyOnLoad(_togglePrefab);
        }

        private static void SetDropdownPrefab(HomeController __instance)
        {
            _dropdownPrefab = Object.Instantiate(__instance.set_drp_antialiasing);
            _dropdownPrefab.name = "TootTallySettingsDropdownPrefab";
            _dropdownPrefab.onValueChanged = new Dropdown.DropdownEvent();
            _dropdownPrefab.ClearOptions();

            Object.DontDestroyOnLoad(_dropdownPrefab);
        }

        private static void SetInputFieldPrefab(HomeController __instance)
        {
            var inputHolder = new GameObject("InputFieldHolder");
            var rectHolder = inputHolder.AddComponent<RectTransform>();
            rectHolder.anchoredPosition = Vector2.zero;
            rectHolder.sizeDelta = new Vector2(350, 50);
            var inputImageHolder = Object.Instantiate(inputHolder, inputHolder.transform);
            var inputTextHolder = Object.Instantiate(inputImageHolder, inputHolder.transform);
            inputImageHolder.name = "Image";
            inputTextHolder.name = "Text";

            _inputFieldPrefab = inputHolder.AddComponent<TMP_InputField>();

            rectHolder.anchorMax = rectHolder.anchorMin = Vector2.zero;

            //pain... @_@
            _inputFieldPrefab.image = inputImageHolder.AddComponent<Image>();
            RectTransform rectImage = inputImageHolder.GetComponent<RectTransform>();

            rectImage.anchorMin = rectImage.anchorMax = rectImage.pivot = Vector2.zero;
            rectImage.anchoredPosition = new Vector2(0, 4);
            rectImage.sizeDelta = new Vector2(350, 2);

            RectTransform rectText = inputTextHolder.GetComponent<RectTransform>();
            rectText.anchoredPosition = rectText.anchorMin = rectText.anchorMax = rectText.pivot = Vector2.zero;
            rectText.sizeDelta = new Vector2(350, 50);

            _inputFieldPrefab.textComponent = GameObjectFactory.CreateSingleText(inputTextHolder.transform, $"TextLabel", "", Color.white);
            _inputFieldPrefab.textComponent.rectTransform.pivot = new Vector2(0, 0.5f);
            _inputFieldPrefab.textComponent.alignment = TextAlignmentOptions.Left;
            _inputFieldPrefab.textComponent.margin = new Vector4(5, 0, 0, 0);
            _inputFieldPrefab.textComponent.enableWordWrapping = true;
            _inputFieldPrefab.textViewport = _inputFieldPrefab.textComponent.rectTransform;

            Object.DontDestroyOnLoad(_inputFieldPrefab);
        }

        public static GameObject CreateMainSettingPanel(Transform canvasTransform)
        {
            var panel = Object.Instantiate(_panelPrefab, canvasTransform);
            panel.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1940, 0);

            var gridHolder = panel.transform.Find("SettingsPanelGridHolder").gameObject;

            var gridLayoutGroup = gridHolder.AddComponent<GridLayoutGroup>();
            gridLayoutGroup.spacing = new Vector2(25, 25);
            gridLayoutGroup.childAlignment = TextAnchor.UpperCenter;
            gridLayoutGroup.cellSize = new Vector2(400, 85);

            GameObjectFactory.CreateCustomButton(panel.transform, new Vector2(-50, -66), new Vector2(250, 80), "Back", "TTSettingsBackButton", TootTallySettingsManager.OnBackButtonClick);

            return panel;
        }

        public static GameObject CreateSettingPanel(Transform canvasTransform, string name, string headerText, float elementSpacing, Color bgColor)
        {
            var panel = Object.Instantiate(_panelPrefab, canvasTransform);
            panel.name = $"TootTally{name}Panel";
            panel.GetComponent<Image>().color = bgColor;

            panel.transform.Find("TootTallySettingsHeader").GetComponent<TMP_Text>().text = headerText;

            var gridPanel = panel.transform.Find("SettingsPanelGridHolder").gameObject;
            gridPanel.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
            gridPanel.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1);
            gridPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(-950, -150);

            var verticalLayoutGroup = gridPanel.AddComponent<VerticalLayoutGroup>();
            verticalLayoutGroup.childAlignment = TextAnchor.UpperLeft;
            verticalLayoutGroup.childControlHeight = verticalLayoutGroup.childControlWidth = false;
            verticalLayoutGroup.childForceExpandHeight = verticalLayoutGroup.childForceExpandWidth = false;
            verticalLayoutGroup.childScaleHeight = verticalLayoutGroup.childScaleWidth = false;
            verticalLayoutGroup.padding = new RectOffset(100, 100, 20, 20);
            verticalLayoutGroup.spacing = elementSpacing;

            return panel;
        }

        public static Slider CreateSlider(Transform canvasTransform, string name, float min, float max, bool integerOnly)
        {
            var slider = Object.Instantiate(_sliderPrefab, canvasTransform);
            slider.name = name;
            slider.maxValue = max;
            slider.minValue = min;
            slider.wholeNumbers = integerOnly;

            return slider;
        }

        public static Slider CreateVerticalSlider(Transform canvasTransform, string name, Vector2 position, Vector2 size)
        {
            var slider = Object.Instantiate(_sliderPrefab, canvasTransform);
            slider.name = name;

            RectTransform sliderRect = slider.GetComponent<RectTransform>();
            sliderRect.sizeDelta = size;
            sliderRect.anchoredPosition = position;
            sliderRect.eulerAngles = new Vector3(0, 0, -90);

            Object.DestroyImmediate(slider.transform.Find("Handle Slide Area/Handle/SliderHandleText").gameObject);

            slider.minValue = slider.value = 0;
            slider.maxValue = 1;
            slider.wholeNumbers = false;

            return slider;
        }

        public static Toggle CreateToggle(Transform canvasTransform, string name, Vector2 size, string text, ConfigEntry<bool> config = null)
        {
            var toggle = Object.Instantiate(_togglePrefab, canvasTransform);
            RectTransform rect = toggle.GetComponent<RectTransform>();
            rect.pivot = Vector3.zero;
            rect.anchoredPosition = Vector3.zero;
            rect.sizeDelta = size;
            toggle.name = name;
            if (config != null)
            {
                toggle.isOn = config.Value;
                toggle.onValueChanged.AddListener((value) => config.Value = value);
            }
            else
                toggle.isOn = false;


            toggle.transform.Find("Label").GetComponent<Text>().text = text;
            return toggle;
        }

        public static Dropdown CreateDropdown(Transform canvasTransform, string name)
        {
            var dropdown = Object.Instantiate(_dropdownPrefab, canvasTransform);
            dropdown.name = name;
            return dropdown;
        }

        public static TMP_InputField CreateInputField(Transform canvasTransform, string name, Vector2 size, float fontSize, string text, bool isPassword)
        {
            var inputField = Object.Instantiate(_inputFieldPrefab, canvasTransform);
            inputField.name = name;
            inputField.GetComponent<RectTransform>().sizeDelta = size;
            inputField.transform.Find("Image").GetComponent<RectTransform>().sizeDelta = new Vector2(size.x, 2);
            inputField.transform.Find("Text").GetComponent<RectTransform>().sizeDelta = size;
            inputField.textComponent.GetComponent<RectTransform>().sizeDelta = size;
            inputField.textComponent.fontSize = fontSize;
            inputField.text = text;
            inputField.inputType = isPassword ? TMP_InputField.InputType.Password : TMP_InputField.InputType.Standard;

            return inputField;
        }

    }
}
