using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphUpdater : MonoBehaviour
{
    private float timeToUpdateGraphs;
    [SerializeField] private float timeBetweenUpdatingGraphs;
    [SerializeField] private LineGraph graph;
    [SerializeField] private Animator animator;
    [SerializeField] private RectTransform openCloseGraphButton;

    // Start is called before the first frame update
    private void Start()
    {
        UpdateGraph();

        timeToUpdateGraphs = Time.time + timeBetweenUpdatingGraphs;

        SetOpenCloseButtonToCorrectWay();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Time.time >= timeToUpdateGraphs)
        {
            timeToUpdateGraphs = Time.time + timeBetweenUpdatingGraphs;

            UpdateGraph();
        }
    }

    public void UpdateGraph()
    {
        int[] newValue = new int[4];
        foreach (int index in CivilizationsManager.Instance.activeIndexes)
        {
            newValue[index] = CivilizationsManager.Instance.allCivilizations[index].antsAlive;
        }

        graph.AddNewValue(newValue);
    }

    public void OpenOrCloseMenu()
    {
        animator.SetBool("open", !animator.GetBool("open"));
        SetOpenCloseButtonToCorrectWay();
    }

    private void SetOpenCloseButtonToCorrectWay()
    {
        openCloseGraphButton.localRotation = Quaternion.Euler(0, 0, (animator.GetBool("open") ? 1 : -1) * 90);
    }
}
