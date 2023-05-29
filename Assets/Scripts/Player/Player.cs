using Unity.VisualScripting;
using UnityEngine;

internal enum InputDirection
{
    Left,
    Right,
    Neutral
}

public class Player : MonoBehaviour
{
    private static Player instance;
    public static Player Instance { get { return instance; }}
    
    [SerializeField] private float forwardSpeed = 30f;
    [SerializeField] private float lateralSpeed = 60f;
    [SerializeField] private float rollSpeed = 0.5f;
    [SerializeField] private float rotationMaxAngle = 30f;
    [SerializeField] private float rollbackSpeed = 10f;
    [SerializeField] private float lanePosition = 10f;
    [SerializeField] private float distanceBetweenLanes = 10f;
    [SerializeField] private Transform planeModelTransform;
    [SerializeField] private Transform propellerTransform;

    private Quaternion _currentRotationAngle;
    private PlayerMesh _playerMesh;
    private Vector3 _currentPosition;
    private Vector3 _targetPosition;
    private Quaternion _targetRotation;
    private Lane _currentLane;
    private Lane _targetLane;
    private PlayerState _currentState;
    private GameManager _gameManager;
    private InputManager _inputManager;
    private bool _canMove;
    private float _rotationAngle;
    private float _propellerRotation = 0f;
    private float _lanePositionTolerance = 0.1f;
    private const float PropellerRotationSpeed = 4000f;

    private void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); }
        else { instance = this; }
    }

    private void Start()
    {
        _playerMesh = planeModelTransform.GetComponent<PlayerMesh>();
        
        if (_playerMesh != null)
        {
            _playerMesh.collided.AddListener(PlayerMeshOnCollision);
        }
        
        _gameManager = GameManager.Instance;
        _inputManager = InputManager.Instance;

        _currentLane = Lane.Middle;
        _currentState = PlayerState.OnLane;
        _canMove = true;
        _currentPosition = transform.position;
        _targetPosition = _currentPosition;
    }

    private void FixedUpdate()
    {
        ManageMovement();
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
    

    private void ManageMovement()
    {
        var lateralInput = InputManager.Instance.GetLateralMovementNormalized();
        InputDirection inputDirection = InputDirection.Neutral;

        _currentPosition = transform.position;
        
        switch (lateralInput)
        {
            case < 0:
                inputDirection = InputDirection.Left;
                break;
            case > 0:
                inputDirection = InputDirection.Right;
                break;
        }
        
        if (inputDirection == InputDirection.Left)
        {
            if (_currentPosition.x == _gameManager.RightLaneStartPoint.x)
            {
                _targetPosition = new Vector3(_gameManager.MiddleLaneStartPoint.x, _currentPosition.y,
                    _currentPosition.z);
            }
            if (_currentPosition.x == _gameManager.MiddleLaneStartPoint.x)
            {
                _targetPosition = new Vector3(_gameManager.LeftLaneStartPoint.x, _currentPosition.y,
                    _currentPosition.z);
            }
        }

        if (inputDirection == InputDirection.Right)
        {
            if (_currentPosition.x == _gameManager.LeftLaneStartPoint.x)
            {
                _targetPosition = new Vector3(_gameManager.MiddleLaneStartPoint.x, _currentPosition.y,
                    _currentPosition.z);
            }
            if (_currentPosition.x == _gameManager.MiddleLaneStartPoint.x)
            {
                _targetPosition = new Vector3(_gameManager.RightLaneStartPoint.x, _currentPosition.y,
                    _currentPosition.z);
            }
        }
        
        Move();
    }

    private void Move()
    {
        transform.position =
            Vector3.MoveTowards(_currentPosition, _targetPosition, lateralSpeed * Time.deltaTime);
    }

    private void UpdateCurrentLane()
    {
        _lanePositionTolerance = 0.1f;
        
        if (_gameManager.LeftLaneStartPoint.x - _lanePositionTolerance <= _currentPosition.x && _currentPosition.x <= _gameManager.LeftLaneStartPoint.x + _lanePositionTolerance)
        {
            _currentLane = Lane.Left;
            return;
        }
        if (_gameManager.MiddleLaneStartPoint.x - _lanePositionTolerance <= _currentPosition.x && _currentPosition.x <= _gameManager.MiddleLaneStartPoint.x + _lanePositionTolerance)
        {
            _currentLane = Lane.Middle;
            return;
        }
        if (_gameManager.RightLaneStartPoint.x - _lanePositionTolerance <= _currentPosition.x && _currentPosition.x <= _gameManager.RightLaneStartPoint.x + _lanePositionTolerance)
        {
            _currentLane = Lane.Right;
            return;
        }
    }

    private void RotatePropeller()
    {
        _propellerRotation = (_propellerRotation + (PropellerRotationSpeed * Time.deltaTime)) % 360f;
        propellerTransform.rotation = Quaternion.Euler(0, 0, _propellerRotation);
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