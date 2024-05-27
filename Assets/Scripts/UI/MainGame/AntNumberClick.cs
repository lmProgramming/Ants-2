using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AntNumberClick : MonoBehaviour, IPointerDownHandler
{
    private int civIndex;
    public Image antImage;
    private bool active = false;

    public void SetCivIndex(int index)
    {
        civIndex = index;
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        Select();
    }

    public void Select()
    {
        if (active)
        {
            GameManager.Instance.ChangeCivIndexOfCurrentlyShownPheromonesInTiles(-1);

            Deselect();
        }
        else
        {
            AntNumberClick[] antNumberClicks = FindObjectsOfType<AntNumberClick>();

            for (int i = 0; i < antNumberClicks.Length; i++)
            {
                antNumberClicks[i].Deselect();
            }

            antImage.color = new Color(1, 1, 1, 0.7f);

            GameManager.Instance.ChangeCivIndexOfCurrentlyShownPheromonesInTiles(civIndex);

            active = true;
        }

        PheromoneUpdater.Instance.UpdateAllSprites();
    }

    public void Deselect()
    {
        antImage.color = Color.white;

        active = false;
    }
}
