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

    public TMP_InputField passwordInputNewUser;
    public TMP_InputField passwordInputExistingUser;
    public TMP_Text successText; // Pole tekstowe dla komunikatu o sukcesie
    public TMP_Text errorText;   // Pole tekstowe dla komunikatu o b��dzie

    private string storedPassword = ""; // Zmienna do przechowywania has�a u�ytkownika
    private bool isFingerprintSet = false; // Czy linie papilarne s� zapisane

    private void Start()
    {
        // Inicjalizacja uwierzytelniania linii papilarnych
        fingerprintAuthentication = gameObject.AddComponent<FingerprintAuthentication>();

        if (!fingerprintAuthentication.IsFingerprintAvailable())
        {
            fingerprintButton.interactable = false; // Wy��cz przycisk, je�li linie papilarne nie s� dost�pne
            Debug.Log("Fingerprint authentication is not available.");
        }
        else
        {
            Debug.Log("Fingerprint authentication is available!");
        }

        // Wymuszamy domy�lny stan paneli
        newUserPanel.SetActive(false);
        existingUserPanel.SetActive(false);

        // Ustawienie domy�lnego panelu
        ShowNewUserPanel();

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
        storedPassword = passwordInputNewUser.text;
        isFingerprintSet = true; // Zak�adamy, �e linie papilarne zosta�y skonfigurowane
        successText.text = "U�ytkownik skonfigurowany. U�yj has�a lub linii papilarnych do logowania.";
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

        if (passwordInputExistingUser.text == storedPassword)
        {
            successText.text = "Logowanie zako�czone sukcesem!";
            successText.gameObject.SetActive(true);
            LoadCalendarScene();
        }
        else
        {
            errorText.text = "B��dne has�o. Spr�buj ponownie.";
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
