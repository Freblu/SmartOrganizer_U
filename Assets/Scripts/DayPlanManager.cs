using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Unity.Notifications.Android;
/**
 * @class DayPlanManager
 * @brief Mened¿er zarz¹dzaj¹cy zadaniami w planie dnia.
 * 
 * Klasa obs³uguje dodawanie, edytowanie, usuwanie zadañ oraz zarz¹dzanie powiadomieniami 
 * dla wybranego dnia. Zawiera mechanizmy do pracy z list¹ zadañ, ich reprezentacj¹ 
 * w UI oraz obs³ugê wyboru grafiki i czasu zadania.
 */
public class DayPlanManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text dayTitleText; ///< Tytu³ wyœwietlaj¹cy datê wybranego dnia.
    public Transform taskListText; ///< Kontener dla listy zadañ wyœwietlanej w UI.
    public TMP_InputField taskInputField; ///< Pole tekstowe do wprowadzania opisu zadania.
    public TMP_Dropdown reminderDropdown; ///< Dropdown do wyboru przypomnienia o zadaniu.
    public TMP_Dropdown taskTimeDropdown; ///< Dropdown do wyboru czasu zadania.
    public Button addTaskButton; ///< Przycisk dodawania nowego zadania.
    public Button clearTasksButton; ///< Przycisk do czyszczenia listy zadañ.
    public Button saveEditButton; ///< Przycisk do zapisywania edytowanego zadania.
    public Button backToCalendarButton; ///< Przycisk powrotu do widoku kalendarza.
    public TMP_Text statusText; ///< Tekst statusu do wyœwietlania komunikatów u¿ytkownikowi.
    public TMP_Dropdown taskIconDropdown; ///< Dropdown do wyboru ikony dla zadania.
    public List<Sprite> taskIcons; ///< Lista ikon mo¿liwych do przypisania do zadañ.

    private List<Task> tasks = new List<Task>(); ///< Lista przechowuj¹ca zadania dla wybranego dnia.
    private int selectedDay; ///< Wybrany dzieñ.
    private int selectedMonth; ///< Wybrany miesi¹c.
    private int selectedYear; ///< Wybrany rok.

    private int editIndex = -1; ///< Indeks aktualnie edytowanego zadania (-1 oznacza brak edycji).

    /**
    * @class Task
    * @brief Reprezentacja pojedynczego zadania.
    * 
    * Klasa przechowuje szczegó³y zadania, takie jak opis, czas wykonania i indeks przypisanej ikony.
    */
    [Serializable]
    private class Task
    {
        public string description; ///< Opis zadania.
        public string time; ///< Czas zadania w formacie HH:mm.
        public int iconIndex; ///< Indeks ikony przypisanej do zadania.

        public Task(string description, string time, int iconIndex)
        {
            this.description = description;
            this.time = time;
            this.iconIndex = iconIndex;
        }
        /**
        * @brief Przeci¹¿enie metody ToString.
        * 
        * Zwraca reprezentacjê zadania w formacie "HH:mm - Opis".
        * @return Ci¹g znaków reprezentuj¹cy zadanie.
        */
        public override string ToString()
        {
            return $"{time} - {description}";
        }
    }

    /**
   * @brief Metoda wywo³ywana podczas inicjalizacji komponentu.
   * 
   * Inicjalizuje tytu³ dnia, przypisuje zdarzenia do przycisków, 
   * ukrywa przycisk zapisu edycji oraz wype³nia dropdowny czasów i ikon zadañ.
   */
    private void Start()
    {
        selectedDay = PlayerPrefs.GetInt("SelectedDay", DateTime.Now.Day); ///< Pobiera wybrany dzieñ z PlayerPrefs lub ustawia bie¿¹cy dzieñ.
        selectedMonth = PlayerPrefs.GetInt("CurrentMonth", DateTime.Now.Month); ///< Pobiera wybrany miesi¹c z PlayerPrefs lub ustawia bie¿¹cy miesi¹c.
        selectedYear = PlayerPrefs.GetInt("CurrentYear", DateTime.Now.Year); ///< Pobiera wybrany rok z PlayerPrefs lub ustawia bie¿¹cy rok.

        dayTitleText.text = $"Plan dnia: {selectedDay}/{selectedMonth}/{selectedYear}"; ///< Ustawia tytu³ dnia w formacie `Plan dnia: dzieñ/miesi¹c/rok`.

        // Przypisanie funkcji do przycisków
        addTaskButton.onClick.AddListener(AddOrEditTask); ///< Przycisk dodawania zadania.
        clearTasksButton.onClick.AddListener(ClearTasks); ///< Przycisk czyszczenia listy zadañ.
        saveEditButton.onClick.AddListener(SaveEditedTask); ///< Przycisk zapisywania edytowanego zadania.
        backToCalendarButton.onClick.AddListener(ReturnToCalendar); ///< Przycisk powrotu do widoku kalendarza.

        UpdateTaskList(); ///< Aktualizuje listê zadañ w UI.
        saveEditButton.gameObject.SetActive(false); ///< Ukrywa przycisk zapisu edycji.

        PopulateTimeDropdown(); ///< Wype³nia dropdown czasów zadania.
        PopulateDropdown(); ///< Wype³nia dropdown ikon zadañ.
    }

    /**
     * @brief Wype³nia dropdown czasów zadania.
     * 
     * Dodaje opcje czasów w odstêpach co 15 minut dla ka¿dej godziny w ci¹gu doby (24 godziny).
     */
    private void PopulateTimeDropdown()
    {
        taskTimeDropdown.ClearOptions(); ///< Czyœci istniej¹ce opcje dropdownu.
        List<string> timeOptions = new List<string>(); ///< Lista opcji czasów.

        for (int hour = 0; hour < 24; hour++) // 0 - 23 godziny
        {
            for (int minute = 0; minute < 60; minute += 15) // Co 15 minut
            {
                string time = $"{hour:D2}:{minute:D2}"; ///< Format czasu w postaci `hh:mm`.
                timeOptions.Add(time);
            }
        }

        taskTimeDropdown.AddOptions(timeOptions); ///< Dodaje opcje do dropdownu.
        taskTimeDropdown.value = 0; ///< Domyœlnie wybiera pierwszy element.
    }

    /**
     * @brief Wype³nia dropdown ikon zadañ.
     * 
     * £aduje sprite'y z zasobów projektu i dodaje je jako opcje w dropdownie ikon zadañ.
     */
    private void PopulateDropdown()
    {
        taskIconDropdown.options.Clear(); ///< Czyœci istniej¹ce opcje dropdownu.

        // Wczytaj sprite'y z folderu w projekcie
        Sprite naukaSprite = Resources.Load<Sprite>("Sprites/nauka"); ///< Ikona nauki.
        Sprite egzaminSprite = Resources.Load<Sprite>("Sprites/egzamin"); ///< Ikona egzaminu.
        Sprite silowniaSprite = Resources.Load<Sprite>("Sprites/silownia"); ///< Ikona si³owni.
        Sprite muzykaSprite = Resources.Load<Sprite>("Sprites/muzyka"); ///< Ikona muzyki.
        Sprite CzasDlaSiebieSprite = Resources.Load<Sprite>("Sprites/CzasDlaSiebie"); ///< Ikona "Czas dla siebie".
        Sprite GraNaInstrumencieSprite = Resources.Load<Sprite>("Sprites/GraNaInstrumencie"); ///< Ikona gry na instrumencie.
        Sprite porzadkiSprite = Resources.Load<Sprite>("Sprites/porzadki"); ///< Ikona porz¹dków.
        Sprite rozrywkaSprite = Resources.Load<Sprite>("Sprites/rozrywka"); ///< Ikona rozrywki.
        Sprite zakupySprite = Resources.Load<Sprite>("Sprites/zakupy"); ///< Ikona zakupów.
        Sprite SpotkanieTowarzyskieSprite = Resources.Load<Sprite>("Sprites/SpotkanieTowarzyskie"); ///< Ikona spotkania towarzyskiego.

        // Dodaj opcje do dropdownu
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

        taskIconDropdown.value = 0; ///< Domyœlnie wybiera pierwsz¹ opcjê.
    }

    /**
  * @brief Pobiera indeks czasu w dropdownie na podstawie wartoœci tekstowej.
  * 
  * Metoda iteruje przez opcje dropdownu czasu i zwraca indeks, który odpowiada podanej wartoœci czasu.
  * 
  * @param time Tekst reprezentuj¹cy czas w formacie "HH:mm".
  * @return Indeks opcji w dropdownie, lub 0, jeœli nie znaleziono.
  */
    private int GetTimeDropdownIndex(string time)
    {
        for (int i = 0; i < taskTimeDropdown.options.Count; i++)
        {
            if (taskTimeDropdown.options[i].text == time)
            {
                return i;
            }
        }
        return 0; ///< Jeœli nie znaleziono, zwróæ domyœlny indeks.
    }

    /**
     * @brief Planuje powiadomienie dla zadania w kalendarzu.
     * 
     * Metoda tworzy powiadomienie na platformie Android, które zostanie wyœwietlone 
     * o okreœlonej godzinie przed rozpoczêciem zadania. Jeœli czas powiadomienia jest 
     * w przesz³oœci, powiadomienie nie zostanie utworzone.
     * 
     * @param taskDescription Opis zadania.
     * @param taskTime Czas zadania w formacie "HH:mm".
     * @param reminderMinutes Liczba minut przed zadaniem, kiedy powiadomienie ma siê wyœwietliæ.
     */
    private void ScheduleNotification(string taskDescription, string taskTime, int reminderMinutes)
    {
#if UNITY_ANDROID
    string channelId = "day_plan_channel";

    var existingChannel = AndroidNotificationCenter.GetNotificationChannel(channelId);
    if (string.IsNullOrEmpty(existingChannel.Id))
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = channelId, ///< Identyfikator kana³u powiadomieñ.
            Name = "Powiadomienia planu dnia", ///< Nazwa kana³u.
            Importance = Importance.High, ///< Wa¿noœæ powiadomieñ.
            Description = "Powiadomienia o zadaniach w planie dnia", ///< Opis kana³u.
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    DateTime taskDateTime = DateTime.Parse($"{selectedYear}-{selectedMonth:D2}-{selectedDay:D2} {taskTime}");
    DateTime notificationTime = taskDateTime.AddMinutes(-reminderMinutes);

    if (notificationTime < DateTime.Now)
    {
        Debug.LogWarning("Czas powiadomienia jest w przesz³oœci. Powiadomienie nie zostanie utworzone.");
        return;
    }

    var notification = new AndroidNotification
    {
        Title = "Przypomnienie", ///< Tytu³ powiadomienia.
        Text = $"Za {reminderMinutes} minut: {taskDescription}", ///< Treœæ powiadomienia.
        FireTime = notificationTime, ///< Czas wyœwietlenia powiadomienia.
    };

    AndroidNotificationCenter.SendNotification(notification, channelId);
    Debug.Log($"Powiadomienie zaplanowane na: {notificationTime}");
#endif
    }

    /**
     * @brief Dodaje lub edytuje zadanie w liœcie zadañ.
     * 
     * Metoda sprawdza poprawnoœæ wprowadzonych danych, dodaje nowe zadanie lub edytuje istniej¹ce,
     * ustawia powiadomienie na podstawie wybranego czasu przypomnienia, a nastêpnie zapisuje zmiany.
     */
    public void AddOrEditTask()
    {
        if (taskInputField == null || taskTimeDropdown == null || taskIconDropdown == null || reminderDropdown == null || statusText == null)
        {
            Debug.LogError("Jeden z elementów UI nie zosta³ przypisany w Inspectorze.");
            return;
        }

        string newTaskDescription = taskInputField.text?.Trim();
        if (string.IsNullOrEmpty(newTaskDescription))
        {
            Debug.LogWarning("Pole opisu zadania jest puste.");
            statusText.text = "Opis zadania jest puste.";
            return;
        }

        string newTaskTime = taskTimeDropdown.options[taskTimeDropdown.value].text;
        if (!DateTime.TryParse(newTaskTime, out DateTime taskTime))
        {
            Debug.LogError("Nieprawid³owy format godziny: " + newTaskTime);
            statusText.text = "B³¹d w formacie godziny.";
            return;
        }

        // Po³¹cz datê z godzin¹ wybranego dnia
        DateTime taskDateTime = new DateTime(
            selectedYear,
            selectedMonth,
            selectedDay,
            taskTime.Hour,
            taskTime.Minute,
            0
        );

        int reminderTime = reminderDropdown.value switch
        {
            0 => 60, ///< 1 godzina przed.
            1 => 120, ///< 2 godziny przed.
            2 => 15, ///< 15 minut przed.
            3 => 0, ///< W momencie rozpoczêcia.
            _ => 0
        };

        if (editIndex == -1)
        {
            tasks.Add(new Task(newTaskDescription, taskDateTime.ToString("yyyy-MM-dd HH:mm"), taskIconDropdown.value)); ///< Dodaje nowe zadanie.
            ScheduleNotification(newTaskDescription, taskDateTime.ToString("HH:mm"), reminderTime); ///< Planuje powiadomienie.
            Debug.Log($"Dodano nowe zadanie: {newTaskDescription} o {taskDateTime}");
            statusText.text = "Zadanie dodane.";
        }
        else
        {
            tasks[editIndex] = new Task(newTaskDescription, taskDateTime.ToString("yyyy-MM-dd HH:mm"), taskIconDropdown.value); ///< Aktualizuje istniej¹ce zadanie.
            editIndex = -1;
            saveEditButton.gameObject.SetActive(false);
            addTaskButton.GetComponentInChildren<TMP_Text>().text = "Dodaj Zadanie";
            statusText.text = "Zadanie zaktualizowane.";
        }

        taskInputField.text = ""; ///< Resetuje pole tekstowe.
        taskTimeDropdown.value = 0; ///< Resetuje dropdown na domyœln¹ opcjê.
        UpdateTaskList(); ///< Odœwie¿a listê zadañ.
        SaveTasks(); ///< Zapisuje zmiany.
    }



    /**
     * @brief Rozpoczyna edycjê wybranego zadania.
     * 
     * Metoda wype³nia pole tekstowe wartoœciami wybranego zadania oraz zmienia tekst 
     * i funkcjê przycisku na zapis edycji.
     * 
     * @param index Indeks zadania do edycji w liœcie.
     */
    public void EditTask(int index)
    {
        if (index >= 0 && index < tasks.Count)
        {
            Task taskToEdit = tasks[index];
            taskInputField.text = taskToEdit.description; ///< Wype³nia pole tekstowe opisem zadania.
            taskTimeDropdown.value = GetTimeDropdownIndex(taskToEdit.time); ///< Ustawia czas w dropdownie.
            taskIconDropdown.value = taskToEdit.iconIndex; ///< Ustawia ikonê w dropdownie.

            editIndex = index; ///< Ustawia bie¿¹cy indeks edytowanego zadania.
            saveEditButton.gameObject.SetActive(true); ///< Wyœwietla przycisk zapisu edycji.
            addTaskButton.GetComponentInChildren<TMP_Text>().text = "Zapisz Edycjê"; ///< Zmienia tekst przycisku na "Zapisz Edycjê".
            statusText.text = $"Edycja zadania {index + 1}"; ///< Wyœwietla komunikat o edycji zadania.
        }
    }

    /**
     * @brief Zapisuje zmiany w edytowanym zadaniu.
     * 
     * Metoda aktualizuje treœæ i ustawienia wybranego zadania, zapisuje zmiany 
     * oraz odœwie¿a listê zadañ.
     */
    public void SaveEditedTask()
    {
        if (editIndex >= 0 && editIndex < tasks.Count)
        {
            string updatedDescription = taskInputField.text; ///< Pobiera zaktualizowany opis.
            string updatedTime = taskTimeDropdown.options[taskTimeDropdown.value].text; ///< Pobiera zaktualizowany czas.
            int updatedIcon = taskIconDropdown.value; ///< Pobiera zaktualizowan¹ ikonê.

            tasks[editIndex] = new Task(updatedDescription, updatedTime, updatedIcon); ///< Aktualizuje wybrane zadanie.
            taskInputField.text = ""; ///< Czyœci pole tekstowe.
            taskTimeDropdown.value = 0; ///< Resetuje dropdown czasu.
            editIndex = -1; ///< Resetuje indeks edycji.

            saveEditButton.gameObject.SetActive(false); ///< Ukrywa przycisk zapisu edycji.
            addTaskButton.GetComponentInChildren<TMP_Text>().text = "Dodaj Zadanie"; ///< Przywraca tekst przycisku.
            UpdateTaskList(); ///< Odœwie¿a listê zadañ.
            SaveTasks(); ///< Zapisuje zmiany w pamiêci.
            statusText.text = "Zadanie zaktualizowane."; ///< Wyœwietla komunikat o aktualizacji.
        }
    }

    /**
     * @brief Czyœci listê zadañ.
     * 
     * Metoda usuwa wszystkie zadania, odœwie¿a widok listy oraz zapisuje zmiany.
     */
    public void ClearTasks()
    {
        tasks.Clear(); ///< Usuwa wszystkie zadania.
        UpdateTaskList(); ///< Odœwie¿a widok listy zadañ.
        SaveTasks(); ///< Zapisuje zmiany w pamiêci.
        statusText.text = "Lista zadañ zosta³a wyczyszczona."; ///< Wyœwietla komunikat o wyczyszczeniu listy.
    }

    /**
     * @brief Odœwie¿a widok listy zadañ.
     * 
     * Metoda usuwa wszystkie istniej¹ce elementy listy i tworzy nowe elementy na podstawie 
     * aktualnej zawartoœci listy zadañ.
     */
    private void UpdateTaskList()
    {
        // Usuñ stare elementy z listy
        foreach (Transform child in taskListText)
        {
            Destroy(child.gameObject); ///< Usuwa istniej¹ce elementy z widoku.
        }

        // Jeœli lista jest pusta, wyœwietl komunikat
        if (tasks.Count == 0)
        {
            Debug.LogWarning("Lista zadañ jest pusta!"); ///< Loguje informacjê o pustej liœcie.
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
            layoutGroup.childAlignment = TextAnchor.MiddleLeft; ///< Ustawia wyrównanie elementów.
            layoutGroup.spacing = 10; ///< Ustawia odstêpy miêdzy elementami.

            // Tekst zadania
            TextMeshProUGUI taskText = new GameObject("TaskText").AddComponent<TextMeshProUGUI>();
            taskText.transform.SetParent(taskItem.transform);
            taskText.text = tasks[i].ToString(); ///< Ustawia tekst zadania.
            taskText.fontSize = 36; ///< Ustawia rozmiar czcionki.

            // Ikona zadania
            GameObject iconObject = new GameObject("TaskIcon");
            iconObject.transform.SetParent(taskItem.transform);
            Image icon = iconObject.AddComponent<Image>();
            icon.sprite = taskIcons[tasks[i].iconIndex]; ///< Ustawia ikonê zadania.
            RectTransform iconRect = icon.GetComponent<RectTransform>();
            iconRect.sizeDelta = new Vector2(50, 50); ///< Ustawia rozmiar ikony (50x50).
            icon.preserveAspect = true; ///< Zachowuje proporcje obrazu.

            // Przycisk edycji
            GameObject editButtonObject = new GameObject("EditButton");
            editButtonObject.transform.SetParent(taskItem.transform);
            Button editButton = editButtonObject.AddComponent<Button>();

            // Ustawienie rozmiaru przycisku
            RectTransform editButtonRect = editButtonObject.AddComponent<RectTransform>();
            editButtonRect.sizeDelta = new Vector2(100, 40); ///< Rozmiar przycisku.

            // Tekst przycisku
            TextMeshProUGUI buttonText = editButtonObject.AddComponent<TextMeshProUGUI>();
            buttonText.text = "[Edytuj]"; ///< Tekst przycisku edycji.
            buttonText.fontSize = 30; ///< Rozmiar czcionki.
            buttonText.color = Color.red; ///< Kolor tekstu.
            buttonText.alignment = TextAlignmentOptions.Center; ///< Wyrównanie tekstu.

            // Funkcja przycisku edycji
            editButton.onClick.AddListener(() => EditTask(index)); ///< Przypisuje funkcjê edycji do przycisku.
        }
    }


    /**
  * @brief Zapisuje listê zadañ do pamiêci urz¹dzenia.
  * 
  * Metoda serializuje listê zadañ do formatu JSON i zapisuje j¹ w `PlayerPrefs`
  * z kluczem powi¹zanym z bie¿¹cym dniem.
  */
    private void SaveTasks()
    {
        string taskKey = $"Day_{selectedDay}_Tasks"; ///< Klucz identyfikuj¹cy zadania dla wybranego dnia.
        string json = JsonUtility.ToJson(new TaskListWrapper(tasks)); ///< Serializuje listê zadañ do formatu JSON.
        PlayerPrefs.SetString(taskKey, json); ///< Zapisuje dane JSON w `PlayerPrefs`.
        PlayerPrefs.Save(); ///< Utrwala zmiany w pamiêci urz¹dzenia.
    }

    /**
     * @brief Wczytuje listê zadañ z pamiêci urz¹dzenia.
     * 
     * Metoda deserializuje zapisane dane JSON z `PlayerPrefs` i odtwarza listê zadañ 
     * dla wybranego dnia. Jeœli dane nie istniej¹, inicjalizuje pust¹ listê.
     */
    private void LoadTasks()
    {
        string taskKey = $"Day_{selectedDay}_Tasks"; ///< Klucz identyfikuj¹cy zadania dla wybranego dnia.
        string savedTasks = PlayerPrefs.GetString(taskKey, ""); ///< Pobiera zapisane dane JSON z `PlayerPrefs`.

        if (!string.IsNullOrEmpty(savedTasks))
        {
            TaskListWrapper taskListWrapper = JsonUtility.FromJson<TaskListWrapper>(savedTasks); ///< Deserializuje dane JSON.
            tasks = taskListWrapper?.tasks ?? new List<Task>(); ///< Przywraca listê zadañ lub inicjalizuje pust¹ listê.
        }
        else
        {
            tasks = new List<Task>(); ///< Inicjalizuje pust¹ listê, jeœli dane nie istniej¹.
        }
    }

    /**
     * @brief Przechodzi do widoku kalendarza.
     * 
     * Metoda zmienia scenê aplikacji na scenê o nazwie "CalendarScene".
     */
    public void ReturnToCalendar()
    {
        SceneManager.LoadScene("CalendarScene"); ///< £aduje scenê "CalendarScene".
    }

    /**
     * @class TaskListWrapper
     * @brief Klasa pomocnicza dla serializacji listy zadañ.
     * 
     * Klasa opakowuje listê zadañ, umo¿liwiaj¹c jej serializacjê do formatu JSON.
     */
    [Serializable]
    private class TaskListWrapper
    {
        public List<Task> tasks; ///< Lista zadañ do zapisania.

        /**
         * @brief Konstruktor klasy `TaskListWrapper`.
         * 
         * @param tasks Lista zadañ do opakowania.
         */
        public TaskListWrapper(List<Task> tasks)
        {
            this.tasks = tasks; ///< Inicjalizuje listê zadañ.
        }
    }

}
