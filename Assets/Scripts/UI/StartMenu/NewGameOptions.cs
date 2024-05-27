using UnityEngine;
using UnityEngine.UI;

public class NewGameOptions : ListOfSelectableOptionsPanel
{
    public GameObject scrollRectContent;
    public float baseScrollRectContentHeight;

    public override void Start()
    {
        base.Start();

        RectTransform rectTransformOfContent = scrollRectContent.GetComponent<RectTransform>();
        rectTransformOfContent.sizeDelta = new Vector2(rectTransformOfContent.sizeDelta.x, baseScrollRectContentHeight + yChange * selectables.Count);
    }
}
