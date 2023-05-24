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
    [SerializeField] private float ROLLSPEED = 10f;
    [SerializeField] private float ROTATIONMAXANGLE = 30f;
    [SerializeField] private AnimationCurve rollbackAnimationCurve;
    [SerializeField] private Transform planeModel;
    [SerializeField] private Transform propeller;

    private Quaternion initialRotation;

    private void Start()
    {
        initialRotation = planeModel.rotation;
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
            // Roll to the right.
            planeModel.Rotate(0, 0, -ROLLSPEED);
            if (planeModel.rotation.z < -ROTATIONMAXANGLE)
            {
                planeModel.rotation = new Quaternion(0, 0, -ROTATIONMAXANGLE, 0);
            }
            return;
        }
        
        if (lateralInput < 0)
        {
            // Roll to the left.
            planeModel.Rotate(0, 0, ROLLSPEED);
            return;
        }

        
        // ----------------- ROLLBACK MOVEMENT -----------------
   
        // Back to stable position.
        var returnRollSpeed = 20f;
        var targetRotation = initialRotation;
        var currentRotationAngle = new Quaternion(0, 0, planeModel.rotation.z, 0);
        // Mathf.MoveTowardsAngle()
        // planeModel.rotation = Quaternion.Slerp(currentRotationAngle, Quaternion.identity, Time.deltaTime * returnRollSpeed);
        // planeModel.rotation = Quaternion.Slerp(currentRotationAngle, Quaternion.identity, 0.6f);
        planeModel.rotation = Quaternion.Slerp(planeModel.rotation, targetRotation, Time.deltaTime * returnRollSpeed);
        if (Quaternion.Angle(planeModel.rotation, targetRotation) < 0.3f)
        {
            // Stop the rotation and snap to the exact initial rotation
            planeModel.rotation = targetRotation;
        }
    }
}