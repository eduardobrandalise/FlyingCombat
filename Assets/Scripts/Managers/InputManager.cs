using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class InputManager : MonoBehaviour
{
    private static InputManager instance;
    public static InputManager Instance { get { return instance; } }

    public UnityEvent pausePressed;
    
    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        if (instance != null && instance != this) { Destroy(this.gameObject); }
        else { instance = this; }
        
        playerInputActions = new PlayerInputActions();
        
        EnableLateralInput();
        
        playerInputActions.Player.Pause.performed += PauseOnPerformed;
    }

    private void OnDestroy()
    {
        playerInputActions.Player.Pause.performed -= PauseOnPerformed;
    }

    private void PauseOnPerformed(InputAction.CallbackContext obj)
    {
        pausePressed.Invoke();
    }

    public float GetLateralMovementNormalized()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        inputVector = inputVector.normalized;

        float xInputVector = inputVector.x;
        return xInputVector;
    }

    private void EnableLateralInput()
    {
        playerInputActions.Player.Enable();
    }
}