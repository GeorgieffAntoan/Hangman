using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Oculus.Platform;
using UnityEngine;

public static class NativeDeepLinkController
{
    //We do not want to launch the platform app when we pause due to deep linking to another application. This flag is used to control this behavior.
    public static bool LaunchingOtherApp;

    public static AndroidJavaObject UnityActivity;
    public static AndroidJavaClass DeepLinkHelperClass;

    public static void DeepLinkToApplication(string appId, string packageName, string extraData = null, string roomId = null)
    {
        if (UnityActivity == null || DeepLinkHelperClass == null)
            Initialize();

        LaunchingOtherApp = true;

        DeepLinkData data = new DeepLinkData();
        data.SenderAppId = GetAppIDFromConfig();
        data.ExtraData = extraData;
        data.MultiplayerRoomId = roomId;

#if !UNITY_EDITOR
        if (packageName.Contains("vrhealth"))
            data.ExtraData = data.ExtraData + ";" + GetAndroidDeviceID();
#endif
        string dataJsonStr = JsonConvert.SerializeObject(data);

        Debug.Log(dataJsonStr);

        Debug.Log("Launching app with data string " + dataJsonStr);

#if UNITY_ANDROID && !UNITY_EDITOR
        DeepLinkHelperClass.CallStatic("LaunchApp", UnityActivity, appId, packageName, dataJsonStr);
#endif
    }

    public static DeepLinkData GetIntentData()
    {
        if (UnityActivity == null)
            Initialize();

#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaObject intent = UnityActivity.Call<AndroidJavaObject>("getIntent");
        try
        {
            return JsonConvert.DeserializeObject<DeepLinkData>(SafeCallStringMethod(intent, "getStringExtra",
                "extraData"));
        }
        catch
        {
            return null;
        }
#else
        return null;
#endif
    }

    public static string GetIntentUri()
    {
        if (UnityActivity == null)
            Initialize();

#if UNITY_EDITOR && !UNITY_ANDROID
        AndroidJavaObject intent = UnityActivity.Call<AndroidJavaObject>("getIntent");
        try
        {
            return SafeCallStringMethod(intent, "toUri", 0);
        }
        catch
        {
            return null;
        }
#else
        return null;
#endif
    }

    public static string GetIntentDataString()
    {
        if (UnityActivity == null)
            Initialize();

        AndroidJavaObject intent = UnityActivity.Call<AndroidJavaObject>("getIntent");
        try
        {
            return SafeCallStringMethod(intent, "getDataString");
        }
        catch
        {
            return null;
        }
    }

    public static void LogIntent()
    {
        if (UnityActivity == null)
            Initialize();

#if !UNITY_EDITOR && UNITY_ANDROID
        AndroidJavaClass intentLoggerClass = new AndroidJavaClass("com.rendever.deeplinkhelper.IntentLogger");

        AndroidJavaObject intent = UnityActivity.Call<AndroidJavaObject>("getIntent");
        try
        {
            intentLoggerClass.CallStatic("logFullContent", intent);
        }
        catch
        {
            Debug.LogException(new Exception("Failed to log intent data"));
        }
#endif
    }

    static void Initialize()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        DeepLinkHelperClass = new AndroidJavaClass("com.rendever.deeplinkhelper.DeepLinkHelper");

        AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        UnityActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
#endif
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

    //We need this because normal Unity calls to native methods crash if the native method returns null. 
    public static string SafeCallStringMethod(AndroidJavaObject javaObject, string methodName, params object[] args)
    {
#if UNITY_2018_2_OR_NEWER
        if (args == null) args = new object[] { null };
        IntPtr methodID = AndroidJNIHelper.GetMethodID<string>(javaObject.GetRawClass(), methodName, args, false);
        jvalue[] jniArgs = AndroidJNIHelper.CreateJNIArgArray(args);

        try
        {
            IntPtr returnValue = AndroidJNI.CallObjectMethod(javaObject.GetRawObject(), methodID, jniArgs);
            if (IntPtr.Zero != returnValue)
            {
                var val = AndroidJNI.GetStringUTFChars(returnValue);
                AndroidJNI.DeleteLocalRef(returnValue);
                return val;
            }
        }
        finally
        {
            AndroidJNIHelper.DeleteJNIArgArray(args, jniArgs);
        }

        return null;
#else
            return  javaObject.Call<string>(methodName, args);
#endif
    }

    public static string GetAndroidDeviceID()
    {

        AndroidJavaObject jo = new AndroidJavaObject("android.os.Build");
        string serial = jo.GetStatic<string>("SERIAL");
        return serial;
    }
}
