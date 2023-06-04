using UnityEngine;

public class PlayerMovement2 : MonoBehaviour
{
    
    // ------------General properties------------
    private Player _player;
    private PlayerData _playerData;
    private GameManager _gameManager;

    private InputDirection _inputDirection;
    private Lane _destinationLane;
    private Vector3 _currentPosition;
    private Vector3 _forwardMovementVector;
    private Vector3 _lateralMovementVector;

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
        _player = Player.Instance;
        _playerData = PlayerData.Instance;
        _gameManager = GameManager.Instance;

        _destinationLane = _player.CurrentLane;
        _currentPosition = transform.position;
        _forwardMovementVector = Vector3.zero;
        _lateralMovementVector = Vector3.zero;
    }

    private void Move()
    {
        UpdateCurrentPosition();

        MoveForward();
        MoveLateral();

        transform.position = _currentPosition + _forwardMovementVector + _lateralMovementVector;
    }
    
    private void MoveForward()
    {
        _forwardMovementVector = Vector3.forward * (_playerData.ForwardSpeed * Time.deltaTime);
    }

    private void MoveLateral()
    {
        Vector3 targetPosition = GetTargetPositionForLane(_destinationLane);
        targetPosition.z = _currentPosition.z; // Ignore the Z-axis of the target position
        _lateralMovementVector = Vector3.MoveTowards(_currentPosition, targetPosition, _playerData.LateralSpeed * Time.deltaTime) - _currentPosition;
    }
    
    private Vector3 GetTargetPositionForLane(Lane lane)
    {
        if (lane == Lane.Left)
            return _gameManager.LeftLaneStartPoint;
        else if (lane == Lane.Middle)
            return _gameManager.MiddleLaneStartPoint;
        else if (lane == Lane.Right)
            return _gameManager.RightLaneStartPoint;

        // Default to current position if lane is unknown
        return _currentPosition;
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