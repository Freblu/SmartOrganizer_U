using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class LocationService : MonoBehaviour
{
    public TMP_Text latitudeText;  // Tekst do wy�wietlenia szeroko�ci geograficznej
    public TMP_Text longitudeText; // Tekst do wy�wietlenia d�ugo�ci geograficznej

    void Start()
    {
        StartCoroutine(GetLocation());
    }



    IEnumerator GetLocation()
    {
        // Sprawd�, czy u�ytkownik zezwoli� na dost�p do lokalizacji
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Lokalizacja jest wy��czona na urz�dzeniu!");
            yield break;
        }

        // Uruchom us�ug� lokalizacji
        Input.location.Start();

        // Czekaj na uruchomienie us�ugi (do 20 sekund)
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Je�li us�uga nie uruchomi�a si�, zako�cz
        if (maxWait <= 0 || Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Nie mo�na uzyska� lokalizacji.");
            yield break;
        }
        else
        {
            // Pobierz aktualn� lokalizacj�
            float latitude = Input.location.lastData.latitude;
            float longitude = Input.location.lastData.longitude;

            // Wy�wietl lokalizacj� w UI
            if (latitudeText != null) latitudeText.text = "Latitude: " + latitude;
            if (longitudeText != null) longitudeText.text = "Longitude: " + longitude;

            Debug.Log("Lokalizacja uzyskana: Latitude: " + latitude + ", Longitude: " + longitude);
        }

        // Zatrzymaj us�ug� lokalizacji
        Input.location.Stop();
    }
}
