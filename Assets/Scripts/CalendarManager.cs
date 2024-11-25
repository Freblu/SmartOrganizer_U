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
    public TMP_Text calendarTitle; // Tytu� kalendarza (np. "M�j Kalendarz")
    public TMP_Text quoteText; // Inspiruj�cy cytat
    public Button previousMonthButton; // Przycisk do przej�cia do poprzedniego miesi�ca
    public Button nextMonthButton; // Przycisk do przej�cia do nast�pnego miesi�ca

    private List<GameObject> dayObjects = new List<GameObject>(); // Lista dni w kalendarzu
    private int currentYear;
    private int currentMonth;

    // Zarz�dzanie zadaniami
    public TMP_Text taskListText;           // Pole tekstowe do wy�wietlania zada� (TextMeshPro)
    public TMP_InputField taskInputField;    // Pole wej�ciowe do dodawania nowych zada� (TextMeshPro)
    public Button addTaskButton;             // Przycisk do dodawania zada�
    public Button clearTasksButton;          // Przycisk do usuwania wszystkich zada�
    public Button saveEditButton;            // Przycisk do zapisywania edycji zadania

    private List<string> tasks = new List<string>(); // Lista zada�
    private int editIndex = -1; // Indeks zadania do edycji (-1 oznacza brak edycji)

    private void Start()
    {
        // Ustaw inspiruj�cy cytat
        quoteText.text = "\"Ka�dy wielki cel zaczyna si� od ma�ego planu\"";

        // Ustaw pocz�tkowy rok i miesi�c na aktualne warto�ci
        currentYear = DateTime.Now.Year;
        currentMonth = DateTime.Now.Month;

        // Inicjalizacja przycisk�w nawigacji miesi�cy
        previousMonthButton.onClick.AddListener(PreviousMonth);
        nextMonthButton.onClick.AddListener(NextMonth);

        // Generowanie kalendarza
        GenerateCalendar();

        // Inicjalizacja przycisk�w do zarz�dzania zadaniami
        addTaskButton.onClick.AddListener(AddOrEditTask);
        clearTasksButton.onClick.AddListener(ClearTasks);

        // Wczytaj zadania
        LoadTasks();
        UpdateTaskList();
    }

    // Funkcja generuj�ca dni kalendarza na podstawie rzeczywistego miesi�ca i roku
    public void GenerateCalendar()
    {
        // Wyczy�� istniej�ce dni z kalendarza
        foreach (GameObject day in dayObjects)
        {
            Destroy(day);
        }
        dayObjects.Clear();

        // Ustaw nazw� miesi�ca i roku
        calendarTitle.text = $"{GetMonthName(currentMonth)} {currentYear}";

        // Znajd� pierwszy dzie� miesi�ca
        DateTime firstDayOfMonth = new DateTime(currentYear, currentMonth, 1);
        int daysInMonth = DateTime.DaysInMonth(currentYear, currentMonth);
        int startDayOfWeek = (int)firstDayOfMonth.DayOfWeek;

        // Dodaj puste dni dla wyr�wnania do pocz�tku tygodnia
        for (int i = 0; i < startDayOfWeek; i++)
        {
            GameObject emptyDay = Instantiate(dayPrefab, calendarGrid);
            emptyDay.GetComponentInChildren<TMP_Text>().text = ""; // Puste pole
            dayObjects.Add(emptyDay);
        }

        // Generowanie dni miesi�ca
        for (int day = 1; day <= daysInMonth; day++)
        {
            GameObject dayObj = Instantiate(dayPrefab, calendarGrid);
            TMP_Text dayText = dayObj.GetComponentInChildren<TMP_Text>();
            dayText.text = day.ToString();

            // Logika oznaczania niekt�rych dni na czerwono jako zaplanowane (przyk�adowo co pi�ty dzie�)
            if (IsPlannedDay(day))
            {
                dayText.color = Color.red;
            }

            dayObjects.Add(dayObj);
        }
    }

    // Funkcja sprawdzaj�ca, czy dany dzie� jest zaplanowany (przyk�adowa logika)
    private bool IsPlannedDay(int day)
    {
        return day % 5 == 0;
    }

    // Przej�cie do poprzedniego miesi�ca
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

    // Przej�cie do nast�pnego miesi�ca
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

    // Pobranie nazwy miesi�ca
    private string GetMonthName(int month)
    {
        return new DateTime(currentYear, month, 1).ToString("MMMM");
    }

    // Zarz�dzanie zadaniami: dodawanie, edycja, czyszczenie, zapis i odczyt
    public void AddOrEditTask()
    {
        string newTask = taskInputField.text;

        if (!string.IsNullOrEmpty(newTask))
        {
            if (editIndex == -1)
            {
                // Dodaj nowe zadanie, je�li nie jest to edycja
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
            saveEditButton.gameObject.SetActive(true); // Poka� przycisk zapisu edycji
            addTaskButton.GetComponentInChildren<TMP_Text>().text = "Zapisz Edycj�";
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