using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerrainManipulator : MonoBehaviour
{
    public TerrainManipulatorOptionUI.TerrainOption selectedTerrainOption;

    public int brushSize = 3;
    public SizeOption.Shape brushShape = SizeOption.Shape.Circle;
    public float brushIntensity = 1;

    public void ManipulateTerrain(TerrainManipulatorOptionUI.TerrainOption option, Vector2Int gridPosition, Vector2Int lastGridPosPlaced)
    {
        selectedTerrainOption = option;

        switch (option)
        {
            case TerrainManipulatorOptionUI.TerrainOption.Wall:
                CreateWalls(gridPosition, lastGridPosPlaced);
                break;
            case TerrainManipulatorOptionUI.TerrainOption.Food:
                CreateFood(gridPosition, lastGridPosPlaced);
                break;
            case TerrainManipulatorOptionUI.TerrainOption.Erase:
                Erase(gridPosition, lastGridPosPlaced);
                break;
            case TerrainManipulatorOptionUI.TerrainOption.Pheromone:
                if (GameManager.Instance.civIndexOfCurrentlyShownPheromonesInTiles >= 0)
                {
                    CreatePheromones(gridPosition, lastGridPosPlaced);
                }
                else { MainGameHelpUI.Instance.ShowAttemptedSpawnNoCivIndexSelected(); }
                break;
            case TerrainManipulatorOptionUI.TerrainOption.Water:
                CreateWater(gridPosition, lastGridPosPlaced);
                break;
            default:
                throw new ArgumentException();
        }
    }

    private void CreateWalls(Vector2Int gridPosition, Vector2Int lastGridPosition)
    {
        GridTile[] tiles = GetGridTilesFromPointerMovement(gridPosition, lastGridPosition, brushSize, brushShape);
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].ChangeToWall();
        }

        RemoveEntitiesInsideTiles(tiles, true, true, true);
    }

    private void CreateWater(Vector2Int gridPosition, Vector2Int lastGridPosition)
    {
        GridTile[] tiles = GetGridTilesFromPointerMovement(gridPosition, lastGridPosition, brushSize, brushShape);
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].ChangeToWater();
        }

        RemoveEntitiesInsideTiles(tiles, removeColonies: true);
    }

    private void CreateFood(Vector2Int gridPosition, Vector2Int lastGridPosition)
    {
        GridTile[] tiles = GetGridTilesFromPointerMovement(gridPosition, lastGridPosition, brushSize, brushShape);

        for (int i = 0; i < tiles.Length; i++)
        {
            if (!tiles[i].GetColony())
            {
                if (tiles[i].GetTileState() == TileState.Food)
                {
                    tiles[i].AddFood(800 * Time.unscaledDeltaTime * brushIntensity);
                }
                else if (tiles[i].GetTileState() == TileState.Pheromone)
                {
                    tiles[i].ChangeToFood(UnityEngine.Random.Range(50, 100) * brushIntensity);
                }
            }
            else
            {
                tiles[i].GetColony().DepositFood(8 * Time.unscaledDeltaTime * brushIntensity);
            }
        }
    }

    private void Erase(Vector2Int gridPosition, Vector2Int lastGridPosition)
    {
        GridTile[] tiles = GetGridTilesFromPointerMovement(gridPosition, lastGridPosition, brushSize, brushShape);
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].ChangeToPheromone();
        }
    }

    private void CreatePheromones(Vector2Int gridPosition, Vector2Int lastGridPosition)
    {
        GridTile[] tiles = GetGridTilesFromPointerMovement(gridPosition, lastGridPosition, brushSize, brushShape);

        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i].GetTileState() == TileState.Pheromone)
            {
                tiles[i].AddFoodPheromone(GameManager.Instance.civIndexOfCurrentlyShownPheromonesInTiles, 1);
                tiles[i].AddPathPheromone(GameManager.Instance.civIndexOfCurrentlyShownPheromonesInTiles, 1);
                tiles[i].UpdateSprite();
            }
        }
    }

    public GridTile[] GetGridTilesFromPointerMovement(Vector2Int gridPosition, Vector2Int lastGridPosPlaced, int brushSize, SizeOption.Shape brushShape)
    {
        GridTile[] tiles;

        // List<Ant> antsInsideCircles = new List<Ant>();
        // todo improve maybe some more
        if (!gridPosition.Equals(lastGridPosPlaced))
        {
            HashSet<GridTile> tilesUnique = new HashSet<GridTile>();

            Vector2Int[] tilesPassed = MathExt.FindGridIntersections(lastGridPosPlaced, ((Vector2)(lastGridPosPlaced - gridPosition)).normalized, (lastGridPosPlaced - gridPosition).magnitude).ToArray();

            for (int j = 0; j < tilesPassed.Length; j++)
            {
                tilesUnique.UnionWith(GetGridTileInShapeAround(gridPosition + tilesPassed[j], brushSize, brushShape));
            }

            tiles = tilesUnique.ToArray();
        }
        else
        {
            tiles = GetGridTileInShapeAround(gridPosition, brushSize, brushShape);
        }

        return tiles.ToArray();
    }

    public GridTile[] GetGridTileInShapeAround(Vector2Int gridPosition, int brushSize, SizeOption.Shape brushShape)
    {
        return GetGridTileInShapeAround(gridPosition, brushSize, brushShape, brushIntensity);
    }

    public GridTile[] GetGridTileInShapeAround(Vector2Int gridPosition, int brushSize, SizeOption.Shape brushShape, float brushIntensity)
    {
        switch (brushShape)
        {
            case SizeOption.Shape.Circle:
                return MapGenerator.Instance.CircleAroundExcludingMapBorderWalls(gridPosition, brushSize);
            case SizeOption.Shape.Rect:
                return MapGenerator.Instance.GetXTilesInRectFromCentreGridPosExcludingMapBorderWalls(gridPosition, brushSize);
            case SizeOption.Shape.Scatter:
                return MapGenerator.Instance.ScatterAroundExcludingMapBorderWalls(gridPosition, brushSize, brushIntensity / 2);
            default:
                return null;
        }
    }

    public void RemoveEntitiesInsideTiles(GridTile[] tiles, bool removeColonies=false, bool removeAnts=false, bool removeCreatures=false)
    {
        if (!removeColonies && !removeAnts && !removeCreatures)
        {
            Debug.LogWarning("Not deleting anything, pointless");
        }

        Vector2Int minGridPos = new Vector2Int(int.MaxValue, int.MaxValue);
        Vector2Int maxGridPos = new Vector2Int(int.MinValue, int.MinValue);
        
        // TODO to slow
        
        if (removeColonies)
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                minGridPos = new Vector2Int(Mathf.Min(minGridPos.x, tiles[i].gridPos.x), Mathf.Min(minGridPos.y, tiles[i].gridPos.y));
                maxGridPos = new Vector2Int(Mathf.Max(maxGridPos.x, tiles[i].gridPos.x), Mathf.Max(maxGridPos.y, tiles[i].gridPos.y));

                if (tiles[i].GetColony() != null)
                {
                    tiles[i].GetColony().GetDestroyed();
                }
            }
        }

        if (removeAnts)
        {
            for (int j = 0; j < AntsManager.Instance.existingAnts.Count; j++)
            {
                Ant ant = AntsManager.Instance.existingAnts[j];
                GridTile antGridTile = MapGenerator.Instance.TileAt(ant.Position);

                if (antGridTile.gridPos.x < minGridPos.x || antGridTile.gridPos.y < minGridPos.y ||
                    antGridTile.gridPos.x > maxGridPos.x || antGridTile.gridPos.y > maxGridPos.y) continue;
                
                for (int i = 0; i < tiles.Length; i++)
                {
                    if (tiles[i] == antGridTile)
                    {
                        ant.Die();
                        j--;
                        break;
                    }
                }
            }
        }

        if (removeCreatures)
        {
            for (int j = 0; j < CreatureManager.Instance.existingCreatures.Count; j++)
            {
                Creature creature = CreatureManager.Instance.existingCreatures[j];
                GridTile antGridTile = MapGenerator.Instance.TileAt(creature.Position);

                if (antGridTile.gridPos.x < minGridPos.x || antGridTile.gridPos.y < minGridPos.y ||
                    antGridTile.gridPos.x > maxGridPos.x || antGridTile.gridPos.y > maxGridPos.y) continue;
                for (int i = 0; i < tiles.Length; i++)
                {
                    if (tiles[i] == antGridTile)
                    {
                        creature.Die();
                        j--;
                        break;
                    }
                }
            }
        }
    }

    public void ChangeBrushSize(int newBrushSize)
    {
        brushSize = newBrushSize;
    }

    internal void ChangeBrushType(SizeOption.Shape shape)
    {
        brushShape = shape;
    }
}
