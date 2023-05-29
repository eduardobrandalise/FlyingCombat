using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Transform leftSpawnerPoint;
    [SerializeField] private Transform middleSpawnerPoint;
    [SerializeField] private Transform rightSpawnerPoint;
    [SerializeField] private GameObject rocket;
    [SerializeField] private float enemySpawningRate = 1f;
    
    private float _spawningTimer = 0f;

    private void FixedUpdate()
    {
        ManageEnemySpawning();
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
        var chosenLane = PickRandomLane();
        
        switch (chosenLane)
        {
            case Lane.Left:
                InstantiateEnemy(leftSpawnerPoint.transform.position);
                break;
            case Lane.Middle:
                InstantiateEnemy(middleSpawnerPoint.transform.position);
                break;
            case Lane.Right:
                InstantiateEnemy(rightSpawnerPoint.transform.position);
                break;
        }
    }

    private void InstantiateEnemy(Vector3 lanePosition)
    {
        GameObject instantiatedObject = Instantiate(rocket, transform.position, Quaternion.identity);
        instantiatedObject.transform.position = lanePosition;
        Rocket rocketComponent = instantiatedObject.GetComponent<Rocket>();
        instantiatedObject.GetComponent<Rocket>().enabled = true;
    }

    private Lane PickRandomLane()
    {
        List<Lane> lanes = new List<Lane>(Enum.GetValues(typeof(Lane)).Cast<Lane>());
        int randomIndex = UnityEngine.Random.Range(0, lanes.Count);
        Lane randomLane = lanes[randomIndex];
        return randomLane;
    }
}