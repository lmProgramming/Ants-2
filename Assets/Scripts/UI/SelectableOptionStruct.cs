using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

[System.Serializable]
public class SelectableOptionsStruct
{
    public enum Type
    {
        Slider,
        Toggle
    }
    public Type type;

    public Vector2 bounds;
    public bool wholeNumbers;

    public float defaultValue;

    public string textLabel;
    public string helpLabel;

    public string id;

    [Header("Ad settings")]
    public bool ad = false;
    public bool activateOnlyWithAd = false;
    public float adDefaultValue;
    public Vector2 adBounds;
    public string adTextLabel;
    public SelectableOptionsStruct adOptions;

    public string GenerateID()
    {
        string[] words = textLabel.Split(' ');

        if (words.Length > 0)
        {
            words[0] = CultureInfo.CurrentCulture.TextInfo.ToLower(words[0]);
            for (int i = 1; i < words.Length; i++)
            {
                words[i] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(words[i]);
            }
        }

        id = string.Join("", words);
        id = Regex.Replace(id, @"[^a-zA-Z]", "");

        return id;
    }
}
