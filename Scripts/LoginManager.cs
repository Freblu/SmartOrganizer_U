using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoginManager : MonoBehaviour
{
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
        // Ukryj komunikaty o sukcesie i b��dzie na pocz�tku
        successText.gameObject.SetActive(false);
        errorText.gameObject.SetActive(false);

        ShowNewUserPanel(); // Domy�lnie pokazujemy widok nowego u�ytkownika

        // Przypisywanie metod do przycisk�w
        switchToNewUserButton.onClick.AddListener(ShowNewUserPanel);
        switchToExistingUserButton.onClick.AddListener(ShowExistingUserPanel);
        loginButton.onClick.AddListener(LoginWithPassword);
        fingerprintButton.onClick.AddListener(LoginWithFingerprint);
        saveNewUserButton.onClick.AddListener(SaveNewUser);
        setFingerprintButton.onClick.AddListener(SetFingerprint);
    }

    // Funkcja wy�wietlaj�ca panel nowego u�ytkownika
    public void ShowNewUserPanel()
    {
        newUserPanel.SetActive(true);
        existingUserPanel.SetActive(false);
        ClearStatusMessages(); // Wyczy�� komunikaty przy prze��czaniu paneli
    }

    // Funkcja wy�wietlaj�ca panel sta�ego u�ytkownika
    public void ShowExistingUserPanel()
    {
        newUserPanel.SetActive(false);
        existingUserPanel.SetActive(true);
        ClearStatusMessages(); // Wyczy�� komunikaty przy prze��czaniu paneli
    }

    // Funkcja ustawiaj�ca has�o i linie papilarne dla nowego u�ytkownika
    public void SaveNewUser()
    {
        storedPassword = passwordInputNewUser.text;
        isFingerprintSet = true; // Zak�adamy, �e linie papilarne zosta�y skonfigurowane
        successText.text = "U�ytkownik skonfigurowany. U�yj has�a lub linii papilarnych do logowania.";
        successText.gameObject.SetActive(true); // Poka� komunikat o sukcesie
        errorText.gameObject.SetActive(false); // Ukryj komunikat o b��dzie
    }

    // Funkcja symuluj�ca zapisanie linii papilarnych
    public void SetFingerprint()
    {
        isFingerprintSet = true;
        successText.text = "Linie papilarne zosta�y zapisane.";
        successText.gameObject.SetActive(true); // Poka� komunikat o sukcesie
        errorText.gameObject.SetActive(false); // Ukryj komunikat o b��dzie
    }

    // Funkcja logowania has�em
    public void LoginWithPassword()
    {
        ClearStatusMessages(); // Wyczy�� poprzednie komunikaty

        if (passwordInputExistingUser.text == storedPassword)
        {
            successText.text = "Logowanie zako�czone sukcesem!";
            successText.gameObject.SetActive(true); // Poka� komunikat o sukcesie
            LoadCalendarScene();
        }
        else
        {
            errorText.text = "B��dne has�o. Spr�buj ponownie.";
            errorText.gameObject.SetActive(true); // Poka� komunikat o b��dzie
        }
    }

    // Funkcja logowania liniami papilarnymi
    public void LoginWithFingerprint()
    {
        ClearStatusMessages(); // Wyczy�� poprzednie komunikaty

        if (isFingerprintSet)
        {
            successText.text = "Logowanie liniami papilarnymi zako�czone sukcesem!";
            successText.gameObject.SetActive(true); // Poka� komunikat o sukcesie
            LoadCalendarScene();
        }
        else
        {
            errorText.text = "Brak zapisanych linii papilarnych. Skonfiguruj konto najpierw.";
            errorText.gameObject.SetActive(true); // Poka� komunikat o b��dzie
        }
    }

    // Funkcja �adowania sceny kalendarza
    private void LoadCalendarScene()
    {
        SceneManager.LoadScene("CalendarScene");
    }

    // Funkcja do wyczyszczenia komunikat�w o b��dzie i sukcesie
    private void ClearStatusMessages()
    {
        successText.gameObject.SetActive(false);
        errorText.gameObject.SetActive(false);
    }
}