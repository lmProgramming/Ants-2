using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class HintManager : MonoBehaviour
{
    public static HintManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("showAntIconTopHint", 1) == 1)
        {
            if (CivilizationsManager.Instance.activeCivilizations.Count > 1)
            {
                MainGameHelpUI.Instance.ShowHintToClickTopAntsIcon();

                PlayerPrefs.SetInt("showAntIconTopHint", 0);
            }
        }
    }
}
