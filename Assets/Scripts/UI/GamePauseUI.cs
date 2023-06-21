using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;

    private void Start()
    {
        resumeButton.onClick.AddListener(() =>
        {
            GameManager.Instance.TogglePauseGame();
        });
        
        mainMenuButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.MainMenuScene);
        });
        
        GameManager.Instance.gamePaused.AddListener(Show);
        GameManager.Instance.gameUnpaused.AddListener(Hide);
        
        Hide();
    }

    private void Show()
    {
        gameObject.SetActive(true);
        resumeButton.Select();
    }
    
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
