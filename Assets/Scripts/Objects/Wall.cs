using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private Camera _camera;

    void Start()
    {
        _camera = Camera.main;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        DestroyWhenOutsideScreen();
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
