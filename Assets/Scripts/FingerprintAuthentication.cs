using UnityEngine;

public class FingerprintAuthentication : MonoBehaviour
{
    private AndroidJavaObject fingerprintAuth;

    private bool authenticationSuccess = false;

    void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            try
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                fingerprintAuth = new AndroidJavaObject("com.example.fingerprintauth.FingerprintAuthentication", activity);
            }
            catch (AndroidJavaException e)
            {
                Debug.LogError("Fingerprint authentication setup failed: " + e.Message);
            }
        }
        else
        {
            Debug.LogWarning("Fingerprint authentication is only available on Android.");
        }
    }

    public bool IsFingerprintAvailable()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            try
            {
                return fingerprintAuth.Call<bool>("isFingerprintAvailable");
            }
            catch (AndroidJavaException e)
            {
                Debug.LogError("Error checking fingerprint availability: " + e.Message);
            }
        }
        return false;
    }

    public bool AuthenticateWithFingerprint()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            try
            {
                authenticationSuccess = fingerprintAuth.Call<bool>("authenticateWithFingerprint");
            }
            catch (AndroidJavaException e)
            {
                Debug.LogError("Error during fingerprint authentication: " + e.Message);
            }
        }
        else
        {
            Debug.LogWarning("Fingerprint authentication is only available on Android.");
        }
        return authenticationSuccess;
    }
}
