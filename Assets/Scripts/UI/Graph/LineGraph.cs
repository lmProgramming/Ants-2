using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class LineGraph : MonoBehaviour
{
    private int graphWidth = 256;
    private int graphHeight = 128;

    public Color backgroundColor;

    public int lineWidth = 2;

    private Texture2D graphTexture;

    private float maxValue;

    private List<int>[] valueList;

    private float lineLength;

    [SerializeField] private int maxNumberOfLines = 100;

    [SerializeField] private RectTransform topWhiteBar;

    [SerializeField] 
    private RectTransform graphContainer;

    private void Start()
    {
        graphWidth = (int)topWhiteBar.sizeDelta.x;
        graphHeight = (int)graphContainer.sizeDelta.y;

        graphContainer.sizeDelta = new Vector2(graphWidth, graphHeight);

        graphTexture = new Texture2D(graphWidth, graphHeight);
        RawImage rawImage = GetComponent<RawImage>();

        rawImage.texture = graphTexture;
        rawImage.texture.filterMode = FilterMode.Point;

        lineLength = CalculateLineLength();

        valueList = new List<int>[4];

        for (int i = 0; i < valueList.Length; i++)
        {
            valueList[i] = new List<int>();
        }

        pixels = new Color[graphTexture.width * graphTexture.height];

        AddNewValue(new []{ 0, 0, 0, 0});
    }

    private Color[] pixels;

    private void ClearTexture(Color color)
    {
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }
    }

    private float CalculateLineLength()
    {
        return (float)graphWidth / maxNumberOfLines;
    }

    public void AddNewValue(int[] newValue)
    {
        lineLength = CalculateLineLength();

        ClearTexture(backgroundColor);

        maxValue = 10;

        for (int index = 0; index < newValue.Length; index++)
        {
            valueList[index].Add(newValue[index]);

            if (valueList[index].Count >= maxNumberOfLines)
            {
                valueList[index].RemoveAt(0);
            }

            maxValue = Mathf.Max(maxValue, valueList[index].Max());
        }

        maxValue *= 1.2f;

        foreach (int civIndex in CivilizationsManager.Instance.activeIndexes)
        {
            Color color = CivilizationsManager.Instance.GetColorByIndex(civIndex);

            DrawGraph(valueList[civIndex], color);
        }

        graphTexture.SetPixels(pixels);
        graphTexture.Apply();
    }

    private float widthToTraverse;

    private void DrawGraph(List<int> valueList, Color color)
    {
        int lastYPosition = 0;
        int xPosition = graphWidth;
        int yPosition;

        widthToTraverse = lineLength;

        for (int i = valueList.Count - 1; i >= 1; i--)
        {
            yPosition = (int)(valueList[i] / maxValue * graphHeight);

            xPosition = DrawLine(valueList, color, lastYPosition, xPosition, yPosition, i);

            widthToTraverse += lineLength;

            lastYPosition = yPosition;
        }
    }

    private int DrawLine(List<int> valueList, Color color, int lastYPosition, int xPosition, int yPosition, int i)
    {
        if (i < valueList.Count - 1)
        {
            int x;
            for (x = 0; x < widthToTraverse; x++)
            {
                xPosition -= 1;
                int arrayCentreIndex = xPosition + (int)Mathf.Lerp(lastYPosition, yPosition, x / widthToTraverse) * graphWidth;

                if (xPosition < 0)
                {
                    continue;
                }

                for (int y = -lineWidth; y < lineWidth; y++)
                {
                    int arrayIndex = arrayCentreIndex + y * graphWidth;

                    if (arrayIndex < 0 || arrayIndex >= pixels.Length)
                    {
                        continue;
                    }

                    pixels[arrayIndex] = color;
                }                
            }

            widthToTraverse -= x;
        }

        return xPosition;
    }
}