using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField] private RocketMesh rocketMesh;
    [SerializeField] private ParticleSystem explosionParticle;
    
    private Camera _mainCamera;
    private const float Speed = 80f;

    private void Start()
    {
        _mainCamera = Camera.main;

        transform.rotation = Quaternion.Euler(-90, 0, 0);
        if (rocketMesh != null)
        {
            rocketMesh.collided.AddListener(RocketMeshOnCollision);
        }
    }

    private void FixedUpdate()
    {
        Move();
        DestroyWhenOutsideScreen();
        
    }

    private void Move()
    {
        var forwardMovement = Vector3.back * Speed;
        transform.position += (forwardMovement) * Time.deltaTime;
    }

    private void RocketMeshOnCollision(Collider other)
    {
        print(other.gameObject.name);
        ParticleSystem instantiatedObject = Instantiate(explosionParticle, transform.position, Quaternion.identity);
        CinemachineShake.Instance.ShakeCamera(2f, 0.5f);
        Destroy(gameObject);
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
}
