using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISimulationManager : MonoBehaviour
{
    public Slider simulationSpeedSlider;
    public TextMeshProUGUI simulationSpeedSliderText;

    //public TextMeshProUGUI simulationTime;
    //public TextMeshProUGUI antsAlive;

    public TextMeshProUGUI fps;

    private float timeLeftUntilUpdate;
    public float updateInterval;

    public TextMeshProUGUI[] antsNumbers;
    public int[] antsNumbersIndexes;

    private void Start()
    {
        timeLeftUntilUpdate = updateInterval;

        simulationSpeedSlider.maxValue = Mathf.Sqrt(PlayerPrefs.GetInt("maximumAllowedSimulationSpeed", 4));

        ChangeSimulationSpeedFromSlider();
    }

    // Update is called once per frame
    public void ChangeSimulationSpeedFromSlider()
    {
        float realSimulationSpeedFromSliderValue = simulationSpeedSlider.value * simulationSpeedSlider.value;
        GameManager.Instance.ChangeSimulationSpeed(realSimulationSpeedFromSliderValue);
        simulationSpeedSliderText.SetText(realSimulationSpeedFromSliderValue.ToString("0.##"));
    }

    private int lastFrameIndex;
    private float[] frameDeltaTimeArray;

    private void Awake()
    {
        frameDeltaTimeArray = new float[60];
        for (int i = 0; i < frameDeltaTimeArray.Length; i++)
        {
            frameDeltaTimeArray[i] = 1 / 60;
        }
    }

    private float CalculateFPS()
    {
        float total = 0f;
        foreach (float deltaTime in frameDeltaTimeArray)
        {
            total += deltaTime;
        }

        return frameDeltaTimeArray.Length / total;
    }

    public void Update()
    {
        timeLeftUntilUpdate -= Time.unscaledDeltaTime;
        if (timeLeftUntilUpdate <= 0)
        {
            timeLeftUntilUpdate = updateInterval;

            UpdateUI();

            frameDeltaTimeArray[lastFrameIndex] = Time.unscaledDeltaTime;
            lastFrameIndex = (lastFrameIndex + 1) % frameDeltaTimeArray.Length;
            //fps.SetText(Mathf.RoundToInt(CalculateFPS()).ToString());
        }
    }

    public void UpdateUI()
    {
        //simulationTime.SetText("Simulation time: " + GameManager.Instance.simulationTime.ToString("0.##"));
        //antsAlive.SetText("Ants alive: " + AntsManager.Instance.antsCount.ToString());
        for (int i = 0; i < antsNumbersIndexes.Length; i++)
        {
            antsNumbers[i].SetText(CivilizationsManager.Instance.allCivilizations[antsNumbersIndexes[i]].antsAlive.ToString());
        }
    }
}
