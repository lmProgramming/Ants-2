using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TerrainManipulatorOptionUI : MonoBehaviour, IPointerDownHandler
{
    public Image option;
    public Image outline;
    
    public string nameOfOption;

    public enum TerrainOption
    {
        Food,
        Wall,
        Erase,
        Colony,
        Ant,
        Soldier,
        Queen,
        Spider,
        Pheromone,
        AntKiller,
        None,
        Beetle,
        Water
    }

    public TerrainOption terrainOption;

    public TerrainManipulatorSelectorUI terrainManipulatorSelectorUI;

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        terrainManipulatorSelectorUI.Select(this);
    }

    private void Start()
    {
        if (terrainOption == TerrainOption.Ant || terrainOption == TerrainOption.Soldier || terrainOption == TerrainOption.Queen)
        {
            GameManager.Instance.changedCivIndexOfCurrentlyShownPheromonesInTiles.AddListener(ChangeAntOptionSpriteToMatchGameManager);
        }
    }

    public void ChangeAntOptionSpriteToMatchGameManager()
    {
        option.sprite = AntsManager.Instance.GetAntSprite(GetAntTypeFromTerrainOption(), GameManager.Instance.civIndexOfCurrentlyShownPheromonesInTiles);
    }

    public Ant.AntType GetAntTypeFromTerrainOption()
    {
        switch (terrainOption)
        {
            case TerrainOption.Soldier:
                return Ant.AntType.Soldier;
            case TerrainOption.Queen:
                return Ant.AntType.Queen;
            default:
                return Ant.AntType.Worker;
        }
    }
}
