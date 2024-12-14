using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    public Button changeThemeButton; // Przycisk zmiany motywu
    public Button backButton;        // Przycisk powrotu do kalendarza
    public Button logoutButton;      // Przycisk wylogowania
    public TMP_Text statusText;      // Pole tekstowe dla komunikat�w
    public TMP_Dropdown reminderDropdown; // Lista rozwijana dla przypomnie�

    private bool isDarkTheme; // Flaga dla zmiany motywu

    private void Start()
    {
        // Wczytaj aktualny stan motywu z PlayerPrefs
        isDarkTheme = PlayerPrefs.GetInt("IsDarkTheme", 0) == 1;

        // Przypisz funkcje do przycisk�w
        changeThemeButton.onClick.AddListener(ChangeTheme);
        backButton.onClick.AddListener(BackToCalendar);
        logoutButton.onClick.AddListener(Logout);

        // Ustaw aktualny motyw
        ApplyTheme();

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
    }

    // Funkcja zmiany motywu
    private void ChangeTheme()
    {
        isDarkTheme = !isDarkTheme;

        // Zapisz motyw w PlayerPrefs
        PlayerPrefs.SetInt("IsDarkTheme", isDarkTheme ? 1 : 0);
        PlayerPrefs.Save();

        // Zastosuj motyw
        ApplyTheme();

        // Opcjonalnie: wy�wietl komunikat w statusText
        if (statusText != null)
        {
            statusText.text = isDarkTheme ? "Motyw: Ciemny" : "Motyw: Jasny";
        }

        Debug.Log("Motyw zmieniony na: " + (isDarkTheme ? "Ciemny" : "Jasny"));
    }

    // Funkcja ustawiaj�ca motyw
    private void ApplyTheme()
    {
        Camera.main.backgroundColor = isDarkTheme ? Color.black : Color.white;

        if (statusText != null)
        {
            statusText.color = isDarkTheme ? Color.white : Color.black;
        }

        Debug.Log("Zastosowano motyw: " + (isDarkTheme ? "Ciemny" : "Jasny"));
    }

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
