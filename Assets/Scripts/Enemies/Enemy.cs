using System;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject enemyModel;
    [SerializeField] private ParticleSystem deathParticleSystem;
    [SerializeField] private float movementSpeed = 80f;

    private Player _player;
    private Camera _camera;
    private SoundManager _soundManager;
    private GameManager _gameManager;

    private void Start()
    {
        _player = Player.Instance;
        _gameManager = GameManager.Instance;
        _camera = Camera.main;
        _soundManager = SoundManager.Instance;

        _player.collided.AddListener(CheckCollision);
    }

    private void CheckCollision(Collider colliderObject)
    {
        if (colliderObject.gameObject == enemyModel.gameObject) Die();
    }

    // private void LateUpdate()
    // {
    //     PositionNextToPlayer();
    //     PositionInFrontOfPlayer();
    // }

    private void FixedUpdate()
    {
        Move();
        DestroyWhenOutsideScreen();
    }

    private void Move()
    {
        var forwardMovement = Vector3.back * movementSpeed;
        transform.position += (forwardMovement) * Time.deltaTime;
    }

    private void DestroyWhenOutsideScreen()
    {
        if (IsBehindCamera())
        {
            Destroy(gameObject);
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

        yRotation = _player.Position.x < transform.position.x ? 45f : -45f;

        var currentRotation = transform.rotation;
        Vector3 rotationAngle = new Vector3(currentRotation.x, yRotation, currentRotation.z);

        deathParticleSystem.gameObject.transform.rotation = Quaternion.Euler(rotationAngle);
        deathParticleSystem.Play();

        _soundManager.PlayExplosionSound(transform.position);
    }

    private void PositionInFrontOfPlayer()
    {
        var currentPosition = transform.position;
        var newPosition = new Vector3(_player.Position.x, _player.Position.y, _player.Position.z + 50);

        transform.position = newPosition;
    }

    private void PositionNextToPlayer()
    {
        var currentPosition = transform.position;
        var newPosition = new Vector3(currentPosition.x, currentPosition.y, _player.Position.z);

        transform.position = newPosition;
    }
}