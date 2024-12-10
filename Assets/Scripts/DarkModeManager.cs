using UnityEngine;
using UnityEngine.UI;

public class DarkModeManager : MonoBehaviour
{
    [SerializeField]
    public DarkMode darkMode;
    private AndroidJavaObject lightSensorPlugin;

    private const string ModeKey = "DarkMode"; // Klucz w PlayerPrefs
    public const string AutoKey = "AutoMode"; // Klucz w PlayerPrefs
    private int ADM;

    [SerializeField]
    private Mode currentMode; // Obecny tryb
    [SerializeField]
    private Mode lightMode; // Obecny tryb bazowany na poziomie œwiat³a
    [SerializeField]
    private Image background; // Obiekt z komponentem Image


    private void Start()
    {
        // Wczytaj aktualny stan motywu z PlayerPrefs
        int savedMode = PlayerPrefs.GetInt(ModeKey, (int)Mode.Light);

        // Ustaw kolory HEX
        darkMode.SetColors("#81D0FF", "#000546"); // Bia³y dla Light, czarny dla Dark

        // Za³aduj zapisany tryb (domyœlnie Light)
        currentMode = (Mode)PlayerPrefs.GetInt(ModeKey, (int)Mode.Light);

        //Przygotuj odczyt poziomu œwiat³a
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
    public enum Mode
    {
        Light,
        Dark
    }


    public void ToggleMode()
    {
        // Prze³¹cz tryb
        currentMode = currentMode == Mode.Light ? Mode.Dark : Mode.Light;

        // Zapisz tryb w PlayerPrefs
        PlayerPrefs.SetInt(ModeKey, (int)currentMode);
        PlayerPrefs.Save();

        // Zaktualizuj kolor t³a
        UpdateBackgroundColor();
    }


    public float GetLightLevel()
    {
        if (lightSensorPlugin != null)
        {
            return lightSensorPlugin.Call<float>("getLightLevel");
        }
        return 0f;
    }


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
            // Ustaw kolor w zale¿noœci od trybu
            background.color = currentMode == Mode.Light ? darkMode.lightColor : darkMode.darkColor;
        }
    }

    void Update()
    {
        if (PlayerPrefs.GetInt(AutoKey) == 1)
        {
            UpdateBackgroundColor();
        }
    }

    private float GetBrightnessLevel()
    {
        return 0.4f;
    }



    void OnDestroy()
    {
        if (lightSensorPlugin != null)
        {
            lightSensorPlugin.Call("stop");
        }
    }

}