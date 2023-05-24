using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [SerializeField] private float forwardSpeed = 30f;
    [SerializeField] private float lateralSpeed = 10f;
    [SerializeField] private float rollSpeed = 0.5f;
    [SerializeField] private float rotationMaxAngle = 30f;
    [SerializeField] private float rollbackSpeed = 10f;
    [SerializeField] private AnimationCurve rollbackAnimationCurve;
    [SerializeField] private Transform planeModelTransform;
    [SerializeField] private Transform propellerTransform;

    private Quaternion _currentRotationAngle;
    private float _rotationAngle;
    private float _current, _target;
    private float _propellerRotation = 50f;
    
    private void FixedUpdate()
    {
        Move();
        
    }

    private void LateUpdate()
    {
        RotatePropeller();
    }

    private void Move()
    {
        // ----------------- FORWARD MOVEMENT -----------------
        var lateralInput = InputManager.Instance.GetLateralMovementNormalized();
        var lateralMovement = new Vector3(lateralInput, 0, 0) * lateralSpeed;
        var forwardMovement = Vector3.forward * forwardSpeed;
        // transform.position += ((Vector3.forward + new Vector3(lateralInput, 0, 0)) * SPEED) * Time.deltaTime;
        transform.position += (forwardMovement + lateralMovement) * Time.deltaTime;
        
        // ----------------- ROTATION MOVEMENT -----------------
        
        if (lateralInput > 0)
        {
            // Roll to the RIGHT.
            var targetRotationRight = Quaternion.Euler(new Vector3(0, 0, -rotationMaxAngle));
            var rollMovement = rollSpeed * Time.deltaTime;
            _currentRotationAngle = planeModelTransform.rotation;
            
            planeModelTransform.rotation = Quaternion.Slerp(_currentRotationAngle, targetRotationRight, rollMovement);
            
            return;
        }
        
        if (lateralInput < 0)
        {
            // Roll to the LEFT.
            var targetRotationLeft = Quaternion.Euler(new Vector3(0, 0, rotationMaxAngle));
            var rollMovement = rollSpeed * Time.deltaTime;
            _currentRotationAngle = planeModelTransform.rotation;
            
            planeModelTransform.rotation = Quaternion.Slerp(_currentRotationAngle, targetRotationLeft, rollMovement);
            
            return;
        }

        // ----------------- ROLLBACK MOVEMENT -----------------
   
        // Back to stable position.
        var targetRotation = Quaternion.identity;
        var rollbackMovement = rollbackSpeed * Time.deltaTime;
        
        _currentRotationAngle = new Quaternion(0, 0, planeModelTransform.rotation.z, 0);
        planeModelTransform.rotation = Quaternion.Slerp(planeModelTransform.rotation, targetRotation, rollbackMovement);
        if (Quaternion.Angle(planeModelTransform.rotation, targetRotation) < 0.3f)
        {
            // Stop the rotation and snap to the exact initial rotation
            planeModelTransform.rotation = targetRotation;
        }
    }

    private void RotatePropeller()
    {
        _propellerRotation = (_propellerRotation + (forwardSpeed * _propellerRotation * Time.deltaTime)) % 360f;
        propellerTransform.rotation = Quaternion.Euler(0, 0, _propellerRotation);
    }
}