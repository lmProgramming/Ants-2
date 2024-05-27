using UnityEngine;

public class Debugger : MonoBehaviour
{
    public GameObject UI;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) || (Input.GetKey(KeyCode.M) && Random.value > 0.9f))
        {
            FoodSpawner.Instance.SpawnFoodAt(GameInput.WorldPointerPosition);
        }
        if (Input.GetKey(KeyCode.K) && Random.value > 0.3f)
        {
            MapGenerator.Instance.AddPheromoneAt(GameInput.WorldPointerPosition, 0, true, 1);
        }
        if (Input.GetKey(KeyCode.L) && Random.value > 0.3f)
        {
            MapGenerator.Instance.AddPheromoneAt(GameInput.WorldPointerPosition, 0, false, 1);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            AntSpawner.Instance.SpawnAnt(GameInput.WorldPointerPosition, 0, CivilizationsManager.Instance.activeCivilizations[0], Ant.AntType.Worker);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            MapGenerator.Instance.CircleAround(MapGenerator.GridPos(GameInput.WorldPointerPosition), 5);
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            GridTile gridTile = MapGenerator.Instance.TileAt(GameInput.WorldPointerPosition);
            int civIndex = GameManager.Instance.civIndexOfCurrentlyShownPheromonesInTiles;
            Debug.Log("Type: " + gridTile.GetTileState() + "  pathpher " + gridTile.PheromoneAmount(civIndex, 0) + "  foodpher " + gridTile.PheromoneAmount(civIndex, 1) + " depletedPher " + gridTile.PheromoneAmount(civIndex, 2) + " foodamount " + gridTile.foodAmount + "  position " + gridTile.gridPos + " " + (gridTile.GetColony() ? " COLONY " : " " + " arrayIndex " + gridTile.arrayIndex + " isNearWall " + gridTile.NearWall));
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            GridTile gridTile = MapGenerator.Instance.TileAt(GameInput.WorldPointerPosition);

            Debug.Log("Type: " + gridTile.GetTileState() + "ant on " + gridTile.GetCreatureOnTile() + " " + IGridTileUpdater.Instance.antOnTileCivIndexesA[gridTile.arrayIndex]);
        }
        if (Input.GetKey(KeyCode.I)) 
        {
            Ant ant = AntsManager.Instance.existingAnts[0];
            float lowest = 100;

            for (int i = 0; i < AntsManager.Instance.existingAnts.Count; i++)
            {
                if (Vector2.Distance(AntsManager.Instance.existingAnts[i].Position, GameInput.WorldPointerPosition) < lowest)
                {
                    lowest = Vector2.Distance(AntsManager.Instance.existingAnts[i].Position, GameInput.WorldPointerPosition);
                    ant = AntsManager.Instance.existingAnts[i];
                }
            }

            ant.antBrain.DebugBehaviour();
        }
        //if (Input.GetKeyDown(KeyCode.LeftShift))
        //{
        //    GridTile gridTile = MapGenerator.Instance.TileAt(GameInput.WorldPointerPosition);
        //    Ant ant = gridTile.creatureOnTile;

        //    if (ant != null)
        //    {
        //        if (ant.battle != null)
        //        {
        //            Debug.Log("DEBUGGING ANT BATTLE");
        //            Debug.Log(ant.name);

        //            for (int j = 0; j < ant.battle.sides.Count; j++)
        //            {
        //                for (int k = 0; k < ant.battle.sides[j].Count; k++)
        //                {
        //                    Debug.Log(ant.battle.sides[j][k] != null ? ant.battle.sides[j][k].name : "NULL");
        //                }
        //            }
        //        }
        //        else
        //        {
        //            Debug.Log("BATTLE NULL");
        //        }
        //    }
        //}
        if (Input.GetKey(KeyCode.Y))
        {
            for (int i = 0; i < AntsManager.Instance.existingAnts.Count; i++)
            {
                AntsManager.Instance.existingAnts[i].antMovement.Position = GameInput.WorldPointerPosition;
            }
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            for (int i = 0; i < AntsManager.Instance.existingAnts.Count; i++)
            {
                AntsManager.Instance.existingAnts[i].antMovement.RotateToBack();
            }
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            for (int i = 0; i < AntsManager.Instance.existingAnts.Count; i++)
            {
                AntsManager.Instance.existingAnts[i].antMovement.RotateToRandomBackDir();
            }
        }
        if (Input.GetKey(KeyCode.U))
        {
            for (int i = 0; i < AntsManager.Instance.existingAnts.Count; i++)
            {
                AntsManager.Instance.existingAnts[i].transform.right = GameInput.WorldPointerPosition - (Vector2)AntsManager.Instance.existingAnts[i].transform.position;
            }
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Vector2Int p = AntsManager.Instance.GridPositionFromPosition(GameInput.WorldPointerPosition);
            Debug.Log(p);
            Debug.Log(AntsManager.Instance.listOfAntsInGrid[p.x, p.y].Count);
            Ant[] ants = AntsManager.Instance.FastAntsInCircle(GameInput.WorldPointerPosition, 10);
            for (int i = 0; i < ants.Length; i++)
            {
                Debug.Log(ants[i] + " " + ants[i].Position);
            }
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            for (int i = 0; i < AntsManager.Instance.existingAnts.Count; i++)
            {
                Ant ant = AntsManager.Instance.existingAnts[i];

                ant.antMovement.Position = ant.Civilization.colonies[Random.Range(0, ant.Civilization.colonies.Count)].PositionOfAntAtAngleFromColonyCentre(ant.transform.rotation.eulerAngles.z);
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //CivilizationsManager.Instance.activeCivilizations[0].colonies[0].TEST();
        }
    }
}
