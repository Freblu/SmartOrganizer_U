using UnityEngine;
using TMPro;

/**
 * @class BatterySensorManager
 * @brief Klasa obs³uguj¹ca odczytywanie i wyœwietlanie poziomu baterii.
 * 
 * Monitoruje poziom na³adowania baterii i wyœwietla status w polu tekstowym.
 */
public class BatterySensorManager : MonoBehaviour
{
    public TMP_Text batteryStatusText; ///< Pole tekstowe do wyœwietlania statusu baterii.

    /**
     * @brief Inicjalizacja czujnika baterii.
     * 
     * Sprawdza, czy pole tekstowe zosta³o przypisane.
     */
    void Start()
    {
        if (batteryStatusText == null)
        {
            Debug.LogWarning("Brak przypisanego BatteryStatusText w Inspectorze!");
        }
    }

    /**
     * @brief Aktualizacja statusu baterii w ka¿dej klatce.
     */
    void Update()
    {
        UpdateBatteryStatus();
    }

    /**
     * @brief Pobiera poziom na³adowania baterii i aktualizuje pole tekstowe.
     */
    private void UpdateBatteryStatus()
    {
        // Pobierz poziom na³adowania baterii
        float batteryLevel = SystemInfo.batteryLevel * 100f; // Konwertuj na procenty
        BatteryStatus batteryStatus = SystemInfo.batteryStatus; // Pobierz status ³adowania

        // Stwórz wiadomoœæ do wyœwietlenia
        string statusMessage = $"Poziom baterii: {batteryLevel:0}%\nStatus: {batteryStatus}";

        // Wyœwietl wiadomoœæ w polu tekstowym
        if (batteryStatusText != null)
        {
            batteryStatusText.text = statusMessage;
        }
    }
}
 