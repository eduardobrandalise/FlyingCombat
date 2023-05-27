using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Branda.Utils;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    [SerializeField] private Transform enemySpawner;
    [SerializeField] private float laneLenght = 400f;

    public Vector3 MiddleLaneStartPoint { get; private set; }
    public Vector3 MiddleLaneEndPoint { get; private set; }
    public Vector3 LeftLaneStartPoint { get; private set; }
    public Vector3 LeftLaneEndPoint { get; private set; }
    public Vector3 RightLaneStartPoint { get; private set; }
    public Vector3 RightLaneEndPoint { get; private set; }
    
    private void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); }
        else { instance = this; }
    }

    private void Start()
    {
        SetupLanePositions();
    }

    private void SetupLanePositions()
    {
        var distanceBetweenLanes = Player.Instance.GetDistanceBetweenLanes();
        
        MiddleLaneStartPoint = Player.Instance.GetPlayerPosition();
        MiddleLaneEndPoint = MiddleLaneStartPoint + new Vector3(0, 0, laneLenght);
        LeftLaneStartPoint = MiddleLaneStartPoint - new Vector3(distanceBetweenLanes, 0, 0);
        LeftLaneEndPoint = LeftLaneStartPoint + new Vector3(0, 0, laneLenght);
        RightLaneStartPoint = MiddleLaneStartPoint + new Vector3(distanceBetweenLanes, 0, 0);
        RightLaneEndPoint = RightLaneStartPoint + new Vector3(0, 0, laneLenght);
    }
}
