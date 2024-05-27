using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject simulationUI;
    public GameObject pausedMenuUI;
    public GameObject settingsPanel;

    public PausedMenuUI pausedMenuManager;

    private float previousSimulationSpeed;

    public void ShowPausedMenu()
    {
        simulationUI.SetActive(false);
        pausedMenuUI.SetActive(true);

        PauseGame();
    }

    public void ShowSimulationUI()
    {
        simulationUI.SetActive(true);
        pausedMenuUI.SetActive(false);

        RestoreFormerSimulationSpeed();
    }

    public void ShowSettingsPanel()
    {
        settingsPanel.SetActive(true);
    }

    public void HideSettingsPanel()
    {
        settingsPanel.SetActive(false);
    }

    public void PauseGame()
    {
        previousSimulationSpeed = GameManager.Instance.simulationSpeed;
        GameManager.Instance.ChangeSimulationSpeed(0);

        if (PlayerPrefs.GetInt("blurBackgroundPause", 0) == 1 && CameraManager.SupportsRender() && CameraManager.Instance.SupportsRenderOfPausedMenuGameTexture())
        {
            CameraManager.Instance.EnableBlur();

            CameraManager.Instance.TurnOnCameraRenderingToTexture();

            pausedMenuManager.TurnOn();
        }

        CameraManager.Instance.updateCamera = false;
    }

    public void RestoreFormerSimulationSpeed()
    {
        if (PlayerPrefs.GetInt("blurBackgroundPause", 0) == 1 && CameraManager.SupportsRender() && CameraManager.Instance.SupportsRenderOfPausedMenuGameTexture())
        {
            CameraManager.Instance.TurnOffCameraRenderingToTexture();

            CameraManager.Instance.DisableBlur();

            pausedMenuManager.TurnOff();
        }

        CameraManager.Instance.updateCamera = true;

        GameManager.Instance.ChangeSimulationSpeed(previousSimulationSpeed);
    }
}
