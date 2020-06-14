using UnityEngine;

public class UserInputController : MonoBehaviour
{
    public static bool ButtonOneDown;
    public static bool ButtonOneUp;
    public static bool ButtonOneHeld;
    public static bool ButtonOneClick;
    public static bool ButtonTwoDown;
    public static bool ButtonTwoUp;
    public static bool ButtonTwoHeld;
    public static bool ButtonThreeDown;
    public static bool ButtonThreeUp;
    public static bool ButtonThreeHeld;
    public static bool ButtonFourDown;
    public static bool ButtonFourUp;
    public static bool ButtonFourHeld;

    public static bool TriggerDown;
    public static bool TriggerHeld;
    public static bool TriggerUp;

    public static Vector2 PrimaryThumbstick = Vector2.zero;
    public static Vector2 SecondaryThumbstick = Vector2.zero;

    void Update ()
    {
        GearVrUpdate();

#if UNITY_EDITOR || UNITY_STANDALONE
        ButtonTwoDown = Input.GetKeyDown(KeyCode.Escape);
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            CameraController.getInstance().TurnLeft();
        if (Input.GetKeyDown(KeyCode.RightArrow))
            CameraController.getInstance().TurnRight();
        ButtonOneDown = Input.GetMouseButtonDown(0);
        ButtonOneUp = Input.GetMouseButtonUp(0);
        ButtonOneHeld = Input.GetMouseButton(0);
        ButtonOneClick = Input.GetMouseButtonDown(0);
#endif
    }

    void GearVrUpdate()
    {
        OVRInput.Controller currentController = OVRInput.GetActiveController();
        if (currentController == OVRInput.Controller.Gamepad)
        {
            ButtonOneDown = OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.Gamepad);
            ButtonOneUp = OVRInput.GetUp(OVRInput.Button.One, OVRInput.Controller.Gamepad);
            ButtonOneHeld = OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.Gamepad);
            ButtonOneClick = OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.Gamepad);

            ButtonTwoDown = OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.Gamepad);
            ButtonTwoUp = OVRInput.GetUp(OVRInput.Button.Two, OVRInput.Controller.Gamepad);
            ButtonTwoHeld = OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.Gamepad);

            PrimaryThumbstick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.Gamepad);
            SecondaryThumbstick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick, OVRInput.Controller.Gamepad);
        }
        else if (currentController == OVRInput.Controller.Touchpad)
        {
            ButtonOneDown = Input.GetMouseButtonDown(0);
            ButtonOneUp = Input.GetMouseButtonUp(0);
            ButtonOneHeld = Input.GetMouseButton(0);
            ButtonOneClick = OVRInput.GetDown(OVRInput.Button.One);

            ButtonTwoDown = OVRInput.GetDown(OVRInput.Button.Back);
            ButtonTwoUp = OVRInput.GetUp(OVRInput.Button.Back);
            ButtonTwoHeld = OVRInput.Get(OVRInput.Button.Back);

            if (OVRInput.GetDown(OVRInput.Button.DpadLeft))
                CameraController.getInstance().TurnLeft();
            else if (OVRInput.GetDown(OVRInput.Button.DpadRight))
                CameraController.getInstance().TurnRight();
        }
        else if (currentController == OVRInput.Controller.LTrackedRemote ||
                 currentController == OVRInput.Controller.RTrackedRemote)
        {
            ButtonOneDown = OVRInput.GetDown(OVRInput.Button.One) ||
                            OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger);
            ButtonOneUp = OVRInput.GetUp(OVRInput.Button.One) || OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger);
            ButtonOneHeld = OVRInput.Get(OVRInput.Button.One) || OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger);
            ButtonOneClick = OVRInput.GetDown(OVRInput.Button.One) ||
                             OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger);

            ButtonTwoDown = OVRInput.GetDown(OVRInput.Button.Back);
            ButtonTwoUp = OVRInput.GetUp(OVRInput.Button.Back);
            ButtonTwoHeld = OVRInput.Get(OVRInput.Button.Back);

            if (OVRInput.GetDown(OVRInput.Button.DpadLeft))
                CameraController.getInstance().TurnLeft();

            if (OVRInput.GetDown(OVRInput.Button.DpadRight))
                CameraController.getInstance().TurnRight();
        }
        else if (currentController == OVRInput.Controller.Touch ||
                currentController == OVRInput.Controller.LTouch || currentController == OVRInput.Controller.RTouch)
        {
            TriggerDown = OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) || OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger);
            TriggerUp = OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger) || OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger);
            TriggerHeld = OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) || OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger);

            ButtonOneDown = OVRInput.GetDown(OVRInput.Button.One);
            ButtonOneUp = OVRInput.GetUp(OVRInput.Button.One);
            ButtonOneHeld = OVRInput.Get(OVRInput.Button.One);

            ButtonTwoDown = OVRInput.GetDown(OVRInput.Button.Two);
            ButtonTwoHeld = OVRInput.Get(OVRInput.Button.Two);
            ButtonTwoUp = OVRInput.GetUp(OVRInput.Button.Two);

            ButtonThreeDown = OVRInput.GetDown(OVRInput.Button.Three);
            ButtonThreeHeld = OVRInput.Get(OVRInput.Button.Three);
            ButtonThreeUp = OVRInput.GetUp(OVRInput.Button.Three);

            ButtonFourDown = OVRInput.GetDown(OVRInput.Button.Four);
            ButtonFourHeld = OVRInput.Get(OVRInput.Button.Four);
            ButtonFourUp = OVRInput.GetUp(OVRInput.Button.Four);

            if (currentController == OVRInput.Controller.Touch)
            {
                PrimaryThumbstick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);
                SecondaryThumbstick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
            }
            else if (currentController == OVRInput.Controller.RTouch)
                PrimaryThumbstick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);
            else if (currentController == OVRInput.Controller.LTouch)
                PrimaryThumbstick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);

            if (OVRInput.GetDown(OVRInput.Button.DpadLeft) && !RaycastHelper.IsPointerOverUIObject())
                CameraController.getInstance().TurnLeft();

            if (OVRInput.GetDown(OVRInput.Button.DpadRight) && !RaycastHelper.IsPointerOverUIObject())
                CameraController.getInstance().TurnRight();
        }
        else
        {
            ButtonOneDown = Input.GetMouseButtonDown(0);
            ButtonOneHeld = Input.GetMouseButton(0);
            ButtonOneUp = Input.GetMouseButtonUp(0);
        }
    }
}
