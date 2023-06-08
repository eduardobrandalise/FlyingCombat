using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class FloorTile : MonoBehaviour
{
    public UnityEvent destroyed;

    private Camera _mainCamera;
    
    private void Start()
    {
        _mainCamera = Camera.main;
    }
    
    private void LateUpdate()
    {
        RepositionWhenOutsideScreen();
    }
    
    private void RepositionWhenOutsideScreen()
    {
        if (IsBehindCamera())
        {
            Reposition();
        }
    }

    private void Reposition()
    {
        Vector3 currentPosition = transform.position;
        float offset = 4000f;
            
        transform.position += new Vector3(0,0,offset);
    }

    private bool IsBehindCamera()
    {
        float selfZ = transform.position.z + 1000;
        float cameraZ = _mainCamera.transform.position.z;

        return selfZ < cameraZ;
    }
}
