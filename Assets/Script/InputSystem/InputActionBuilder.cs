using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;


public class InputActionBuilder
{
    public bool isPressed;
    public bool isActivating;
    public InputAction action;

    public InputActionBuilder(InputAction action)
    {
        this.action = action;
        action.performed += (InputAction.CallbackContext context) => { isPressed = true; };
        action.canceled += (InputAction.CallbackContext context) => { isPressed = false; };
    }

    public void AddPerformed(Action<InputAction.CallbackContext> performed)
    {
        action.performed += performed;
    }
    public void RemovePerformed(Action<InputAction.CallbackContext> performed)
    {
        action.performed -= performed;
    }

    public void AddCanceled(Action<InputAction.CallbackContext> canceled)
    {
        action.canceled += canceled;
    }

    public void RemoveCanceled(Action<InputAction.CallbackContext> canceled)
    {
        action.canceled -= canceled;
    }

    public void Enable()
    {
        action.Enable();
    }
    public void Disable()
    {
        action.Disable();
    }

}
