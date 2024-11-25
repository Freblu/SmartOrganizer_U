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
    public TMP_Text calendarTitle; // Tytu� kalendarza (np. "M�j Kalendarz")
    public TMP_Text quoteText; // Inspiruj�cy cytat
    public Button previousMonthButton; // Przycisk do przej�cia do poprzedniego miesi�ca
    public Button nextMonthButton; // Przycisk do przej�cia do nast�pnego miesi�ca
    public Button settingsButton; // Przycisk do przej�cia do ustawie�

    private List<GameObject> dayObjects = new List<GameObject>(); // Lista dni w kalendarzu
    private int currentYear;
    private int currentMonth;

    private const string DayPlanKey = "SelectedDay"; // Klucz do zapami�tania wybranego dnia

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
        settingsButton.onClick.AddListener(OpenSettings); // Przycisk ustawie�

        // Generowanie kalendarza
        GenerateCalendar();

        // Inicjalizacja przycisk�w do zarz�dzania zadaniami
        addTaskButton.onClick.AddListener(AddOrEditTask);
        clearTasksButton.onClick.AddListener(ClearTasks);

        // Wczytaj zadania
        LoadTasks();
        UpdateTaskList();
    }

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
            emptyDay.GetComponent<Button>().interactable = false; // Wy��cz interakcj�
            dayObjects.Add(emptyDay);
        }

        // Generowanie dni miesi�ca
        for (int day = 1; day <= daysInMonth; day++)
        {
            GameObject dayObj = Instantiate(dayPrefab, calendarGrid);
            TMP_Text dayText = dayObj.GetComponentInChildren<TMP_Text>();
            dayText.text = day.ToString();

            // Przycisk otwieraj�cy scen� planu dnia
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
        PlayerPrefs.SetInt(DayPlanKey, day); // Zapami�taj wybrany dzie�
        PlayerPrefs.SetInt("CurrentYear", currentYear); // Zapami�taj rok
        PlayerPrefs.SetInt("CurrentMonth", currentMonth); // Zapami�taj miesi�c
        PlayerPrefs.Save();
        SceneManager.LoadScene("DayPlanScene"); // Otw�rz scen� DayPlan
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