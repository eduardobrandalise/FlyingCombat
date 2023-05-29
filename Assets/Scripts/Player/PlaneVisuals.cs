using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneVisuals : MonoBehaviour
{
    [SerializeField] private Transform propelerTransform;

    private float propellerRotation = 50f;
    private float playerSpeed = 100f;

    private void LateUpdate()
    {
        propellerRotation = (propellerRotation + (playerSpeed * propellerRotation * Time.deltaTime)) % 360f;
        propelerTransform.rotation = Quaternion.Euler(0, 0, propellerRotation);
    }
}
