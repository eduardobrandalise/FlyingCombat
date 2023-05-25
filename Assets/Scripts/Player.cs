using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [SerializeField] private float forwardSpeed = 30f;
    [SerializeField] private float lateralSpeed = 60f;
    [SerializeField] private float rollSpeed = 0.5f;
    [SerializeField] private float rotationMaxAngle = 30f;
    [SerializeField] private float rollbackSpeed = 10f;
    [SerializeField] private float lanePosition = 5f;
    [SerializeField] private Transform planeModelTransform;
    [SerializeField] private Transform propellerTransform;

    private Quaternion _currentRotationAngle;
    private float _rotationAngle;
    private float _current, _target;
    private float _propellerRotation = 0f;
    private const float PropellerRotationSpeed = 2000f;
    
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
        var forwardMovement = Vector3.forward * forwardSpeed;
        // transform.position += ((Vector3.forward + new Vector3(lateralInput, 0, 0)) * SPEED) * Time.deltaTime;
        transform.position += (forwardMovement) * Time.deltaTime;
        
        // ----------------- ROTATION MOVEMENT -----------------

        Vector3 currentPosition;
        Vector3 targetPosition;
        if (lateralInput > 0)
        {
            // Roll to the RIGHT.
            var targetRotationRight = Quaternion.Euler(new Vector3(0, 0, rotationMaxAngle));
            var rollMovement = rollSpeed * Time.deltaTime;
            _currentRotationAngle = planeModelTransform.rotation;
            
            planeModelTransform.rotation = Quaternion.Slerp(_currentRotationAngle, targetRotationRight, rollMovement);
            
            // Move to the right lane.
            currentPosition = transform.position;
            targetPosition = new Vector3(lanePosition, currentPosition.y, currentPosition.z);
            
            transform.position = Vector3.Lerp(currentPosition, targetPosition, lateralSpeed * Time.deltaTime);
            return;
        }
        
        if (lateralInput < 0)
        {
            // Roll to the LEFT.
            var targetRotationLeft = Quaternion.Euler(new Vector3(0, 0, -rotationMaxAngle));
            var rollMovement = rollSpeed * Time.deltaTime;
            _currentRotationAngle = planeModelTransform.rotation;
            
            planeModelTransform.rotation = Quaternion.Slerp(_currentRotationAngle, targetRotationLeft, rollMovement);
            
            // Move to the left lane.
            currentPosition = transform.position;
            targetPosition = new Vector3(-lanePosition, currentPosition.y, currentPosition.z);
            
            transform.position = Vector3.Lerp(currentPosition, targetPosition, lateralSpeed * Time.deltaTime);
            
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
        
        // Back to the middle lane
        currentPosition = transform.position;
        targetPosition = new Vector3(0, currentPosition.y, currentPosition.z);
            
        transform.position = Vector3.Lerp(currentPosition, targetPosition, lateralSpeed * Time.deltaTime);
    }

    private void RotatePropeller()
    {
        _propellerRotation = (_propellerRotation + (PropellerRotationSpeed * Time.deltaTime)) % 360f;
        propellerTransform.rotation = Quaternion.Euler(0, 0, _propellerRotation);
    }
}