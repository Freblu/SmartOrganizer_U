using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class SettingsManager : MonoBehaviour
{
    [Header("UI Elements")]
    public DarkMode darkMode;
    public Toggle ToggleAutoDM;
    public TMP_Text ToggleADMText;
    public Button changeThemeButton;
    public Button backButton;
    public Button logoutButton;
    public Button addUserButton; // Przycisk dodania nowego u¿ytkownika
    public GameObject addUserPanel; // Panel dodawania u¿ytkownika
    public TMP_InputField usernameInput; // Pole wprowadzania nazwy u¿ytkownika
    public TMP_InputField passwordInput; // Pole wprowadzania has³a
    public Button confirmAddUserButton; // Przycisk zatwierdzaj¹cy dodanie u¿ytkownika
    public TMP_Text statusText;
    public TMP_Dropdown reminderDropdown;
    public GameObject changeThemeObject;
    private bool isDarkTheme;
    [SerializeField] private Mode currentMode;
    [SerializeField] private Mode lightMode;
    [SerializeField] private Image background;

    private const string ModeKey = "DarkMode";
    private const string AutoKey = "AutoMode";

 
    private void Start()
    {
        // Wczytaj aktualny stan motywu
        int savedMode = PlayerPrefs.GetInt(ModeKey, (int)Mode.Light);

        // Przypisz funkcje do przycisków
        changeThemeButton.onClick.AddListener(ToggleMode);
        backButton.onClick.AddListener(BackToCalendar);
        logoutButton.onClick.AddListener(Logout);
        addUserButton.onClick.AddListener(OpenAddUserPanel);

        // Subskrybuj zdarzenie potwierdzenia dodania u¿ytkownika
        if (confirmAddUserButton != null)
        {
            confirmAddUserButton.onClick.AddListener(AddUser);
        }

        // Ustawienia pocz¹tkowe
        if (statusText != null)
        {
            statusText.text = "";
        }

        if (reminderDropdown != null)
        {
            reminderDropdown.onValueChanged.AddListener(SetReminder);
            reminderDropdown.value = PlayerPrefs.GetInt("ReminderTime", 0);
        }

        if (PlayerPrefs.HasKey(AutoKey))
        {
            bool savedState = PlayerPrefs.GetInt(AutoKey) == 1;
            ToggleAutoDM.isOn = savedState;
        }

        ToggleAutoDM.onValueChanged.AddListener(delegate { UpdateButtonVisibility(); });

        darkMode.SetColors("#81D0FF", "#000546");

        currentMode = (Mode)PlayerPrefs.GetInt(ModeKey, (int)Mode.Light);
        UpdateBackgroundColor();

        // Ukryj panel dodawania u¿ytkownika na starcie
        if (addUserPanel != null)
        {
            addUserPanel.SetActive(false);
        }

        /*
          var latLon = new LatLng(51.5074, -0.1278); // Przyk³adowa lokalizacja: Londyn
        Vector3 worldPos = map.GeoToWorldPosition(latLon); // Konwersja geolokalizacji na wspó³rzêdne Unity

        // Tworzymy marker w œwiecie Unity
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
        SaveSettingsToDatabase();
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

    private void SetReminder(int optionIndex)
    {
        PlayerPrefs.SetInt("ReminderTime", optionIndex);
        PlayerPrefs.Save();

        string reminderText = optionIndex switch
        {
            0 => "1 godzina przed spotkaniem",
            1 => "2 godziny przed spotkaniem",
            2 => "15 minut przed spotkaniem",
            3 => "W momencie rozpoczêcia spotkania",
            _ => "Brak przypomnienia"
        };

        if (statusText != null)
        {
            statusText.text = $"Ustawiono przypomnienie: {reminderText}";
        }

        Debug.Log($"Wybrano opcjê przypomnienia: {reminderText}");
        SaveSettingsToDatabase();
    }

    private void OpenAddUserPanel()
    {
        if (addUserPanel != null)
        {
            addUserPanel.SetActive(true); // Wyœwietl panel
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
                statusText.text = "U¿ytkownik o podanej nazwie ju¿ istnieje!";
                Debug.LogWarning("U¿ytkownik o tej nazwie ju¿ istnieje!");
                return;
            }

            // Zapisz login i has³o
            PlayerPrefs.SetString($"User_{username}", password);
            PlayerPrefs.SetString("NewUserPanelActive", "true");
            PlayerPrefs.SetString("LastAddedUser", username); // Przechowaj login ostatniego u¿ytkownika
            PlayerPrefs.Save();

            Debug.Log($"Dodano nowego u¿ytkownika: {username}");
            SceneManager.LoadScene("LoginScene"); // Przenieœ do LoginScene
        }
        else
        {
            statusText.text = "Proszê wype³niæ wszystkie pola!";
            Debug.LogWarning("Pola u¿ytkownika s¹ puste!");
        }
    }

    private void BackToCalendar()
    {
        SceneManager.LoadScene("CalendarScene");
    }

    [System.Serializable]
    public class UserSettings
    {
        public string username; // nazwa u¿ytkownika
        public bool isDarkMode; // Motyw (np. "light" lub "dark")
        public bool isAutoMode; // Automatyczny tryb ciemny
        public int reminderOption; // Wybrana opcja przypomnienia

        // Konstruktor, aby ³atwo inicjalizowaæ obiekt
        public UserSettings(string username, bool isDarkMode, bool isAutoMode, int reminderOption)
        {
            this.username = username;
            this.isDarkMode = isDarkMode;
            this.isAutoMode = isAutoMode;
            this.reminderOption = reminderOption;
        }
    }

    public void SaveSettingsToDatabase()
    {
        string username = PlayerPrefs.GetString("LoggedInUser", "Guest");
        Debug.Log(username);
        // Przygotuj dane do wys³ania
        UserSettings settings = new UserSettings(username, currentMode == Mode.Dark, ToggleAutoDM.isOn, reminderDropdown.value);

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

    private void Logout()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("LoginScene");
    }
}
