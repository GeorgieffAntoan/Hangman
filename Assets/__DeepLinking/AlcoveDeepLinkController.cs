using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AlcoveDeepLinkController
{
    public static bool UseOculus = true;

    public static void DeepLinkToApplication(ulong appId, string packageName, string extraData = null,
        string roomId = null, DeepLinkErrorDelegate errorCallback = null,
        DeepLinkSuccessDelegate successCallback = null)
    {
        if (UseOculus)
            OculusDeepLinkController.DeepLinkToApplication(appId,packageName,extraData,roomId,errorCallback,successCallback);
        else
            NativeDeepLinkController.DeepLinkToApplication(appId.ToString(),packageName,extraData,roomId);
    }

}
