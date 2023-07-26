using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private Player _player;
    private Camera _camera;

    void Start()
    {
        _player = Player.Instance;
        _camera = Camera.main;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
        DestroyWhenOutsideScreen();
    }
    
    private void Move()
    {
        float forwardSpeed = _player.Data.currentForwardSpeed;
        Vector3 forwardMovement = Vector3.back * forwardSpeed;
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
}
