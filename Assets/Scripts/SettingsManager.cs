using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    public Button changeThemeButton; // Przycisk zmiany motywu
    public Button backButton;        // Przycisk powrotu do kalendarza
    public TMP_Text statusText;      // Pole tekstowe dla komunikatów (opcjonalne)

    private bool isDarkTheme; // Flaga dla zmiany motywu

    private void Start()
    {
        // Wczytaj aktualny stan motywu z PlayerPrefs
        isDarkTheme = PlayerPrefs.GetInt("IsDarkTheme", 0) == 1;

        // Przypisz funkcje do przycisków
        changeThemeButton.onClick.AddListener(ChangeTheme);
        backButton.onClick.AddListener(BackToCalendar);

        // Ustaw aktualny motyw
        ApplyTheme();

        // Opcjonalnie: wyczyœæ status na pocz¹tku
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

        // Opcjonalnie: wyœwietl komunikat w statusText
        if (statusText != null)
        {
            statusText.text = isDarkTheme ? "Motyw: Ciemny" : "Motyw: Jasny";
        }

        Debug.Log("Motyw zmieniony na: " + (isDarkTheme ? "Ciemny" : "Jasny"));
    }

    // Funkcja ustawiaj¹ca motyw
    private void ApplyTheme()
    {
        // Przyk³ad zmiany koloru t³a
        Camera.main.backgroundColor = isDarkTheme ? Color.black : Color.white;

        // Mo¿esz dodaæ inne elementy zmieniaj¹ce wygl¹d motywu
        Debug.Log("Zastosowano motyw: " + (isDarkTheme ? "Ciemny" : "Jasny"));
    }

    // Funkcja powrotu do kalendarza
    private void BackToCalendar()
    {
        SceneManager.LoadScene("CalendarScene");
    }
}