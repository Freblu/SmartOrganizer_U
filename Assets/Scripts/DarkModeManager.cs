using UnityEngine;
using UnityEngine.UI;

/**
 * @class DarkModeManager
 * @brief Mened¿er zarz¹dzaj¹cy trybami jasnym i ciemnym w aplikacji.
 */
public class DarkModeManager : MonoBehaviour
{
    [SerializeField]
    public DarkMode darkMode; ///< Obiekt reprezentuj¹cy konfiguracjê kolorów dla trybów.
    private AndroidJavaObject lightSensorPlugin; ///< Obiekt Java do komunikacji z czujnikiem œwiat³a.

    private const string ModeKey = "DarkMode"; ///< Klucz w PlayerPrefs do przechowywania trybu.
    public const string AutoKey = "AutoMode"; ///< Klucz w PlayerPrefs do przechowywania stanu automatycznego trybu.
    private int ADM; ///< WskaŸnik dla trybu automatycznego.

    [SerializeField]
    private Mode currentMode; ///< Obecny tryb (jasny lub ciemny).
    [SerializeField]
    private Mode lightMode; ///< Tryb zale¿ny od poziomu œwiat³a.
    [SerializeField]
    private Image background; ///< Obiekt z komponentem Image do zmiany koloru t³a.

    /**
     * @brief Inicjalizuje mened¿er trybu jasnego i ciemnego.
     */
    private void Start()
    {
        int savedMode = PlayerPrefs.GetInt(ModeKey, (int)Mode.Light);
        darkMode.SetColors("#81D0FF", "#000546");

        currentMode = (Mode)PlayerPrefs.GetInt(ModeKey, (int)Mode.Light);

        if (Application.platform == RuntimePlatform.Android)
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                lightSensorPlugin = new AndroidJavaObject("com.example.lightsensor.LightSensorPlugin", activity);
                lightSensorPlugin.Call("start");
            }
        }

        UpdateBackgroundColor();
    }

    /**
     * @enum Mode
     * @brief Okreœla tryby aplikacji: jasny lub ciemny.
     */
    public enum Mode
    {
        Light, ///< Tryb jasny.
        Dark ///< Tryb ciemny.
    }

    /**
     * @brief Prze³¹cza miêdzy trybem jasnym a ciemnym.
     */
    public void ToggleMode()
    {
        currentMode = currentMode == Mode.Light ? Mode.Dark : Mode.Light;
        PlayerPrefs.SetInt(ModeKey, (int)currentMode);
        PlayerPrefs.Save();
        UpdateBackgroundColor();
    }

    /**
     * @brief Pobiera poziom œwiat³a z czujnika.
     */
    public float GetLightLevel()
    {
        if (lightSensorPlugin != null)
        {
            return lightSensorPlugin.Call<float>("getLightLevel");
        }
        return 0f;
    }

    /**
     * @brief Aktualizuje kolor t³a aplikacji w zale¿noœci od wybranego trybu.
     */
    private void UpdateBackgroundColor()
    {
        if (background == null)
        {
            Debug.LogError("Background Image is not assigned!");
            return;
        }

        if (PlayerPrefs.GetInt(AutoKey) == 1)
        {
            float brightnessLevel = GetLightLevel();
            lightMode = brightnessLevel < 0.5f ? Mode.Dark : Mode.Light;
            background.color = lightMode == Mode.Light ? darkMode.lightColor : darkMode.darkColor;
        }
        else
        {
            background.color = currentMode == Mode.Light ? darkMode.lightColor : darkMode.darkColor;
        }
    }

    /**
     * @brief Metoda wywo³ywana w ka¿dej klatce.
     */
    private void Update()
    {
        if (PlayerPrefs.GetInt(AutoKey) == 1)
        {
            UpdateBackgroundColor();
        }
    }

    /**
     * @brief Zwraca sta³¹ wartoœæ poziomu jasnoœci otoczenia.
     */
    private float GetBrightnessLevel()
    {
        return 0.4f;
    }

    /**
     * @brief Zatrzymuje monitorowanie czujnika œwiat³a podczas niszczenia obiektu.
     */
    private void OnDestroy()
    {
        if (lightSensorPlugin != null)
        {
            lightSensorPlugin.Call("stop");
        }
    }
}
