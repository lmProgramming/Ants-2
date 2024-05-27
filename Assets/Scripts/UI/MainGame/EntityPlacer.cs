using UnityEngine;

public class EntityPlacer : MonoBehaviour
{
    public void PlaceEntity(TerrainManipulatorOptionUI.TerrainOption entity, Vector2 position, int civIndex)
    {
        switch (entity)
        {
            case TerrainManipulatorOptionUI.TerrainOption.Colony:
                if (civIndex >= 0)
                {
                    PlaceColony(position, civIndex);
                }
                else { MainGameHelpUI.Instance.ShowAttemptedSpawnNoCivIndexSelected(); }
                break;
            case TerrainManipulatorOptionUI.TerrainOption.Ant:
                if (civIndex >= 0)
                {
                    PlaceAnt(position, civIndex, Ant.AntType.Worker);
                }
                else { MainGameHelpUI.Instance.ShowAttemptedSpawnNoCivIndexSelected(); }
                break;
            case TerrainManipulatorOptionUI.TerrainOption.Soldier:
                if (civIndex >= 0)
                {
                    PlaceAnt(position, civIndex, Ant.AntType.Soldier);
                }
                else { MainGameHelpUI.Instance.ShowAttemptedSpawnNoCivIndexSelected(); }
                break;
            case TerrainManipulatorOptionUI.TerrainOption.Queen:
                if (civIndex >= 0)
                {
                    PlaceAnt(position, civIndex, Ant.AntType.Queen);
                }
                else { MainGameHelpUI.Instance.ShowAttemptedSpawnNoCivIndexSelected(); }
                break;
            case TerrainManipulatorOptionUI.TerrainOption.Spider:
                PlaceSpider(position);
                break;
            case TerrainManipulatorOptionUI.TerrainOption.Beetle:
                PlaceBeetle(position);
                break;
            default:
                return;
        }
    }

    public void PlaceColony(Vector2 position, int civIndex)
    {
        Civilization civ = CivilizationsManager.Instance.GetCivFromIndex(civIndex);

        civ.SpawnColonyAt(position, true);
    }

    public void PlaceAnt(Vector2 position, int civIndex, Ant.AntType antType)
    {
        Civilization civ = CivilizationsManager.Instance.GetCivFromIndex(civIndex);

        AntSpawner.Instance.SpawnAnt(position, Random.Range(-360, 360), civ, antType, popAnimation:true);
    }

    public void PlaceSpider(Vector2 position)
    {
        CreatureManager.Instance.SpawnCreature(CreatureManager.CreatureType.Spider, position, popAnimation: true);
    }

    public void PlaceBeetle(Vector2 position)
    {
        CreatureManager.Instance.SpawnCreature(CreatureManager.CreatureType.Beetle, position, popAnimation: true);
    }
}
