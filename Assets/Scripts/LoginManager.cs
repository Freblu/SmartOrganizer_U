using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

public class LoginManager : MonoBehaviour
{
    private FingerprintAuthentication fingerprintAuthentication;

    public string registerURL = "http://localhost/reg_user.php"; // Œcie¿ka do Twojego skryptu PHP

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
        fingerprintAuthentication = gameObject.AddComponent<FingerprintAuthentication>();

        // Sprawdzenie dostêpnoœci logowania liniami papilarnymi
        if (!fingerprintAuthentication.IsFingerprintAvailable())
        {
            fingerprintButton.interactable = false;
            Debug.Log("Fingerprint authentication is not available.");
        }
        else
        {
            Debug.Log("Fingerprint authentication is available!");
        }

        // Wymuszenie domyœlnego stanu paneli
        DeactivateAllPanels();

        if (PlayerPrefs.HasKey("NewUserPanelActive") && PlayerPrefs.GetString("NewUserPanelActive") == "true")
        {
            ShowNewUserPanel();
            PlayerPrefs.DeleteKey("NewUserPanelActive");

            string lastAddedUser = PlayerPrefs.GetString("LastAddedUser", "");
            if (!string.IsNullOrEmpty(lastAddedUser))
            {
                usernameInputNewUser.text = lastAddedUser;
                passwordInputNewUser.text = ""; // Wyczyszczenie pola has³a
                Debug.Log($"Ostatnio dodany u¿ytkownik: {lastAddedUser}");
            }
        }
        else
        {
            ShowExistingUserPanel();
        }

        // Ukrycie komunikatów o b³êdach
        ClearStatusMessages();

        // Przypisanie funkcji do przycisków
        AssignButtonListeners();
    }

    private void AssignButtonListeners()
    {
        switchToNewUserButton.onClick.AddListener(ShowNewUserPanel);
        switchToExistingUserButton.onClick.AddListener(ShowExistingUserPanel);
        loginButton.onClick.AddListener(LoginWithPassword);
        fingerprintButton.onClick.AddListener(LoginWithFingerprint);
        saveNewUserButton.onClick.AddListener(SaveNewUser);
        setFingerprintButton.onClick.AddListener(SetFingerprint);
    }

    private void DeactivateAllPanels()
    {
        newUserPanel.SetActive(false);
        existingUserPanel.SetActive(false);
    }

    public void ShowNewUserPanel()
    {
        DeactivateAllPanels();
        newUserPanel.SetActive(true);
        ClearStatusMessages();
    }

    public void ShowExistingUserPanel()
    {
        DeactivateAllPanels();
        existingUserPanel.SetActive(true);
        ClearStatusMessages();
    }

    public void SaveNewUser()
    {
        string username = usernameInputNewUser.text.Trim();
        string password = passwordInputNewUser.text.Trim();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            DisplayError("Proszê wype³niæ wszystkie pola!");
            return;
        }

        if (PlayerPrefs.HasKey($"User_{username}"))
        {
            DisplayError("U¿ytkownik o tej nazwie ju¿ istnieje!");
            return;
        }

        PlayerPrefs.SetString($"User_{username}", password);
        PlayerPrefs.SetString("LastAddedUser", username);
        PlayerPrefs.Save();

        DisplaySuccess("Nowy u¿ytkownik zosta³ zapisany. Mo¿esz siê teraz zalogowaæ.");
    }

    public void SetFingerprint()
    {
        isFingerprintSet = true;
        DisplaySuccess("Linie papilarne zosta³y zapisane.");
    }

    public void LoginWithPassword()
    {
        ClearStatusMessages();

        string enteredUsername = usernameInputExistingUser.text.Trim();
        string enteredPassword = passwordInputExistingUser.text.Trim();

        if (PlayerPrefs.HasKey($"User_{enteredUsername}") &&
            PlayerPrefs.GetString($"User_{enteredUsername}") == enteredPassword)
        {
            DisplaySuccess("Logowanie zakoñczone sukcesem!");
            LoadCalendarScene();
        }
        else
        {
            DisplayError("B³êdne dane logowania. Spróbuj ponownie.");
        }
    }

    public void LoginWithFingerprint()
    {
        ClearStatusMessages();

        if (isFingerprintSet && fingerprintAuthentication.AuthenticateWithFingerprint())
        {
            DisplaySuccess("Logowanie liniami papilarnymi zakoñczone sukcesem!");
            LoadCalendarScene();
        }
        else
        {
            DisplayError("Brak zapisanych linii papilarnych lub logowanie nie powiod³o siê.");
        }
    }

    private void LoadCalendarScene()
    {
        SceneManager.LoadScene("CalendarScene");
    }

    private void ClearStatusMessages()
    {
        successText.gameObject.SetActive(false);
        errorText.gameObject.SetActive(false);
    }

    private void DisplaySuccess(string message)
    {
        successText.text = message;
        successText.gameObject.SetActive(true);
        errorText.gameObject.SetActive(false);
    }

    private void DisplayError(string message)
    {
        errorText.text = message;
        errorText.gameObject.SetActive(true);
        successText.gameObject.SetActive(false);
    }
}
