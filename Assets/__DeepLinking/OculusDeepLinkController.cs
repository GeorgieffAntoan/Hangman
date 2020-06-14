using Newtonsoft.Json;
using Oculus.Platform;
using UnityEngine;

public delegate void DeepLinkSuccessDelegate(string message);
public delegate void DeepLinkErrorDelegate(string errorMessage);

public static class OculusDeepLinkController
{
    public static bool LaunchingOtherApp = false;


    public static void DeepLinkToApplication(ulong appId, string packageName, string extraData = null, string roomId = null, DeepLinkErrorDelegate errorCallback = null, DeepLinkSuccessDelegate successCallback = null)
    {
        LaunchingOtherApp = true;

        DeepLinkData data = new DeepLinkData();
        data.SenderAppId = GetAppIDFromConfig();
        data.ExtraData = extraData;
        data.MultiplayerRoomId = roomId;

#if !UNITY_EDITOR
        data.DeviceId = GetAndroidDeviceID();
#endif

#if !UNITY_EDITOR
        if (packageName.Contains("vrhealth")){
            data.ExtraData = data.ExtraData + ";" + GetAndroidDeviceID();
        }
#endif

        var options = new ApplicationOptions(); options.SetDeeplinkMessage(JsonConvert.SerializeObject(data));
        Oculus.Platform.Application.LaunchOtherApp(appId, options).OnComplete(delegate(Message<string> message)
        {
            if (message.IsError)
            {
                Debug.Log("Error launching other app: " + message.GetError().Message);
                LaunchingOtherApp = false;
                if (errorCallback != null)
                    errorCallback(message.GetError().Message);
            }
            else
            {
                if (successCallback != null)
                {
                    successCallback(message.Data);
                }
            }
        });
    }

    public static string GetAppIDFromConfig()
    {
        if (UnityEngine.Application.platform == RuntimePlatform.Android)
        {
            return PlatformSettings.MobileAppID;
        }
        else
        {
            return PlatformSettings.AppID;
        }
    }

    public static string GetAndroidDeviceID()
    {
        AndroidJavaObject jo = new AndroidJavaObject("android.os.Build");
        string serial = jo.GetStatic<string>("SERIAL");
        return serial;
    }
}
