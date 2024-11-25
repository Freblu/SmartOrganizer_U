using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    public Button changeThemeButton; // Przycisk zmiany motywu
    public Button backButton;        // Przycisk powrotu do kalendarza
    public TMP_Text statusText;      // Pole tekstowe dla komunikat�w (opcjonalne)

    private bool isDarkTheme; // Flaga dla zmiany motywu

    private void Start()
    {
        // Wczytaj aktualny stan motywu z PlayerPrefs
        isDarkTheme = PlayerPrefs.GetInt("IsDarkTheme", 0) == 1;

        // Przypisz funkcje do przycisk�w
        changeThemeButton.onClick.AddListener(ChangeTheme);
        backButton.onClick.AddListener(BackToCalendar);

        // Ustaw aktualny motyw
        ApplyTheme();

        // Opcjonalnie: wyczy�� status na pocz�tku
        if (statusText != null)
        {
            statusText.text = "";
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
        // Przyk�ad zmiany koloru t�a
        Camera.main.backgroundColor = isDarkTheme ? Color.black : Color.white;

        // Mo�esz doda� inne elementy zmieniaj�ce wygl�d motywu
        Debug.Log("Zastosowano motyw: " + (isDarkTheme ? "Ciemny" : "Jasny"));
    }

    // Funkcja powrotu do kalendarza
    private void BackToCalendar()
    {
        SceneManager.LoadScene("CalendarScene");
    }
}