using UnityEngine;

/**
 * @class DarkMode
 * @brief Klasa reprezentuj¹ca tryb jasny i ciemny aplikacji.
 * 
 * Klasa pozwala na definiowanie kolorów trybu jasnego i ciemnego 
 * oraz ich ustawianie przy u¿yciu kodów HEX.
 */
[System.Serializable]
public class DarkMode
{
    public Color lightColor; ///< Kolor u¿ywany w trybie jasnym.
    public Color darkColor; ///< Kolor u¿ywany w trybie ciemnym.

    private const string ModeKey = "DarkMode"; ///< Klucz identyfikuj¹cy tryb w PlayerPrefs.

    /**
     * @brief Ustawia kolory trybu jasnego i ciemnego przy u¿yciu kodów HEX.
     * 
     * Metoda konwertuje kody HEX na obiekty typu `Color` i przypisuje je odpowiednim zmiennym.
     * W przypadku nieprawid³owego formatu HEX generuje komunikaty b³êdów w konsoli.
     * 
     * @param lightHex Kod koloru HEX dla trybu jasnego.
     * @param darkHex Kod koloru HEX dla trybu ciemnego.
     */
    public void SetColors(string lightHex, string darkHex)
    {
        if (ColorUtility.TryParseHtmlString(lightHex, out Color parsedLightColor))
        {
            lightColor = parsedLightColor; ///< Przypisuje kolor dla trybu jasnego.
        }
        else
        {
            Debug.LogError($"Invalid Light Color HEX: {lightHex}"); ///< Loguje b³¹d dla nieprawid³owego kodu HEX.
        }

        if (ColorUtility.TryParseHtmlString(darkHex, out Color parsedDarkColor))
        {
            darkColor = parsedDarkColor; ///< Przypisuje kolor dla trybu ciemnego.
        }
        else
        {
            Debug.LogError($"Invalid Dark Color HEX: {darkHex}"); ///< Loguje b³¹d dla nieprawid³owego kodu HEX.
        }
    }
}
