using System;
using System.Collections.Generic;
using UnityEngine;
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

    private List<GameObject> dayObjects = new List<GameObject>(); // Lista dni w kalendarzu
    private int currentYear;
    private int currentMonth;

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

        // Generowanie kalendarza
        GenerateCalendar();

        // Inicjalizacja przycisków do zarz¹dzania zadaniami
        addTaskButton.onClick.AddListener(AddOrEditTask);
        clearTasksButton.onClick.AddListener(ClearTasks);

        // Wczytaj zadania
        LoadTasks();
        UpdateTaskList();
    }

    // Funkcja generuj¹ca dni kalendarza na podstawie rzeczywistego miesi¹ca i roku
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
            dayObjects.Add(emptyDay);
        }

        // Generowanie dni miesi¹ca
        for (int day = 1; day <= daysInMonth; day++)
        {
            GameObject dayObj = Instantiate(dayPrefab, calendarGrid);
            TMP_Text dayText = dayObj.GetComponentInChildren<TMP_Text>();
            dayText.text = day.ToString();

            // Logika oznaczania niektórych dni na czerwono jako zaplanowane (przyk³adowo co pi¹ty dzieñ)
            if (IsPlannedDay(day))
            {
                dayText.color = Color.red;
            }

            dayObjects.Add(dayObj);
        }
    }

    // Funkcja sprawdzaj¹ca, czy dany dzieñ jest zaplanowany (przyk³adowa logika)
    private bool IsPlannedDay(int day)
    {
        return day % 5 == 0;
    }

    // Przejœcie do poprzedniego miesi¹ca
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

    // Przejœcie do nastêpnego miesi¹ca
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

    // Pobranie nazwy miesi¹ca
    private string GetMonthName(int month)
    {
        return new DateTime(currentYear, month, 1).ToString("MMMM");
    }

    // Zarz¹dzanie zadaniami: dodawanie, edycja, czyszczenie, zapis i odczyt
    public void AddOrEditTask()
    {
        string newTask = taskInputField.text;

        if (!string.IsNullOrEmpty(newTask))
        {
            if (editIndex == -1)
            {
                // Dodaj nowe zadanie, jeœli nie jest to edycja
                tasks.Add(newTask);
            }
            else
            {
                // Zapisz edytowane zadanie
                tasks[editIndex] = newTask;
                editIndex = -1; // Resetuj tryb edycji
                saveEditButton.gameObject.SetActive(false); // Ukryj przycisk zapisu edycji
                addTaskButton.GetComponentInChildren<TMP_Text>().text = "Dodaj Zadanie";
            }
            taskInputField.text = "";
            UpdateTaskList();
            SaveTasks(); // Zapisz zmiany
        }
        else
        {
            Debug.Log("Pole zadania jest puste!");
        }
    }

    public void EditTask(int index)
    {
        if (index >= 0 && index < tasks.Count)
        {
            taskInputField.text = tasks[index];
            editIndex = index;
            saveEditButton.gameObject.SetActive(true); // Poka¿ przycisk zapisu edycji
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
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (string task in tasks)
        {
            sb.Append(task + ";");
        }
        PlayerPrefs.SetString("Tasks", sb.ToString());
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