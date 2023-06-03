using UnityEngine;
using UnityEngine.Serialization;

internal enum InputDirection
{
    None,
    Left,
    Right
}

public class Player : MonoBehaviour
{
    private static Player instance;

    public static Player Instance
    {
        get { return instance; }
    }

    [SerializeField] private float forwardSpeed = 30f;
    [SerializeField] private float lateralSpeed = 60f;
    
    [SerializeField] private float lanePosition = 10f;
    [SerializeField] private float distanceBetweenLanes = 10f;
    [SerializeField] private Transform planeModelTransform;

    private PlayerMesh _playerMesh;
    private Vector3 _currentPosition;
    private Vector3 _targetPosition;
    private Lane _currentLane;
    private Lane _targetLane;
    private float _lanePositionTolerance = 0.1f;
    private PlayerState _currentState;
    private GameManager _gameManager;
    private InputManager _inputManager;
    private InputDirection _inputDirection;
    private bool _canMove;
    private float _propellerRotation = 0f;
    private const float PropellerRotationSpeed = 4000f;
    
    //--------------ROTATION VARIABLES--------------
    [SerializeField] private float rotationSpeed = 0.5f;
    [SerializeField] private float rotationMaxAngle = 30f;
    [SerializeField] private float rollbackSpeed = 10f;
    [SerializeField] private float _rotationTime = 0.65f; // Total time for both forward and backward rotation
    private Quaternion _currentRotationAngle;
    private float _rotationAngle;
    private Quaternion _rotationTarget;
    private Quaternion _initialRotation;
    private float forwardRotationProgress = 0.8f;

    private float _forwardRotationTime;
    private float _backwardRotationTime;
    private bool _isRotating = false;
    private bool _isForwardRotation = true;
    private float _rotationTimer;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        _playerMesh = planeModelTransform.GetComponent<PlayerMesh>();

        if (_playerMesh != null)
        {
            _playerMesh.collided.AddListener(PlayerMeshOnCollision);
        }
        
        InitializeVariables();
    }

    private void FixedUpdate()
    {
        ManageMovement();
    }

    private void OnDestroy()
    {
        if (_playerMesh != null)
        {
            _playerMesh.collided.RemoveListener(PlayerMeshOnCollision);
        }
    }

    private void ManageMovement()
    {
        _currentPosition = transform.position;

        HandleInput();

        if (_inputDirection == InputDirection.Left)
        {
            if (_currentPosition.x == _gameManager.RightLaneStartPoint.x)
            {
                _targetPosition = new Vector3(_gameManager.MiddleLaneStartPoint.x, _currentPosition.y,
                    _currentPosition.z);
                StartRotation();
            }

            if (_currentPosition.x == _gameManager.MiddleLaneStartPoint.x)
            {
                _targetPosition = new Vector3(_gameManager.LeftLaneStartPoint.x, _currentPosition.y,
                    _currentPosition.z);
                StartRotation();
            }
        }

        if (_inputDirection == InputDirection.Right)
        {
            if (_currentPosition.x == _gameManager.LeftLaneStartPoint.x)
            {
                _targetPosition = new Vector3(_gameManager.MiddleLaneStartPoint.x, _currentPosition.y,
                    _currentPosition.z);
                StartRotation();
            }

            if (_currentPosition.x == _gameManager.MiddleLaneStartPoint.x)
            {
                _targetPosition = new Vector3(_gameManager.RightLaneStartPoint.x, _currentPosition.y,
                    _currentPosition.z);
                StartRotation();
            }
        }
        
        Move();
    }

    private void HandleInput()
    {
        var lateralInput = InputManager.Instance.GetLateralMovementNormalized();

        if (lateralInput < 0)
        {
            _inputDirection = InputDirection.Left;
        }
        else if (lateralInput > 0)
        {
            _inputDirection = InputDirection.Right;
        }
        else
        {
            _inputDirection = InputDirection.None;
        }
    }

    private void Move()
    {
        var forwardMovement = Vector3.forward * (forwardSpeed * Time.deltaTime);
        _targetPosition = forwardMovement + _targetPosition;
        
        transform.position =
            Vector3.MoveTowards(_currentPosition, _targetPosition, lateralSpeed * Time.deltaTime);

        var difference = Mathf.Abs(_targetPosition.x - _currentPosition.x);
        // Regressive progress from 1 to 0.
        var progress = (1f - (1f - difference)) / distanceBetweenLanes;

        if (progress < 0.01f)
        {
            // Snap to lane.
            transform.position = _targetPosition;
        }

        if (_isRotating)
        {
            RotatePlane();
        }
    }
    
    private void StartRotation()
    {
        // Only start the rotation if it is not already rotating
        if (!_isRotating && _inputDirection != InputDirection.None)
        {
            // Set the target rotation based on the input direction
            if (_inputDirection == InputDirection.Left)
            {
                _rotationTarget = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, rotationMaxAngle);
            }
            else if (_inputDirection == InputDirection.Right)
            {
                _rotationTarget = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, -rotationMaxAngle);
            }

            // Reset rotation-related variables
            _rotationTimer = 0f;
            _isForwardRotation = true;

            // Set isRotating to true to start the rotation
            _isRotating = true;
        }
    }
    
    private void RotatePlane()
    {
        _rotationTimer += Time.deltaTime;

        // Check if the forward rotation phase is active
        if (_isForwardRotation)
        {
            float step = rotationSpeed * Time.deltaTime / _forwardRotationTime;
            _initialRotation = Quaternion.RotateTowards(_initialRotation, _rotationTarget, step);

            // Check if the forward rotation phase is complete
            if (_rotationTimer >= _forwardRotationTime)
            {
                _rotationTimer = 0f;
                _isForwardRotation = false;
            }
        }
        else // Backward rotation phase
        {
            float step = rotationSpeed * Time.deltaTime / _backwardRotationTime;
            _initialRotation = Quaternion.RotateTowards(_initialRotation, transform.rotation, step);

            // Check if the backward rotation phase is complete
            if (_rotationTimer >= _backwardRotationTime)
            {
                _rotationTimer = 0f;
                _isForwardRotation = true;

                // Rotation is complete, set isRotating to false
                _isRotating = false;
            }
        }

        planeModelTransform.rotation = _initialRotation;
    }

    private void InitializeVariables()
    {
        // ------General properties.------
        _gameManager = GameManager.Instance;
        _inputManager = InputManager.Instance;

        _currentLane = Lane.Middle;
        _currentState = PlayerState.OnLane;
        
        _inputDirection = InputDirection.None;

        _targetPosition = transform.position;
        
        // ------Rotation properties.------
        // The time is distance divided by speed.
        // _rotationTime = rotationSpeed / distanceBetweenLanes;
        
        // Calculate the rotation times based on the desired progress
        _forwardRotationTime = _rotationTime * forwardRotationProgress;
        _backwardRotationTime = _rotationTime - _forwardRotationTime;

        _initialRotation = transform.rotation;
        _rotationTarget = Quaternion.Euler(0, 0, rotationMaxAngle);
    }

    private void PlayerMeshOnCollision(Collider other)
    {
        // Debug.Log("Collision Event triggered by " + other.gameObject.name);
    }

    private void OnCollisionEnter(Collision collision)
    {
        print(collision.gameObject.name);
    }

    public Vector3 GetPlayerPosition()
    {
        return transform.position;
    }

    public float GetDistanceBetweenLanes()
    {
        return distanceBetweenLanes;
    }
}