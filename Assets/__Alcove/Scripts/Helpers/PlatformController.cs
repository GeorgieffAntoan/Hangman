using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public class PlatformController : SingletonMonoBehaviour<PlatformController>
{
    public static bool IsOculus = true;
    
    [Header("Platform specific rig")]
    public GameObject m_Rig;

    //Motion Controllers
    private GameObject _LeftHand;
    private GameObject _RightHand;
    private GameObject _LeftHandModel;
    private GameObject _RightHandModel;
    private bool _MotionControllersActive;

    //Pointers
    private GvrReticlePointer _Reticle;
    private GvrLaserPointer _Laser;

    private OVRInput.Controller _ActiveController;

    void Awake()
    {
        _Reticle = FindObjectOfType<GvrReticlePointer>();
        _Laser = FindObjectOfType<GvrLaserPointer>();
        SetupGearVrOrGo();

#if UNITY_EDITOR || UNITY_STANDALONE
        if (OVRPlugin.GetSystemHeadsetType().ToString().Contains("Rift"))
            SetupForRift();
        else
            ActivateMouse();
#endif
    }

    void Update()
    {
        SetupInputForPlatform();
    }


    void SetupForRift()
    {
        IsOculus = true;
        RaycastHelper.singleton.rayTransform = m_Rig.transform.Find("TrackingSpace/CenterEyeAnchor");
        CameraController.MainCameraObj = m_Rig.transform.Find("TrackingSpace/CenterEyeAnchor").gameObject;
        _LeftHand = m_Rig.transform.Find("TrackingSpace/LeftHandAnchor").gameObject;
        _RightHand = m_Rig.transform.Find("TrackingSpace/RightHandAnchor").gameObject;
        _LeftHandModel =
            m_Rig.transform
                .Find(
                    "TrackingSpace/LeftHandAnchor/LeftControllerAnchor/OVRControllerPrefab/OculusTouchForRiftLeftModel")
                .gameObject;
        _RightHandModel =
            m_Rig.transform
                .Find(
                    "TrackingSpace/RightHandAnchor/RightControllerAnchor/OVRControllerPrefab/OculusTouchForRiftRightModel")
                .gameObject;
        ActivateRightController();
    }

    void SetupGearVrOrGo()
    {
        IsOculus = true;

        RaycastHelper.singleton.rayTransform = m_Rig.transform.Find("TrackingSpace/CenterEyeAnchor");
        CameraController.MainCameraObj = m_Rig.transform.Find("TrackingSpace/CenterEyeAnchor").gameObject;
        _LeftHand = m_Rig.transform.Find("TrackingSpace/LeftHandAnchor").gameObject;
        _RightHand = m_Rig.transform.Find("TrackingSpace/RightHandAnchor").gameObject;

        _LeftHandModel =
            m_Rig.transform.Find("TrackingSpace/LeftHandAnchor/LeftControllerAnchor/OVRControllerPrefab/OculusGoControllerModel").gameObject;
        _RightHandModel =
            m_Rig.transform.Find("TrackingSpace/RightHandAnchor/RightControllerAnchor/OVRControllerPrefab/OculusGoControllerModel").gameObject;

        ActivateRightController();
    }

    void SetupInputForPlatform()
    {
        OVRInput.Controller currentController = OVRInput.GetActiveController();
        if (currentController == _ActiveController)
            return;

        _ActiveController = currentController;

        if (currentController == OVRInput.Controller.LTrackedRemote)
        {
            ActivateLeftController();
        }
        else if (currentController == OVRInput.Controller.RTrackedRemote)
        {
            ActivateRightController();
        }
        else if (currentController == OVRInput.Controller.Touchpad || currentController == OVRInput.Controller.Gamepad)
        {
            ActivateReticle();
        }
    }

    [ContextMenu("Activate Reticle")]
    public void ActivateReticle()
    {
        RaycastHelper.UseMouse = false;
        HideMotionControllers();

        if (_Laser)
            _Laser.gameObject.SetActive(false);
        if (_Reticle)
            _Reticle.gameObject.SetActive(true);
        GvrPointerInputModule.Pointer = _Reticle;

        if (RaycastHelper.singleton != null && CameraController.MainCameraObj != null)
            RaycastHelper.singleton.rayTransform = CameraController.MainCameraObj.transform;

        if (_RightHand)
            _RightHand.SetActive(false);
        if (_LeftHand)
            _LeftHand.SetActive(false);

        ReticleVisibilityController.HideOneFrame = true;
        _MotionControllersActive = false;
        _Reticle.raycastMode = GvrBasePointer.RaycastMode.Camera;
    }

    [ContextMenu("Activate Mouse")]
    public void ActivateMouse()
    {
        HideMotionControllers();

        ActivateReticle();
        _Laser.raycastMode = GvrBasePointer.RaycastMode.Mouse;
        _Reticle.raycastMode = GvrBasePointer.RaycastMode.Mouse;
        GvrPointerInputModule.Pointer = _Reticle;
        _MotionControllersActive = false;

        ReticleVisibilityController.StayHidden = true;

        Debug.Log("Mouse activated");
    }

    [ContextMenu("Activate Left Controller")]
    public void ActivateLeftController()
    {
        RaycastHelper.UseMouse = false;
        _MotionControllersActive = true;
        if (!_LeftHandModel.activeSelf)
            ShowMotionControllers();

        ActivateController(_LeftHand);

        if (_RightHand)
            _RightHand.SetActive(false);
        if (_LeftHand)
            _LeftHand.SetActive(true);
    }

    [ContextMenu("Activate Right Controller")]
    public void ActivateRightController()
    {
        RaycastHelper.UseMouse = false;
        _MotionControllersActive = true;
        if (!_RightHandModel.activeSelf)
            ShowMotionControllers();


        ActivateController(_RightHand);

        if (_RightHand)
            _RightHand.SetActive(true);
        if (_LeftHand)
            _LeftHand.SetActive(false);
    }

    public void ActivateController(GameObject controllerObj)
    {
        RaycastHelper.UseMouse = false;
        if (_Laser)
            _Laser.gameObject.SetActive(true);
        if (_Reticle)
            _Reticle.gameObject.SetActive(false);
        GvrPointerInputModule.Pointer = _Laser;

        if (RaycastHelper.singleton && _Laser)
            RaycastHelper.singleton.rayTransform = _Laser.transform;

        _MotionControllersActive = true;
        _Laser.raycastMode = GvrBasePointer.RaycastMode.Direct;
    }

    public void ShowMotionControllers()
    {
        if (!_MotionControllersActive)
            return;

        if (_LeftHandModel)
            _LeftHandModel.SetActive(true);
        if (_RightHandModel)
            _RightHandModel.SetActive(true);
    }

    public void HideMotionControllers()
    {
        if (_LeftHandModel)
            _LeftHandModel.SetActive(false);
        if (_RightHandModel)
            _RightHandModel.SetActive(false);
    }

    [ContextMenu("Hide reticle")]
    public void HideReticle()
    {
        if (_Reticle)
            _Reticle.GetComponent<MeshRenderer>().enabled = false;

        if (_Laser)
            _Laser.gameObject.SetActive(false);
    }

    [ContextMenu("Show reticle")]
    public void ShowReticle()
    {
        if (_Reticle)
            _Reticle.GetComponent<MeshRenderer>().enabled = true;
        if (_Laser && _MotionControllersActive)
            _Laser.gameObject.SetActive(true);
    }
}