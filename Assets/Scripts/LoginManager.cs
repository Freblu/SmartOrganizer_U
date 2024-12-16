using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoginManager : MonoBehaviour
{
    private FingerprintAuthentication fingerprintAuthentication;

    public GameObject newUserPanel; // Panel dla nowego u�ytkownika
    public GameObject existingUserPanel; // Panel dla sta�ego u�ytkownika

    public Button switchToNewUserButton;
    public Button switchToExistingUserButton;
    public Button loginButton;
    public Button fingerprintButton; // Przycisk symuluj�cy logowanie liniami papilarnymi
    public Button saveNewUserButton;
    public Button setFingerprintButton;

    public TMP_InputField usernameInputNewUser; // Pole wprowadzania loginu nowego u�ytkownika
    public TMP_InputField passwordInputNewUser; // Pole wprowadzania has�a nowego u�ytkownika
    public TMP_InputField usernameInputExistingUser; // Pole wprowadzania loginu istniej�cego u�ytkownika
    public TMP_InputField passwordInputExistingUser; // Pole wprowadzania has�a istniej�cego u�ytkownika
    public TMP_Text successText; // Pole tekstowe dla komunikatu o sukcesie
    public TMP_Text errorText;   // Pole tekstowe dla komunikatu o b��dzie

    private string storedUsername = ""; // Zmienna do przechowywania loginu u�ytkownika
    private string storedPassword = ""; // Zmienna do przechowywania has�a u�ytkownika
    private bool isFingerprintSet = false; // Czy linie papilarne s� zapisane

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

        // Wymuszamy domy�lny stan paneli
        newUserPanel.SetActive(false);
        existingUserPanel.SetActive(false);

        // Sprawd�, czy NewUserPanel powinien by� aktywowany
        if (PlayerPrefs.HasKey("NewUserPanelActive") && PlayerPrefs.GetString("NewUserPanelActive") == "true")
        {
            ShowNewUserPanel();
            PlayerPrefs.DeleteKey("NewUserPanelActive");

            string lastAddedUser = PlayerPrefs.GetString("LastAddedUser", "");
            if (!string.IsNullOrEmpty(lastAddedUser))
            {
                passwordInputNewUser.text = ""; // Wyczyszczenie pola has�a
                Debug.Log($"Ostatnio dodany u�ytkownik: {lastAddedUser}");
            }
        }
        else
        {
            ShowExistingUserPanel();
        }

        // Ukryj komunikaty
        successText.gameObject.SetActive(false);
        errorText.gameObject.SetActive(false);

        // Przypisanie funkcji do przycisk�w
        switchToNewUserButton.onClick.AddListener(ShowNewUserPanel);
        switchToExistingUserButton.onClick.AddListener(ShowExistingUserPanel);
        loginButton.onClick.AddListener(LoginWithPassword);
        fingerprintButton.onClick.AddListener(LoginWithFingerprint);
        saveNewUserButton.onClick.AddListener(SaveNewUser);
        setFingerprintButton.onClick.AddListener(SetFingerprint);
    }

    // Funkcja wy��czaj�ca wszystkie panele
    private void DeactivateAllPanels()
    {
        newUserPanel.SetActive(false);
        existingUserPanel.SetActive(false);
    }

    // Pokazuje panel nowego u�ytkownika
    public void ShowNewUserPanel()
    {
        DeactivateAllPanels();
        newUserPanel.SetActive(true);
        ClearStatusMessages();
    }

    // Pokazuje panel sta�ego u�ytkownika
    public void ShowExistingUserPanel()
    {
        DeactivateAllPanels();
        existingUserPanel.SetActive(true);
        ClearStatusMessages();
    }

    // Zapisanie danych nowego u�ytkownika
    public void SaveNewUser()
    {
        string username = passwordInputNewUser.text.Trim();
        string password = passwordInputNewUser.text.Trim();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            errorText.text = "Prosz� wype�ni� wszystkie pola!";
            errorText.gameObject.SetActive(true);
            return;
        }

        if (PlayerPrefs.HasKey($"User_{username}"))
        {
            errorText.text = "U�ytkownik o tej nazwie ju� istnieje!";
            errorText.gameObject.SetActive(true);
            return;
        }

        PlayerPrefs.SetString($"User_{username}", password);
        PlayerPrefs.Save();

        successText.text = "Nowy u�ytkownik zosta� zapisany. Mo�esz si� teraz zalogowa�.";
        successText.gameObject.SetActive(true);
        errorText.gameObject.SetActive(false);
    }

    // Symuluje ustawienie linii papilarnych
    public void SetFingerprint()
    {
        isFingerprintSet = true;
        successText.text = "Linie papilarne zosta�y zapisane.";
        successText.gameObject.SetActive(true);
        errorText.gameObject.SetActive(false);
    }

    // Logowanie has�em
    public void LoginWithPassword()
    {
        ClearStatusMessages();

        string enteredUsername = passwordInputExistingUser.text.Trim();
        string enteredPassword = passwordInputExistingUser.text.Trim();

        if (PlayerPrefs.HasKey($"User_{enteredUsername}") &&
            PlayerPrefs.GetString($"User_{enteredUsername}") == enteredPassword)
        {
            successText.text = "Logowanie zako�czone sukcesem!";
            successText.gameObject.SetActive(true);
            LoadCalendarScene();
        }
        else
        {
            errorText.text = "B��dne dane logowania. Spr�buj ponownie.";
            errorText.gameObject.SetActive(true);
        }
    }


    // Logowanie liniami papilarnymi
    public void LoginWithFingerprint()
    {
        ClearStatusMessages();

        if (isFingerprintSet && fingerprintAuthentication.AuthenticateWithFingerprint())
        {
            successText.text = "Logowanie liniami papilarnymi zako�czone sukcesem!";
            successText.gameObject.SetActive(true);
            LoadCalendarScene();
        }
        else
        {
            errorText.text = "Brak zapisanych linii papilarnych lub logowanie nie powiod�o si�.";
            errorText.gameObject.SetActive(true);
        }
    }

    // �adowanie sceny kalendarza
    private void LoadCalendarScene()
    {
        SceneManager.LoadScene("CalendarScene");
    }

    // Czyszczenie komunikat�w
    private void ClearStatusMessages()
    {
        successText.gameObject.SetActive(false);
        errorText.gameObject.SetActive(false);
    }
}
