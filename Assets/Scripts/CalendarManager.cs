using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class CalendarManager : MonoBehaviour
{
    // Kalendarz
    public GameObject dayPrefab; // Prefab dnia kalendarza
    public Transform calendarGrid; // Siatka kalendarza
    public TMP_Text calendarTitle; // Tytu³ kalendarza (np. "Mój Kalendarz")
    public TMP_Text quoteText; // Inspiruj¹cy cytat
    public Button previousMonthButton; // Przycisk do przejœcia do poprzedniego miesi¹ca
    public Button nextMonthButton; // Przycisk do przejœcia do nastêpnego miesi¹ca
    public Button settingsButton; // Przycisk do przejœcia do ustawieñ

    private List<GameObject> dayObjects = new List<GameObject>(); // Lista dni w kalendarzu
    private int currentYear;
    private int currentMonth;

    private const string DayPlanKey = "SelectedDay"; // Klucz do zapamiêtania wybranego dnia

    // Zarz¹dzanie zadaniami
    public TMP_Text taskListText;           // Pole tekstowe do wyœwietlania zadañ (TextMeshPro)
    public TMP_InputField taskInputField;    // Pole wejœciowe do dodawania nowych zadañ (TextMeshPro)
    public Button addTaskButton;             // Przycisk do dodawania zadañ
    public Button clearTasksButton;          // Przycisk do usuwania wszystkich zadañ
    public Button saveEditButton;            // Przycisk do zapisywania edycji zadania

    private List<string> tasks = new List<string>(); // Lista zadañ
    private int editIndex = -1; // Indeks zadania do edycji (-1 oznacza brak edycji)

    private void Start()
    {
        // Ustaw inspiruj¹cy cytat
        quoteText.text = "\"Ka¿dy wielki cel zaczyna siê od ma³ego planu\"";

        // Ustaw pocz¹tkowy rok i miesi¹c na aktualne wartoœci
        currentYear = DateTime.Now.Year;
        currentMonth = DateTime.Now.Month;

        // Inicjalizacja przycisków nawigacji miesiêcy
        previousMonthButton.onClick.AddListener(PreviousMonth);
        nextMonthButton.onClick.AddListener(NextMonth);
        settingsButton.onClick.AddListener(OpenSettings); // Przycisk ustawieñ

        // Generowanie kalendarza
        GenerateCalendar();

        // Inicjalizacja przycisków do zarz¹dzania zadaniami
        addTaskButton.onClick.AddListener(AddOrEditTask);
        clearTasksButton.onClick.AddListener(ClearTasks);

        // Wczytaj zadania
        LoadTasks();
        UpdateTaskList();
    }

    public void GenerateCalendar()
    {
        // Wyczyœæ istniej¹ce dni z kalendarza
        foreach (GameObject day in dayObjects)
        {
            Destroy(day);
        }
        dayObjects.Clear();

        // Ustaw nazwê miesi¹ca i roku
        calendarTitle.text = $"{GetMonthName(currentMonth)} {currentYear}";

        // ZnajdŸ pierwszy dzieñ miesi¹ca
        DateTime firstDayOfMonth = new DateTime(currentYear, currentMonth, 1);
        int daysInMonth = DateTime.DaysInMonth(currentYear, currentMonth);
        int startDayOfWeek = (int)firstDayOfMonth.DayOfWeek;

        // Dodaj puste dni dla wyrównania do pocz¹tku tygodnia
        for (int i = 0; i < startDayOfWeek; i++)
        {
            GameObject emptyDay = Instantiate(dayPrefab, calendarGrid);
            emptyDay.GetComponentInChildren<TMP_Text>().text = ""; // Puste pole
            emptyDay.GetComponent<Button>().interactable = false; // Wy³¹cz interakcjê
            dayObjects.Add(emptyDay);
        }

        // Generowanie dni miesi¹ca
        for (int day = 1; day <= daysInMonth; day++)
        {
            GameObject dayObj = Instantiate(dayPrefab, calendarGrid);
            TMP_Text dayText = dayObj.GetComponentInChildren<TMP_Text>();
            dayText.text = day.ToString();

            // Przycisk otwieraj¹cy scenê planu dnia
            Button dayButton = dayObj.GetComponent<Button>();
            if (dayButton != null)
            {
                int selectedDay = day;
                dayButton.onClick.AddListener(() => OpenDayPlan(selectedDay));
            }

            dayObjects.Add(dayObj);
        }
    }

    private string GetMonthName(int month)
    {
        return new DateTime(currentYear, month, 1).ToString("MMMM");
    }

    private void PreviousMonth()
    {
        if (currentMonth == 1)
        {
            currentMonth = 12;
            currentYear--;
        }
        else
        {
            currentMonth--;
        }
        GenerateCalendar();
    }

    private void NextMonth()
    {
        if (currentMonth == 12)
        {
            currentMonth = 1;
            currentYear++;
        }
        else
        {
            currentMonth++;
        }
        GenerateCalendar();
    }

    private void OpenSettings()
    {
        SceneManager.LoadScene("SettingsScene");
    }

    private void OpenDayPlan(int day)
    {
        PlayerPrefs.SetInt(DayPlanKey, day); // Zapamiêtaj wybrany dzieñ
        PlayerPrefs.SetInt("CurrentYear", currentYear); // Zapamiêtaj rok
        PlayerPrefs.SetInt("CurrentMonth", currentMonth); // Zapamiêtaj miesi¹c
        PlayerPrefs.Save();
        SceneManager.LoadScene("DayPlanScene"); // Otwórz scenê DayPlan
    }

    public void AddOrEditTask()
    {
        string newTask = taskInputField.text;

        if (!string.IsNullOrEmpty(newTask))
        {
            if (editIndex == -1)
            {
                tasks.Add(newTask);
            }
            else
            {
                tasks[editIndex] = newTask;
                editIndex = -1;
                saveEditButton.gameObject.SetActive(false);
                addTaskButton.GetComponentInChildren<TMP_Text>().text = "Dodaj Zadanie";
            }
            taskInputField.text = "";
            UpdateTaskList();
            SaveTasks();
        }
    }

    public void EditTask(int index)
    {
        if (index >= 0 && index < tasks.Count)
        {
            taskInputField.text = tasks[index];
            editIndex = index;
            saveEditButton.gameObject.SetActive(true);
            addTaskButton.GetComponentInChildren<TMP_Text>().text = "Zapisz Edycjê";
        }
    }

    private void UpdateTaskList()
    {
        taskListText.text = "";
        for (int i = 0; i < tasks.Count; i++)
        {
            taskListText.text += $"{i + 1}. {tasks[i]} <color=#FF0000FF>[Edytuj]</color>\n";
        }
    }

    public void ClearTasks()
    {
        tasks.Clear();
        UpdateTaskList();
        SaveTasks();
    }

    private void SaveTasks()
    {
        PlayerPrefs.SetString("Tasks", string.Join(";", tasks));
        PlayerPrefs.Save();
    }

    private void LoadTasks()
    {
        tasks.Clear();
        string savedTasks = PlayerPrefs.GetString("Tasks", "");
        if (!string.IsNullOrEmpty(savedTasks))
        {
            tasks.AddRange(savedTasks.Split(';'));
            tasks.RemoveAll(task => string.IsNullOrEmpty(task));
        }
    }
}