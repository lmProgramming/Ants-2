using UnityEngine.UI;

public class SliderOptions : SelectableOption
{
    public Slider slider;

    public override float Value()
    {
        return slider.value;
    }

    public override void SetupSelectable(SelectableOptionsStruct options)
    {
        slider.minValue = options.bounds.x;

        slider.maxValue = options.bounds.y;

        slider.wholeNumbers = options.wholeNumbers;

        saveAsInt = options.wholeNumbers;

        slider.SetValueWithoutNotify(options.defaultValue);

        base.SetupSelectable(options);
    }

    public override void Start()
    {
        base.Start();

        selectable = slider;
    }

    public override void SetValue(float value)
    {
        slider.value = value;
    }

    public override void WatchedAd()
    {
        slider.interactable = true;

        slider.minValue = adOptions.bounds.x;

        slider.maxValue = adOptions.bounds.y;

        if (adOptions.defaultValue != -1)
            SetValue(adOptions.defaultValue);

        slider.gameObject.SetActive(true);

        base.WatchedAd();
    }

    public override void UpdateText()
    {
        label.SetText(options.textLabel + ": " + (slider.wholeNumbers ? slider.value : slider.value.ToString("F2")));

        if (updatedTextAction != null)
        {
            updatedTextAction.Invoke();
        }
    }
}
