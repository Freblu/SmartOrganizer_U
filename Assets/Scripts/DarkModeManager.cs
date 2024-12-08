using UnityEngine;
using UnityEngine.UI;

public class DarkModeManager : MonoBehaviour
{
    [SerializeField]
    public DarkMode darkMode;
    private void Start()
    {
            // Wczytaj aktualny stan motywu z PlayerPrefs
    int savedMode = PlayerPrefs.GetInt(ModeKey, (int)Mode.Light);

    // Ustaw kolory HEX
    darkMode.SetColors("#81D0FF", "#000546"); // Bia³y dla Light, czarny dla Dark

        // Za³aduj zapisany tryb (domyœlnie Light)
        currentMode = (Mode) PlayerPrefs.GetInt(ModeKey, (int) Mode.Light);
    UpdateBackgroundColor();
    }
    public enum Mode
    {
        Light,
        Dark
    }

    [SerializeField]
    private Mode currentMode; // Obecny tryb

    [SerializeField]
    private Image background; // Obiekt z komponentem Image


    private const string ModeKey = "DarkMode"; // Klucz w PlayerPrefs


    public void ToggleMode()
    {
        // Prze³¹cz tryb
        currentMode = currentMode == Mode.Light ? Mode.Dark : Mode.Light;

        // Zapisz tryb w PlayerPrefs
        PlayerPrefs.SetInt(ModeKey, (int)currentMode);
        PlayerPrefs.Save();

        // Zaktualizuj kolor t³a
        UpdateBackgroundColor();
    }

private void UpdateBackgroundColor()
{
    if (background == null)
    {
        Debug.LogError("Background Image is not assigned!");
        return;
    }

    // Ustaw kolor w zale¿noœci od trybu
    background.color = currentMode == Mode.Light ? darkMode.lightColor : darkMode.darkColor;
}
}