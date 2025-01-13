using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

public class LoginManager : MonoBehaviour
{
    public string registerURL = "http://localhost/reg_user.php"; // Œcie¿ka do Twojego skryptu PHP

    public GameObject newUserPanel; // Panel dla nowego u¿ytkownika
    public GameObject existingUserPanel; // Panel dla istniej¹cego u¿ytkownika

    public Button switchToNewUserButton;
    public Button switchToExistingUserButton;
    public Button loginButton;
    public Button saveNewUserButton;

    public TMP_InputField usernameInputNewUser; // Pole wprowadzania loginu nowego u¿ytkownika
    public TMP_InputField passwordInputNewUser; // Pole wprowadzania has³a nowego u¿ytkownika
    public TMP_InputField usernameInputExistingUser; // Pole wprowadzania loginu istniej¹cego u¿ytkownika
    public TMP_InputField passwordInputExistingUser; // Pole wprowadzania has³a istniej¹cego u¿ytkownika
    public TMP_Text successText; // Pole tekstowe dla komunikatu o sukcesie
    public TMP_Text errorText;   // Pole tekstowe dla komunikatu o b³êdzie

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

    // Pokazuje panel istniej¹cego u¿ytkownika
    public void ShowExistingUserPanel()
    {
        DeactivateAllPanels();
        existingUserPanel.SetActive(true);
        ClearStatusMessages();
    }

    // Zapisanie danych nowego u¿ytkownika
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

        successText.text = "Nowy u¿ytkownik zosta³ zapisany. Mo¿esz siê teraz zalogowaæ.";
        successText.gameObject.SetActive(true);
        errorText.gameObject.SetActive(false);

        // Opcjonalnie prze³¹cz na panel logowania
        ShowExistingUserPanel();
    }

    // Logowanie has³em
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
