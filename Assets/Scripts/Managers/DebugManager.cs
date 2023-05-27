using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    private static DebugManager instance;
    public static DebugManager Instance { get { return instance; } }

    [SerializeField] private bool debug;
    
    private Vector3 _middleLaneStartPoint;
    private Vector3 _middleLaneEndPoint;
    private Vector3 _leftLaneStartPoint;
    private Vector3 _leftLaneEndPoint;
    private Vector3 _rightLaneStartPoint;
    private Vector3 _rightLaneEndPoint;

    private void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); }
        else { instance = this; }
    }

    private void Start()
    {
        SetupDebugLanes();
    }

    private void Update()
    {
        if (debug)
        {
            DrawLaneLines();
        }
    }

    private void SetupDebugLanes()
    {
        var distanceBetweenLanes = Player.Instance.GetDistanceBetweenLanes();
        const float laneLength = 400f;

        _middleLaneStartPoint = Player.Instance.GetPlayerPosition();
        _middleLaneEndPoint = _middleLaneStartPoint + new Vector3(0, 0, laneLength);
        _leftLaneStartPoint = _middleLaneStartPoint - new Vector3(distanceBetweenLanes, 0, 0);
        _leftLaneEndPoint = _leftLaneStartPoint + new Vector3(0, 0, laneLength);
        _rightLaneStartPoint = _middleLaneStartPoint + new Vector3(distanceBetweenLanes, 0, 0);
        _rightLaneEndPoint = _rightLaneStartPoint + new Vector3(0, 0, laneLength);
    }

    public void DrawLaneLines()
    {
        Debug.DrawLine(_middleLaneStartPoint, _middleLaneEndPoint, Color.red);
        Debug.DrawLine(_leftLaneStartPoint, _leftLaneEndPoint, Color.red);
        Debug.DrawLine(_rightLaneStartPoint, _rightLaneEndPoint, Color.red);
    }
}
