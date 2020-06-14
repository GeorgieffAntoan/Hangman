using System;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class RaycastHelper : MonoBehaviour
{

    private static RaycastHelper _singleton;
    public static RaycastHelper singleton
    {
        get
        {
            if (_singleton == null)
                _singleton = GameObject.FindObjectOfType<RaycastHelper>();

            return _singleton;
        }
    }

    public static bool UseMouse = false;

    [NonSerialized]
    public Transform rayTransform;

    private static GvrPointerInputModule _GvrInputModule;

    void Awake()
    {
        _singleton = this;
    }

    public static Ray GetRay()
    {
        if (UseMouse)
            return GvrPointerInputModule.Pointer.PointerCamera.ScreenPointToRay(Input.mousePosition);
        else
            return GvrBasePointer.CalculateRay(GvrPointerInputModule.Pointer, GvrPointerInputModule.Pointer.raycastMode).ray;
    }

    public Ray GetDirectRay()
    {
        return new Ray(rayTransform.position, rayTransform.forward);
    }

    public Ray GetCameraRay()
    {
        return new Ray(CameraController.MainCamera.transform.position, CameraController.MainCamera.transform.forward);
    }

    public void PositionObjectInFrontOfRaySource(GameObject objToPosition, float distance = 2.5f)
    {
        Vector3 targetPos = new Vector3(0f, 0f, 0f);
        targetPos.z = Mathf.Cos(rayTransform.eulerAngles.y * Mathf.Deg2Rad) * distance;
        targetPos.x = Mathf.Sin(rayTransform.eulerAngles.y * Mathf.Deg2Rad) * distance;
        objToPosition.transform.position = targetPos;

        objToPosition.transform.rotation = Quaternion.LookRotation(objToPosition.transform.position - rayTransform.position);
        objToPosition.transform.eulerAngles = new Vector3(0f, objToPosition.transform.eulerAngles.y, 0f);
    }

    public static bool IsPointerOverUIObject()
    {

        if (_GvrInputModule == null)
            _GvrInputModule = FindObjectOfType<GvrPointerInputModule>();

        if (_GvrInputModule == null)
            return false;

        return _GvrInputModule.Impl.CurrentEventData != null && _GvrInputModule.Impl.CurrentEventData.pointerEnter != null;
    }
}