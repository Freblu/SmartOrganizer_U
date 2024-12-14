using UnityEngine;

[System.Serializable]
public class DarkMode
{
    public Color lightColor;
    public Color darkColor;

    private const string ModeKey = "DarkMode"; // Klucz w PlayerPrefs
    // Metoda ustawiaj¹ca kolory przy u¿yciu kodów HEX
    public void SetColors(string lightHex, string darkHex)
    {
        if (ColorUtility.TryParseHtmlString(lightHex, out Color parsedLightColor))
        {
            lightColor = parsedLightColor;
        }
        else
        {
            Debug.LogError($"Invalid Light Color HEX: {lightHex}");
        }

        if (ColorUtility.TryParseHtmlString(darkHex, out Color parsedDarkColor))
        {
            darkColor = parsedDarkColor;
        }
        else
        {
            Debug.LogError($"Invalid Dark Color HEX: {darkHex}");
        }

    }


}