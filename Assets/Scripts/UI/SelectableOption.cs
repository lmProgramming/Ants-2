using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class SelectableOption : MonoBehaviour
{
    public SelectableOptionsStruct options;

    public Selectable selectable;
    public TextMeshProUGUI label;

    public string id;

    public SelectableOptionsStruct.Type type;

    public SelectableOptionsStruct adOptions;
    public GameObject adObject;

    public UnityAction updatedTextAction;

    public bool saveAsInt = false;

    public abstract float Value();
    public abstract void SetValue(float value);

    // Start is called before the first frame update
    public void Setup(SelectableOptionsStruct options)
    {
        CopyVariables(options);

        SetupSelectable(options);
    }

    public virtual void SetupSelectable(SelectableOptionsStruct options) 
    {
        label.SetText(options.textLabel);

        selectable.interactable = !(options.ad && options.activateOnlyWithAd);
    }

    public virtual void Start()
    {
        UpdateText();
    }

    public void SetDefault()
    {
        SetValue(options.defaultValue);
    }

    public void CopyVariables(SelectableOptionsStruct options)
    {
        this.options = options;

        type = options.type;

        saveAsInt = options.wholeNumbers || type == SelectableOptionsStruct.Type.Toggle;

        adOptions = new SelectableOptionsStruct();
        adOptions.bounds = options.adBounds;
        adOptions.defaultValue = options.adDefaultValue;
        adOptions.textLabel = options.adTextLabel;

        id = options.GenerateID();
    }

    public void SetupAdPrompt(GameObject adObject)
    {
        this.adObject = adObject;

        TextMeshProUGUI text = adObject.GetComponentInChildren<TextMeshProUGUI>();
        text.SetText(adOptions.textLabel);

        Button button = adObject.GetComponentInChildren<Button>();

        button.onClick.AddListener(() =>
        {
            //text.SetText("Tried to watch, error");
            AdsManager.Instance.ShowRewardedAd((bool rewardEarned) =>
            {
                //Debug.Log(rewardEarned);
                if (rewardEarned)
                {
                    // Reward was earned
                    WatchedAd();
                }
                else
                {
                    text.SetText("Reward not earned");
                }
            });
        });
    }

    public virtual void WatchedAd()
    {
        Destroy(adObject);

        UpdateText();
    }

    public virtual void UpdateText()
    {
        label.SetText(options.textLabel);

        if (updatedTextAction != null)
        {
            updatedTextAction.Invoke();
        }
    }
}
