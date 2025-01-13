using UnityEngine;
using TMPro;

/**
 * @class OrientationSensorManager
 * @brief Klasa obs³uguj¹ca orientacjê urz¹dzenia.
 * 
 * Monitoruje zmiany orientacji urz¹dzenia i dostosowuje widocznoœæ sceny
 * przez modyfikacjê orthographicSize kamery.
 */
public class OrientationSensorManager : MonoBehaviour
{
    public Camera mainCamera; ///< G³ówna kamera sceny.
    public TMP_Text orientationText; ///< Tekst wyœwietlaj¹cy aktualn¹ orientacjê urz¹dzenia.

    private string currentOrientation = "Portrait"; ///< Domyœlna orientacja urz¹dzenia.

    public float portraitSize = 5f; ///< Orthographic size dla orientacji pionowej.
    public float landscapeSize = 18f; ///< Orthographic size dla orientacji poziomej.

    /**
     * @brief Inicjalizacja czujnika orientacji.
     * 
     * Sprawdza, czy wszystkie wymagane obiekty zosta³y przypisane.
     */
    void Start()
    {
        if (mainCamera == null)
        {
            Debug.LogWarning("Brak przypisanej kamery w Inspectorze.");
            return;
        }

        UpdateCameraSize(); ///< Ustawienie pocz¹tkowej orientacji.
    }

    /**
     * @brief Metoda wywo³ywana co klatkê.
     * 
     * Sprawdza aktualn¹ orientacjê urz¹dzenia i dostosowuje orthographicSize kamery.
     */
    void Update()
    {
        UpdateCameraSize();
    }

    /**
     * @brief Aktualizuje orientacjê i dostosowuje orthographicSize kamery.
     */
    private void UpdateCameraSize()
    {
        string newOrientation = Screen.width > Screen.height ? "Landscape" : "Portrait";

        if (newOrientation != currentOrientation)
        {
            currentOrientation = newOrientation;

            if (currentOrientation == "Landscape")
            {
                mainCamera.orthographicSize = landscapeSize; ///< Dostosuj widocznoœæ dla trybu poziomego.
                Debug.Log("Tryb poziomy. Zmieniono rozmiar kamery.");
            }
            else
            {
                mainCamera.orthographicSize = portraitSize; ///< Dostosuj widocznoœæ dla trybu pionowego.
                Debug.Log("Tryb pionowy. Zmieniono rozmiar kamery.");
            }

            if (orientationText != null)
            {
                orientationText.text = $"Orientation: {currentOrientation}";
            }
        }
    }
}
