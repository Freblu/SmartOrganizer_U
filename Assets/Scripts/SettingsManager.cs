using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
/*
 * using Mapbox.Unity.Map;
using Mapbox.Unity.Location;
using Mapbox.Utils;
using Mapbox.Unity.MeshGeneration.Factories;
*/
public class SettingsManager : MonoBehaviour
{
    [Header("UI Elements")]
    public DarkMode darkMode;
    public Toggle ToggleAutoDM;
    public TMP_Text ToggleADMText;
    public Button changeThemeButton;
    public Button backButton;
    public Button logoutButton;
    public Button addUserButton; // Przycisk dodania nowego użytkownika
    public GameObject addUserPanel; // Panel dodawania użytkownika
    public TMP_InputField usernameInput; // Pole wprowadzania nazwy użytkownika
    public TMP_InputField passwordInput; // Pole wprowadzania hasła
    public Button confirmAddUserButton; // Przycisk zatwierdzający dodanie użytkownika
    public TMP_Text statusText;
    public TMP_Text batteryStatusText; ///< Pole tekstowe do wyświetlania statusu baterii.
    public GameObject changeThemeObject;
    private bool isDarkTheme;
    [SerializeField] private Mode currentMode;
    [SerializeField] private Mode lightMode;
    [SerializeField] private Image background;

    private const string ModeKey = "DarkMode";
    private const string AutoKey = "AutoMode";
    private int AutoTog;

    /*
     public AbstractMap map; // Mapbox map component
    public GameObject markerPrefab; // Prefab dla markera
    */

    private void Start()
    {
        // Wczytaj aktualny stan motywu
        int savedMode = PlayerPrefs.GetInt(ModeKey, (int)Mode.Light);

        // Przypisz funkcje do przycisków
        changeThemeButton.onClick.AddListener(ToggleMode);
        backButton.onClick.AddListener(BackToCalendar);
        logoutButton.onClick.AddListener(Logout);
        addUserButton.onClick.AddListener(OpenAddUserPanel);

        // Subskrybuj zdarzenie potwierdzenia dodania użytkownika
        if (confirmAddUserButton != null)
        {
            confirmAddUserButton.onClick.AddListener(AddUser);
        }

        // Ustawienia początkowe
        if (statusText != null)
        {
            statusText.text = "";
        }


  
            bool savedState = PlayerPrefs.GetInt(AutoKey) == 1;
            ToggleAutoDM.isOn = savedState;
            changeThemeObject.SetActive(!ToggleAutoDM.isOn);
            ToggleADMText.text = ToggleAutoDM.isOn ? "Auto" : "Manual";

        ToggleAutoDM.onValueChanged.AddListener(delegate { UpdateButtonVisibility(); });

        darkMode.SetColors("#81D0FF", "#000546");

        currentMode = (Mode)PlayerPrefs.GetInt(ModeKey, (int)Mode.Light);
        UpdateBackgroundColor();

        // Ukryj panel dodawania użytkownika na starcie
        if (addUserPanel != null)
        {
            addUserPanel.SetActive(false);
        }

        /*
          var latLon = new LatLng(51.5074, -0.1278); // Przykładowa lokalizacja: Londyn
        Vector3 worldPos = map.GeoToWorldPosition(latLon); // Konwersja geolokalizacji na współrzędne Unity

        // Tworzymy marker w świecie Unity
        Instantiate(markerPrefab, worldPos, Quaternion.identity);
        */
    }

    private void UpdateButtonVisibility()
    {
        changeThemeObject.SetActive(!ToggleAutoDM.isOn);
        if (ToggleADMText != null)
        {
            ToggleADMText.text = ToggleAutoDM.isOn ? "Auto" : "Manual";
        }
        AutoTog = PlayerPrefs.GetInt(AutoKey) == 1 ? 0 : 1;
        PlayerPrefs.SetInt(AutoKey, (int)AutoTog);
        PlayerPrefs.Save();
        UpdateBackgroundColor();
    }



    public enum Mode
    {
        Light,
        Dark
    }

    public void ToggleMode()
    {
        currentMode = currentMode == Mode.Light ? Mode.Dark : Mode.Light;
        PlayerPrefs.SetInt(ModeKey, (int)currentMode);
        PlayerPrefs.Save();
        UpdateBackgroundColor();
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
            float brightnessLevel = GetBrightnessLevel();
            lightMode = brightnessLevel < 0.5f ? Mode.Dark : Mode.Light;
            background.color = lightMode == Mode.Light ? darkMode.lightColor : darkMode.darkColor;
        }
        else
        {
            background.color = currentMode == Mode.Light ? darkMode.lightColor : darkMode.darkColor;
        }
    }

    private float GetBrightnessLevel()
    {
        return 0.4f;
    }



    private void OpenAddUserPanel()
    {
        if (addUserPanel != null)
        {
            addUserPanel.SetActive(true); // Wyświetl panel
        }
    }

    private void AddUser()
    {
        string username = usernameInput.text.Trim();
        string password = passwordInput.text.Trim();

        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
            if (PlayerPrefs.HasKey($"User_{username}"))
            {
                statusText.text = "Użytkownik o podanej nazwie już istnieje!";
                Debug.LogWarning("Użytkownik o tej nazwie już istnieje!");
                return;
            }

            // Zapisz login i hasło
            PlayerPrefs.SetString($"User_{username}", password);
            PlayerPrefs.SetString("NewUserPanelActive", "true");
            PlayerPrefs.SetString("LastAddedUser", username); // Przechowaj login ostatniego użytkownika
            PlayerPrefs.Save();

            Debug.Log($"Dodano nowego użytkownika: {username}");
            SceneManager.LoadScene("LoginScene"); // Przenieś do LoginScene
        }
        else
        {
            statusText.text = "Proszę wypełnić wszystkie pola!";
            Debug.LogWarning("Pola użytkownika są puste!");
        }
    }

    private void UpdateBatteryStatus()
    {
        if (SystemInfo.batteryLevel < 0)
        {
            if (batteryStatusText != null)
            {
                batteryStatusText.text = "Poziom baterii niedostępny.";
            }
            return;
        }

        float batteryLevel = SystemInfo.batteryLevel * 100f;
        BatteryStatus batteryStatus = SystemInfo.batteryStatus;

        string statusMessage = $"Poziom baterii: {batteryLevel:0}%\nStatus: {batteryStatus}";

        if (batteryStatusText != null)
        {
            batteryStatusText.text = statusMessage;
        }
    }

    [System.Serializable]
    public class UserSettings
    {
        public string username; // nazwa u¿ytkownika
        public bool isDarkMode; // Motyw (np. "light" lub "dark")
        public bool isAutoMode; // Automatyczny tryb ciemny

        // Konstruktor, aby ³atwo inicjalizowaæ obiekt
        public UserSettings(string username, bool isDarkMode, bool isAutoMode)
        {
            this.username = username;
            this.isDarkMode = isDarkMode;
            this.isAutoMode = isAutoMode;
        }
    }

    public void SaveSettingsToDatabase()
    {
        string username = PlayerPrefs.GetString("LoggedInUser", "Guest");
        Debug.Log(username);
        // Przygotuj dane do wys³ania
        UserSettings settings = new UserSettings(username, currentMode == Mode.Dark, ToggleAutoDM.isOn);

        string json = JsonUtility.ToJson(settings);

        StartCoroutine(SendSettingsToServer(json));
    }

    private IEnumerator SendSettingsToServer(string json)
    {
        UnityWebRequest request = new UnityWebRequest("http://localhost/save_settings.php", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Ustawienia zapisane pomyœlnie: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("B³¹d zapisu ustawieñ: " + request.error);
        }
    }

    private void BackToCalendar()
    {
        SceneManager.LoadScene("CalendarScene");
    }

    private void Logout()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("LoginScene");
    }
}
