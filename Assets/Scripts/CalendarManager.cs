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
    public TMP_Text calendarTitle; // Tytu³ kalendarza
    public TMP_Text quoteText; // Inspiruj¹cy cytat
    public Button previousMonthButton; // Przycisk do przejœcia do poprzedniego miesi¹ca
    public Button nextMonthButton; // Przycisk do przejœcia do nastêpnego miesi¹ca
    public Button settingsButton; // Przycisk do przejœcia do ustawieñ

    private int currentYear;
    private int currentMonth;

    // Zarz¹dzanie zadaniami
    public Transform taskListContent; // Kontener dla dynamicznie generowanych zadañ
    public TMP_InputField taskInputField; // Pole wejœciowe do dodawania/edycji zadañ
    public Button addTaskButton; // Przycisk do dodawania zadañ
    public Button clearTasksButton; // Przycisk do czyszczenia zadañ

    private List<string> tasks = new List<string>();
    private int editIndex = -1;

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

        // Inicjalizacja przycisków do zarz¹dzania zadaniami
        addTaskButton.onClick.AddListener(AddOrEditTask);
        clearTasksButton.onClick.AddListener(ClearTasks);

        // Generowanie kalendarza
        GenerateCalendar();

        // Wczytaj zadania
        LoadTasks();
        UpdateTaskList();
    }

    public void GenerateCalendar()
    {
        foreach (Transform child in calendarGrid)
        {
            Destroy(child.gameObject);
        }

        calendarTitle.text = $"{GetMonthName(currentMonth)} {currentYear}";

        DateTime firstDayOfMonth = new DateTime(currentYear, currentMonth, 1);
        int daysInMonth = DateTime.DaysInMonth(currentYear, currentMonth);
        int startDayOfWeek = (int)firstDayOfMonth.DayOfWeek;

        for (int i = 0; i < startDayOfWeek; i++)
        {
            GameObject emptyDay = Instantiate(dayPrefab, calendarGrid);
            emptyDay.GetComponentInChildren<TMP_Text>().text = ""; // Puste pole
            emptyDay.GetComponent<Button>().interactable = false; // Wy³¹cz interakcjê
        }

        for (int day = 1; day <= daysInMonth; day++)
        {
            GameObject dayObj = Instantiate(dayPrefab, calendarGrid);
            TMP_Text dayText = dayObj.GetComponentInChildren<TMP_Text>();
            dayText.text = day.ToString();

            Button dayButton = dayObj.GetComponent<Button>();
            if (dayButton != null)
            {
                int selectedDay = day;
                dayButton.onClick.AddListener(() => OpenDayPlan(selectedDay));
            }
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
        PlayerPrefs.SetInt("SelectedDay", day); // Zapamiêtaj wybrany dzieñ
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
                addTaskButton.GetComponentInChildren<TMP_Text>().text = "Dodaj Zadanie";
            }
            taskInputField.text = "";
            UpdateTaskList();
            SaveTasks();
        }
    }

    private void UpdateTaskList()
    {
        foreach (Transform child in taskListContent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < tasks.Count; i++)
        {
            int index = i;

            // Tworzenie kontenera dla zadania
            GameObject taskContainer = new GameObject($"Task_{i}");
            taskContainer.transform.SetParent(taskListContent);
            taskContainer.AddComponent<HorizontalLayoutGroup>();

            // Dodaj tekst zadania
            GameObject taskTextObject = new GameObject("TaskText");
            taskTextObject.transform.SetParent(taskContainer.transform);
            TMP_Text taskText = taskTextObject.AddComponent<TextMeshProUGUI>();
            taskText.text = tasks[i];
            taskText.fontSize = 36;

            // Dodaj przycisk "Edytuj"
            GameObject editButtonObject = new GameObject("EditButton");
            editButtonObject.transform.SetParent(taskContainer.transform);
            Button editButton = editButtonObject.AddComponent<Button>();
            TMP_Text editButtonText = editButtonObject.AddComponent<TextMeshProUGUI>();
            editButtonText.text = "Edytuj";
            editButtonText.color = Color.red;
            editButtonText.fontSize = 36;

            editButton.onClick.AddListener(() => EditTask(index));
        }
    }

    public void EditTask(int index)
    {
        if (index >= 0 && index < tasks.Count)
        {
            taskInputField.text = tasks[index];
            editIndex = index;
            addTaskButton.GetComponentInChildren<TMP_Text>().text = "Zapisz Edycjê";
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
