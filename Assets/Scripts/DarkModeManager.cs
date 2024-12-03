using UnityEngine;
using UnityEngine.UI;

public class DarkModeManager : MonoBehaviour
{
    [SerializeField]
    private DarkMode darkMode;
    public enum Mode
    {
        Light,
        Dark
    }

    // Enum do wyboru trybu
    [SerializeField]
    private Mode currentMode;

    // Obiekt z komponentem Image (np. t³o)
    [SerializeField]
    private Image background;

    private void Start()
    {

        // Ustaw kolory HEX
        darkMode.SetColors("#81D0FF", "#005587"); // Bia³y dla Light, czarny dla Dark
        // Ustaw pocz¹tkowy kolor na podstawie trybu
        UpdateBackgroundColor();
    }

    // Publiczna metoda do zmiany trybu
    public void ToggleMode()
    {
        Debug.Log("ToggleMode clicked!");
        // Prze³¹czamy tryb
        currentMode = currentMode == Mode.Light ? Mode.Dark : Mode.Light;
        UpdateBackgroundColor();
    }

    // Aktualizuje kolor na podstawie wybranego trybu
    private void UpdateBackgroundColor()
    {
        if (background == null)
        {
            Debug.LogError("Background Image is not assigned!");
            return;
        }

        // Ustawiamy kolor w zale¿noœci od trybu
        background.color = currentMode == Mode.Light ? darkMode.lightColor : darkMode.darkColor;
    }


}