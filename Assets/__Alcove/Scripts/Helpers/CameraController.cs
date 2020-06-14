using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VR;

public class CameraController : SingletonMonoBehaviour<CameraController>
{
    public static GameObject CameraContainer;
    public static GameObject MainCameraObj;
    public static Camera MainCamera;

    public static Ray CameraRay
    {
        get { return new Ray(MainCameraObj.transform.position, MainCameraObj.transform.forward); }
    }

    void Awake()
    {
        //Set statics
        MainCamera = transform.Find("TrackingSpace/CenterEyeAnchor").GetComponent<Camera>();
        MainCameraObj = MainCamera.gameObject;
        CameraContainer = gameObject;
    }

    public void TurnLeft()
    {
        CameraContainer.transform.Rotate(0f, -22.5f, 0f);
    }

    public void TurnRight()
    {
        CameraContainer.transform.Rotate(0f, 22.5f, 0f);
    }

    public void PositionObjectInFrontOfCamera(GameObject obj, float distance = 2.5f)
    {
        Vector3 targetPos = MainCameraObj.transform.position;
        targetPos.z = targetPos.z + Mathf.Cos(MainCameraObj.transform.eulerAngles.y * Mathf.Deg2Rad) * distance;
        targetPos.x = targetPos.x + Mathf.Sin(MainCameraObj.transform.eulerAngles.y * Mathf.Deg2Rad) * distance;
        obj.transform.position = targetPos;

        obj.transform.rotation = Quaternion.LookRotation(obj.transform.position - MainCameraObj.transform.position);
        obj.transform.eulerAngles = new Vector3(0f, obj.transform.eulerAngles.y, 0f);
    }

    public void TurnObjectToFaceCamera(GameObject obj)
    {
        obj.transform.rotation = Quaternion.LookRotation(obj.transform.position - MainCameraObj.transform.position);
        obj.transform.eulerAngles = new Vector3(0f, obj.transform.eulerAngles.y, 0f);
    }
}
