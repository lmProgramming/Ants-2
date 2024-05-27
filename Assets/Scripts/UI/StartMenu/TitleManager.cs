using UnityEngine;

public class TitleManager : MonoBehaviour
{
    public GameObject menuButtons;
    public GameObject newGameOptions;
    public GameObject options;

    public static bool premium = false;

    public void Start()
    {
        if (PlayerPrefs.GetInt("firstTimePlaying", 0) == 0)
        {
            PlayerPrefs.SetInt("firstTimePlaying", 1);
            FirstTimePlaying();
        }
#if UNITY_EDITOR
        //premium = true;
#endif
    }

    public void FirstTimePlaying()
    {
        LoadSaveManager.Instance.CreateASaveFolder();
    }

    public void ShowMenuButtons(bool show)
    {
        menuButtons.SetActive(show);
    }

    public void ShowNewGameOptions(bool show)
    {
        newGameOptions.SetActive(show);

        ShowMenuButtons(!show);
    }

    public void ShowOptions(bool show)
    {
        options.SetActive(show);

        ShowMenuButtons(!show);
    }

    public void YouTube()
    {
        Application.OpenURL("https://www.youtube.com/channel/UCd7gFXGXiNCgerrbJxbrhHA");
    }

    public void Rate()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.LMProgramming.AntsSimulator2TotalWar");
    }
}
