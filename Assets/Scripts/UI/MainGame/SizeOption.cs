using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SizeOption : MonoBehaviour
{    
    public enum Shape
    {
        Circle,
        Rect,
        Scatter
    }

    [field: SerializeField]
    public Shape shape { get; private set; }
    [field:SerializeField]
    public int size { get; private set; }

    public SizeOptionsManager sizeOptionsManager;

    public Button button;

    public void Setup(Shape shape, int size, SizeOptionsManager sizeOptionsManager)
    {
        this.shape = shape;
        this.size  = size;
        this.sizeOptionsManager = sizeOptionsManager;

        button.onClick.AddListener(Selected);
    }

    public void Selected()
    {
        sizeOptionsManager.SelectedOption(this);
    }

    public static bool[,] GenerateRandomSprayPattern(int width, int height, float sprayDensity, float percentDistFromCentreToStartScattering)
    {
        float halfWidth = width / 2;
        float halfHeight = height / 2;

        // Calculate the center position
        Vector2 center = new Vector2(halfWidth, halfHeight);

        bool[,] boolMap = new bool[width, height];

        // Loop through each pixel of the texture
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Calculate the distance from the center
                float distance = Vector2.Distance(center, new Vector2(x, y)) / halfWidth;

                boolMap[x, y] = false;

                if (distance < 1)
                {
                    // Calculate the probability of painting the pixel
                    float probability = 1 - sprayDensity + distance * 0.5f;

                    // Randomly decide whether to paint the pixel based on the probability
                    if (Random.value >= probability)
                    {
                        // Generate a random color for the pixel
                        boolMap[x, y] = true;
                    }
                }
            }
        }

        return boolMap;
    }
}
