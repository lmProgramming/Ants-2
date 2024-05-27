using System.Collections.Generic;
using UnityEngine;

public class TitleSettings : ListOfSelectableOptionsPanel
{
    private SliderOptions soundSlider;

    public override void Start()
    {
        base.Start();

        foreach (SelectableOption option in selectables)
        {
            if (option.id == "soundVolume")
            {
                option.updatedTextAction += UpdateSoundVolume;
                soundSlider = (SliderOptions) option;
            }
        }
    }

    public void UpdateSoundVolume()
    {
        PlayerPrefs.SetFloat("soundVolume", soundSlider.Value());

        SoundManager.Instance.SetVolume(soundSlider.Value() / 100);

        PlayerPrefs.Save();
    }
}
