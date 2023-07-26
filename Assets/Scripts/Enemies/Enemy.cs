using System;
using System.Diagnostics;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

enum EnemyState
{
    Moving,
    Dashing,
    Charging,
    Explode
}

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject enemyModel;
    [SerializeField] private ParticleSystem deathParticleSystem;
    [SerializeField] private ParticleSystem explosionParticleSystem;
    [SerializeField] private float movementSpeed = 0f;
    [SerializeField] private float dashSpeed = 10f;

    private Player _player;
    private Camera _camera;
    private SoundManager _soundManager;
    private GameManager _gameManager;

    private EnemyState _currentState;
    private Vector3 _currentPosition;
    private float attackDistance = 40f;
    private float shakeDuration = 1f;
    private float shakeAmplitude = 0.1f;
    private Vector3 initialPosition;
    private float chargingTimer;
    private bool startedShaking = false;


    private void Start()
    {
        _player = Player.Instance;
        _gameManager = GameManager.Instance;
        _camera = Camera.main;
        _soundManager = SoundManager.Instance;

        _player.collided.AddListener(CheckCollision);

        _currentState = EnemyState.Moving;
        _currentPosition = transform.position;
    }

    private void CheckCollision(Collider colliderObject)
    {
        if (colliderObject.gameObject == enemyModel.gameObject) Die();
    }

    private void FixedUpdate()
    {
        UpdateCurrentPosition();
        ManageState();
        DestroyWhenOutsideScreen();
        
        
    }

    private void ManageState()
    {
        switch (_currentState)
        {
            case EnemyState.Moving:
                Move();
                break;
            case EnemyState.Dashing:
                MoveBesidePlayer();
                break;
            case EnemyState.Charging:
                if (!startedShaking)
                {
                    StartCharging();
                    startedShaking = true;
                }
                
                if (chargingTimer > 0f)
                {
                    Charge();
                }
                break;
            case EnemyState.Explode:
                enemyModel.SetActive(false);
                Instantiate(explosionParticleSystem, transform.position, quaternion.identity);
                DestroyItself();
                break;
        }
    }

    private void Move()
    {
        float amplitude = 0.2f;
        float speed = 5f;
        float forwardSpeed = _player.Data.currentForwardSpeed + movementSpeed;
        Vector3 forwardMovement = Vector3.back * forwardSpeed;
        Vector3 hoverMovement = Vector3.up * (amplitude * Mathf.Cos(speed * Time.time));
        transform.position += (forwardMovement) * Time.deltaTime;
        
        if (transform.position.z - _player.Position.z < attackDistance)
        {
            _currentState = EnemyState.Dashing;
        }
    }

    private void MoveBesidePlayer()
    {
        float targetZPosition = _player.Position.z;
        Vector3 targetPosition = new Vector3(_currentPosition.x, _currentPosition.y, targetZPosition);
        Vector3 movementVector = Vector3.MoveTowards(_currentPosition, targetPosition, dashSpeed * Time.deltaTime) - _currentPosition;
        
        transform.position = _currentPosition + movementVector;

        if (_currentPosition.z - _player.Position.z < 1f)
        {
            _currentState = EnemyState.Charging;
        }
    }

    // private void Hover()
    // {
    //     float amplitude = 0.2f;
    //     float speed = 6f;
    //     float movement = amplitude * Mathf.Cos(speed * Time.time);
    //     
    //     _verticalMovementVector = Vector3.up * movement;
    // }

    private void StartCharging()
    {
        chargingTimer = shakeDuration;
        initialPosition = _currentPosition;
    }

    private void Charge()
    {
        Shake();
        
        chargingTimer -= Time.deltaTime;

        if (!(chargingTimer <= 0f)) return;
        transform.position = initialPosition; // Reset to the initial position after shaking is done
        _currentState = EnemyState.Explode;
    }

    private void Shake()
    {
        float shakeAmountX = Random.Range(-1f, 1f) * shakeAmplitude;
        float shakeAmountY = Random.Range(-1f, 1f) * shakeAmplitude;
        float shakeAmountZ = Random.Range(-1f, 1f) * shakeAmplitude;

        Vector3 shakePosition = new Vector3(shakeAmountX, shakeAmountY, shakeAmountZ) + initialPosition;
        transform.position = shakePosition;
    }

    private void DestroyWhenOutsideScreen()
    {
        if (IsBehindCamera())
        {
            DestroyItself();
        }
    }

    private bool IsBehindCamera()
    {
        float selfZ = transform.position.z;
        float cameraZ = _camera.transform.position.z;
        float offset = 50f;
        
        return selfZ + offset < cameraZ;
    }

    private void Die()
    {
        enemyModel.gameObject.SetActive(false);

        float yRotation = 0f;

        var transformCached = transform;
        
        yRotation = _player.Position.x < transformCached.position.x ? 45f : -45f;

        var currentRotation = transformCached.rotation;
        Vector3 rotationAngle = new Vector3(currentRotation.x, yRotation, currentRotation.z);

        _soundManager.PlayExplosionSound(transformCached.position);
        
        Instantiate(deathParticleSystem, transform.position, Quaternion.Euler(rotationAngle));

        DestroyItself();
    }

    private void PositionInFrontOfPlayer()
    {
        var currentPosition = transform.position;
        var newPosition = new Vector3(_player.Position.x, _player.Position.y, _player.Position.z + 50);

        transform.position = newPosition;
    }

    /// <summary>
    /// Destroy itself.
    /// </summary>
    /// <param name="delay">The optional amount of time to delay before destroying itself.</param>
    private void DestroyItself(float delay = 0f)
    {
        Destroy(gameObject, delay);
    }

    private void UpdateCurrentPosition()
    {
        _currentPosition = transform.position;
    }
}