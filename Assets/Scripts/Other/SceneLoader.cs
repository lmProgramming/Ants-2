using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void ChangedScene()
    {
        PlayerPrefs.Save();
    }

    public void Home()
    {
        //if (LoadSaveManager.Instance != null)
        //{
        //    GameManager.Instance.SaveGame();
        //}
        SceneManager.LoadScene("StartMenu");
    }

    public void MainGame()
    {
        ChangedScene();
        //if (LoadSaveManager.Instance != null)
        //{
        //    GameManager.Instance.SaveGame();
        //}
        SceneManager.LoadScene("MainGame");
    }
}
