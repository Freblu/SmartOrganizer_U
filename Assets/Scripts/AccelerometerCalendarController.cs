using UnityEngine;
/**
 * @class AccelerometerCalendarController
 * @brief Klasa obs³uguj¹ca sterowanie kalendarzem za pomoc¹ przechylenia urz¹dzenia.
 * 
 * Klasa wykorzystuje akcelerometr urz¹dzenia do zmiany miesi¹ca w kalendarzu 
 * w zale¿noœci od przechylenia w prawo lub w lewo. W przypadku wykrycia 
 * braku akcelerometru funkcjonalnoœæ jest automatycznie wy³¹czana.
 */
public class AccelerometerCalendarController : MonoBehaviour
{
    private Vector3 acceleration; ///< Wektor danych z akcelerometru.
    private CalendarManager calendarManager; ///< Odwo³anie do obiektu zarz¹dzaj¹cego kalendarzem.

    private const float tiltThreshold = 0.5f; ///< Próg przechylenia dla zmiany miesi¹ca.
    private bool hasChangedMonth = false; ///< Flaga zapobiegaj¹ca wielokrotnej zmianie miesi¹ca.
    private bool isAccelerometerAvailable = true; ///< Czy akcelerometr jest dostêpny na urz¹dzeniu.

    /**
     * @brief Metoda inicjalizuj¹ca dzia³anie akcelerometru i sprawdzaj¹ca jego dostêpnoœæ.
     * 
     * Sprawdza, czy akcelerometr jest obs³ugiwany przez urz¹dzenie, oraz odnajduje obiekt 
     * `CalendarManager` w scenie. W przypadku braku akcelerometru wy³¹cza obs³ugê tej funkcji.
     */
    void Start()
    {
        // SprawdŸ, czy aplikacja dzia³a na symulatorze lub czy akcelerometr jest dostêpny
        if (!SystemInfo.supportsAccelerometer || Application.isEditor)
        {
            Debug.LogWarning("Akcelerometr jest niedostêpny lub aplikacja dzia³a na symulatorze. Funkcjonalnoœæ przechy³u zosta³a wy³¹czona.");
            isAccelerometerAvailable = false;
            return; ///< Wy³¹cz funkcjonalnoœæ akcelerometru.
        }

        // ZnajdŸ obiekt CalendarManager w scenie
        calendarManager = Object.FindFirstObjectByType<CalendarManager>();

        if (calendarManager == null)
        {
            Debug.LogError("Nie znaleziono obiektu CalendarManager w scenie!"); ///< Loguj b³¹d, jeœli kalendarz nie zosta³ znaleziony.
            enabled = false; ///< Wy³¹cz komponent, jeœli kalendarz nie jest dostêpny.
        }
    }

    /**
     * @brief Metoda aktualizuj¹ca stan akcelerometru w czasie rzeczywistym.
     * 
     * Sprawdza aktualny stan przechylenia urz¹dzenia. W przypadku przechylenia w prawo 
     * lub w lewo wywo³uje odpowiednie metody do zmiany miesi¹ca w kalendarzu. Odblokowuje 
     * ponown¹ zmianê miesi¹ca, gdy urz¹dzenie wraca do neutralnej pozycji.
     */
    void Update()
    {
        if (!isAccelerometerAvailable) return; ///< WyjdŸ, jeœli akcelerometr jest wy³¹czony.

        // Odczytaj dane z akcelerometru
        acceleration = Input.acceleration;

        // SprawdŸ, czy urz¹dzenie jest przechylone w prawo
        if (acceleration.x > tiltThreshold && !hasChangedMonth)
        {
            Debug.Log("Urz¹dzenie przechylone w prawo! Zmiana na nastêpny miesi¹c.");
            calendarManager.NextMonth(); ///< Zmieñ miesi¹c na nastêpny.
            hasChangedMonth = true; ///< Zablokuj kolejn¹ zmianê.
        }
        // SprawdŸ, czy urz¹dzenie jest przechylone w lewo
        else if (acceleration.x < -tiltThreshold && !hasChangedMonth)
        {
            Debug.Log("Urz¹dzenie przechylone w lewo! Zmiana na poprzedni miesi¹c.");
            calendarManager.PreviousMonth(); ///< Zmieñ miesi¹c na poprzedni.
            hasChangedMonth = true; ///< Zablokuj kolejn¹ zmianê.
        }
        // Odblokuj zmianê miesi¹ca, gdy urz¹dzenie wróci do neutralnej pozycji
        else if (Mathf.Abs(acceleration.x) < 0.2f)
        {
            hasChangedMonth = false; ///< Odblokuj zmianê miesi¹ca.
        }
    }
}
