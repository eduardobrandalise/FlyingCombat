using UnityEngine.SceneManagement;

public static class SceneLoader {
    
    public enum Scene {
        GameScene,
        LoadingScene,
        MainMenuScene
    }
    
    private static Scene targetScene;

    public static void Load(Scene targetScene) {
        SceneLoader.targetScene = targetScene;

        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }
    
    public static void ReloadGameScene()
    {
        Load(Scene.GameScene);
    }

    public static void LoaderCallback() {
        SceneManager.LoadScene(targetScene.ToString());
    }
}