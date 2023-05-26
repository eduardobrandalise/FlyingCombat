using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

internal enum MoveTo
{
    Left,
    Right
}

public class Player : MonoBehaviour
{
    [SerializeField] private float forwardSpeed = 30f;
    [SerializeField] private float lateralSpeed = 60f;
    [SerializeField] private float rollSpeed = 0.5f;
    [SerializeField] private float rotationMaxAngle = 30f;
    [SerializeField] private float rollbackSpeed = 10f;
    [SerializeField] private float lanePosition = 10f;
    [SerializeField] private Transform planeModelTransform;
    [SerializeField] private Transform propellerTransform;

    private Quaternion _currentRotationAngle;
    private PlayerMesh _playerMesh;
    private Vector3 _currentPosition;
    private Vector3 _targetPosition;
    private Quaternion _targetRotation;
    private Lane _currentLane;
    private bool _canChangeLanes;
    private float _rotationAngle;
    private float _propellerRotation = 0f;
    private const float PropellerRotationSpeed = 4000f;

    private void Start()
    {
        _playerMesh = planeModelTransform.GetComponent<PlayerMesh>();
        
        if (_playerMesh != null)
        {
            _playerMesh.collided.AddListener(PlayerMeshOnCollision);
        }

        _currentLane = Lane.Middle;
        _canChangeLanes = true;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void LateUpdate()
    {
        RotatePropeller();
    }
    
    private void OnDestroy()
    {
        if (_playerMesh != null)
        {
            _playerMesh.collided.RemoveListener(PlayerMeshOnCollision);
        }
    }
    
    private void PlayerMeshOnCollision(Collider other)
    {
        // Debug.Log("Collision Event triggered by " + other.gameObject.name);
    }

    private void Move()
    {
        print(_canChangeLanes);
        // print(_currentLane);
        
        const float tolerance = 0.1f;
        if (_currentPosition.x != 0 || Math.Abs(_currentPosition.x - lanePosition) > tolerance || Math.Abs(_currentPosition.x - (-lanePosition)) > tolerance) return;
        
        var lateralInput = InputManager.Instance.GetLateralMovementNormalized();
        
        // ----------------- FORWARD MOVEMENT -----------------
        MoveForward();

        // ----------------- SIDEWAYS MOVEMENT -----------------
        
        if (_canChangeLanes && lateralInput != 0)
        {
           MoveTo moveTo = lateralInput > 0 ? MoveTo.Right : MoveTo.Left;
           ChangeLane(moveTo);
        }
        
        // ----------------- ROLLBACK MOVEMENT -----------------

        if (lateralInput == 0)
        {
            // RollbackToCenterLane();
        }
        
        print(_currentLane);
    }

    private void ChangeLane(MoveTo moveTo)
    {
        int targetLaneIndex;

        switch (moveTo)
        {
            case MoveTo.Left:
                if (_currentLane != Lane.Left)
                {
                    _currentPosition = transform.position;
                    _targetPosition = new Vector3(lanePosition, _currentPosition.y, _currentPosition.z);
            
                    transform.position = Vector3.Lerp(_currentPosition, _targetPosition, lateralSpeed * Time.deltaTime);

                    targetLaneIndex = (int)_currentLane - 1;
                    _currentLane = (Lane)targetLaneIndex;

                }
                break;
            case MoveTo.Right:
                if (_currentLane != Lane.Right)
                {
                    targetLaneIndex = (int)_currentLane + 1;
                    _currentLane = (Lane)targetLaneIndex;
                }
                break;
        }
    }

    private void MoveForward()
    {
        var forwardMovement = Vector3.forward * forwardSpeed;
        transform.position += (forwardMovement) * Time.deltaTime;
    }

    private void MoveSidewaysHolding(float lateralInput)
    {
        if (lateralInput > 0)
        {
            // Roll to the RIGHT.
            var targetRotationRight = Quaternion.Euler(new Vector3(0, 0, -rotationMaxAngle));
            var rollMovement = rollSpeed * Time.deltaTime;
            _currentRotationAngle = planeModelTransform.rotation;
            
            planeModelTransform.rotation = Quaternion.Slerp(_currentRotationAngle, targetRotationRight, rollMovement);
            
            // Move to the right lane.
            _currentPosition = transform.position;
            _targetPosition = new Vector3(lanePosition, _currentPosition.y, _currentPosition.z);
            
            transform.position = Vector3.Lerp(_currentPosition, _targetPosition, lateralSpeed * Time.deltaTime);
            return;
        }
        
        if (lateralInput < 0)
        {
            // Roll to the LEFT.
            var targetRotationLeft = Quaternion.Euler(new Vector3(0, 0, rotationMaxAngle));
            var rollMovement = rollSpeed * Time.deltaTime;
            _currentRotationAngle = planeModelTransform.rotation;
            
            planeModelTransform.rotation = Quaternion.Slerp(_currentRotationAngle, targetRotationLeft, rollMovement);
            
            // Move to the left lane.
            _currentPosition = transform.position;
            _targetPosition = new Vector3(-lanePosition, _currentPosition.y, _currentPosition.z);
            
            transform.position = Vector3.Lerp(_currentPosition, _targetPosition, lateralSpeed * Time.deltaTime);
            
            return;
        }
    }

    private void RollbackToCenterLane()
    {
        // Back to stable position.
        _targetRotation = Quaternion.identity;
        var rollbackMovement = rollbackSpeed * Time.deltaTime;
        var rotation = planeModelTransform.rotation;
        
        rotation = Quaternion.Slerp(rotation, _targetRotation, rollbackMovement);
        
        _currentRotationAngle = new Quaternion(0, 0, rotation.z, 0);
        
        planeModelTransform.rotation = rotation;
        
        if (Quaternion.Angle(planeModelTransform.rotation, _targetRotation) < 1.5f)
        {
            // Stop the rotation and snap to the exact initial rotation
            planeModelTransform.rotation = _targetRotation;
        }

        // Back to the middle lane
        _currentPosition = transform.position;
        _targetPosition = new Vector3(0, _currentPosition.y, _currentPosition.z);

        transform.position = Vector3.Lerp(_currentPosition, _targetPosition, rollbackMovement);
    }

    private void RotatePropeller()
    {
        _propellerRotation = (_propellerRotation + (PropellerRotationSpeed * Time.deltaTime)) % 360f;
        propellerTransform.rotation = Quaternion.Euler(0, 0, _propellerRotation);
    }

    private void OnCollisionEnter(Collision collision)
    {
        print(collision.gameObject.name);
    }
}