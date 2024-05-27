using UnityEngine;

[System.Serializable]
public class AdOptions
{
    public bool ad = false;
    public string adText;
    public bool activateWithAdOnly = false;
    public Vector2 adBounds = Vector2.zero;
}
