using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

/**
 * @class LoginManager
 * @brief Klasa odpowiedzialna za zarządzanie logowaniem i rejestracją użytkowników.
 * 
 * Klasa umożliwia użytkownikowi logowanie za pomocą loginu i hasła oraz tworzenie nowych kont użytkowników.
 */
public class LoginManager : MonoBehaviour
{
    public string registerURL = "http://localhost/reg_user.php"; ///< Ścieżka do skryptu PHP obsługującego rejestrację użytkowników.

    public GameObject newUserPanel; ///< Panel dla nowego użytkownika.
    public GameObject existingUserPanel; ///< Panel dla istniejącego użytkownika.

    public Button switchToNewUserButton; ///< Przycisk przełączający na panel rejestracji nowego użytkownika.
    public Button switchToExistingUserButton; ///< Przycisk przełączający na panel logowania istniejącego użytkownika.
    public Button loginButton; ///< Przycisk logowania.
    public Button saveNewUserButton; ///< Przycisk zapisu nowego użytkownika.

    public TMP_InputField usernameInputNewUser; ///< Pole tekstowe do wprowadzania loginu nowego użytkownika.
    public TMP_InputField passwordInputNewUser; ///< Pole tekstowe do wprowadzania hasła nowego użytkownika.
    public TMP_InputField usernameInputExistingUser; ///< Pole tekstowe do wprowadzania loginu istniejącego użytkownika.
    public TMP_InputField passwordInputExistingUser; ///< Pole tekstowe do wprowadzania hasła istniejącego użytkownika.
    public TMP_Text successText; ///< Pole tekstowe wyświetlające komunikaty o sukcesie.
    public TMP_Text errorText; ///< Pole tekstowe wyświetlające komunikaty o błędach.
    public TMP_Text batteryStatusText; ///< Pole tekstowe do wyświetlania statusu baterii.
    /**
     * @brief Metoda inicjalizująca klasę LoginManager.
     * 
     * Ustawia początkowe stany paneli, ukrywa komunikaty oraz przypisuje funkcje do przycisków.
     */
    private void Start()
    {
        // Ukryj oba panele na starcie
        newUserPanel.SetActive(false);
        existingUserPanel.SetActive(false);

        // Sprawdź, czy NewUserPanel powinien być aktywny
        if (PlayerPrefs.HasKey("NewUserPanelActive") && PlayerPrefs.GetString("NewUserPanelActive") == "true")
        {
            ShowNewUserPanel();
            PlayerPrefs.DeleteKey("NewUserPanelActive");
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
        loginButton.onClick.AddListener(SendLogin);
        saveNewUserButton.onClick.AddListener(RegisterUser);
    }

    /**
     * @brief Wyłącza wszystkie panele.
     * 
     * Używane do ukrycia zarówno panelu rejestracji, jak i panelu logowania.
     */
    private void DeactivateAllPanels()
    {
        newUserPanel.SetActive(false);
        existingUserPanel.SetActive(false);
    }


    /**
  * @brief Pokazuje panel nowego użytkownika.
  * 
  * Wyłącza wszystkie aktywne panele i włącza panel dla nowego użytkownika.
  * Czyści wszystkie komunikaty statusowe, aby przygotować interfejs do nowej konfiguracji.
  */
    public void ShowNewUserPanel()
    {
        DeactivateAllPanels();
        newUserPanel.SetActive(true);
        ClearStatusMessages();
    }

    /**
     * @brief Pokazuje panel istniejącego użytkownika.
     * 
     * Wyłącza wszystkie aktywne panele i włącza panel logowania dla istniejącego użytkownika.
     * Czyści wszystkie komunikaty statusowe, aby przygotować interfejs do logowania.
     */
    public void ShowExistingUserPanel()
    {
        DeactivateAllPanels();
        existingUserPanel.SetActive(true);
        ClearStatusMessages();
    }

    /**
     * @brief Zapisuje dane nowego użytkownika.
     * 
     * Waliduje dane wejściowe, takie jak nazwa użytkownika i hasło. Jeśli dane są poprawne, 
     * zapisuje je w `PlayerPrefs`. Wyświetla odpowiedni komunikat statusowy i opcjonalnie 
     * przełącza użytkownika na panel logowania.
     */
    public void SaveNewUser()
    {
        string username = usernameInputNewUser.text.Trim();
        string password = passwordInputNewUser.text.Trim();

        // Walidacja danych wejściowych
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            errorText.text = "Proszę wypełnić wszystkie pola!";
            errorText.gameObject.SetActive(true);
            return;
        }

        if (PlayerPrefs.HasKey($"User_{username}"))
        {
            errorText.text = "Użytkownik o tej nazwie już istnieje!";
            errorText.gameObject.SetActive(true);
            return;
        }

        // Zapis danych użytkownika
        PlayerPrefs.SetString($"User_{username}", password);
        PlayerPrefs.SetString("LastAddedUser", username); // Opcjonalnie do późniejszego użycia
        PlayerPrefs.Save();

        successText.text = $"Nowy użytkownik '{username}' został skonfigurowany poprawnie.";
        successText.gameObject.SetActive(true);
        errorText.gameObject.SetActive(false);

        // Opcjonalnie przełącz na panel logowania
        ShowExistingUserPanel();
    }

    /**
     * @brief Logowanie użytkownika za pomocą loginu i hasła.
     * 
     * Sprawdza, czy wprowadzone dane logowania są zgodne z zapisanymi w `PlayerPrefs`.
     * Jeśli dane są poprawne, ładuje scenę kalendarza. W przypadku błędu wyświetla odpowiedni komunikat.
     */
    public void LoginWithPassword()
    {
        ClearStatusMessages();

        string enteredUsername = usernameInputExistingUser.text.Trim();
        string enteredPassword = passwordInputExistingUser.text.Trim();

        if (string.IsNullOrEmpty(enteredUsername) || string.IsNullOrEmpty(enteredPassword))
        {
            errorText.text = "Proszę wypełnić wszystkie pola!";
            errorText.gameObject.SetActive(true);
            return;
        }

        if (PlayerPrefs.HasKey($"User_{enteredUsername}") &&
            PlayerPrefs.GetString($"User_{enteredUsername}") == enteredPassword)
        {
            successText.text = "Logowanie zakończone sukcesem!";
            successText.gameObject.SetActive(true);
            LoadCalendarScene();
        }
        else
        {
            errorText.text = "Błędne dane logowania. Spróbuj ponownie.";
            errorText.gameObject.SetActive(true);
        }
    }

    public void RegisterUser()
    {
        string username = usernameInputNewUser.text.Trim();
        string password = passwordInputNewUser.text.Trim();
        StartCoroutine(SendRegisterRequest(username, password));
    }

    IEnumerator SendRegisterRequest(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        using (UnityWebRequest www = UnityWebRequest.Post(registerURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("OdpowiedŸ serwera: " + www.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Błąd połączenia: " + www.error);
            }
        }
    }

    ///BAZA DANYCH LOGOWANIE/// 

    IEnumerator Login()
    {
        // Pobierz dane z pól tekstowych
        string enteredUsername = usernameInputExistingUser.text.Trim();
        string enteredPassword = passwordInputExistingUser.text.Trim();

        // Przygotuj dane do wysłania na serwer
        WWWForm form = new WWWForm();
        form.AddField("username", enteredUsername);
        form.AddField("password", enteredPassword);

        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/log_user.php", form))
        {
            // Wyœlij żądanie do serwera
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                // Parsuj odpowiedŸ serwera
                string responseText = www.downloadHandler.text;
                Debug.Log($"OdpowiedŸ serwera: {responseText}");

                try
                {
                    // Zakładamy, że serwer zwraca JSON w formacie:
                    // {"status": "success", "message": "Login successful", "id": 1}
                    var response = JsonUtility.FromJson<ServerResponse>(responseText);

                    if (response.status == "success")
                    {
                        // Ustaw nazwê zalogowanego użytkownika w PlayerPrefs
                        PlayerPrefs.SetString("LoggedInUser", enteredUsername);
                        PlayerPrefs.Save();

                        // Pokaż komunikat sukcesu
                        successText.text = "Logowanie zakoñczone sukcesem!";
                        successText.gameObject.SetActive(true);

                        // Załaduj kolejną scenê (np. kalendarz)
                        LoadCalendarScene();
                    }
                    else
                    {
                        // Obsługa błêdów z serwera (np. nieprawidłowe dane logowania)
                        errorText.text = response.message;
                        errorText.gameObject.SetActive(true);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Błąd parsowania odpowiedzi: {e.Message}");
                    errorText.text = "Wystąpił problem z serwerem. Spróbuj ponownie póŸniej.";
                    errorText.gameObject.SetActive(true);
                }
            }
            else
            {
                // Obsługa błêdów po stronie UnityWebRequest
                Debug.LogError($"Błąd połączenia: {www.error}");
                errorText.text = "Nie udało siê połączyæ z serwerem.";
                errorText.gameObject.SetActive(true);
            }
        }
    }

    // Klasa reprezentująca odpowiedŸ serwera
    [System.Serializable]
    public class ServerResponse
    {
        public string status; // np. "success" lub "error"
        public string message; // Komunikat zwrotny od serwera
        public int id; // Opcjonalnie: ID użytkownika
    }
    public void SendLogin()
    {
        StartCoroutine(Login());
    }
    ///-BAZA DANYCH-///


    private void UpdateBatteryStatus()
    {
        if (SystemInfo.batteryLevel < 0)
        {
            if (batteryStatusText != null)
            {
                batteryStatusText.text = "Poziom baterii niedostępny.";
            }
            return;
        }

        float batteryLevel = SystemInfo.batteryLevel * 100f;
        BatteryStatus batteryStatus = SystemInfo.batteryStatus;

        string statusMessage = $"Poziom baterii: {batteryLevel:0}%\nStatus: {batteryStatus}";

        if (batteryStatusText != null)
        {
            batteryStatusText.text = statusMessage;
        }
    }
    /**
     * @brief Ładuje scenę kalendarza.
     * 
     * Przełącza użytkownika na scenę o nazwie "CalendarScene", w której znajdują się funkcje zarządzania kalendarzem.
     */
    private void LoadCalendarScene()
    {
        SceneManager.LoadScene("CalendarScene");
    }

    /**
     * @brief Czyści wszystkie komunikaty statusowe.
     * 
     * Wyłącza wyświetlanie tekstów sukcesu i błędów w interfejsie użytkownika.
     */
    private void ClearStatusMessages()
    {
        successText.gameObject.SetActive(false);
        errorText.gameObject.SetActive(false);
    }

}
