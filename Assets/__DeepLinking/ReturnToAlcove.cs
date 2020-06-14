using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToAlcove : MonoBehaviour
{
    public static bool ReturnReturnAutomatically = true;

    public static ulong AlcoveGoAppId = 2855012877858033;
    public static string AlcovePackageName = "com.aarpinnovation.alcove";

    void OnApplicationQuit()
    {
        AlcoveDeepLinkController.DeepLinkToApplication(AlcoveGoAppId, AlcovePackageName, "AppReturn: " + Application.productName);
    }

    //This will launch the platform application when the Oculus Home button is pressed instead of displaying the Quit/Resume dialog
    void OnApplicationPause(bool isPaused)
    {
        if (isPaused && OVRPlugin.userPresent && !NativeDeepLinkController.LaunchingOtherApp)
        {
            AlcoveDeepLinkController.DeepLinkToApplication(AlcoveGoAppId, AlcovePackageName, "AppReturn: " + Application.productName);
        }
    }

    public static void ReturnNow(string extraData = null, string multiplayerRoomId = null)
    {
        AlcoveDeepLinkController.DeepLinkToApplication(AlcoveGoAppId, Application.identifier, extraData, multiplayerRoomId);
    }
}
