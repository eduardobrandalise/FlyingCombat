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
    [SerializeField] public float DistanceBetweenLanes { get; private set; } = 10f;

    public Vector3 MiddleLaneStartPosition { get; private set; }
    public Vector3 MiddleLaneEndPosition { get; private set; }
    public Vector3 LeftLaneStartPosition { get; private set; }
    public Vector3 LeftLaneEndPosition { get; private set; }
    public Vector3 RightLaneStartPosition { get; private set; }
    public Vector3 RightLaneEndPosition { get; private set; }

    private bool _isGamePaused = false;


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
        // MiddleLaneStartPoint = Player.Instance.GetPlayerPosition();
        MiddleLaneStartPosition = new Vector3(Player.Instance.Position.x, Player.Instance.Position.y, 0);
        MiddleLaneEndPosition = MiddleLaneStartPosition + new Vector3(0, 0, laneLenght);
        LeftLaneStartPosition = MiddleLaneStartPosition - new Vector3(DistanceBetweenLanes, 0, 0);
        LeftLaneEndPosition = LeftLaneStartPosition + new Vector3(0, 0, laneLenght);
        RightLaneStartPosition = MiddleLaneStartPosition + new Vector3(DistanceBetweenLanes, 0, 0);
        RightLaneEndPosition = RightLaneStartPosition + new Vector3(0, 0, laneLenght);
    }
    
    public Vector3 GetLaneStartPosition(Lane lane)
    {
        var dic = new Dictionary<string, string>();
        
        return lane switch
        {
            Lane.Left => LeftLaneStartPosition,
            Lane.Middle => MiddleLaneStartPosition,
            Lane.Right => RightLaneStartPosition,
            _ => MiddleLaneStartPosition
        };
    }
    
    /// <summary>
    /// Sets the scale at which time passes for the specified duration in seconds.
    /// The scale goes from 0 (paused) to 1 (normal).
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="timeScale"></param>
    public void SetTimeScale(float timeScale, float duration)
    {
        if (_isGamePaused)
        {
            print("Game is already paused.");
            return;
        }

        StartCoroutine(SlowTimeCoroutine(duration, timeScale));
    }
    
    private IEnumerator SlowTimeCoroutine(float duration, float timeScale)
    {
        _isGamePaused = true;
        Time.timeScale = timeScale; // Set the time scale to 0 to pause the game

        yield return new WaitForSecondsRealtime(duration); // Wait for the specified duration in real time

        Time.timeScale = 1f; // Set the time scale back to 1 to resume the game
        _isGamePaused = false;
    }
}
