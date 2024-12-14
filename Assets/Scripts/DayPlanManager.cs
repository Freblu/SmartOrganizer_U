using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class DayPlanManager : MonoBehaviour
{
    public TMP_Text dayTitleText;           // Tytu� dnia (np. "Plan dnia 1")
    public TMP_Text taskListText;          // Pole tekstowe do wy�wietlania zada�
    public TMP_InputField taskInputField;  // Pole wej�ciowe do dodawania nowych zada�
    public Button addTaskButton;           // Przycisk dodawania zada�
    public Button clearTasksButton;        // Przycisk usuwania wszystkich zada�
    public Button saveEditButton;          // Przycisk zapisywania edycji zadania
    public Button backToCalendarButton;    // Przycisk powrotu do kalendarza
    public TMP_Text statusText;            // Pole tekstowe dla komunikat�w

    private List<string> tasks = new List<string>(); // Lista zada� dla bie��cego dnia
    private const string DayPlanKey = "SelectedDay"; // Klucz do odczytu wybranego dnia
    private int selectedDay;                        // Wybrany dzie�
    private int editIndex = -1;                     // Indeks edytowanego zadania (-1 oznacza brak edycji)

    private void Start()
    {
        // Pobierz numer wybranego dnia z PlayerPrefs
        selectedDay = PlayerPrefs.GetInt(DayPlanKey, 1);

        // Ustaw tytu� dnia
        dayTitleText.text = $"Plan dnia {selectedDay}";

        // Wczytaj zadania dla tego dnia
        LoadTasks();

        // Przypisz funkcje do przycisk�w
        addTaskButton.onClick.AddListener(AddOrEditTask);
        clearTasksButton.onClick.AddListener(ClearTasks);
        saveEditButton.onClick.AddListener(SaveEditedTask);
        backToCalendarButton.onClick.AddListener(ReturnToCalendar);

        // Zaktualizuj list� zada�
        UpdateTaskList();

        // Ukryj przycisk zapisu edycji na pocz�tku
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
                // Edytuj istniej�ce zadanie
                tasks[editIndex] = newTask;
                editIndex = -1;
                saveEditButton.gameObject.SetActive(false); // Ukryj przycisk edycji
                addTaskButton.GetComponentInChildren<TMP_Text>().text = "Dodaj Zadanie";
                statusText.text = "Zadanie zaktualizowane.";
            }

            taskInputField.text = ""; // Wyczy�� pole wej�ciowe
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

    // Usuwanie wszystkich zada�
    public void ClearTasks()
    {
        tasks.Clear();
        UpdateTaskList();
        SaveTasks();
        statusText.text = "Lista zada� zosta�a wyczyszczona.";
    }

    // Aktualizacja listy zada�
    private void UpdateTaskList()
    {
        taskListText.text = "";

        for (int i = 0; i < tasks.Count; i++)
        {
            taskListText.text += $"{i + 1}. {tasks[i]}\n";
        }
    }

    // Zapisywanie zada� w PlayerPrefs
    private void SaveTasks()
    {
        string taskKey = $"Day_{selectedDay}_Tasks"; // Klucz zale�ny od dnia
        PlayerPrefs.SetString(taskKey, string.Join(";", tasks)); // Zapisz list� zada� jako ci�g znak�w
        PlayerPrefs.Save();
    }

    // Wczytywanie zada� z PlayerPrefs
    private void LoadTasks()
    {
        string taskKey = $"Day_{selectedDay}_Tasks"; // Klucz zale�ny od dnia
        string savedTasks = PlayerPrefs.GetString(taskKey, ""); // Pobierz zapisane zadania
        tasks = new List<string>(savedTasks.Split(';'));
        tasks.RemoveAll(task => string.IsNullOrEmpty(task)); // Usu� puste elementy
    }

    // Powr�t do kalendarza
    private void ReturnToCalendar()
    {
        SceneManager.LoadScene("CalendarScene"); // Wr�� do kalendarza
    }
}