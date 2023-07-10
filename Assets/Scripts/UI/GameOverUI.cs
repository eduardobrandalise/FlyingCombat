using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button mainMenuButton;

    private void Start()
    {
        playAgainButton.onClick.AddListener(ResetGame);
        mainMenuButton.onClick.AddListener(LoadMainMenu);
        
        GameManager.Instance.gameStateChanged.AddListener(ShowOnGameOver);

        Hide();
    }

    private void ShowOnGameOver(GameManager.State state)
    {
        if (state == GameManager.State.GameOver) { Show(); }
    }

    private void ResetGame()
    {
        SceneLoader.ReloadGameScene();
    }

    private void LoadMainMenu()
    {
        SceneLoader.Load(SceneLoader.Scene.MainMenuScene);
    }

    private void Show()
    {
        gameObject.SetActive(true);
        playAgainButton.Select();
    }
    
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}