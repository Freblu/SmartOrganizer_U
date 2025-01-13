using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/**
 * @class LoginManager
 * @brief Klasa odpowiedzialna za zarz¹dzanie logowaniem i rejestracj¹ u¿ytkowników.
 * 
 * Klasa umo¿liwia u¿ytkownikowi logowanie za pomoc¹ loginu i has³a oraz tworzenie nowych kont u¿ytkowników.
 */
public class LoginManager : MonoBehaviour
{
    public string registerURL = "http://localhost/reg_user.php"; ///< Œcie¿ka do skryptu PHP obs³uguj¹cego rejestracjê u¿ytkowników.

    public GameObject newUserPanel; ///< Panel dla nowego u¿ytkownika.
    public GameObject existingUserPanel; ///< Panel dla istniej¹cego u¿ytkownika.

    public Button switchToNewUserButton; ///< Przycisk prze³¹czaj¹cy na panel rejestracji nowego u¿ytkownika.
    public Button switchToExistingUserButton; ///< Przycisk prze³¹czaj¹cy na panel logowania istniej¹cego u¿ytkownika.
    public Button loginButton; ///< Przycisk logowania.
    public Button saveNewUserButton; ///< Przycisk zapisu nowego u¿ytkownika.

    public TMP_InputField usernameInputNewUser; ///< Pole tekstowe do wprowadzania loginu nowego u¿ytkownika.
    public TMP_InputField passwordInputNewUser; ///< Pole tekstowe do wprowadzania has³a nowego u¿ytkownika.
    public TMP_InputField usernameInputExistingUser; ///< Pole tekstowe do wprowadzania loginu istniej¹cego u¿ytkownika.
    public TMP_InputField passwordInputExistingUser; ///< Pole tekstowe do wprowadzania has³a istniej¹cego u¿ytkownika.
    public TMP_Text successText; ///< Pole tekstowe wyœwietlaj¹ce komunikaty o sukcesie.
    public TMP_Text errorText; ///< Pole tekstowe wyœwietlaj¹ce komunikaty o b³êdach.

    /**
     * @brief Metoda inicjalizuj¹ca klasê LoginManager.
     * 
     * Ustawia pocz¹tkowe stany paneli, ukrywa komunikaty oraz przypisuje funkcje do przycisków.
     */
    private void Start()
    {
        // Ukryj oba panele na starcie
        newUserPanel.SetActive(false);
        existingUserPanel.SetActive(false);

        // SprawdŸ, czy NewUserPanel powinien byæ aktywny
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
        loginButton.onClick.AddListener(LoginWithPassword);
        saveNewUserButton.onClick.AddListener(SaveNewUser);
    }

    /**
     * @brief Wy³¹cza wszystkie panele.
     * 
     * U¿ywane do ukrycia zarówno panelu rejestracji, jak i panelu logowania.
     */
    private void DeactivateAllPanels()
    {
        newUserPanel.SetActive(false);
        existingUserPanel.SetActive(false);
    }


    /**
  * @brief Pokazuje panel nowego u¿ytkownika.
  * 
  * Wy³¹cza wszystkie aktywne panele i w³¹cza panel dla nowego u¿ytkownika.
  * Czyœci wszystkie komunikaty statusowe, aby przygotowaæ interfejs do nowej konfiguracji.
  */
    public void ShowNewUserPanel()
    {
        DeactivateAllPanels();
        newUserPanel.SetActive(true);
        ClearStatusMessages();
    }

    /**
     * @brief Pokazuje panel istniej¹cego u¿ytkownika.
     * 
     * Wy³¹cza wszystkie aktywne panele i w³¹cza panel logowania dla istniej¹cego u¿ytkownika.
     * Czyœci wszystkie komunikaty statusowe, aby przygotowaæ interfejs do logowania.
     */
    public void ShowExistingUserPanel()
    {
        DeactivateAllPanels();
        existingUserPanel.SetActive(true);
        ClearStatusMessages();
    }

    /**
     * @brief Zapisuje dane nowego u¿ytkownika.
     * 
     * Waliduje dane wejœciowe, takie jak nazwa u¿ytkownika i has³o. Jeœli dane s¹ poprawne, 
     * zapisuje je w `PlayerPrefs`. Wyœwietla odpowiedni komunikat statusowy i opcjonalnie 
     * prze³¹cza u¿ytkownika na panel logowania.
     */
    public void SaveNewUser()
    {
        string username = usernameInputNewUser.text.Trim();
        string password = passwordInputNewUser.text.Trim();

        // Walidacja danych wejœciowych
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

        // Zapis danych u¿ytkownika
        PlayerPrefs.SetString($"User_{username}", password);
        PlayerPrefs.SetString("LastAddedUser", username); // Opcjonalnie do póŸniejszego u¿ycia
        PlayerPrefs.Save();

        successText.text = $"Nowy u¿ytkownik '{username}' zosta³ skonfigurowany poprawnie.";
        successText.gameObject.SetActive(true);
        errorText.gameObject.SetActive(false);

        // Opcjonalnie prze³¹cz na panel logowania
        ShowExistingUserPanel();
    }

    /**
     * @brief Logowanie u¿ytkownika za pomoc¹ loginu i has³a.
     * 
     * Sprawdza, czy wprowadzone dane logowania s¹ zgodne z zapisanymi w `PlayerPrefs`.
     * Jeœli dane s¹ poprawne, ³aduje scenê kalendarza. W przypadku b³êdu wyœwietla odpowiedni komunikat.
     */
    public void LoginWithPassword()
    {
        ClearStatusMessages();

        string enteredUsername = usernameInputExistingUser.text.Trim();
        string enteredPassword = passwordInputExistingUser.text.Trim();

        if (string.IsNullOrEmpty(enteredUsername) || string.IsNullOrEmpty(enteredPassword))
        {
            errorText.text = "Proszê wype³niæ wszystkie pola!";
            errorText.gameObject.SetActive(true);
            return;
        }

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

    /**
     * @brief £aduje scenê kalendarza.
     * 
     * Prze³¹cza u¿ytkownika na scenê o nazwie "CalendarScene", w której znajduj¹ siê funkcje zarz¹dzania kalendarzem.
     */
    private void LoadCalendarScene()
    {
        SceneManager.LoadScene("CalendarScene");
    }

    /**
     * @brief Czyœci wszystkie komunikaty statusowe.
     * 
     * Wy³¹cza wyœwietlanie tekstów sukcesu i b³êdów w interfejsie u¿ytkownika.
     */
    private void ClearStatusMessages()
    {
        successText.gameObject.SetActive(false);
        errorText.gameObject.SetActive(false);
    }

}
