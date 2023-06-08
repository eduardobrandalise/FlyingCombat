using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Floor : MonoBehaviour
{
    [SerializeField] private GameObject floorTile;

    private MeshRenderer _floorMeshRenderer;
    
    private void Start()
    {
        _floorMeshRenderer = floorTile.GetComponent<MeshRenderer>();
    }
}
