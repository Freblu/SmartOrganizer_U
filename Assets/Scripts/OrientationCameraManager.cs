using UnityEngine;

/**
 * @class OrientationCameraManager
 * @brief Klasa obs³uguj¹ca dopasowanie widoku kamery do orientacji urz¹dzenia.
 */
public class OrientationCameraManager : MonoBehaviour
{
    public Camera mainCamera; ///< G³ówna kamera sceny.
    public float portraitSize = 5f; ///< Rozmiar kamery w orientacji pionowej (orthographic size).
    public float landscapeSize = 7f; ///< Rozmiar kamery w orientacji poziomej (orthographic size).

    private string currentOrientation = "Portrait"; ///< Domyœlna orientacja urz¹dzenia.

    /**
     * @brief Inicjalizacja.
     * Sprawdza przypisanie kamery i ustawia pocz¹tkowe dopasowanie widoku.
     */
    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main; // Automatycznie przypisz g³ówn¹ kamerê
        }

        if (mainCamera == null)
        {
            Debug.LogError("Brak przypisanej kamery!");
            return;
        }

        UpdateCameraView(); ///< Ustaw pocz¹tkowy widok kamery.
    }

    /**
     * @brief Metoda wywo³ywana co klatkê.
     * Sprawdza orientacjê urz¹dzenia i aktualizuje widok kamery.
     */
    void Update()
    {
        UpdateCameraView();
    }

    /**
     * @brief Aktualizuje widok kamery w zale¿noœci od orientacji.
     */
    private void UpdateCameraView()
    {
        string newOrientation = Screen.width > Screen.height ? "Landscape" : "Portrait";

        if (newOrientation != currentOrientation)
        {
            currentOrientation = newOrientation;

            if (currentOrientation == "Landscape")
            {
                Debug.Log("Tryb poziomy. Dostosowano kamerê.");
                mainCamera.orthographicSize = landscapeSize; // Zwiêksz rozmiar widoku dla poziomego
            }
            else
            {
                Debug.Log("Tryb pionowy. Dostosowano kamerê.");
                mainCamera.orthographicSize = portraitSize; // Zmniejsz rozmiar widoku dla pionowego
            }
        }
    }
}
