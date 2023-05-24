using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [SerializeField] private float FORWARDSPEED = 30f;
    [SerializeField] private float LATERALSPEED = 10f;
    [SerializeField] private float ROLLSPEED = 0.5f;
    [SerializeField] private float ROTATIONMAXANGLE = 30f;
    [SerializeField] private float ROLLBACKSPEED = 10f;
    [SerializeField] private AnimationCurve rollbackAnimationCurve;
    [SerializeField] private Transform planeModelTransform;
    [SerializeField] private Transform propeller;

    private Quaternion initialRotation;
    private float _rotationAngle;
    private float _current, _target;

    private void Start()
    {
        initialRotation = planeModelTransform.rotation;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        // ----------------- FORWARD MOVEMENT -----------------
        var lateralInput = InputManager.Instance.GetLateralMovementNormalized();
        var lateralMovement = new Vector3(lateralInput, 0, 0) * LATERALSPEED;
        var forwardMovement = Vector3.forward * FORWARDSPEED;
        // transform.position += ((Vector3.forward + new Vector3(lateralInput, 0, 0)) * SPEED) * Time.deltaTime;
        transform.position += (forwardMovement + lateralMovement) * Time.deltaTime;
        
        // ----------------- ROTATION MOVEMENT -----------------

        if (lateralInput > 0)
        {
            // Roll to the RIGHT.
            _rotationAngle = -ROTATIONMAXANGLE;
            _current = Mathf.MoveTowards(_current, _rotationAngle, ROLLSPEED * Time.deltaTime);
            Vector3 rotationGoal = new Vector3(0, 0, _rotationAngle);
            planeModelTransform.rotation = Quaternion.Lerp(Quaternion.Euler(rotationGoal), Quaternion.Euler(Vector3.zero), rollbackAnimationCurve.Evaluate(_current));
            return;
        }
        
        if (lateralInput < 0)
        {
            // Roll to the LEFT.
            _rotationAngle = ROTATIONMAXANGLE;
            Vector3 rotationGoal = new Vector3(0, 0, _rotationAngle);
            planeModelTransform.rotation = Quaternion.Lerp(Quaternion.Euler(rotationGoal), Quaternion.Euler(Vector3.zero), ROLLSPEED * Time.deltaTime);
            return;
        }

        // ----------------- ROLLBACK MOVEMENT -----------------
   
        // Back to stable position.
        var targetRotation = initialRotation;
        var rollbackMovement = ROLLBACKSPEED * Time.deltaTime;
        
        var currentRotationAngle = new Quaternion(0, 0, planeModelTransform.rotation.z, 0);
        // Mathf.MoveTowardsAngle()
        // planeModel.rotation = Quaternion.Slerp(currentRotationAngle, Quaternion.identity, Time.deltaTime * returnRollSpeed);
        // planeModel.rotation = Quaternion.Slerp(currentRotationAngle, Quaternion.identity, 0.6f);
        planeModelTransform.rotation = Quaternion.Slerp(planeModelTransform.rotation, targetRotation, Time.deltaTime * ROLLBACKSPEED);
        if (Quaternion.Angle(planeModelTransform.rotation, targetRotation) < 0.3f)
        {
            // Stop the rotation and snap to the exact initial rotation
            planeModelTransform.rotation = targetRotation;
        }
    }
}