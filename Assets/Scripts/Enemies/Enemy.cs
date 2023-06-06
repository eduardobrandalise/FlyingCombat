using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject enemyModel;
    
    private Player _player;

    private void Start()
    {
        _player = Player.Instance;
        _player.Collided.AddListener(CheckCollision);
    }

    private void CheckCollision(Collider colliderObject)
    {
        if (colliderObject.gameObject == enemyModel.gameObject) Die();
    }

    private void LateUpdate()
    {
        var currentPosition = transform.position;
        var newPosition = new Vector3(currentPosition.x, currentPosition.y, _player.GetPlayerPosition().z);
        
        transform.position = newPosition;
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
