using UnityEngine;

public class TitleBackgroundGameManager : MonoBehaviour
{
    public Camera cameraMain;

    private void Start()
    {
        GameManager.Instance.ChangeCivIndexOfCurrentlyShownPheromonesInTiles(-1);

        int i = 0;
        while (i < 50)
        {
            Vector2 cameraPos = cameraMain.transform.position;

            foreach (Civilization civ in CivilizationsManager.Instance.activeCivilizations)
            {
                foreach (Colony colony in civ.colonies)
                {
                    Vector2 colonyPos = colony.transform.position;

                    if (colonyPos.x > cameraPos.x - 35 && colonyPos.x < cameraPos.x + 35 && colonyPos.y > cameraPos.y - 20 && colonyPos.y < cameraPos.y + 20)
                    {
                        return;
                    }
                }
            }

            cameraMain.transform.position = new Vector3(Random.Range(-35, 35), Random.Range(-45, 45), -10);
            i++;
        }
    }
}
