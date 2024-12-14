using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class DayPlanManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text dayTitleText;
    public Transform taskListText;
    public TMP_InputField taskInputField;
    public TMP_InputField taskTimeInputField;
    public Button addTaskButton;
    public Button clearTasksButton;
    public Button saveEditButton;
    public Button backToCalendarButton;
    public TMP_Text statusText;
    public TMP_Dropdown taskIconDropdown; // Dropdown do wyboru grafiki zadania
    public List<Sprite> taskIcons;       // Lista grafik dla zadañ

    private List<Task> tasks = new List<Task>();
    private const string DayPlanKey = "SelectedDay";
    private int selectedDay;
    private int editIndex = -1;

    [Serializable]
    private class Task
    {
        public string description;
        public string time;
        public int iconIndex;

        public Task(string description, string time, int iconIndex)
        {
            this.description = description;
            this.time = time;
            this.iconIndex = iconIndex;
        }

        public override string ToString()
        {
            return $"{time} - {description}";
        }
    }

    private void Start()
    {
        selectedDay = PlayerPrefs.GetInt(DayPlanKey, 1);
        dayTitleText.text = $"Plan dnia {selectedDay}";
        LoadTasks();

        addTaskButton.onClick.AddListener(AddOrEditTask);
        clearTasksButton.onClick.AddListener(ClearTasks);
        saveEditButton.onClick.AddListener(SaveEditedTask);
        backToCalendarButton.onClick.AddListener(ReturnToCalendar);

        UpdateTaskList();
        saveEditButton.gameObject.SetActive(false);

        PopulateDropdown();
    }

    private void PopulateDropdown()
    {
        taskIconDropdown.options.Clear();

        // Wczytaj sprite'y z folderu w projekcie
        Sprite naukaSprite = Resources.Load<Sprite>("Sprites/nauka");
        Sprite egzaminSprite = Resources.Load<Sprite>("Sprites/egzamin");
        Sprite silowniaSprite = Resources.Load<Sprite>("Sprites/silownia");
        Sprite muzykaSprite = Resources.Load<Sprite>("Sprites/muzyka");
        Sprite CzasDlaSiebieSprite = Resources.Load<Sprite>("Sprites/CzasDlaSiebie");
        Sprite GraNaInstrumencieSprite = Resources.Load<Sprite>("Sprites/GraNaInstrumencie");
        Sprite porzadkiSprite = Resources.Load<Sprite>("Sprites/porzadki");
        Sprite rozrywkaSprite = Resources.Load<Sprite>("Sprites/rozrywka");
        Sprite zakupySprite = Resources.Load<Sprite>("Sprites/zakupy");
        Sprite SpotkanieTowarzyskieSprite = Resources.Load<Sprite>("Sprites/SpotkanieTowarzyskie");

        taskIconDropdown.options.Add(new TMP_Dropdown.OptionData("nauka", naukaSprite, Color.black));
        taskIconDropdown.options.Add(new TMP_Dropdown.OptionData("egzamin", egzaminSprite, Color.black));
        taskIconDropdown.options.Add(new TMP_Dropdown.OptionData("silownia", silowniaSprite, Color.white));
        taskIconDropdown.options.Add(new TMP_Dropdown.OptionData("muzyka", muzykaSprite, Color.white));
        taskIconDropdown.options.Add(new TMP_Dropdown.OptionData("CzasDlaSiebie", CzasDlaSiebieSprite, Color.black));
        taskIconDropdown.options.Add(new TMP_Dropdown.OptionData("GraNaInstrumencie", GraNaInstrumencieSprite, Color.black));
        taskIconDropdown.options.Add(new TMP_Dropdown.OptionData("porzadki", porzadkiSprite, Color.black));
        taskIconDropdown.options.Add(new TMP_Dropdown.OptionData("rozrywka", rozrywkaSprite, Color.black));
        taskIconDropdown.options.Add(new TMP_Dropdown.OptionData("zakupy", zakupySprite, Color.black));
        taskIconDropdown.options.Add(new TMP_Dropdown.OptionData("SpotkanieTowarzyskie", SpotkanieTowarzyskieSprite, Color.black));

        taskIconDropdown.value = 0;
    }


    public void AddOrEditTask()
    {
        string newTaskDescription = taskInputField.text.Trim();
        string newTaskTime = taskTimeInputField.text.Trim();
        int selectedIcon = taskIconDropdown.value;

        if (!string.IsNullOrEmpty(newTaskDescription) && !string.IsNullOrEmpty(newTaskTime))
        {
            if (editIndex == -1)
            {
                tasks.Add(new Task(newTaskDescription, newTaskTime, selectedIcon));
                Debug.Log($"Dodano nowe zadanie: {newTaskDescription} o {newTaskTime}");
                statusText.text = "Zadanie dodane.";
            }
            else
            {
                tasks[editIndex] = new Task(newTaskDescription, newTaskTime, selectedIcon);
                Debug.Log($"Zaktualizowano zadanie: {newTaskDescription} o {newTaskTime}");
                editIndex = -1;
                saveEditButton.gameObject.SetActive(false);
                addTaskButton.GetComponentInChildren<TMP_Text>().text = "Dodaj Zadanie";
                statusText.text = "Zadanie zaktualizowane.";
            }

            taskInputField.text = "";
            taskTimeInputField.text = "";

            UpdateTaskList();
            SaveTasks();
        }
        else
        {
            statusText.text = "Pole zadania lub czasu jest puste!";
            Debug.LogWarning("Nie wprowadzono opisu lub czasu zadania!");
        }
    }

    public void EditTask(int index)
    {
        if (index >= 0 && index < tasks.Count)
        {
            Task taskToEdit = tasks[index];
            taskInputField.text = taskToEdit.description;
            taskTimeInputField.text = taskToEdit.time;
            taskIconDropdown.value = taskToEdit.iconIndex;
            editIndex = index;
            saveEditButton.gameObject.SetActive(true);
            addTaskButton.GetComponentInChildren<TMP_Text>().text = "Zapisz Edycjê";
            statusText.text = $"Edycja zadania {index + 1}";
        }
    }

    public void SaveEditedTask()
    {
        if (editIndex >= 0 && editIndex < tasks.Count)
        {
            string updatedDescription = taskInputField.text;
            string updatedTime = taskTimeInputField.text;
            int updatedIcon = taskIconDropdown.value;

            tasks[editIndex] = new Task(updatedDescription, updatedTime, updatedIcon);
            taskInputField.text = "";
            taskTimeInputField.text = "";
            editIndex = -1;
            saveEditButton.gameObject.SetActive(false);
            addTaskButton.GetComponentInChildren<TMP_Text>().text = "Dodaj Zadanie";
            UpdateTaskList();
            SaveTasks();
            statusText.text = "Zadanie zaktualizowane.";
        }
    }

    public void ClearTasks()
    {
        tasks.Clear();
        UpdateTaskList();
        SaveTasks();
        statusText.text = "Lista zadañ zosta³a wyczyszczona.";
    }

    private void UpdateTaskList()
    {
        // Usuñ stare elementy z listy
        foreach (Transform child in taskListText)
        {
            Destroy(child.gameObject);
        }

        // Jeœli lista jest pusta, wyœwietl komunikat
        if (tasks.Count == 0)
        {
            Debug.LogWarning("Lista zadañ jest pusta!");
            return;
        }

        // Dodaj nowe elementy do listy
        for (int i = 0; i < tasks.Count; i++)
        {
            int index = i;

            // Tworzenie elementu zadania
            GameObject taskItem = new GameObject($"Task_{i}");
            taskItem.transform.SetParent(taskListText, false);
            LayoutRebuilder.ForceRebuildLayoutImmediate(taskListText.GetComponent<RectTransform>());
            // Uk³ad poziomy elementu
            HorizontalLayoutGroup layoutGroup = taskItem.AddComponent<HorizontalLayoutGroup>();
            layoutGroup.childAlignment = TextAnchor.MiddleLeft;
            layoutGroup.spacing = 10;

            // Tekst zadania
            TextMeshProUGUI taskText = new GameObject("TaskText").AddComponent<TextMeshProUGUI>();
            taskText.transform.SetParent(taskItem.transform);
            taskText.text = tasks[i].ToString();
            taskText.fontSize = 36;

            // Ikona zadania
            Image icon = new GameObject("TaskIcon").AddComponent<Image>();
            icon.transform.SetParent(taskItem.transform);
            icon.sprite = taskIcons[tasks[i].iconIndex];
            icon.rectTransform.sizeDelta = new Vector2(36, 36);

            // Przycisk edycji
            Button editButton = new GameObject("EditButton").AddComponent<Button>();
            editButton.transform.SetParent(taskItem.transform);
            TextMeshProUGUI buttonText = editButton.gameObject.AddComponent<TextMeshProUGUI>();
            buttonText.text = "[Edytuj]";
            buttonText.fontSize = 30;
            buttonText.color = Color.red;

            // Funkcja przycisku edycji
            editButton.onClick.AddListener(() => EditTask(index));
        }
    }

    private void SaveTasks()
    {
        string taskKey = $"Day_{selectedDay}_Tasks";
        string json = JsonUtility.ToJson(new TaskListWrapper(tasks));
        PlayerPrefs.SetString(taskKey, json);
        PlayerPrefs.Save();
    }

    private void LoadTasks()
    {
        string taskKey = $"Day_{selectedDay}_Tasks";
        string savedTasks = PlayerPrefs.GetString(taskKey, "");

        if (!string.IsNullOrEmpty(savedTasks))
        {
            TaskListWrapper taskListWrapper = JsonUtility.FromJson<TaskListWrapper>(savedTasks);
            tasks = taskListWrapper?.tasks ?? new List<Task>();
        }
        else
        {
            tasks = new List<Task>();
        }
    }

    public void ReturnToCalendar()
    {
        SceneManager.LoadScene("CalendarScene");
    }

    [Serializable]
    private class TaskListWrapper
    {
        public List<Task> tasks;

        public TaskListWrapper(List<Task> tasks)
        {
            this.tasks = tasks;
        }
    }
}