using System;
using Branda.Utils;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    private static Player _instance;
    public static Player Instance { get { return _instance; } }
    
    public CollisionEvent collided;
    [System.Serializable] public class CollisionEvent : UnityEvent<Collider> { }

    public UnityEvent died;

    [SerializeField] private Transform planeModelTransform;

    private GameManager _gameManager;
    private PlayerData _playerData;
    private SoundManager _soundManager;
    private PlayerMovement _movement;
    private PlayerMesh _playerMesh;
    private CinemachineImpulseSource _impulseSource;
    private int _lives = 3;
    public Lane CurrentLane { get; private set; } = Lane.Middle;
    public Lane DestinationLane  { get; private set; } = Lane.Middle;
    public PlayerState CurrentState { get; private set; } = PlayerState.Idle;
    public Vector3 Position => GetPosition();
    
    private InputDirection _inputDirection;

    private void Awake()
    {
        if (_instance != null && _instance != this) { Destroy(gameObject); }
        else { _instance = this; }
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
                        DestinationLane = (Lane)targetLaneIndex;
                    }
                }
                else if (_inputDirection == InputDirection.Right)
                {
                    if (CurrentLane != Lane.Right)
                    {
                        CurrentState = PlayerState.Dashing;
                        
                        var targetLaneIndex = (int)CurrentLane + 1;
                        DestinationLane = (Lane)targetLaneIndex;
                    }
                }
                else if (_inputDirection == InputDirection.None) { }
                break;
            
            case PlayerState.Dashing:
                if (CurrentLane == DestinationLane)
                {
                    CurrentState = PlayerState.Idle;
                }
                break;
            
            case PlayerState.Returning:
                var tolerance = _playerData.LaneSnappingTolerance;
                var currentPosition = transform.position;
                
                if (_gameManager.GetLaneStartPosition(CurrentLane).x - tolerance <= currentPosition.x && currentPosition.x <= _gameManager.GetLaneStartPosition(CurrentLane).x + tolerance)
                {
                    CurrentState = PlayerState.Idle;
                    DestinationLane = CurrentLane;
                }
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
        _soundManager = SoundManager.Instance;
        
        _movement = gameObject.GetComponent<PlayerMovement>();
    }

    private void PlayerMeshOnCollision(Collider other)
    {
        if (other.gameObject.layer == (int)Layer.Enemy)
        {
            if (CurrentState == PlayerState.Dashing)
            {
                CurrentState = PlayerState.Returning;
                collided.Invoke(other);
                _gameManager.SetTimeScale(0f, 0.12f);
            }
            else if (CurrentState == PlayerState.Idle)
            {
                RemoveLife();
                _impulseSource = GetComponent<CinemachineImpulseSource>();
                _impulseSource.GenerateImpulse();
                _soundManager.PlayHitSound(GetPosition());
            }
        }
        else if (other.gameObject.layer == (int)Layer.Obstacle)
        {
            RemoveLife();
            print("Player collided with: " + other.name);
        }
    }

    private void RemoveLife()
    {
        _lives -= 1;
        if (_lives <= 0) { Die(); }
    }

    private void Die()
    {
        died.Invoke();
    }

    private void UpdateCurrentLane()
    {
        var lanePositionTolerance = _playerData.LaneSnappingTolerance;
        var currentPosition = transform.position;
        
        if (_gameManager.LeftLaneStartPosition.x - lanePositionTolerance <= currentPosition.x && currentPosition.x <= _gameManager.LeftLaneStartPosition.x + lanePositionTolerance)
        {
            CurrentLane = Lane.Left;
            CurrentState = PlayerState.Idle;
            DestinationLane = CurrentLane;
        }
        else if (_gameManager.MiddleLaneStartPosition.x - lanePositionTolerance <= currentPosition.x && currentPosition.x <= _gameManager.MiddleLaneStartPosition.x + lanePositionTolerance)
        {
            CurrentLane = Lane.Middle;
            CurrentState = PlayerState.Idle;
            DestinationLane = CurrentLane;
        }
        else if (_gameManager.RightLaneStartPosition.x - lanePositionTolerance <= currentPosition.x && currentPosition.x <= _gameManager.RightLaneStartPosition.x + lanePositionTolerance)
        {
            CurrentLane = Lane.Right;
            CurrentState = PlayerState.Idle;
            DestinationLane = CurrentLane;
        }
    }

    private Vector3 GetPosition()
    {
        return transform.position;
    }

    private int GetRemainingLives()
    {
        return _lives;
    }
}