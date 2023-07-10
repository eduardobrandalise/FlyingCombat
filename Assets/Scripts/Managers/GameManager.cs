using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum State
    {
        GamePlaying,
        GameOver,
    }
    
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public UnityEvent gamePaused;
    public UnityEvent gameUnpaused;
    public GameStateChangedEvent gameStateChanged;
    [Serializable] public class GameStateChangedEvent : UnityEvent<State> { }
    
    [SerializeField] private Transform enemySpawner;
    [SerializeField] private float laneLenght = 400f;
    [SerializeField] public float DistanceBetweenLanes { get; private set; } = 10f;

    public Vector3 MiddleLaneStartPosition { get; private set; }
    public Vector3 MiddleLaneEndPosition { get; private set; }
    public Vector3 LeftLaneStartPosition { get; private set; }
    public Vector3 LeftLaneEndPosition { get; private set; }
    public Vector3 RightLaneStartPosition { get; private set; }
    public Vector3 RightLaneEndPosition { get; private set; }

    private State _currentGameState;
    private bool _isGamePaused = false;

    private void Awake()
    {
        if (_instance != null && _instance != this) { Destroy(gameObject); }
        else { _instance = this; }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (_isGamePaused) { TogglePauseGame(); }
    }

    private void Start()
    {
        Player.Instance.died.AddListener(EndSession);
        InputManager.Instance.pausePressed.AddListener(TogglePauseGame);

        gameStateChanged ??= new GameStateChangedEvent();
        
        SetupLanePositions();
        ChangeGameStatusTo(State.GamePlaying);
    }

    private void OnDestroy()
    {
        Player.Instance.died.RemoveListener(EndSession);
        InputManager.Instance.pausePressed.RemoveListener(TogglePauseGame);
    }

    private void EndSession()
    {
        ChangeGameStatusTo(State.GameOver);
        TogglePauseGame();
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

    private void ChangeGameStatusTo(State state)
    {
        _currentGameState = state;
        gameStateChanged.Invoke(_currentGameState);
    }
    
    public void TogglePauseGame() {
        _isGamePaused = !_isGamePaused;
        
        if (_isGamePaused) {
            Time.timeScale = 0f;
            
            gamePaused.Invoke();
        }
        else {
            Time.timeScale = 1f;
            
            gameUnpaused.Invoke();
        }
    }
    
    /// <summary>
    /// Sets the scale at which time passes for a specified duration.
    /// </summary>
    /// <param name="duration">Duration in seconds.</param>
    /// <param name="timeScale">The scale goes from 0 (paused) to 1 (normal).</param>
    public void SetTimeScaleWithDuration(float timeScale, float duration)
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

    public State GetCurrentState()
    {
        return _currentGameState;
    }
}
