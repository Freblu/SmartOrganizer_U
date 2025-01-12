using UnityEngine;

public class AccelerometerCalendarController : MonoBehaviour
{
    private Vector3 acceleration; // Odczyt z akcelerometru
    private CalendarManager calendarManager; // Odwo³anie do mened¿era kalendarza

    private const float tiltThreshold = 0.5f; // Próg wykrywania przechylenia
    private bool hasChangedMonth = false; // Flaga, aby unikn¹æ wielokrotnej zmiany miesi¹ca
    private bool isAccelerometerAvailable = true; // Czy akcelerometr jest dostêpny

    void Start()
    {
        // SprawdŸ, czy aplikacja dzia³a na symulatorze lub czy akcelerometr jest dostêpny
        if (!SystemInfo.supportsAccelerometer || Application.isEditor)
        {
            Debug.LogWarning("Akcelerometr jest niedostêpny lub aplikacja dzia³a na symulatorze. Funkcjonalnoœæ przechy³u zosta³a wy³¹czona.");
            isAccelerometerAvailable = false;
            return; // Wy³¹cz funkcjonalnoœæ akcelerometru
        }

        // ZnajdŸ obiekt CalendarManager w scenie
        calendarManager = Object.FindFirstObjectByType<CalendarManager>();


        if (calendarManager == null)
        {
            Debug.LogError("Nie znaleziono obiektu CalendarManager w scenie!");
            enabled = false;
        }
    }

    void Update()
    {
        if (!isAccelerometerAvailable) return; // WyjdŸ, jeœli akcelerometr jest wy³¹czony

        // Odczytaj dane z akcelerometru
        acceleration = Input.acceleration;

        // SprawdŸ, czy urz¹dzenie jest przechylone w prawo
        if (acceleration.x > tiltThreshold && !hasChangedMonth)
        {
            Debug.Log("Urz¹dzenie przechylone w prawo! Zmiana na nastêpny miesi¹c.");
            calendarManager.NextMonth();
            hasChangedMonth = true; // Zablokuj kolejn¹ zmianê
        }
        // SprawdŸ, czy urz¹dzenie jest przechylone w lewo
        else if (acceleration.x < -tiltThreshold && !hasChangedMonth)
        {
            Debug.Log("Urz¹dzenie przechylone w lewo! Zmiana na poprzedni miesi¹c.");
            calendarManager.PreviousMonth();
            hasChangedMonth = true; // Zablokuj kolejn¹ zmianê
        }
        // Odblokuj zmianê miesi¹ca, gdy urz¹dzenie wróci do neutralnej pozycji
        else if (Mathf.Abs(acceleration.x) < 0.2f)
        {
            hasChangedMonth = false;
        }
    }
}
