using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using static SettingsManager;

public class SettingsManager : MonoBehaviour
{
    [SerializeField]
    public DarkMode darkMode;
    public Toggle ToggleAutoDM; // Referencja do ToggleAutoDM
    public TMP_Text ToggleADMText;
    public Button changeThemeButton; // Przycisk zmiany motywu
    public Button backButton;        // Przycisk powrotu do kalendarza
    public Button logoutButton;      // Przycisk wylogowania
    public TMP_Text statusText;      // Pole tekstowe dla komunikatów
    public TMP_Dropdown reminderDropdown; // Lista rozwijana dla przypomnieñ
    public GameObject changeThemeObject;
    private bool isDarkTheme; // Flaga dla zmiany motywu
    [SerializeField]
    private Mode currentMode; // Obecny tryb
    [SerializeField]
    private Mode lightMode; // Obecny tryb bazowany na poziomie œwiat³a
    [SerializeField]
    private Image background; // Obiekt z komponentem Image


    private const string ModeKey = "DarkMode"; // Klucz w PlayerPrefs
    public const string AutoKey = "AutoMode"; // Klucz w PlayerPrefs
    private int ADM;

    private void Start()
    {
        // Wczytaj aktualny stan motywu z PlayerPrefs
        int savedMode = PlayerPrefs.GetInt(ModeKey, (int)Mode.Light);

            // Przypisz funkcje do przycisków
            changeThemeButton.onClick.AddListener(ToggleMode);
        backButton.onClick.AddListener(BackToCalendar);
        logoutButton.onClick.AddListener(Logout);

        // Opcjonalnie: wyczyœæ status na pocz¹tku
        if (statusText != null)
        {
            statusText.text = "";
        }

        // Ustawienia domyœlne dla przypomnieñ
        if (reminderDropdown != null)
        {
            reminderDropdown.onValueChanged.AddListener(SetReminder);
            reminderDropdown.value = PlayerPrefs.GetInt("ReminderTime", 0); // Wczytaj zapisane ustawienie przypomnienia
        }

        // ustaw toggle na poprzedni¹ pozycje
        if (PlayerPrefs.HasKey(AutoKey))
        {
            bool savedState = PlayerPrefs.GetInt(AutoKey) == 1;
            ToggleAutoDM.isOn = savedState;
        }

        // Ustaw pocz¹tkow¹ widocznoœæ
        UpdateButtonVisibility();
        // Subskrybuj siê do zdarzenia zmiany Toggle
        ToggleAutoDM.onValueChanged.AddListener(delegate { UpdateButtonVisibility(); });


        // Ustaw kolory HEX
        darkMode.SetColors("#81D0FF", "#000546"); // Bia³y dla Light, czarny dla Dark

        // Za³aduj zapisany tryb (domyœlnie Light)
        currentMode = (Mode)PlayerPrefs.GetInt(ModeKey, (int)Mode.Light);
        UpdateBackgroundColor();

    }

    void SaveToggleState()
    {
        // Zapisz stan Toggle w PlayerPrefs (1 dla "true", 0 dla "false")
        PlayerPrefs.SetInt(AutoKey, ToggleAutoDM.isOn ? 1 : 0);
        PlayerPrefs.Save(); // Upewnij siê, ¿e zapisano dane
    }
    void UpdateButtonVisibility()
    {
        // Ukryj lub poka¿ przycisk w zale¿noœci od stanu Toggle
        changeThemeObject.SetActive(!ToggleAutoDM.isOn);
        if (ToggleADMText != null)
        {
            ToggleADMText.text = ToggleAutoDM.isOn ? "Auto" : "Manual";
        }
        SaveToggleState();
        UpdateBackgroundColor();
    }

    public enum Mode
    {
       Light,
       Dark
    }

    public void ToggleADM()
    {
        ADM = ToggleAutoDM.isOn ? 1 : 0;
        PlayerPrefs.SetInt(AutoKey, (int)ADM);
        PlayerPrefs.Save();
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
            // Ustaw kolor w zale¿noœci od trybu
            background.color = currentMode == Mode.Light ? darkMode.lightColor : darkMode.darkColor;
        }
    }

    private float GetBrightnessLevel()
    {
        return 0.4f;
    }


    // Funkcja ustawiaj¹ca przypomnienia
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
    }



    // Funkcja powrotu do kalendarza
    private void BackToCalendar()
    {
        SceneManager.LoadScene("CalendarScene");
    }

    // Funkcja wylogowania
    private void Logout()
    {
        PlayerPrefs.DeleteAll(); // Usuniêcie zapisanych danych (opcjonalne)
        SceneManager.LoadScene("LoginScene"); // PrzejdŸ do strony logowania
    }
}