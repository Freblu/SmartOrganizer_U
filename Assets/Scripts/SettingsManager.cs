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
    public TMP_Text statusText;      // Pole tekstowe dla komunikat�w
    public TMP_Dropdown reminderDropdown; // Lista rozwijana dla przypomnie�
    private bool isDarkTheme; // Flaga dla zmiany motywu

    private void Start()
    {
        // Wczytaj aktualny stan motywu z PlayerPrefs
        int savedMode = PlayerPrefs.GetInt(ModeKey, (int)Mode.Light);
        // Przypisz funkcje do przycisk�w
        changeThemeButton.onClick.AddListener(ToggleMode);
        backButton.onClick.AddListener(BackToCalendar);
        logoutButton.onClick.AddListener(Logout);

        // Opcjonalnie: wyczy�� status na pocz�tku
        if (statusText != null)
        {
            statusText.text = "";
        }

        // Ustawienia domy�lne dla przypomnie�
        if (reminderDropdown != null)
        {
            reminderDropdown.onValueChanged.AddListener(SetReminder);
            reminderDropdown.value = PlayerPrefs.GetInt("ReminderTime", 0); // Wczytaj zapisane ustawienie przypomnienia
        }


        // Ustaw kolory HEX
        darkMode.SetColors("#81D0FF", "#005587"); // Bia�y dla Light, czarny dla Dark

        // Za�aduj zapisany tryb (domy�lnie Light)
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
        // Prze��cz tryb
        currentMode = currentMode == Mode.Light ? Mode.Dark : Mode.Light;

        // Zapisz tryb w PlayerPrefs
        PlayerPrefs.SetInt(ModeKey, (int)currentMode);
        PlayerPrefs.Save();

        // Zaktualizuj kolor t�a
        UpdateBackgroundColor();
    }

    private void UpdateBackgroundColor()
    {
        if (background == null)
        {
            Debug.LogError("Background Image is not assigned!");
            return;
        }

        // Ustaw kolor w zale�no�ci od trybu
        background.color = currentMode == Mode.Light ? darkMode.lightColor : darkMode.darkColor;
    }
    /*
    private void UpdateModeBasedOnLightLevel()
    {
        // Odczytujemy poziom jasno�ci (tutaj za przyk�ad u�ywamy warto�ci od 0 do 1)
        float brightnessLevel = GetBrightnessLevel();

        // Na podstawie jasno�ci ustawiamy tryb (mo�esz dostosowa� pr�g, aby dopasowa� do swoich potrzeb)
        string modeToSave = brightnessLevel < 0.5f ? "Dark" : "Light";

        // Zapisz tryb do PlayerPrefs
        PlayerPrefs.SetString(ModeKey, modeToSave);
        PlayerPrefs.Save();

        // Zaktualizuj kolor t�a na podstawie nowego trybu
        UpdateBackgroundColor(modeToSave);
    }
    */

    // Funkcja ustawiaj�ca przypomnienia
    private void SetReminder(int optionIndex)
    {
        PlayerPrefs.SetInt("ReminderTime", optionIndex);
        PlayerPrefs.Save();

        string reminderText = optionIndex switch
        {
            0 => "1 godzina przed spotkaniem",
            1 => "2 godziny przed spotkaniem",
            2 => "15 minut przed spotkaniem",
            3 => "W momencie rozpocz�cia spotkania",
            _ => "Brak przypomnienia"
        };

        if (statusText != null)
        {
            statusText.text = $"Ustawiono przypomnienie: {reminderText}";
        }

        Debug.Log($"Wybrano opcj� przypomnienia: {reminderText}");
    }

    // Funkcja powrotu do kalendarza
    private void BackToCalendar()
    {
        SceneManager.LoadScene("CalendarScene");
    }

    // Funkcja wylogowania
    private void Logout()
    {
        PlayerPrefs.DeleteAll(); // Usuni�cie zapisanych danych (opcjonalne)
        SceneManager.LoadScene("LoginScene"); // Przejd� do strony logowania
    }
}