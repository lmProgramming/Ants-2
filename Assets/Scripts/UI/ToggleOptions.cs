using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class ToggleOptions : SelectableOption
{
    public Toggle toggle;

    public override void SetupSelectable(SelectableOptionsStruct options)
    {
        toggle.SetIsOnWithoutNotify(options.defaultValue == 1);

        base.SetupSelectable(options);
    }

    public override void SetValue(float value)
    {
        toggle.SetIsOnWithoutNotify(value == 1);
    }

    public override void Start()
    {
        base.Start();

        selectable = toggle;

        saveAsInt = true;
    }

    public override void WatchedAd()
    {
        base.WatchedAd();

        if (adOptions.defaultValue != -1)
            SetValue(adOptions.defaultValue);

        toggle.interactable = true;
    }

    public override float Value()
    {
        return toggle.isOn ? 1 : 0;
    }
}
