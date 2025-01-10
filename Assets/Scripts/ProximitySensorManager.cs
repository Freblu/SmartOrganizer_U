using UnityEngine;

public class ProximitySensorManager : MonoBehaviour
{
    private AndroidJavaObject proximitySensor; // Obiekt do komunikacji z pluginem
    private bool isSensorAvailable = false;
    private float proximityValue = -1;

    void Start()
    {
        Debug.Log("Inicjalizacja czujnika zbli¿eniowego...");

        // Inicjalizacja czujnika zbli¿eniowego
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            proximitySensor = new AndroidJavaObject("com.example.proximitysensor.ProximitySensorPlugin", activity);

            // SprawdŸ dostêpnoœæ czujnika
            isSensorAvailable = proximitySensor != null;
        }

        if (isSensorAvailable)
        {
            Debug.Log("Czujnik zbli¿eniowy jest dostêpny.");
            StartSensor();
        }
        else
        {
            Debug.LogWarning("Czujnik zbli¿eniowy nie jest dostêpny na tym urz¹dzeniu.");
        }
    }

    void Update()
    {
        if (isSensorAvailable)
        {
            // Pobierz aktualn¹ wartoœæ czujnika
            proximityValue = proximitySensor.Call<float>("getProximityValue");
            Debug.Log($"Aktualna wartoœæ czujnika zbli¿eniowego: {proximityValue}");

            // Przyk³adowa logika: jeœli czujnik wykrywa bliskoœæ
            if (proximityValue >= 0 && proximityValue < 5)
            {
                Debug.Log("Wykryto bliskoœæ. Przechodzenie do nastêpnego miesi¹ca.");
                CalendarManager calendarManager = FindObjectOfType<CalendarManager>();
                if (calendarManager != null)
                {
                    calendarManager.NextMonth();
                    Debug.Log("Przesuniêto kalendarz do nastêpnego miesi¹ca.");
                }
                else
                {
                    Debug.LogError("Nie znaleziono obiektu CalendarManager. Upewnij siê, ¿e jest obecny w scenie.");
                }
            }
        }
        else
        {
            Debug.LogWarning("Czujnik zbli¿eniowy nie jest dostêpny. Update nie dzia³a.");
        }
    }

    public void StartSensor()
    {
        if (isSensorAvailable)
        {
            Debug.Log("Uruchamianie czujnika zbli¿eniowego...");
            proximitySensor.Call("start");
            Debug.Log("Czujnik zbli¿eniowy uruchomiony.");
        }
        else
        {
            Debug.LogWarning("Nie mo¿na uruchomiæ czujnika zbli¿eniowego, poniewa¿ nie jest dostêpny.");
        }
    }

    public void StopSensor()
    {
        if (isSensorAvailable)
        {
            Debug.Log("Zatrzymywanie czujnika zbli¿eniowego...");
            proximitySensor.Call("stop");
            Debug.Log("Czujnik zbli¿eniowy zatrzymany.");
        }
        else
        {
            Debug.LogWarning("Nie mo¿na zatrzymaæ czujnika zbli¿eniowego, poniewa¿ nie jest dostêpny.");
        }
    }

    private void OnDestroy()
    {
        Debug.Log("ProximitySensorManager zosta³ zniszczony. Zatrzymywanie czujnika zbli¿eniowego...");
        StopSensor(); // Zatrzymaj czujnik, gdy obiekt zostanie zniszczony
    }
}
