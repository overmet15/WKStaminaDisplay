using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WKStaminaSlider.MonoBehaviours;

namespace WKStaminaSlider
{
    [BepInPlugin(MyGUID, PluginName, VersionString)]
    public class WKStaminaSliderPlugin : BaseUnityPlugin // Its so funny how its actualy a MonoBehaviour
    {
        private const string MyGUID = "com.overmet15.WKStaminaSlider";
        private const string PluginName = "WKStaminaSlider";
        private const string VersionString = "1.1.0";

        // Stamina
        public static ConfigEntry<int> staminaDistanceFromCenter;
        public static ConfigEntry<bool> staminaSliderTransitionEnabled;
        public static ConfigEntry<float> staminaTransitionSpeed;
        public static ConfigEntry<Color> staminaBackgroundColor;
        public static ConfigEntry<Color> staminaFillColor;

        public static UnityEvent onConfigChange = new UnityEvent();

        void Awake()
        {
            staminaDistanceFromCenter = Config.Bind("General", "Stamina Slider Distance From Center", 150);
            staminaSliderTransitionEnabled = Config.Bind("General", "Stamina Slider Transiotion Enabled", true);
            staminaTransitionSpeed = Config.Bind("General", "Stamina Slider Transition Speed", 0.25f);
            staminaBackgroundColor = Config.Bind("General", "Stamina Slider BG color", new Color(1, 1, 1, 0.025f));
            staminaFillColor = Config.Bind("General", "Stamina Slider Fill color", new Color(1, 1, 1, 0.05f));

            staminaDistanceFromCenter.ConfigFile.SettingChanged += OnConfigChanged;

            SceneManager.sceneLoaded += OnSceneLoaded;

            Logger.LogInfo($"{PluginName} v{VersionString} is loaded.");
        }

        void OnConfigChanged(object sender, System.EventArgs e)
        {
            SettingChangedEventArgs settingChangedEventArgs = e as SettingChangedEventArgs;

            if (settingChangedEventArgs == null) return;

            onConfigChange.Invoke();
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player == null)
            {
                return;
            }

            CL_Player cl = player.GetComponent<CL_Player>();

            Transform canvas = GameObject.Find("GameManager/Canvas/Game UI").transform;

            bool isFirst = true;

            foreach (CL_Player.Hand hand in cl.hands)
            {
                GameObject main = null;

                try
                {
                    // Hand slider main transform
                    main = new GameObject("Hand Slider", typeof(RectTransform));
                    main.transform.SetParent(canvas, false);
                    RectTransform mainRect = main.GetComponent<RectTransform>();
                    mainRect.sizeDelta = new Vector2(20, 75);
                    mainRect.pivot = new Vector2(0.5f, 0.5f);
                    mainRect.anchoredPosition = new Vector2(isFirst ? -staminaDistanceFromCenter.Value : staminaDistanceFromCenter.Value, 0);

                    // BG
                    Image bg = main.AddComponent<Image>();
                    bg.color = staminaBackgroundColor.Value;

                    GameObject main2 = new GameObject("Hand Slider Fill Parent", typeof(RectTransform));
                    main2.transform.SetParent(main.transform, false);
                    RectTransform main2Rect = main2.GetComponent<RectTransform>();
                    main2Rect.anchorMin = Vector2.zero;
                    main2Rect.anchorMax = Vector2.one;
                    main2Rect.offsetMin = Vector2.zero;
                    main2Rect.offsetMax = Vector2.zero;

                    GameObject fillObj = new GameObject("Hand Slider Fill", typeof(RectTransform));
                    fillObj.transform.SetParent(main2.transform, false);
                    RectTransform fillObjRect = fillObj.GetComponent<RectTransform>();
                    fillObjRect.anchorMin = Vector2.zero;
                    fillObjRect.anchorMax = Vector2.one;

                    fillObjRect.offsetMin = new Vector2(2.5f, 2.5f);
                    fillObjRect.offsetMax = new Vector2(-2.5f, -2.5f);

                    Image fill = fillObj.AddComponent<Image>();
                    fill.color = staminaFillColor.Value;

                    Slider s = main.AddComponent<Slider>();
                    s.direction = Slider.Direction.BottomToTop;
                    s.fillRect = fillObjRect;

                    main.AddComponent<StaminaSlider>().Setup(hand, s, isFirst);

                    isFirst = false;
                }
                catch (System.Exception e)
                {
                    Logger.LogError("Exception on creating slider, destroying the slider if not null..\n Exception: " + e.Message);
                    if (main != null) Destroy(main);
                }
            }
        }
    }
}
