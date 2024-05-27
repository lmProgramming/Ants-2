using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadScreenManager : MonoBehaviour
{
    public GameObject loadSavesScreen;

    public GameObject savedGamesHolder;

    public GameObject savedGameToLoadPrefab;

    public void CreateLoadMenu()
    {
        DirectoryInfo info = new DirectoryInfo(LoadSaveManager.Instance.saveLocation);
        FileInfo[] fileInfo = info.GetFiles();

        if (fileInfo.Length > 0)
        {
            List<System.DateTime> dateTimes = new List<System.DateTime>();

            List<GameDataHolder> datas = new List<GameDataHolder>();

            foreach (FileInfo file in fileInfo)
            {
                GameDataHolder data = SaveGame.Load(Path.GetFileNameWithoutExtension(file.Name));

                if (data.genuineSave)
                {
                    dateTimes.Add(System.DateTime.FromBinary(data.saveTimeBinary));
                    datas.Add(data);
                }
            }

            for (int i = 0; i < dateTimes.Count; i++)
            {
                System.DateTime latestDate = new System.DateTime(1999, 12, 31);
                int latestIndexDate = -1;

                for (int j = i; j < dateTimes.Count; j++)
                {
                    if (dateTimes[j].CompareTo(latestDate) > 0)
                    {
                        latestDate = dateTimes[j];
                        latestIndexDate = j;
                    }
                }

                System.DateTime tmpDateTime = dateTimes[i];

                dateTimes[latestIndexDate] = dateTimes[i];
                dateTimes[i] = tmpDateTime;

                GameDataHolder tmpData = datas[latestIndexDate];

                datas[latestIndexDate] = datas[i];
                datas[i] = tmpData;
            }

            foreach (GameDataHolder data in datas)
            {
                if (data.genuineSave)
                {
                    loadSavesScreen.SetActive(true);

                    GameObject newSavedGame = Instantiate(savedGameToLoadPrefab);

                    newSavedGame.transform.SetParent(savedGamesHolder.transform);

                    newSavedGame.GetComponent<SavedGameToLoad>().AssignTexts(data);
                }
            }
        }
    }

    public void CloseLoadSavesScreen()
    {
        loadSavesScreen.SetActive(false);

        foreach (Transform child in savedGamesHolder.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
