using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class MainGameHelpUI : MonoBehaviour
{
    public static MainGameHelpUI Instance;

    public GameObject attemptedSpawnNoCivIndexSelected;
    public GameObject nameOfSelectedOption;
    public GameObject clickAntsIconTopHint;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowAttemptedSpawnNoCivIndexSelected()
    {
        attemptedSpawnNoCivIndexSelected.SetActive(true);

        StopCoroutine(nameof(HideAttemptedSpawnNoCivIndexSelected));

        StartCoroutine(nameof(HideAttemptedSpawnNoCivIndexSelected));
    }

    public void ShowHintToClickTopAntsIcon()
    {
        clickAntsIconTopHint.SetActive(true);

        StopCoroutine(nameof(HideHintToClickTopAntsIcon));

        StartCoroutine(nameof(HideHintToClickTopAntsIcon));
    }

    public void ShowNameOfSelectedOption(string text)
    {
        nameOfSelectedOption.GetComponentInChildren<TextMeshProUGUI>().SetText(text);
        nameOfSelectedOption.SetActive(true);

        StopCoroutine(nameof(HideNameOfSelectedOption));

        StartCoroutine(nameof(HideNameOfSelectedOption));
    }

    private IEnumerator HideAttemptedSpawnNoCivIndexSelected()
    {
        yield return new WaitForSecondsRealtime(2f);

        attemptedSpawnNoCivIndexSelected.SetActive(false);
    }

    private IEnumerator HideNameOfSelectedOption()
    {
        yield return new WaitForSecondsRealtime(0.9f);

        nameOfSelectedOption.SetActive(false);
    }

    private IEnumerator HideHintToClickTopAntsIcon()
    {
        yield return new WaitForSecondsRealtime(6f);

        clickAntsIconTopHint.SetActive(false);
    }
}
