using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    private static Player instance;
    public static Player Instance { get { return instance; } }
    
    [SerializeField] private Transform planeModelTransform;

    private GameManager _gameManager;
    private PlayerData _playerData;
    private PlayerMovement2 _movement;
    private PlayerMesh _playerMesh;
    public Lane CurrentLane { get; private set; } = Lane.Middle;
    private Lane _destinationLane;
    public PlayerState CurrentState { get; private set; } = PlayerState.Idle;
    private InputDirection _inputDirection;

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
        
        InitializeProperties();
    }

    private void Update()
    {
        HandleInput();
        ManageState();
    }

    private void HandleInput()
    {
        var lateralInput = InputManager.Instance.GetLateralMovementNormalized();

        if (lateralInput < 0) { _inputDirection = InputDirection.Left; }
        else if (lateralInput > 0) { _inputDirection = InputDirection.Right; }
        else { _inputDirection = InputDirection.None; }
    }

    private void ManageState()
    {
        UpdateCurrentLane();
        
        switch (CurrentState)
        {
            case PlayerState.Idle:
                if (_inputDirection == InputDirection.Left)
                {
                    if (CurrentLane != Lane.Left)
                    {
                        CurrentState = PlayerState.Dashing;
                        
                        var targetLaneIndex = (int)CurrentLane - 1;
                        _destinationLane = (Lane)targetLaneIndex;
                        
                        Dash();
                    }
                }
                else if (_inputDirection == InputDirection.Right)
                {
                    if (CurrentLane != Lane.Right)
                    {
                        CurrentState = PlayerState.Dashing;
                        
                        var targetLaneIndex = (int)CurrentLane + 1;
                        _destinationLane = (Lane)targetLaneIndex;
                        
                        Dash();
                    }
                }
                else if (_inputDirection == InputDirection.None) { }
                break;
            
            case PlayerState.Dashing:
                if (CurrentLane == _destinationLane)
                {
                    CurrentState = PlayerState.Idle;
                }
                break;
            
            case PlayerState.Returning:
                break;
            
            case PlayerState.Hit:
                break;
            
            case PlayerState.Destroyed:
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnDestroy()
    {
        if (_playerMesh != null)
        {
            _playerMesh.collided.RemoveListener(PlayerMeshOnCollision);
        }
    }

    private void InitializeProperties()
    {
        // ------Singletons------
        _playerData = PlayerData.Instance;
        _gameManager = GameManager.Instance;
        _movement = gameObject.GetComponent<PlayerMovement2>();
    }

    private void PlayerMeshOnCollision(Collider other)
    {
        // Debug.Log("Collision Event triggered by " + other.gameObject.name);
    }

    private void OnCollisionEnter(Collision collision)
    {
        print(collision.gameObject.name);
    }
    
    private void UpdateCurrentLane()
    {
        var lanePositionTolerance = _playerData.LaneSnappingTolerance;
        var currentPosition = transform.position;
        
        if (_gameManager.LeftLaneStartPoint.x - lanePositionTolerance <= currentPosition.x && currentPosition.x <= _gameManager.LeftLaneStartPoint.x + lanePositionTolerance)
        {
            CurrentLane = Lane.Left;
        }
        else if (_gameManager.MiddleLaneStartPoint.x - lanePositionTolerance <= currentPosition.x && currentPosition.x <= _gameManager.MiddleLaneStartPoint.x + lanePositionTolerance)
        {
            CurrentLane = Lane.Middle;
        }
        else if (_gameManager.RightLaneStartPoint.x - lanePositionTolerance <= currentPosition.x && currentPosition.x <= _gameManager.RightLaneStartPoint.x + lanePositionTolerance)
        {
            CurrentLane = Lane.Right;
        }
    }

    public Vector3 GetPlayerPosition()
    {
        return transform.position;
    }

    private void Dash()
    {
        _movement.SetDestinationLane(_destinationLane);
    }
}