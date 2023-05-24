using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private static InputManager instance;
    public static InputManager Instance { get { return instance; } }

    private PlayerInputActions playerInputActions;
    
    private void Awake()
    {
        if (instance != null && instance != this) { Destroy(this.gameObject); }
        else { instance = this; }
        
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    public float GetLateralMovementNormalized()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        inputVector = inputVector.normalized;
        
        float xInputVector = inputVector.x;
        return xInputVector;
    }
}
