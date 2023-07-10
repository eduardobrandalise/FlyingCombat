using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;

    private void Start()
    {
        resumeButton.onClick.AddListener(UnpauseGame);
        
        mainMenuButton.onClick.AddListener(LoadMainMenuScene);
        
        GameManager.Instance.gamePaused.AddListener(Show);
        GameManager.Instance.gameUnpaused.AddListener(Hide);
        
        Hide();
    }

    private static void UnpauseGame()
    {
        GameManager.Instance.TogglePauseGame();
    }

    private static void LoadMainMenuScene()
    {
        SceneLoader.Load(SceneLoader.Scene.MainMenuScene);
    }

    private void Show()
    {
        if (GameManager.Instance.GetCurrentState() != GameManager.State.GamePlaying) return;
        
        gameObject.SetActive(true);
        resumeButton.Select();
    }
    
    private void Hide()
    {
        if (GameManager.Instance.GetCurrentState() != GameManager.State.GamePlaying) return;
        
        gameObject.SetActive(false);
    }
}