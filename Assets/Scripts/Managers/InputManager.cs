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
    
    private PlayerInputActions _playerInputActions;

    private void Awake()
    {
        if (instance != null && instance != this) { Destroy(this.gameObject); }
        else { instance = this; }
        
        _playerInputActions = new PlayerInputActions();
        
        EnableLateralInput();
        
        _playerInputActions.Player.Pause.performed += PauseOnPerformed;
    }

    private void OnDestroy()
    {
        _playerInputActions.Player.Pause.performed -= PauseOnPerformed;
    }

    private void PauseOnPerformed(InputAction.CallbackContext obj)
    {
        pausePressed.Invoke();
    }

    public float GetLateralMovementNormalized()
    {
        Vector2 inputVector = _playerInputActions.Player.Move.ReadValue<Vector2>();
        inputVector = inputVector.normalized;

        float xInputVector = inputVector.x;
        return xInputVector;
    }

    private void EnableLateralInput()
    {
        _playerInputActions.Player.Enable();
    }
}