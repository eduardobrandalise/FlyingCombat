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
    
    private void PlayerMeshOnCollision(Collider other)
    {
        // Debug.Log("Collision Event triggered by " + other.gameObject.name);
    }

    private void ManageMovement()
    {
        _currentPosition = transform.position;
        
        var lateralInput = InputManager.Instance.GetLateralMovementNormalized();

        // ----------------- FORWARD MOVEMENT -----------------
        // MoveForward();

        // ----------------- SIDEWAYS MOVEMENT -----------------
        if (!_currentLane.Equals(_targetLane))
        {
            if (_targetLane == Lane.Left && _currentPosition.x < _gameManager.LeftLaneStartPoint.x)
            {
                _targetPosition = new Vector3(_gameManager.LeftLaneStartPoint.x, _currentPosition.y,
                    _currentPosition.z);
                gameObject.transform.position = _targetPosition;
                _currentLane = _targetLane;
                _canMove = true;
                _targetPosition.x = 0;
            }
            else if (_targetLane == Lane.Middle && (_currentPosition.x > _gameManager.MiddleLaneStartPoint.x || _currentPosition.x < _gameManager.MiddleLaneStartPoint.x))
            {
                if (_currentLane == Lane.Left && _currentPosition.x > 0)
                {
                    _targetPosition = new Vector3(_gameManager.MiddleLaneStartPoint.x, _currentPosition.y,
                        _currentPosition.z);
                    gameObject.transform.position = _targetPosition;
                    _currentLane = _targetLane;
                    _canMove = true;
                    _targetPosition.x = 0;
                } else if (_currentLane == Lane.Right && _currentPosition.x < 0)
                {
                    _targetPosition = new Vector3(_gameManager.LeftLaneStartPoint.x, _currentPosition.y,
                        _currentPosition.z);
                    gameObject.transform.position = _targetPosition;
                    _currentLane = _targetLane;
                    _canMove = true;
                    _targetPosition.x = 0;
                }
            }
            else if (_targetLane == Lane.Right && _currentPosition.x > 2)
            {
                _targetPosition = new Vector3(_gameManager.RightLaneStartPoint.x, _currentPosition.y,
                    _currentPosition.z);
                gameObject.transform.position = _targetPosition;
                _currentLane = _targetLane;
                _canMove = true;
                _targetPosition.x = 0;
            }
            
            CheckInputs();
            
            Move(_targetPosition);
        }
        
        
        // ----------------- SIDEWAYS MOVEMENT (OLD) -----------------
        // InputDirection direction = InputDirection.Neutral;
        //
        // switch (lateralInput)
        // {
        //     case > 0:
        //         direction = InputDirection.Right;
        //         break;
        //     case < 0:
        //         direction = InputDirection.Left;
        //         break;
        //     case 0:
        //         direction = InputDirection.Neutral;
        //         break;
        // }
        //
        // Move(direction);
        
        // ----------------- ROLLBACK MOVEMENT -----------------

        if (lateralInput == 0)
        {
            // RollbackToCenterLane();
        }
        
        // print(_currentLane);
    }
    
    private void Move(Vector3 targetPosition)
    {
        transform.position = Vector3.MoveTowards(_currentPosition, targetPosition, lateralSpeed * Time.deltaTime);
    }

    private void CheckInputs()
    {
        if (!_canMove) return;

        var lateralInput = InputManager.Instance.GetLateralMovementNormalized();

        switch (lateralInput)
        {
            case < 0 when _currentLane != Lane.Left:
            {
                var targetLaneIndex = (int)_currentLane - 1;
                _targetLane = (Lane)targetLaneIndex;
                _canMove = false;
                _targetPosition.x -= distanceBetweenLanes * 2;
                break;
            }
            case > 0 when _currentLane != Lane.Right:
            {
                var targetLaneIndex = (int)_currentLane + 1;
                _targetLane = (Lane)targetLaneIndex;
                _canMove = false;
                _targetPosition.x += distanceBetweenLanes * 2;
                break;
            }
        }
    }

    private void MoveSideways(InputDirection inputDirection)
    {
        switch (inputDirection)
        {
            case InputDirection.Left:
                if (_currentLane != Lane.Left)
                {
                    _inputManager.DisableLateralInput();
                    _targetPosition = new Vector3(_currentPosition.x - distanceBetweenLanes, _currentPosition.y, _currentPosition.z);
            
                    // transform.position = Vector3.Lerp(_currentPosition, _targetPosition, lateralSpeed * Time.deltaTime);
                    transform.position = Vector3.MoveTowards(_currentPosition, _targetPosition, lateralSpeed * Time.deltaTime);

                    if (_currentPosition.x <= _gameManager.LeftLaneStartPoint.x)
                    {
                        _currentPosition.x = _gameManager.LeftLaneStartPoint.x;
                        _currentLane = Lane.Left;
                        _targetLane = Lane.Left;
                    }
                    
                    // transform.position = _targetPosition;

                    // targetLaneIndex = (int)_currentLane - 1;
                    // _currentLane = (Lane)targetLaneIndex;

                    // _canChangeLanes = false;
                }
                break;
            case InputDirection.Right:
                if (_currentLane != Lane.Right)
                {
                    _inputManager.DisableLateralInput();
                    _targetPosition = new Vector3(_currentPosition.x + distanceBetweenLanes, _currentPosition.y, _currentPosition.z);
            
                    // transform.position = Vector3.Lerp(_currentPosition, _targetPosition, lateralSpeed * Time.deltaTime);
                    transform.position = Vector3.MoveTowards(_currentPosition, _targetPosition, lateralSpeed * Time.deltaTime);

                    if (_currentPosition.x >= _gameManager.RightLaneStartPoint.x)
                    {
                        _currentPosition.x = _gameManager.RightLaneStartPoint.x;
                        _currentLane = Lane.Right;
                    }

                    // transform.position = _targetPosition;
                    
                    // targetLaneIndex = (int)_currentLane + 1;
                    // _currentLane = (Lane)targetLaneIndex;
                    
                    // _canChangeLanes = false;
                    // _isInputEnabled = false;
                }
                break;
        }
    }

    private void MoveForward()
    {
        var forwardMovement = Vector3.forward * forwardSpeed;
        transform.position += (forwardMovement) * Time.deltaTime;
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