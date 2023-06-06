using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.Serialization;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject enemyModel;
    [SerializeField] private ParticleSystem deathParticleSystem;
    [SerializeField] private float movementSpeed = 80f;
    
    private Player _player;
    private Camera _mainCamera;

    private void Start()
    {
        _player = Player.Instance;
        _mainCamera = Camera.main;

        _player.Collided.AddListener(CheckCollision);
    }

    private void CheckCollision(Collider colliderObject)
    {
        if (colliderObject.gameObject == enemyModel.gameObject) Die();
    }

    private void LateUpdate()
    {
        // PositionNextToPlayer();
        // PositionInFrontOfPlayer();
    }

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
        if (IsBehindCamera()) { Destroy(gameObject); }
    }
    
    private bool IsBehindCamera()
    {
        float selfZ = transform.position.z;
        float cameraZ = _mainCamera.transform.position.z;

        return selfZ < cameraZ;
    }

    private void Die()
    {
        enemyModel.gameObject.SetActive(false);

        float yRotation = 0f;

        yRotation = _player.GetPlayerPosition().x < transform.position.x ? 90f : -90f;

        var currentRotation = transform.rotation;
        Vector3 rotationAngle = new Vector3(currentRotation.x, yRotation, currentRotation.z);
        
        deathParticleSystem.gameObject.transform.rotation = Quaternion.Euler(rotationAngle);
        deathParticleSystem.Play();
        // Destroy(gameObject);
    }

    private void PositionInFrontOfPlayer()
    {
        var currentPosition = transform.position;
        var newPosition = new Vector3(_player.GetPlayerPosition().x, _player.GetPlayerPosition().y, _player.GetPlayerPosition().z + 50);

        transform.position = newPosition;
    }

    private void PositionNextToPlayer()
    {
        var currentPosition = transform.position;
        var newPosition = new Vector3(currentPosition.x, currentPosition.y, _player.GetPlayerPosition().z);

        transform.position = newPosition;
    }
}
