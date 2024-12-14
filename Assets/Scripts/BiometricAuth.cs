using UnityEngine;

public class BiometricAuth : MonoBehaviour
{
    private AndroidJavaObject biometricHelper;

    private void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            try
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                biometricHelper = new AndroidJavaObject("com.example.biometricauth.BiometricHelper", currentActivity);
                Debug.Log("Biometric Helper initialized.");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Biometric initialization failed: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning("Biometria obs³ugiwana tylko na urz¹dzeniach Android.");
        }
    }

    public void AuthenticateUser()
    {
        if (Application.platform == RuntimePlatform.Android && biometricHelper != null)
        {
            biometricHelper.Call("authenticate");
        }
        else
        {
            Debug.LogWarning("Autoryzacja liniami papilarnymi jest dostêpna tylko na urz¹dzeniach Android.");
        }
    }
}