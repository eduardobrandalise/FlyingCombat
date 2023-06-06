using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Transform leftSpawner;
    [SerializeField] private Transform middleSpawner;
    [SerializeField] private Transform rightSpawner;
    [SerializeField] private GameObject enemyShipPrefab;
    [SerializeField] private float enemySpawningRate = 1f;
    [SerializeField] private float distanceFromPlayer = 400f;

    private Player _player;
    private float _spawningTimer = 0f;

    private void Start()
    {
        _player = Player.Instance;
    }

    private void FixedUpdate()
    {
        ManageEnemySpawning();
    }

    private void LateUpdate()
    {
        Move();
    }

    private void Move()
    {
        // var currentPosition = transform.position;
        var newPosition = new Vector3(GameManager.Instance.GetLaneStartPosition(Lane.Middle).x, 
            _player.GetPlayerPosition().y, _player.GetPlayerPosition().z + distanceFromPlayer);

        transform.position = newPosition;
    }

    private void ManageEnemySpawning()
    {
        _spawningTimer += Time.deltaTime;
        
        if (_spawningTimer > enemySpawningRate)
        {
            SpawnEnemy();
            _spawningTimer = 0f;
        }
    }

    private void SpawnEnemy()
    {
        InstantiateEnemy(PickRandomLane());
    }

    private void InstantiateEnemy(Lane lane)
    {
        Vector3 spawnerPosition = lane switch
        {
            Lane.Left => leftSpawner.transform.position,
            Lane.Middle => middleSpawner.transform.position,
            Lane.Right => rightSpawner.transform.position,
            _ => middleSpawner.transform.position
        };

        Instantiate(enemyShipPrefab, spawnerPosition, Quaternion.identity);
        // GameObject instantiatedObject = GameObject instantiatedObject = Instantiate(enemyShipPrefab, spawnerPosition, Quaternion.identity);
        // instantiatedObject.transform.position = spawner.transform.position;
        // Rocket rocketComponent = instantiatedObject.GetComponent<Rocket>();
        // instantiatedObject.GetComponent<Enemy>().enabled = true;
    }

    private Lane PickRandomLane()
    {
        List<Lane> lanes = new List<Lane>(Enum.GetValues(typeof(Lane)).Cast<Lane>());
        int randomIndex = UnityEngine.Random.Range(0, lanes.Count);
        Lane randomLane = lanes[randomIndex];
        return randomLane;
    }
}