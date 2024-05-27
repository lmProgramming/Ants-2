using UnityEngine;
using UnityEngine.UI;

public class PausedMenuUI : MonoBehaviour
{
    public RawImage pausedBlurredBackground;
    public GameObject blurredBackgroundGameObject;

    // Start is called before the first frame update
    public void TurnOn()
    {
        blurredBackgroundGameObject.SetActive(true);
        pausedBlurredBackground.SetNativeSize();
    }

    public void TurnOff()
    {
        blurredBackgroundGameObject.SetActive(false);
    }
}
