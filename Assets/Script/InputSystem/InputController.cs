using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class InputController : MonoBehaviour
{


    public static InputSystem InputSystem;

    public static InputActionBuilder
        WalkAction,
        RunAction,
        JumpAction,
        ZoomAction,
        EscAction,
        AltAction,
        InteractAction,
        MouseAction,
        OpenInventoryAction;
    private void Awake()
    {
        if (InputSystem == null)
        {
            InputSystem = new InputSystem();
            WalkAction = new InputActionBuilder(InputSystem.Player.Move);
            RunAction = new InputActionBuilder(InputSystem.Player.RunFast);
            JumpAction = new InputActionBuilder(InputSystem.Player.Jump);
            InteractAction = new InputActionBuilder(InputSystem.Player.Interact);
            ZoomAction = new InputActionBuilder(InputSystem.Player.Zoom);
            EscAction = new InputActionBuilder(InputSystem.Screen.CursorLock);
            AltAction = new InputActionBuilder(InputSystem.Screen.CursorUnfocus);
            OpenInventoryAction = new InputActionBuilder(InputSystem.Player.OpenInventory);
            MouseAction = new InputActionBuilder(InputSystem.Player.Mouse);
        }

    }
    private void Start()
    {
        InteractAction.Enable();
        WalkAction.Enable();
        RunAction.Enable();
        JumpAction.Enable();
        ZoomAction.Enable();
        EscAction.Enable();
        AltAction.Enable();
        MouseAction.Enable();
        OpenInventoryAction.Enable();
    }
    private void OnDisable()
    {
        InteractAction.Disable();
        WalkAction.Disable();
        JumpAction.Disable();
        RunAction.Disable();
        ZoomAction.Disable();
        EscAction.Disable();
        AltAction.Disable();
        MouseAction.Disable();
        OpenInventoryAction.Disable();
    }

}
