using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class LocationService : MonoBehaviour
{
    public TMP_Text latitudeText;  // Tekst do wyœwietlenia szerokoœci geograficznej
    public TMP_Text longitudeText; // Tekst do wyœwietlenia d³ugoœci geograficznej

    void Start()
    {
        StartCoroutine(GetLocation());
    }



    IEnumerator GetLocation()
    {
        // SprawdŸ, czy u¿ytkownik zezwoli³ na dostêp do lokalizacji
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Lokalizacja jest wy³¹czona na urz¹dzeniu!");
            yield break;
        }

        // Uruchom us³ugê lokalizacji
        Input.location.Start();

        // Czekaj na uruchomienie us³ugi (do 20 sekund)
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Jeœli us³uga nie uruchomi³a siê, zakoñcz
        if (maxWait <= 0 || Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Nie mo¿na uzyskaæ lokalizacji.");
            yield break;
        }
        else
        {
            // Pobierz aktualn¹ lokalizacjê
            float latitude = Input.location.lastData.latitude;
            float longitude = Input.location.lastData.longitude;

            // Wyœwietl lokalizacjê w UI
            if (latitudeText != null) latitudeText.text = "Latitude: " + latitude;
            if (longitudeText != null) longitudeText.text = "Longitude: " + longitude;

            Debug.Log("Lokalizacja uzyskana: Latitude: " + latitude + ", Longitude: " + longitude);
        }

        // Zatrzymaj us³ugê lokalizacji
        Input.location.Stop();
    }
}
