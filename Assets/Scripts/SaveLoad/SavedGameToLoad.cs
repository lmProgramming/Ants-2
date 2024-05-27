using UnityEngine;
using UnityEngine.UI;

public class SavedGameToLoad : MonoBehaviour
{
    public GameDataHolder data;

    public Text savenameText;
    public Text timeOfSaveText;

    public Text creaturesAliveText;
    public Text simulationTimeText;

    // Start is called before the first frame update
    public void AssignTexts(GameDataHolder data)
    {
        this.data = data;

        savenameText.text = data.saveName;
        timeOfSaveText.text = data.saveTimeString;
        //creaturesAliveText.text = Translator.AliveCellsNumTrans(data.creaturesData.Count);
        //simulationTimeText.text = Translator.SimTimeTrans(data.simulationTime);
    }

    private void Start()
    {
        transform.localScale = new Vector3(1, 1, 1);
    }

    public void LoadThisSave()
    {
        //FindObjectOfType<TitleManager>().LoadWorld(data.saveName);
    }
}
