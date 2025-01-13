using UnityEngine; // MonoBehaviour i inne elementy Unity
/**
 * @class FingerprintAuthentication
 * @brief Klasa obs³uguj¹ca uwierzytelnianie za pomoc¹ odcisków palców w systemie Android.
 * 
 * Klasa integruje siê z natywnym API Androida, aby sprawdziæ dostêpnoœæ uwierzytelniania 
 * biometrycznego oraz przeprowadziæ proces uwierzytelniania za pomoc¹ odcisków palców.
 */
public class FingerprintAuthentication : MonoBehaviour
{
    private AndroidJavaObject fingerprintAuth; ///< Obiekt Java umo¿liwiaj¹cy komunikacjê z natywnym API Androida.
    private bool authenticationSuccess = false; ///< Flaga wskazuj¹ca, czy uwierzytelnianie zakoñczy³o siê sukcesem.

    /**
     * @brief Inicjalizuje obiekt `FingerprintAuthentication` podczas uruchamiania sceny.
     * 
     * Metoda sprawdza, czy aplikacja dzia³a na platformie Android. Jeœli tak, 
     * tworzy obiekt Java odpowiedzialny za uwierzytelnianie odciskami palców. 
     * W przypadku problemów loguje odpowiednie b³êdy.
     */
    void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            try
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                fingerprintAuth = new AndroidJavaObject("com.example.fingerprintauth.FingerprintAuthentication", activity); ///< Inicjalizacja obiektu Java.
            }
            catch (AndroidJavaException e)
            {
                Debug.LogError("Fingerprint authentication setup failed: " + e.Message); ///< Loguje b³¹d inicjalizacji.
            }
        }
        else
        {
            Debug.LogWarning("Fingerprint authentication is only available on Android."); ///< Ostrzega, jeœli aplikacja dzia³a na nieobs³ugiwanej platformie.
        }
    }

    /**
     * @brief Sprawdza, czy uwierzytelnianie za pomoc¹ odcisków palców jest dostêpne.
     * 
     * Metoda wywo³uje natywne API Androida w celu sprawdzenia dostêpnoœci 
     * funkcji uwierzytelniania biometrycznego.
     * 
     * @return `true` jeœli uwierzytelnianie jest dostêpne, w przeciwnym razie `false`.
     */
    public bool IsFingerprintAvailable()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            try
            {
                return fingerprintAuth.Call<bool>("isFingerprintAvailable"); ///< Wywo³anie natywnej metody sprawdzaj¹cej dostêpnoœæ.
            }
            catch (AndroidJavaException e)
            {
                Debug.LogError("Error checking fingerprint availability: " + e.Message); ///< Loguje b³êdy zwi¹zane z wywo³aniem metody.
            }
        }
        return false; ///< Zwraca `false` dla nieobs³ugiwanych platform.
    }

    /**
     * @brief Przeprowadza uwierzytelnianie za pomoc¹ odcisków palców.
     * 
     * Metoda wywo³uje natywne API Androida, aby uwierzytelniæ u¿ytkownika przy u¿yciu 
     * odcisków palców. W przypadku sukcesu ustawia flagê `authenticationSuccess` na `true`.
     * 
     * @return `true` jeœli uwierzytelnianie zakoñczy³o siê sukcesem, w przeciwnym razie `false`.
     */
    public bool AuthenticateWithFingerprint()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            try
            {
                authenticationSuccess = fingerprintAuth.Call<bool>("authenticateWithFingerprint"); ///< Wywo³anie metody uwierzytelniania.
            }
            catch (AndroidJavaException e)
            {
                Debug.LogError("Error during fingerprint authentication: " + e.Message); ///< Loguje b³êdy zwi¹zane z procesem uwierzytelniania.
            }
        }
        else
        {
            Debug.LogWarning("Fingerprint authentication is only available on Android."); ///< Ostrzega, jeœli aplikacja dzia³a na nieobs³ugiwanej platformie.
        }
        return authenticationSuccess; ///< Zwraca wynik uwierzytelniania.
    }
}
