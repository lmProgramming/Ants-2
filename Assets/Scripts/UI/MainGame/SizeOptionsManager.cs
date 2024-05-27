using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static SizeOption;

public class SizeOptionsManager : MonoBehaviour
{
    public List<SizeOptionRow> columns = new List<SizeOptionRow>();

    public GameObject sizeOptionPrefab;

    public GameObject leftBar;

    public Transform optionsHolder;

    public Vector2 rightTopEndPosition;

    public float yChange;

    public float xColumnChange;

    public TerrainManipulator terrainManipulator;

    public Animator animator;

    public Image chosenOptionImage;

    private void Start()
    {
        Setup();
    }

    private void Setup()
    {
        Vector2 startPos = rightTopEndPosition - new Vector2(xColumnChange * (columns.Count - 1), 0);

        foreach (SizeOptionRow row in columns)
        {
            row.Setup(sizeOptionPrefab, optionsHolder, startPos, yChange, this);

            startPos += new Vector2(xColumnChange, 0);
        }
    }

    public void SelectedOption(SizeOption option)
    {
        terrainManipulator.ChangeBrushType(option.shape);
        terrainManipulator.ChangeBrushSize(option.size);

        chosenOptionImage.sprite = option.GetComponent<Image>().sprite;
        chosenOptionImage.GetComponent<RectTransform>().sizeDelta = option.GetComponent<RectTransform>().sizeDelta;

        HideMenu();
    }

    public void OpenOrCloseMenu()
    {
        animator.SetBool("open", !animator.GetBool("open"));
    }

    public void OpenMenu()
    {
        animator.SetBool("open", true);
    }

    public void HideMenu()
    {
        animator.SetBool("open", false);
    }

    public void SetVisible(bool active)
    {
        leftBar.SetActive(active);
    }
}

[System.Serializable]
public class SizeOptionRow
{
    public Shape shape;

    public List<int> sizes;

    public Sprite sprite;

    public void Setup(GameObject sizeOptionPrefab, Transform parent, Vector2 startPos, float yChange, SizeOptionsManager sizeOptionsManager)
    {
        Vector2 curPos = startPos;

        foreach (int size in sizes)
        {
            GameObject newSizeOptionGameObject = Object.Instantiate(sizeOptionPrefab, parent);

            if (shape == Shape.Scatter)
            {
                sprite = SprayBrushGenerator.GenerateSprayBrushSprite(size * 4, size * 4, 0.8f, 0f);
            }

            newSizeOptionGameObject.GetComponent<Image>().sprite = sprite;

            newSizeOptionGameObject.GetComponent<RectTransform>().sizeDelta *= (float) size / sizes.Max();

            newSizeOptionGameObject.GetComponent<RectTransform>().anchoredPosition = curPos;

            SizeOption newSizeOption = newSizeOptionGameObject.GetComponent<SizeOption>();

            newSizeOption.Setup(shape, size, sizeOptionsManager);

            curPos += new Vector2(0, yChange);
        }
    }
}

public static class SprayBrushGenerator
{
    public static Sprite GenerateSprayBrushSprite(int width, int height, float sprayDensity, float percentDistFromCentreToStartScattering)
    {
        Texture2D sprayBrushTexture = new Texture2D(width, height);

        bool[,] boolMap = GenerateRandomSprayPattern(width, height, sprayDensity, percentDistFromCentreToStartScattering);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                sprayBrushTexture.SetPixel(x, y, boolMap[x, y] ? Color.white : new Color(0, 0, 0, 0));
            }
        }

        // Apply all changes made to the texture
        sprayBrushTexture.Apply();

        sprayBrushTexture.filterMode = FilterMode.Point;

        Sprite sprayBrushSprite = Sprite.Create(sprayBrushTexture, new Rect(0, 0, width, height), Vector2.one * 0.5f);

        return sprayBrushSprite;
    }
}