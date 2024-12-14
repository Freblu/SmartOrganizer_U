using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class DayPlanManager : MonoBehaviour
{
    public TMP_Text dayTitleText;           // Tytu³ dnia (np. "Plan dnia 1")
    public TMP_Text taskListText;          // Pole tekstowe do wyœwietlania zadañ
    public TMP_InputField taskInputField;  // Pole wejœciowe do dodawania nowych zadañ
    public Button addTaskButton;           // Przycisk dodawania zadañ
    public Button clearTasksButton;        // Przycisk usuwania wszystkich zadañ
    public Button saveEditButton;          // Przycisk zapisywania edycji zadania
    public Button backToCalendarButton;    // Przycisk powrotu do kalendarza
    public TMP_Text statusText;            // Pole tekstowe dla komunikatów

    private List<string> tasks = new List<string>(); // Lista zadañ dla bie¿¹cego dnia
    private const string DayPlanKey = "SelectedDay"; // Klucz do odczytu wybranego dnia
    private int selectedDay;                        // Wybrany dzieñ
    private int editIndex = -1;                     // Indeks edytowanego zadania (-1 oznacza brak edycji)

    private void Start()
    {
        // Pobierz numer wybranego dnia z PlayerPrefs
        selectedDay = PlayerPrefs.GetInt(DayPlanKey, 1);

        // Ustaw tytu³ dnia
        dayTitleText.text = $"Plan dnia {selectedDay}";

        // Wczytaj zadania dla tego dnia
        LoadTasks();

        // Przypisz funkcje do przycisków
        addTaskButton.onClick.AddListener(AddOrEditTask);
        clearTasksButton.onClick.AddListener(ClearTasks);
        saveEditButton.onClick.AddListener(SaveEditedTask);
        backToCalendarButton.onClick.AddListener(ReturnToCalendar);

        // Zaktualizuj listê zadañ
        UpdateTaskList();

        // Ukryj przycisk zapisu edycji na pocz¹tku
        saveEditButton.gameObject.SetActive(false);
    }

    // Dodawanie lub edycja zadania
    public void AddOrEditTask()
    {
        string newTask = taskInputField.text;

        if (!string.IsNullOrEmpty(newTask))
        {
            if (editIndex == -1)
            {
                // Dodaj nowe zadanie
                tasks.Add(newTask);
                statusText.text = "Zadanie dodane.";
            }
            else
            {
                // Edytuj istniej¹ce zadanie
                tasks[editIndex] = newTask;
                editIndex = -1;
                saveEditButton.gameObject.SetActive(false); // Ukryj przycisk edycji
                addTaskButton.GetComponentInChildren<TMP_Text>().text = "Dodaj Zadanie";
                statusText.text = "Zadanie zaktualizowane.";
            }

            taskInputField.text = ""; // Wyczyœæ pole wejœciowe
            UpdateTaskList();
            SaveTasks();
        }
        else
        {
            statusText.text = "Pole zadania jest puste!";
        }
    }

    // Zapisywanie edytowanego zadania
    private void SaveEditedTask()
    {
        if (editIndex >= 0 && editIndex < tasks.Count)
        {
            tasks[editIndex] = taskInputField.text;
            taskInputField.text = "";
            editIndex = -1;
            saveEditButton.gameObject.SetActive(false);
            addTaskButton.GetComponentInChildren<TMP_Text>().text = "Dodaj Zadanie";
            UpdateTaskList();
            SaveTasks();
            statusText.text = "Zadanie zaktualizowane.";
        }
    }

    // Usuwanie wszystkich zadañ
    public void ClearTasks()
    {
        tasks.Clear();
        UpdateTaskList();
        SaveTasks();
        statusText.text = "Lista zadañ zosta³a wyczyszczona.";
    }

    // Aktualizacja listy zadañ
    private void UpdateTaskList()
    {
        taskListText.text = "";

        for (int i = 0; i < tasks.Count; i++)
        {
            taskListText.text += $"{i + 1}. {tasks[i]}\n";
        }
    }

    // Zapisywanie zadañ w PlayerPrefs
    private void SaveTasks()
    {
        string taskKey = $"Day_{selectedDay}_Tasks"; // Klucz zale¿ny od dnia
        PlayerPrefs.SetString(taskKey, string.Join(";", tasks)); // Zapisz listê zadañ jako ci¹g znaków
        PlayerPrefs.Save();
    }

    // Wczytywanie zadañ z PlayerPrefs
    private void LoadTasks()
    {
        string taskKey = $"Day_{selectedDay}_Tasks"; // Klucz zale¿ny od dnia
        string savedTasks = PlayerPrefs.GetString(taskKey, ""); // Pobierz zapisane zadania
        tasks = new List<string>(savedTasks.Split(';'));
        tasks.RemoveAll(task => string.IsNullOrEmpty(task)); // Usuñ puste elementy
    }

    // Powrót do kalendarza
    private void ReturnToCalendar()
    {
        SceneManager.LoadScene("CalendarScene"); // Wróæ do kalendarza
    }
}