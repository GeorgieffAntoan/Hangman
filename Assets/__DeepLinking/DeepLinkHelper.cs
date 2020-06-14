using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Oculus.Platform;
using Oculus.Platform.Models;
using UnityEngine;

public class DeepLinkHelper : MonoBehaviour
{
    public static bool WasLaunchedFromAlcove = false;

    public static DeepLinkData GetDeepLinkData()
    {
        if (WasLaunchedFromAlcove)
        {
            LaunchDetails launchDetails = Oculus.Platform.ApplicationLifecycle.GetLaunchDetails();
            return ParseLaunchDetails(launchDetails);
        }
        return null;
    }

    public static void ExitToAlcove()
    {
        ReturnToAlcove.ReturnNow();
    }

    public static bool CheckForAlcoveLaunch()
    {

        LaunchDetails launchDetails = Oculus.Platform.ApplicationLifecycle.GetLaunchDetails();
        switch (launchDetails.LaunchType)
        {
            case LaunchType.Deeplink:
                DeepLinkData launchData = ParseLaunchDetails(launchDetails);
                if (launchData.SenderAppId.Equals("2855012877858033", StringComparison.InvariantCultureIgnoreCase))
                    return true;
                break;
            default:
                break;
        }

        return false;
    }

    public static DeepLinkData ParseLaunchDetails(LaunchDetails launchDetails)
    {
        try
        {
            DeepLinkData launchData =
                JsonConvert.DeserializeObject<DeepLinkData>(launchDetails.DeeplinkMessage);
            return launchData;
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to parse deep link message. Printing Exception.");
            Debug.LogException(e);

            Debug.Log("Deep link message was " + launchDetails.DeeplinkMessage);
            return null;
        }
    }
}
