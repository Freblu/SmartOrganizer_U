using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoginManager : MonoBehaviour
{
    public GameObject newUserPanel; // Panel dla nowego u¿ytkownika
    public GameObject existingUserPanel; // Panel dla sta³ego u¿ytkownika

    public Button switchToNewUserButton;
    public Button switchToExistingUserButton;
    public Button loginButton;
    public Button fingerprintButton; // Przycisk symuluj¹cy logowanie liniami papilarnymi
    public Button saveNewUserButton;
    public Button setFingerprintButton;

    public TMP_InputField passwordInputNewUser;
    public TMP_InputField passwordInputExistingUser;
    public TMP_Text successText; // Pole tekstowe dla komunikatu o sukcesie
    public TMP_Text errorText;   // Pole tekstowe dla komunikatu o b³êdzie

    private string storedPassword = ""; // Zmienna do przechowywania has³a u¿ytkownika
    private bool isFingerprintSet = false; // Czy linie papilarne s¹ zapisane

    private void Start()
    {
        // Ukryj komunikaty o sukcesie i b³êdzie na pocz¹tku
        successText.gameObject.SetActive(false);
        errorText.gameObject.SetActive(false);

        ShowNewUserPanel(); // Domyœlnie pokazujemy widok nowego u¿ytkownika

        // Przypisywanie metod do przycisków
        switchToNewUserButton.onClick.AddListener(ShowNewUserPanel);
        switchToExistingUserButton.onClick.AddListener(ShowExistingUserPanel);
        loginButton.onClick.AddListener(LoginWithPassword);
        fingerprintButton.onClick.AddListener(LoginWithFingerprint);
        saveNewUserButton.onClick.AddListener(SaveNewUser);
        setFingerprintButton.onClick.AddListener(SetFingerprint);
    }

    // Funkcja wyœwietlaj¹ca panel nowego u¿ytkownika
    public void ShowNewUserPanel()
    {
        newUserPanel.SetActive(true);
        existingUserPanel.SetActive(false);
        ClearStatusMessages(); // Wyczyœæ komunikaty przy prze³¹czaniu paneli
    }

    // Funkcja wyœwietlaj¹ca panel sta³ego u¿ytkownika
    public void ShowExistingUserPanel()
    {
        newUserPanel.SetActive(false);
        existingUserPanel.SetActive(true);
        ClearStatusMessages(); // Wyczyœæ komunikaty przy prze³¹czaniu paneli
    }

    // Funkcja ustawiaj¹ca has³o i linie papilarne dla nowego u¿ytkownika
    public void SaveNewUser()
    {
        storedPassword = passwordInputNewUser.text;
        isFingerprintSet = true; // Zak³adamy, ¿e linie papilarne zosta³y skonfigurowane
        successText.text = "U¿ytkownik skonfigurowany. U¿yj has³a lub linii papilarnych do logowania.";
        successText.gameObject.SetActive(true); // Poka¿ komunikat o sukcesie
        errorText.gameObject.SetActive(false); // Ukryj komunikat o b³êdzie
    }

    // Funkcja symuluj¹ca zapisanie linii papilarnych
    public void SetFingerprint()
    {
        isFingerprintSet = true;
        successText.text = "Linie papilarne zosta³y zapisane.";
        successText.gameObject.SetActive(true); // Poka¿ komunikat o sukcesie
        errorText.gameObject.SetActive(false); // Ukryj komunikat o b³êdzie
    }

    // Funkcja logowania has³em
    public void LoginWithPassword()
    {
        ClearStatusMessages(); // Wyczyœæ poprzednie komunikaty

        if (passwordInputExistingUser.text == storedPassword)
        {
            successText.text = "Logowanie zakoñczone sukcesem!";
            successText.gameObject.SetActive(true); // Poka¿ komunikat o sukcesie
            LoadCalendarScene();
        }
        else
        {
            errorText.text = "B³êdne has³o. Spróbuj ponownie.";
            errorText.gameObject.SetActive(true); // Poka¿ komunikat o b³êdzie
        }
    }

    // Funkcja logowania liniami papilarnymi
    public void LoginWithFingerprint()
    {
        ClearStatusMessages(); // Wyczyœæ poprzednie komunikaty

        if (isFingerprintSet)
        {
            successText.text = "Logowanie liniami papilarnymi zakoñczone sukcesem!";
            successText.gameObject.SetActive(true); // Poka¿ komunikat o sukcesie
            LoadCalendarScene();
        }
        else
        {
            errorText.text = "Brak zapisanych linii papilarnych. Skonfiguruj konto najpierw.";
            errorText.gameObject.SetActive(true); // Poka¿ komunikat o b³êdzie
        }
    }

    // Funkcja ³adowania sceny kalendarza
    private void LoadCalendarScene()
    {
        SceneManager.LoadScene("CalendarScene");
    }

    // Funkcja do wyczyszczenia komunikatów o b³êdzie i sukcesie
    private void ClearStatusMessages()
    {
        successText.gameObject.SetActive(false);
        errorText.gameObject.SetActive(false);
    }
}