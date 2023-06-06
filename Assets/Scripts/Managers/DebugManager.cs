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

    private void Update()
    {
        if (debug)
        {
            DrawLaneLines();
        }
    }
    
    private static void DrawLaneLines()
    {
        var gameManager = GameManager.Instance;
        Debug.DrawLine(gameManager.LeftLaneStartPosition, gameManager.LeftLaneEndPosition, Color.red);
        Debug.DrawLine(gameManager.MiddleLaneStartPosition, gameManager.MiddleLaneEndPosition, Color.red);
        Debug.DrawLine(gameManager.RightLaneStartPosition, gameManager.RightLaneEndPosition, Color.red);
    }
}
