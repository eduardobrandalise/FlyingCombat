using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionParticle : MonoBehaviour
{
    private Camera _mainCamera;
    private const float Speed = 80f;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void FixedUpdate()
    {
        var forwardMovement = Vector3.forward * Speed;
        transform.position += (forwardMovement) * Time.deltaTime;
        
        DestroyWhenOutsideScreen();
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
