/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class WindowGraph : MonoBehaviour 
{
    [SerializeField] private RectTransform graphContainer;
    [SerializeField] private GameObject linePrefab;

    private float maxValue;

    private List<int>[] valueList;
    private List<RectTransform>[] lines;

    [SerializeField] private float lineWidth = 6;
    private float lineLength;
    [SerializeField] private int maxNumberOfLines = 100;

    [SerializeField] private RectTransform topWhiteBar;

    private float graphWidth;
    private float graphHeight;

    private float CalculateLineLength()
    {
        return graphWidth / maxNumberOfLines;
    }

    private void Awake() 
    {
        valueList = new List<int>[4];
        lines = new List<RectTransform>[4];

        for (int i = 0; i < valueList.Length; i++)
        {
            valueList[i] = new List<int>();
            lines[i] = new List<RectTransform>();
        }
    }

    private void Start()
    {
        maxValue = 1;

        graphContainer.sizeDelta = new Vector2(topWhiteBar.sizeDelta.x, graphContainer.sizeDelta.y);

        graphWidth = graphContainer.sizeDelta.x;
        graphHeight = graphContainer.sizeDelta.y;

        lineLength = CalculateLineLength();
    }

    public void AddNewValue(int[] newValue)
    {
        maxValue = 10;

        for (int index = 0; index < newValue.Length; index++)
        {
            valueList[index].Add(newValue[index]);

            if (lines[index].Count < maxNumberOfLines)
            {
                lines[index].Add(CreateNewLine(Vector2.zero, Vector2.zero, CivilizationsManager.Instance.GetColorByIndex(index)));
            }
            else
            {
                valueList[index].RemoveAt(0);
            }

            maxValue = Mathf.Max(maxValue, valueList[index].Max());
        }

        maxValue *= 1.2f;

        foreach (int index in CivilizationsManager.Instance.activeIndexes)
        {
            ShowGraph(valueList[index], lines[index]);
        }
    }

    private void ShowGraph(List<int> valueList, List<RectTransform> lines) 
    {
        Vector2 lastPosition = Vector2.zero;
        float xPosition = graphWidth;

        for (int i = valueList.Count - 1; i >= 0; i--) {
            float yPosition = valueList[i] / maxValue * graphHeight;
            xPosition -= lineLength;
            //Debug.Log(xPosition + "  " + yPosition);
            // Debug.Log(valueList[i] + " " + maxValue + " " + graphHeight);
            Vector2 position = new Vector2(xPosition, yPosition);

            if (i < valueList.Count - 1) {
                OrientLine(lines[i + 1], lastPosition, position);
            }
            lastPosition = position;
        }
    }

    private RectTransform CreateNewLine(Vector2 dotPositionA, Vector2 dotPositionB, Color color) {
        GameObject newLine = Instantiate(linePrefab, graphContainer);
        //gameObject.transform.SetParent(graphContainer, false);

        newLine.GetComponent<Image>().color = color;    

        RectTransform lineTransform = newLine.GetComponent<RectTransform>();
        lineTransform.anchorMin = new Vector2(0, 0);
        lineTransform.anchorMax = new Vector2(0, 0);

        OrientLine(lineTransform, dotPositionA, dotPositionB);

        return lineTransform;
    }

    private void OrientLine(RectTransform lineTransform, Vector2 dotPositionA, Vector2 dotPositionB)
    {
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);

        lineTransform.sizeDelta = new Vector2(distance, lineWidth);

        lineTransform.anchoredPosition = dotPositionA + .5f * distance * dir;
        lineTransform.localEulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(dir));
    }

    public static float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }

    public static float GetAngleFromVectorFloatXZ(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }
}
