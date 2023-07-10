using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Transform leftSpawner;
    [SerializeField] private Transform middleSpawner;
    [SerializeField] private Transform rightSpawner;
    [SerializeField] private GameObject enemyShipPrefab;
    [SerializeField] private WallRefsSO wallRefSo;
    [SerializeField] private float enemySpawningRate = 1f;
    [SerializeField] private float wallSpawningRate = 3f;
    [SerializeField] private float distanceFromPlayer = 400f;

    private Player _player;
    private float _spawningEnemyTimer = 0f;
    private float _spawningWallTimer = 0f;

    private void Start()
    {
        _player = Player.Instance;
    }

    private void FixedUpdate()
    {
        ManageSpawning();
    }

    private void LateUpdate()
    {
        Move();
    }

    /// <summary>
    /// Repositions the Spawner based on the current player's position. It should be called in LateUpdate to take
    /// movement calculations in consideration.
    /// </summary>
    private void Move()
    {
        // var currentPosition = transform.position;
        var newPosition = new Vector3(GameManager.Instance.GetLaneStartPosition(Lane.Middle).x,
            _player.Position.y, _player.Position.z + distanceFromPlayer);

        transform.position = newPosition;
    }

    private void ManageSpawning()
    {
        _spawningEnemyTimer += Time.deltaTime;
        _spawningWallTimer += Time.deltaTime;

        if (_spawningEnemyTimer > enemySpawningRate)
        {
            SpawnEnemy();
            _spawningEnemyTimer = 0f;
        }

        if (_spawningWallTimer > wallSpawningRate)
        {
            // SpawnWall();
            _spawningWallTimer = 0f;
        }
    }

    private void SpawnEnemy()
    {
        InstantiateEnemy(PickRandomLane());
    }

    private void SpawnWall()
    {
        InstantiateWall(PickRandomWall());
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
    }

    private void InstantiateWall(GameObject wall)
    {
        Vector3 spawningPointPosition = middleSpawner.transform.position;
        Vector3 position = new Vector3(spawningPointPosition.x, 0, spawningPointPosition.z);
        Instantiate(wall, position, Quaternion.identity);
    }

    private Lane PickRandomLane()
    {
        List<Lane> lanes = new List<Lane>(Enum.GetValues(typeof(Lane)).Cast<Lane>());
        int randomIndex = Random.Range(0, lanes.Count);
        Lane randomLane = lanes[randomIndex];
        return randomLane;
    }

    private GameObject PickRandomWall()
    {
        int randomIndex = Random.Range(0, wallRefSo.walls.Count);
        return wallRefSo.walls[randomIndex];
    }
}