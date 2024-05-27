using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TopBarUI : MonoBehaviour
{
    public GameObject antNumberInfoPrefab;
    public float antNumberInfoSize;

    public RectTransform simulationSlider;
    public float simulationSliderWidth;

    public RectTransform whiteCircleLeft;
    public RectTransform whiteCircleRight;
    public RectTransform whiteRectangle;

    public float additionalWidth;

    public UISimulationManager UISimulationManager;

    public void GenerateTopBar(List<int> indexesOfAntCivToInclude, bool addSimulationSlider)
    {
        float totalWidth = additionalWidth + indexesOfAntCivToInclude.Count * antNumberInfoSize + (addSimulationSlider ? simulationSliderWidth : 0);

        whiteCircleLeft.anchoredPosition = new Vector3(-totalWidth / 2, whiteCircleLeft.anchoredPosition.y, 0);
        whiteCircleRight.anchoredPosition = new Vector3(totalWidth / 2, whiteCircleRight.anchoredPosition.y, 0);
        whiteRectangle.sizeDelta = new Vector2(totalWidth, whiteRectangle.sizeDelta.y);

        UISimulationManager.antsNumbers = new TextMeshProUGUI[indexesOfAntCivToInclude.Count];
        UISimulationManager.antsNumbersIndexes = new int[indexesOfAntCivToInclude.Count];

        float widthUsedUp = 0;
        for (int i = 0; i < indexesOfAntCivToInclude.Count; i++)
        {
            GameObject newAntNum = Instantiate(antNumberInfoPrefab, transform);
            newAntNum.GetComponent<RectTransform>().anchoredPosition = new Vector3(whiteCircleLeft.anchoredPosition.x + widthUsedUp, newAntNum.GetComponent<RectTransform>().anchoredPosition.y, 0);

            newAntNum.transform.Find("AntImage").GetComponent<Image>().sprite = AntsManager.Instance.GetAntSpriteBasedOffIndex(indexesOfAntCivToInclude[i]);

            newAntNum.transform.Find("BackgroundClickable").GetComponent<AntNumberClick>().SetCivIndex(indexesOfAntCivToInclude[i]);

            widthUsedUp += antNumberInfoSize;

            UISimulationManager.antsNumbers[i] = newAntNum.transform.Find("Num").GetComponent<TextMeshProUGUI>();
            UISimulationManager.antsNumbersIndexes[i] = indexesOfAntCivToInclude[i];

            if (i == 0)
            {
                newAntNum.transform.Find("BackgroundClickable").GetComponent<AntNumberClick>().Select();
            }

            if (i == indexesOfAntCivToInclude.Count - 1)
            {
                newAntNum.transform.Find("Dot").gameObject.SetActive(false);
            }
        }

        if (addSimulationSlider)
        {
            simulationSlider.anchoredPosition = new Vector3(whiteCircleLeft.anchoredPosition.x + widthUsedUp + 150, simulationSlider.anchoredPosition.y, 0);
        }
    }
}
