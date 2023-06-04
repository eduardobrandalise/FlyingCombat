using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    // ------------General properties------------
    private Player _player;
    private PlayerData _playerData;
    private GameManager _gameManager;

    private InputDirection _inputDirection;
    private Lane _destinationLane;
    private Vector3 _currentPosition;
    private Vector3 _targetPosition;

    //--------------ROTATION VARIABLES--------------
    private Quaternion _currentRotationAngle;
    private float _rotationAngle;
    private Quaternion _rotationTarget;
    private Quaternion _initialRotation;
    private float _forwardRotationProgress = 0.8f;

    private float _forwardRotationTime;
    private float _backwardRotationTime;
    private bool _isRotating = false;
    private bool _isForwardRotation = true;
    private float _rotationTimer;
    
    void Start()
    {
        InitializeProperties();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void InitializeProperties()
    {
        // ------Singletons------
         _player = Player.Instance;
         _playerData = PlayerData.Instance;
         _gameManager = GameManager.Instance;
        
        // ------General properties------
        _destinationLane = _player.CurrentLane;
        _currentPosition = transform.position;
        _targetPosition = _currentPosition;

        // ------Rotation properties.------
        // The time is distance divided by speed.
        // _rotationTime = rotationSpeed / distanceBetweenLanes;
        
        // Calculate the rotation times based on the desired progress
        _forwardRotationTime = _playerData.RotationTime * _forwardRotationProgress;
        _backwardRotationTime = _playerData.RotationTime - _forwardRotationTime;

        _initialRotation = transform.rotation;
        _rotationTarget = Quaternion.Euler(0, 0, _playerData.RotationMaxAngle);
    }

    private void Move()
    {
        UpdateCurrentPosition();

        MoveForward();
        
        BuildLateralVector();
        
        transform.position =
            Vector3.MoveTowards(_currentPosition, _targetPosition, _playerData.LateralSpeed * Time.deltaTime);
        
        var difference = Mathf.Abs(_targetPosition.x - _currentPosition.x);
        // Regressive progress from 1 to 0.
        var progress = (1f - (1f - difference)) / _gameManager.DistanceBetweenLanes;
        
        // print(progress);

        if (0 > progress && progress < _playerData.LaneSnappingTolerance)
        {
            // Snap to lane.
            // transform.position =
            //     new Vector3(_currentPosition.x + _targetPosition.x, _currentPosition.y, _currentPosition.z);
            
            print("SNAP TO LANE");
        }
        
        // Spin();
    }
    
    private void MoveForward()
    {
        var forwardMovement = Vector3.forward * (_playerData.ForwardSpeed * Time.deltaTime);
        transform.position = forwardMovement + _currentPosition;
        
        UpdateCurrentPosition();
    }

    public void BuildLateralVector()
    {
        if (_player.CurrentState == PlayerState.Dashing && _destinationLane != _player.CurrentLane)
        {
            if (_destinationLane == Lane.Left) { _targetPosition.x = _gameManager.LeftLaneStartPoint.x; }
            else if (_destinationLane == Lane.Middle) { _targetPosition.x = _gameManager.MiddleLaneStartPoint.x; }
            else if (_destinationLane == Lane.Right) { _targetPosition.x = _gameManager.RightLaneStartPoint.x; }
        }
    }

    
    private float _shipRotation = 0f;
    
    private void Spin()
    {
        _shipRotation = (_shipRotation + (_playerData.RotationSpeed * Time.deltaTime)) % 360f;
        transform.rotation = Quaternion.Euler(_shipRotation, +90, 0);
    }
    
    //TODO: Delete StartRotation() after rotating dash is done. It's here just for consulting purposes.
    private void StartRotation()
    {
        // Only start the rotation if it is not already rotating
        if (!_isRotating && _inputDirection != InputDirection.None)
        {
            // Set the target rotation based on the input direction
            if (_inputDirection == InputDirection.Left)
            {
                _rotationTarget = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, _playerData.RotationMaxAngle);
            }
            else if (_inputDirection == InputDirection.Right)
            {
                _rotationTarget = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, -_playerData.RotationMaxAngle);
            }

            // Reset rotation-related variables
            _rotationTimer = 0f;
            _isForwardRotation = true;

            // Set isRotating to true to start the rotation
            _isRotating = true;
        }
    }

    private void UpdateCurrentPosition()
    {
        _currentPosition = transform.position;
    }

    public void SetDestinationLane(Lane destinationLane)
    {
        _destinationLane = destinationLane;
    }
}
