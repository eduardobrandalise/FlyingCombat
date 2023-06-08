using System;
using Branda.Utils;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    private static Player instance;
    public static Player Instance { get { return instance; } }
    
    public CollisionEvent Collided;
    [System.Serializable] public class CollisionEvent : UnityEvent<Collider> { }

    [SerializeField] private Transform planeModelTransform;

    private GameManager _gameManager;
    private PlayerData _playerData;
    private PlayerMovement _movement;
    private PlayerMesh _playerMesh;
    private CinemachineImpulseSource _impulseSource;
    public Lane CurrentLane { get; private set; } = Lane.Middle;
    public Lane DestinationLane  { get; private set; } = Lane.Middle;
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
        _movement = gameObject.GetComponent<PlayerMovement>();
    }

    private void PlayerMeshOnCollision(Collider other)
    {
        if (other.gameObject.layer == 7)
        {
            if (CurrentState == PlayerState.Dashing)
            {
                _gameManager.SetTimeScale(0f, 0.12f);
                CurrentState = PlayerState.Returning;
                Collided.Invoke(other);
            }
            else if (CurrentState == PlayerState.Idle)
            {
                print("PLAYER WAS HIT");
                _impulseSource = GetComponent<CinemachineImpulseSource>();
                _impulseSource.GenerateImpulse();
                // CinemachineImpulseDefinition cinemachineImpulseDefinition = new CinemachineImpulseDefinition
                // {
                //     m_ImpulseShape = CinemachineImpulseDefinition.ImpulseShapes.Rumble,
                //     // m_ImpulseDuration = 0.5f,
                //     // m_PropagationSpeed = 800f
                // };
                //
                // _impulseSource.m_ImpulseDefinition = cinemachineImpulseDefinition;
                // _impulseSource.GenerateImpulse();
                // UtilsClass.ShakeCamera(0.2f, 0.2f);
            }
        }
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

    public Vector3 GetPlayerPosition()
    {
        return transform.position;
    }
}