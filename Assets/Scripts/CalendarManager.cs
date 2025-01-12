using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/**
 * @class CalendarManager
 * @brief Klasa odpowiedzialna za zarz¹dzanie kalendarzem i zadaniami u¿ytkownika.
 * 
 * Klasa obs³uguje funkcje takie jak generowanie kalendarza, nawigacja miêdzy miesi¹cami, 
 * zarz¹dzanie list¹ zadañ, wyœwietlanie inspiruj¹cych cytatów oraz przejœcie do ustawieñ.
 */

public class CalendarManager : MonoBehaviour
{
    // Kalendarz
    public GameObject dayPrefab; ///< Prefab reprezentuj¹cy pojedynczy dzieñ kalendarza.
    public Transform calendarGrid; ///< Transform siatki, w której wyœwietlane s¹ dni.
    public TMP_Text calendarTitle; ///< Tekst wyœwietlaj¹cy nazwê miesi¹ca i roku.
    public TMP_Text quoteText; ///< Tekst wyœwietlaj¹cy inspiruj¹cy cytat.
    public Button previousMonthButton; ///< Przycisk przejœcia do poprzedniego miesi¹ca.
    public Button nextMonthButton; ///< Przycisk przejœcia do nastêpnego miesi¹ca.
    public Button settingsButton; ///< Przycisk otwieraj¹cy ustawienia.

    private int currentYear; ///< Aktualny rok w kalendarzu.
    private int currentMonth = 1; ///< Aktualny miesi¹c w kalendarzu.

    // Zarz¹dzanie zadaniami
    public Transform taskListContent; ///< Kontener dla dynamicznie generowanej listy zadañ.
    public TMP_InputField taskInputField; ///< Pole tekstowe do wprowadzania nowych zadañ.
    public Button addTaskButton; ///< Przycisk dodawania nowego zadania.
    public Button clearTasksButton; ///< Przycisk czyszczenia listy zadañ.

    private List<string> tasks = new List<string>(); ///< Lista przechowuj¹ca zadania.
    private int editIndex = -1; ///< Indeks edytowanego zadania (-1 oznacza brak edycji).

    /**
  * @brief Metoda inicjalizuj¹ca kalendarz oraz przyciski nawigacyjne.
  * 
  * Ustawia cytat inspiruj¹cy, ustawia datê pocz¹tkow¹ na bie¿¹cy miesi¹c i rok, 
  * przypisuje zdarzenia do przycisków, generuje widok kalendarza oraz wczytuje zadania.
  */
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

    /**
   * @brief Generuje siatkê dni dla bie¿¹cego miesi¹ca w kalendarzu.
   * 
   * Metoda tworzy wizualn¹ reprezentacjê miesi¹ca w siatce kalendarza, 
   * dodaj¹c dni oraz puste pola na pocz¹tku miesi¹ca, zgodnie z dniem tygodnia.
   * Ka¿dy dzieñ jest reprezentowany jako obiekt prefabrykowany.
   */
    public void GenerateCalendar()
    {
        // Usuñ poprzednie elementy kalendarza
        foreach (Transform child in calendarGrid)
        {
            Destroy(child.gameObject); ///< Usuwa ka¿dy obiekt w siatce kalendarza.
        }

        // Ustaw tytu³ kalendarza (nazwa miesi¹ca i rok)
        calendarTitle.text = $"{GetMonthName(currentMonth)} {currentYear}";

        // Oblicz szczegó³y bie¿¹cego miesi¹ca
        DateTime firstDayOfMonth = new DateTime(currentYear, currentMonth, 1); ///< Pierwszy dzieñ miesi¹ca.
        int daysInMonth = DateTime.DaysInMonth(currentYear, currentMonth); ///< Liczba dni w miesi¹cu.
        int startDayOfWeek = (int)firstDayOfMonth.DayOfWeek; ///< Dzieñ tygodnia pierwszego dnia (0 = niedziela).

        // Dodaj puste pola dla dni poprzedzaj¹cych pierwszy dzieñ miesi¹ca
        for (int i = 0; i < startDayOfWeek; i++)
        {
            GameObject emptyDay = Instantiate(dayPrefab, calendarGrid); ///< Tworzy puste pole w siatce.
            emptyDay.GetComponentInChildren<TMP_Text>().text = ""; ///< Ustawia tekst jako pusty.
            emptyDay.GetComponent<Button>().interactable = false; ///< Wy³¹cza interakcjê z przyciskiem.
        }

        // Dodaj dni miesi¹ca
        for (int day = 1; day <= daysInMonth; day++)
        {
            GameObject dayObj = Instantiate(dayPrefab, calendarGrid); ///< Tworzy obiekt dnia.
            TMP_Text dayText = dayObj.GetComponentInChildren<TMP_Text>();
            dayText.text = day.ToString(); ///< Ustawia numer dnia.

            // Dodaj interakcjê do dnia
            Button dayButton = dayObj.GetComponent<Button>();
            if (dayButton != null)
            {
                int selectedDay = day; ///< Przechowuje aktualny dzieñ dla delegata.
                dayButton.onClick.AddListener(() => OpenDayPlan(selectedDay)); ///< Dodaje zdarzenie otwieraj¹ce plan dnia.
            }
        }
    }

    /**
     * @brief Pobiera nazwê miesi¹ca na podstawie jego numeru.
     * 
     * Metoda korzysta z klasy `DateTime` w celu wygenerowania nazwy miesi¹ca w bie¿¹cym jêzyku.
     * 
     * @param month Numer miesi¹ca (1 = styczeñ, 12 = grudzieñ).
     * @return Nazwa miesi¹ca jako ci¹g znaków.
     */
    private string GetMonthName(int month)
    {
        return new DateTime(currentYear, month, 1).ToString("MMMM"); ///< Zwraca nazwê miesi¹ca w bie¿¹cym jêzyku.
    }

    /**
     * @brief Prze³¹cza kalendarz na poprzedni miesi¹c.
     * 
     * Metoda zmniejsza numer miesi¹ca. Jeœli bie¿¹cy miesi¹c to styczeñ, prze³¹cza na grudzieñ 
     * poprzedniego roku. Po zmianie generuje nowy widok kalendarza.
     */
    /**
   * @brief Prze³¹cza kalendarz na poprzedni miesi¹c.
   * 
   * Metoda zmniejsza numer miesi¹ca. Jeœli bie¿¹cy miesi¹c to styczeñ, prze³¹cza na grudzieñ 
   * poprzedniego roku. Po zmianie generuje nowy widok kalendarza.
   */
    public void PreviousMonth()
    {
        if (currentMonth == 1)
        {
            currentMonth = 12; ///< Ustaw grudzieñ dla poprzedniego roku.
            currentYear--; ///< Zmniejsz rok.
        }
        else
        {
            currentMonth--; ///< Zmniejsz numer miesi¹ca.
        }

        Debug.Log($"Zmiana na poprzedni miesi¹c: {currentMonth}, Rok: {currentYear}"); ///< Loguj zmianê miesi¹ca.
        GenerateCalendar(); ///< Odœwie¿ widok kalendarza.
    }

    /**
     * @brief Prze³¹cza kalendarz na nastêpny miesi¹c.
     * 
     * Metoda zwiêksza numer miesi¹ca. Jeœli bie¿¹cy miesi¹c to grudzieñ, prze³¹cza na styczeñ 
     * nastêpnego roku. Po zmianie generuje nowy widok kalendarza.
     */
    public void NextMonth()
    {
        if (currentMonth == 12)
        {
            currentMonth = 1; ///< Ustaw styczeñ dla nastêpnego roku.
            currentYear++; ///< Zwiêksz rok.
        }
        else
        {
            currentMonth++; ///< Zwiêksz numer miesi¹ca.
        }

        Debug.Log($"Zmiana na nastêpny miesi¹c: {currentMonth}, Rok: {currentYear}"); ///< Loguj zmianê miesi¹ca.
        GenerateCalendar(); ///< Odœwie¿ widok kalendarza.
    }

    /**
     * @brief Aktualizuje interfejs u¿ytkownika kalendarza.
     * 
     * Metoda umo¿liwia wprowadzenie dodatkowej logiki zwi¹zanej z aktualizacj¹ interfejsu u¿ytkownika, 
     * takiej jak odœwie¿enie elementów UI w odpowiedzi na zmiany w kalendarzu.
     */
    private void UpdateCalendarUI()
    {
        Debug.Log($"Zaktualizowano widok kalendarza na miesi¹c: {currentMonth}"); ///< Loguj aktualizacjê widoku kalendarza.
    }

    /**
     * @brief Otwiera scenê ustawieñ aplikacji.
     * 
     * Metoda prze³¹cza u¿ytkownika na scenê o nazwie "SettingsScene", gdzie mo¿na zmieniaæ ustawienia aplikacji.
     */
    private void OpenSettings()
    {
        SceneManager.LoadScene("SettingsScene"); ///< £aduje scenê ustawieñ.
    }

    /**
     * @brief Otwiera widok planu dnia dla wybranego dnia.
     * 
     * Metoda zapisuje wybran¹ datê (dzieñ, miesi¹c, rok) w `PlayerPrefs` i prze³¹cza u¿ytkownika na scenê 
     * planu dnia o nazwie "DayPlanScene".
     * 
     * @param day Numer wybranego dnia miesi¹ca.
     */
    private void OpenDayPlan(int day)
    {
        PlayerPrefs.SetInt("SelectedDay", day); ///< Zapamiêtaj wybrany dzieñ.
        PlayerPrefs.SetInt("CurrentYear", currentYear); ///< Zapamiêtaj aktualny rok.
        PlayerPrefs.SetInt("CurrentMonth", currentMonth); ///< Zapamiêtaj aktualny miesi¹c.
        PlayerPrefs.Save(); ///< Zapisz zmiany w `PlayerPrefs`.

        SceneManager.LoadScene("DayPlanScene"); ///< Otwórz scenê "DayPlanScene".
    }


    /**
  * @brief Dodaje lub edytuje zadanie w liœcie zadañ.
  * 
  * Metoda sprawdza, czy wprowadzono tekst zadania. Jeœli brak aktywnej edycji, 
  * dodaje nowe zadanie. W przeciwnym przypadku aktualizuje istniej¹ce zadanie. 
  * Nastêpnie odœwie¿a listê zadañ i zapisuje zmiany.
  */
    public void AddOrEditTask()
    {
        string newTask = taskInputField.text;

        if (!string.IsNullOrEmpty(newTask))
        {
            if (editIndex == -1)
            {
                tasks.Add(newTask); ///< Dodaje nowe zadanie do listy.
            }
            else
            {
                tasks[editIndex] = newTask; ///< Aktualizuje istniej¹ce zadanie.
                editIndex = -1;
                addTaskButton.GetComponentInChildren<TMP_Text>().text = "Dodaj Zadanie";
            }
            taskInputField.text = ""; ///< Czyœci pole tekstowe po dodaniu lub edycji.
            UpdateTaskList();
            SaveTasks(); ///< Zapisuje zmiany w `PlayerPrefs`.
        }
    }

    /**
     * @brief Aktualizuje widok listy zadañ.
     * 
     * Metoda usuwa wszystkie istniej¹ce elementy z listy zadañ, a nastêpnie 
     * dodaje je ponownie na podstawie aktualnej zawartoœci listy.
     */
    private void UpdateTaskList()
    {
        foreach (Transform child in taskListContent)
        {
            Destroy(child.gameObject); ///< Usuwa ka¿dy obiekt w kontenerze zadañ.
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
            taskText.text = tasks[i]; ///< Ustawia treœæ zadania.
            taskText.fontSize = 36;

            // Dodaj przycisk "Edytuj"
            GameObject editButtonObject = new GameObject("EditButton");
            editButtonObject.transform.SetParent(taskContainer.transform);
            Button editButton = editButtonObject.AddComponent<Button>();
            TMP_Text editButtonText = editButtonObject.AddComponent<TextMeshProUGUI>();
            editButtonText.text = "Edytuj"; ///< Tekst przycisku "Edytuj".
            editButtonText.color = Color.red;
            editButtonText.fontSize = 36;

            editButton.onClick.AddListener(() => EditTask(index)); ///< Przypisuje funkcjê edycji do przycisku.
        }
    }

    /**
     * @brief Rozpoczyna edycjê wybranego zadania.
     * 
     * Metoda ustawia zawartoœæ pola tekstowego na treœæ wybranego zadania i 
     * zmienia tryb przycisku na "Zapisz Edycjê".
     * 
     * @param index Indeks zadania w liœcie do edycji.
     */
    public void EditTask(int index)
    {
        if (index >= 0 && index < tasks.Count)
        {
            taskInputField.text = tasks[index]; ///< Wype³nia pole tekstowe treœci¹ zadania.
            editIndex = index; ///< Ustawia aktywny indeks edycji.
            addTaskButton.GetComponentInChildren<TMP_Text>().text = "Zapisz Edycjê"; ///< Zmienia tekst przycisku.
        }
    }

    /**
     * @brief Czyœci listê zadañ.
     * 
     * Usuwa wszystkie zadania z listy, odœwie¿a widok i zapisuje zmiany.
     */
    public void ClearTasks()
    {
        tasks.Clear(); ///< Czyœci listê zadañ.
        UpdateTaskList();
        SaveTasks(); ///< Zapisuje zmiany w `PlayerPrefs`.
    }

    /**
     * @brief Zapisuje listê zadañ do `PlayerPrefs`.
     * 
     * Konwertuje listê zadañ na ci¹g znaków oddzielonych œrednikiem i zapisuje j¹ 
     * w `PlayerPrefs` pod kluczem "Tasks".
     */
    private void SaveTasks()
    {
        PlayerPrefs.SetString("Tasks", string.Join(";", tasks)); ///< Zapisuje listê zadañ.
        PlayerPrefs.Save(); ///< Utrwala zmiany w pamiêci urz¹dzenia.
    }

    /**
     * @brief Wczytuje listê zadañ z `PlayerPrefs`.
     * 
     * Pobiera zapisane zadania, rozdziela je na listê i usuwa puste wpisy.
     */
    private void LoadTasks()
    {
        tasks.Clear(); ///< Czyœci bie¿¹c¹ listê zadañ.
        string savedTasks = PlayerPrefs.GetString("Tasks", ""); ///< Pobiera zapisane zadania.
        if (!string.IsNullOrEmpty(savedTasks))
        {
            tasks.AddRange(savedTasks.Split(';')); ///< Rozdziela ci¹g znaków na listê zadañ.
            tasks.RemoveAll(task => string.IsNullOrEmpty(task)); ///< Usuwa puste zadania.
        }
    }

}