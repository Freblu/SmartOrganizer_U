using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoginManager : MonoBehaviour
{
    private FingerprintAuthentication fingerprintAuthentication;

    public GameObject newUserPanel; // Panel dla nowego u¿ytkownika
    public GameObject existingUserPanel; // Panel dla sta³ego u¿ytkownika

    public Button switchToNewUserButton;
    public Button switchToExistingUserButton;
    public Button loginButton;
    public Button fingerprintButton; // Przycisk symuluj¹cy logowanie liniami papilarnymi
    public Button saveNewUserButton;
    public Button setFingerprintButton;

    public TMP_InputField usernameInputNewUser; // Pole wprowadzania loginu nowego u¿ytkownika
    public TMP_InputField passwordInputNewUser; // Pole wprowadzania has³a nowego u¿ytkownika
    public TMP_InputField usernameInputExistingUser; // Pole wprowadzania loginu istniej¹cego u¿ytkownika
    public TMP_InputField passwordInputExistingUser; // Pole wprowadzania has³a istniej¹cego u¿ytkownika
    public TMP_Text successText; // Pole tekstowe dla komunikatu o sukcesie
    public TMP_Text errorText;   // Pole tekstowe dla komunikatu o b³êdzie

    private string storedUsername = ""; // Zmienna do przechowywania loginu u¿ytkownika
    private string storedPassword = ""; // Zmienna do przechowywania has³a u¿ytkownika
    private bool isFingerprintSet = false; // Czy linie papilarne s¹ zapisane

    private void Start()
    {
        // Inicjalizacja uwierzytelniania linii papilarnych
        fingerprintAuthentication = gameObject.AddComponent<FingerprintAuthentication>();

        if (!fingerprintAuthentication.IsFingerprintAvailable())
        {
            fingerprintButton.interactable = false;
            Debug.Log("Fingerprint authentication is not available.");
        }
        else
        {
            Debug.Log("Fingerprint authentication is available!");
        }

        // Wymuszamy domyœlny stan paneli
        newUserPanel.SetActive(false);
        existingUserPanel.SetActive(false);

        // SprawdŸ, czy NewUserPanel powinien byæ aktywowany
        if (PlayerPrefs.HasKey("NewUserPanelActive") && PlayerPrefs.GetString("NewUserPanelActive") == "true")
        {
            ShowNewUserPanel();
            PlayerPrefs.DeleteKey("NewUserPanelActive");

            string lastAddedUser = PlayerPrefs.GetString("LastAddedUser", "");
            if (!string.IsNullOrEmpty(lastAddedUser))
            {
                passwordInputNewUser.text = ""; // Wyczyszczenie pola has³a
                Debug.Log($"Ostatnio dodany u¿ytkownik: {lastAddedUser}");
            }
        }
        else
        {
            ShowExistingUserPanel();
        }

        // Ukryj komunikaty
        successText.gameObject.SetActive(false);
        errorText.gameObject.SetActive(false);

        // Przypisanie funkcji do przycisków
        switchToNewUserButton.onClick.AddListener(ShowNewUserPanel);
        switchToExistingUserButton.onClick.AddListener(ShowExistingUserPanel);
        loginButton.onClick.AddListener(LoginWithPassword);
        fingerprintButton.onClick.AddListener(LoginWithFingerprint);
        saveNewUserButton.onClick.AddListener(SaveNewUser);
        setFingerprintButton.onClick.AddListener(SetFingerprint);
    }

    // Funkcja wy³¹czaj¹ca wszystkie panele
    private void DeactivateAllPanels()
    {
        newUserPanel.SetActive(false);
        existingUserPanel.SetActive(false);
    }

    // Pokazuje panel nowego u¿ytkownika
    public void ShowNewUserPanel()
    {
        DeactivateAllPanels();
        newUserPanel.SetActive(true);
        ClearStatusMessages();
    }

    // Pokazuje panel sta³ego u¿ytkownika
    public void ShowExistingUserPanel()
    {
        DeactivateAllPanels();
        existingUserPanel.SetActive(true);
        ClearStatusMessages();
    }

    // Zapisanie danych nowego u¿ytkownika
    public void SaveNewUser()
    {
        string username = passwordInputNewUser.text.Trim();
        string password = passwordInputNewUser.text.Trim();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            errorText.text = "Proszê wype³niæ wszystkie pola!";
            errorText.gameObject.SetActive(true);
            return;
        }

        if (PlayerPrefs.HasKey($"User_{username}"))
        {
            errorText.text = "U¿ytkownik o tej nazwie ju¿ istnieje!";
            errorText.gameObject.SetActive(true);
            return;
        }

        PlayerPrefs.SetString($"User_{username}", password);
        PlayerPrefs.Save();

        successText.text = "Nowy u¿ytkownik zosta³ zapisany. Mo¿esz siê teraz zalogowaæ.";
        successText.gameObject.SetActive(true);
        errorText.gameObject.SetActive(false);
    }

    // Symuluje ustawienie linii papilarnych
    public void SetFingerprint()
    {
        isFingerprintSet = true;
        successText.text = "Linie papilarne zosta³y zapisane.";
        successText.gameObject.SetActive(true);
        errorText.gameObject.SetActive(false);
    }

    // Logowanie has³em
    public void LoginWithPassword()
    {
        ClearStatusMessages();

        string enteredUsername = passwordInputExistingUser.text.Trim();
        string enteredPassword = passwordInputExistingUser.text.Trim();

        if (PlayerPrefs.HasKey($"User_{enteredUsername}") &&
            PlayerPrefs.GetString($"User_{enteredUsername}") == enteredPassword)
        {
            successText.text = "Logowanie zakoñczone sukcesem!";
            successText.gameObject.SetActive(true);
            LoadCalendarScene();
        }
        else
        {
            errorText.text = "B³êdne dane logowania. Spróbuj ponownie.";
            errorText.gameObject.SetActive(true);
        }
    }


    // Logowanie liniami papilarnymi
    public void LoginWithFingerprint()
    {
        ClearStatusMessages();

        if (isFingerprintSet && fingerprintAuthentication.AuthenticateWithFingerprint())
        {
            successText.text = "Logowanie liniami papilarnymi zakoñczone sukcesem!";
            successText.gameObject.SetActive(true);
            LoadCalendarScene();
        }
        else
        {
            errorText.text = "Brak zapisanych linii papilarnych lub logowanie nie powiod³o siê.";
            errorText.gameObject.SetActive(true);
        }
    }

    // £adowanie sceny kalendarza
    private void LoadCalendarScene()
    {
        SceneManager.LoadScene("CalendarScene");
    }

    // Czyszczenie komunikatów
    private void ClearStatusMessages()
    {
        successText.gameObject.SetActive(false);
        errorText.gameObject.SetActive(false);
    }
}
