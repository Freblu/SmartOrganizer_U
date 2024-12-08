using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using static SettingsManager;

public class SettingsManager : MonoBehaviour
{
    [SerializeField]
    public DarkMode darkMode;
    public Button changeThemeButton; // Przycisk zmiany motywu
    public Button backButton;        // Przycisk powrotu do kalendarza
    public Button logoutButton;      // Przycisk wylogowania
    public TMP_Text statusText;      // Pole tekstowe dla komunikatów
    public TMP_Dropdown reminderDropdown; // Lista rozwijana dla przypomnieñ
    private bool isDarkTheme; // Flaga dla zmiany motywu

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


        // Ustaw kolory HEX
        darkMode.SetColors("#81D0FF", "#005587"); // Bia³y dla Light, czarny dla Dark

        // Za³aduj zapisany tryb (domyœlnie Light)
        currentMode = (Mode)PlayerPrefs.GetInt(ModeKey, (int)Mode.Light);
        UpdateBackgroundColor();
    }

    public enum Mode
    {
        Light,
        Dark
    }

    [SerializeField]
    private Mode currentMode; // Obecny tryb

    [SerializeField]
    private Image background; // Obiekt z komponentem Image


    private const string ModeKey = "DarkMode"; // Klucz w PlayerPrefs


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

        // Ustaw kolor w zale¿noœci od trybu
        background.color = currentMode == Mode.Light ? darkMode.lightColor : darkMode.darkColor;
    }
    /*
    private void UpdateModeBasedOnLightLevel()
    {
        // Odczytujemy poziom jasnoœci (tutaj za przyk³ad u¿ywamy wartoœci od 0 do 1)
        float brightnessLevel = GetBrightnessLevel();

        // Na podstawie jasnoœci ustawiamy tryb (mo¿esz dostosowaæ próg, aby dopasowaæ do swoich potrzeb)
        string modeToSave = brightnessLevel < 0.5f ? "Dark" : "Light";

        // Zapisz tryb do PlayerPrefs
        PlayerPrefs.SetString(ModeKey, modeToSave);
        PlayerPrefs.Save();

        // Zaktualizuj kolor t³a na podstawie nowego trybu
        UpdateBackgroundColor(modeToSave);
    }
    */

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