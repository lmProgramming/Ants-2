using Stella3D;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using static GridTile;
using Unity.Mathematics;

public sealed class IGridTileUpdater
{
    [Unity.Burst.BurstCompile]
    private struct UpdatePheromonesJob : IJob
    {
        [ReadOnly]
        public NativeArray<int> activeIndexesArray;

        public NativeArray<float3> pheromonesArray;
        [ReadOnly]
        public NativeArray<bool> neighborables;

        [ReadOnly]
        public float mult;
        [ReadOnly]
        public int civIndexOfShownPheromones;
        [ReadOnly]
        public int tilesArrayLength;

        [ReadOnly]
        public NativeArray<Color> baseColorsArray;

        [ReadOnly]
        public Color pathPheromoneColor;
        [ReadOnly]
        public Color foodPheromoneColor;
        [ReadOnly]
        public Color depletedPheromoneColor;

        [ReadOnly]
        public bool diffuse;
        [ReadOnly]
        public float diffusionAmount;
        [ReadOnly]
        public int diffuseDir;
        [ReadOnly]
        public int mapWidth;
        [ReadOnly]
        public int mapHeight;
        [ReadOnly]
        public int borderWallLength;
        [ReadOnly]
        public float diffuseThreshold;

        [ReadOnly]
        public int startIndex;
        [ReadOnly]
        public int endIndex;
        [ReadOnly]
        public NativeArray<TileState> tileStates;
        [ReadOnly]
        public NativeArray<bool> nearWall;

        [WriteOnly]
        public NativeArray<int> civIndexOfAntOnTiles;
        [WriteOnly]
        public NativeArray<Color> size4Colors;

        public void Execute()
        {
            float minimum = 0.05f;

            int difference = endIndex - startIndex;

            for (int i = 0; i <= difference; i++)
            {
                int index = diffuseDir == 1 ? (startIndex + i) : (endIndex - i);
                int j = diffuseDir == 1 ? i : difference - i;

                if (tileStates[index] == TileState.Pheromone)
                {
                    civIndexOfAntOnTiles[index] = -1;

                    foreach (int civIndex in activeIndexesArray)
                    {
                        int arrayIndex = civIndex * tilesArrayLength + index;
                        float3 pher = pheromonesArray[arrayIndex];

                        float pathPheromone = pher.x * mult;
                        if (pathPheromone < minimum)
                        {
                            pathPheromone = 0;
                        }
                        float foodPheromone = pher.y * mult;
                        if (foodPheromone < minimum)
                        {
                            foodPheromone = 0;
                        }

                        float depletedFoodPheromone = pher.z;
                        if (depletedFoodPheromone > 0)
                        {
                            depletedFoodPheromone -= 1;
                        }

                        if (diffuse && neighborables[index])
                        {
                            // for path pheromone
                            if (pathPheromone > diffuseThreshold)
                            {
                                float amountOfDiffusion = (pathPheromone - diffuseThreshold) * diffusionAmount;

                                pathPheromone -= amountOfDiffusion;

                                float diffusionPerTile = amountOfDiffusion / 8;

                                int tmpArrayIndex = arrayIndex - 1 - mapHeight;
                                int tmpWallArrayIndex =  index - 1 - mapHeight;

                                // left column
                                if (!nearWall[tmpWallArrayIndex])
                                    pheromonesArray[tmpArrayIndex] = pheromonesArray[tmpArrayIndex].AddX(diffusionPerTile);
                                tmpArrayIndex++;
                                tmpWallArrayIndex++;
                                if (!nearWall[tmpWallArrayIndex])
                                    pheromonesArray[tmpArrayIndex] = pheromonesArray[tmpArrayIndex].AddX(diffusionPerTile);
                                tmpArrayIndex++;
                                tmpWallArrayIndex++;
                                if (!nearWall[tmpWallArrayIndex])
                                    pheromonesArray[tmpArrayIndex] = pheromonesArray[tmpArrayIndex].AddX(diffusionPerTile);

                                // centre column
                                tmpArrayIndex += mapHeight - 2;
                                tmpWallArrayIndex += mapHeight - 2;
                                if (!nearWall[tmpWallArrayIndex])
                                    pheromonesArray[tmpArrayIndex] = pheromonesArray[tmpArrayIndex].AddX(diffusionPerTile);

                                tmpArrayIndex += 2;
                                tmpWallArrayIndex += 2;
                                if (!nearWall[tmpWallArrayIndex])
                                    pheromonesArray[tmpArrayIndex] = pheromonesArray[tmpArrayIndex].AddX(diffusionPerTile);

                                //right column
                                tmpArrayIndex += mapHeight - 2;
                                tmpWallArrayIndex += mapHeight - 2;
                                if (!nearWall[tmpWallArrayIndex])
                                    pheromonesArray[tmpArrayIndex] = pheromonesArray[tmpArrayIndex].AddX(diffusionPerTile);
                                tmpArrayIndex++;
                                tmpWallArrayIndex++;
                                if (!nearWall[tmpWallArrayIndex])
                                    pheromonesArray[tmpArrayIndex] = pheromonesArray[tmpArrayIndex].AddX(diffusionPerTile);
                                tmpArrayIndex++;
                                tmpWallArrayIndex++;
                                if (!nearWall[tmpWallArrayIndex])
                                    pheromonesArray[tmpArrayIndex] = pheromonesArray[tmpArrayIndex].AddX(diffusionPerTile);
                            }
                            // for food pheromone
                            if (foodPheromone > diffuseThreshold)
                            {
                                float amountOfDiffusion = (foodPheromone - diffuseThreshold) * diffusionAmount;

                                foodPheromone -= amountOfDiffusion;

                                float diffusionPerTile = amountOfDiffusion / 8;

                                int tmpArrayIndex = arrayIndex - 1 - mapHeight;
                                int tmpWallArrayIndex  = index - 1 - mapHeight;

                                // left column
                                if (!nearWall[tmpWallArrayIndex])
                                    pheromonesArray[tmpArrayIndex] = pheromonesArray[tmpArrayIndex].AddY(diffusionPerTile);
                                tmpArrayIndex++;
                                tmpWallArrayIndex++;
                                if (!nearWall[tmpWallArrayIndex])
                                    pheromonesArray[tmpArrayIndex] = pheromonesArray[tmpArrayIndex].AddY(diffusionPerTile);
                                tmpArrayIndex++;
                                tmpWallArrayIndex++;
                                if (!nearWall[tmpWallArrayIndex])
                                    pheromonesArray[tmpArrayIndex] = pheromonesArray[tmpArrayIndex].AddY(diffusionPerTile);

                                // centre column
                                tmpArrayIndex += mapHeight - 2;
                                tmpWallArrayIndex += mapHeight - 2;
                                if (!nearWall[tmpWallArrayIndex])
                                    pheromonesArray[tmpArrayIndex] = pheromonesArray[tmpArrayIndex].AddY(diffusionPerTile);

                                tmpArrayIndex += 2;
                                tmpWallArrayIndex += 2;
                                if (!nearWall[tmpWallArrayIndex])
                                    pheromonesArray[tmpArrayIndex] = pheromonesArray[tmpArrayIndex].AddY(diffusionPerTile);

                                //right column
                                tmpArrayIndex += mapHeight - 2;
                                tmpWallArrayIndex += mapHeight - 2;
                                if (!nearWall[tmpWallArrayIndex])
                                    pheromonesArray[tmpArrayIndex] = pheromonesArray[tmpArrayIndex].AddY(diffusionPerTile);
                                tmpArrayIndex++;
                                tmpWallArrayIndex++;
                                if (!nearWall[tmpWallArrayIndex])
                                    pheromonesArray[tmpArrayIndex] = pheromonesArray[tmpArrayIndex].AddY(diffusionPerTile);
                                tmpArrayIndex++;
                                tmpWallArrayIndex++;
                                if (!nearWall[tmpWallArrayIndex])
                                    pheromonesArray[tmpArrayIndex] = pheromonesArray[tmpArrayIndex].AddY(diffusionPerTile);
                            }
                        }

                        pheromonesArray[arrayIndex] = new float3(pathPheromone, foodPheromone, depletedFoodPheromone);

                        if (civIndexOfShownPheromones == civIndex)
                        {
                            Color color = baseColorsArray[j];

                            if (depletedFoodPheromone > 0)
                            {
                                color = Color.Lerp(color, depletedPheromoneColor, 0.5f + depletedFoodPheromone / 40f);
                            }
                            else if (foodPheromone > 0)
                            {
                                if (pathPheromone > 0)
                                {
                                    color = Color.Lerp(color, Color.Lerp(pathPheromoneColor, foodPheromoneColor, foodPheromone / (pathPheromone + foodPheromone)), (pathPheromone + foodPheromone) / 20f);
                                }
                                else
                                {
                                    color = Color.Lerp(color, foodPheromoneColor, foodPheromone / 20f);
                                }
                            }
                            else if (pathPheromone > 0)
                            {
                                color = Color.Lerp(color, pathPheromoneColor, pathPheromone / 20f);
                            }

                            int index4Times = 4 * j;
                            size4Colors[index4Times] = color;
                            size4Colors[index4Times + 1] = color;
                            size4Colors[index4Times + 2] = color;
                            size4Colors[index4Times + 3] = color;
                        }
                    }
                }
            }
        }
    }

    public static IGridTileUpdater Instance;

    // pseudo NativeArray2D, uses [i][j] => [i * width + j]
    public SharedArray<Vector3, float3> pheromones;
    public NativeArray<float3> pheromonesNA;
    public Vector3[] pheromonesA;
    //GridTile[] gridTiles;

    private NativeArray<bool> neighborables;

    public SMSprite[] smSprites;

    private Color[] baseGrassTextures;

    private int arraySize;

    private NativeArray<int> activeIndexesArray;

    public SharedArray<TileState> tileStates;
    public TileState[] tileStatesA;

    public SharedArray<bool> nearWall;
    public bool[] nearWallA;

    public Colony[] colonyOnTiles;

    public SharedArray<int> antOnTileCivIndexes;
    public int[] antOnTileCivIndexesA;
    public NativeArray<int> antOnTileCivIndexesNA;

    public SharedArray<int> colonyOnTileCivIndex;
    public int[] colonyOnTileCivIndexA;
    public NativeArray<int> colonyOnTileCivIndexNA;

    //public SharedArray<int> tempTest;
    //public int[] tempTestA;
    //public NativeArray<int> tempTestNA;

    public void InitializeLists(GridTile[] gridTiles)
    {
        Instance = this;

        arraySize = gridTiles.Length;

        // 4 times because each Vector3 "2d" array is for respective ant civ 
        pheromones = new SharedArray<Vector3, float3>(arraySize * 4);
        pheromonesA = pheromones;
        pheromonesNA = pheromones;

        for (int i = 0; i < pheromones.Length; i++)
        {
            pheromonesA[i] = new Vector3();
        }

        smSprites = new SMSprite[arraySize];
        baseGrassTextures = new Color[arraySize];

        neighborables = new NativeArray<bool>(arraySize, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

        tileStates = new SharedArray<TileState>(arraySize);
        tileStatesA = tileStates;

        nearWall = new SharedArray<bool>(arraySize);
        nearWallA = nearWall;

        colonyOnTiles = new Colony[arraySize];

        antOnTileCivIndexes = new SharedArray<int>(arraySize);
        antOnTileCivIndexesA = antOnTileCivIndexes;
        antOnTileCivIndexesNA = antOnTileCivIndexes;

        colonyOnTileCivIndex = new SharedArray<int>(arraySize);
        colonyOnTileCivIndexA = colonyOnTileCivIndex;
        colonyOnTileCivIndexNA = colonyOnTileCivIndex;

        //tempTest = new SharedArray<int>(arraySize);
        //tempTestA = tempTest;
        //tempTestNA = tempTest;

        for (int i = 0; i < gridTiles.Length; i++)
        {
            GridTile tile = gridTiles[i];

            neighborables[i] = tile.NineTilesAround().Length == 9;

            smSprites[i] = gridTiles[i].GetSprite();
            baseGrassTextures[i] = gridTiles[i].baseGrassTexture;

            tileStatesA[i] = gridTiles[i].GetTileState();

            antOnTileCivIndexesA[i] = -1;
            colonyOnTileCivIndexA[i] = -1;
        }
    }

    public void ResetAntsOnTiles()
    {
        for (int i = 0; i < antOnTileCivIndexesA.Length; i++)
        {
            antOnTileCivIndexesA[i] = -1;
        }
    }

    public void UpdatePheromones(float mult, int startIndex, int endIndex, bool diffuse, float diffusionAmount, 
        int diffuseDir, int mapWidth, int mapHeight, int borderWallLength, float diffuseThreshold, SpriteManager spriteManager)
    {
        activeIndexesArray = new NativeArray<int>(CivilizationsManager.Instance.activeIndexes.Count, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        activeIndexesArray = JobUtils.GetNativeVertexArrays(activeIndexesArray, CivilizationsManager.Instance.activeIndexes.ToArray());

        NativeArray<Color> baseColors = new NativeArray<Color>(endIndex + 1 - startIndex, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

        JobUtils.GetNativeVertexArrays(baseColors, baseGrassTextures, startIndex, endIndex + 1);

        SharedArray<Color> newColorsSharedArray = new SharedArray<Color>(spriteManager.GetColors());
        NativeArray<Color> newColorsNA = newColorsSharedArray;
        //newColorsNA = JobUtils.GetNativeVertexArrays(newColorsNA, spriteManager.GetColors());
        Color[] newColorsA = newColorsSharedArray; 

        var job = new UpdatePheromonesJob()
        {
            activeIndexesArray = activeIndexesArray,

            pheromonesArray = pheromonesNA,

            mult = mult,
            civIndexOfShownPheromones = GameManager.Instance.civIndexOfCurrentlyShownPheromonesInTiles,
            tilesArrayLength = smSprites.Length,
            pathPheromoneColor = MapGenerator.pathPherColor,
            foodPheromoneColor = MapGenerator.foodPherColor,
            depletedPheromoneColor = MapGenerator.depletedFoodPherColor,
            baseColorsArray = baseColors,
            neighborables = neighborables,
            diffuse = diffuse,
            diffusionAmount = diffusionAmount,
            diffuseDir = diffuseDir,
            mapWidth = mapWidth,
            mapHeight = mapHeight,
            borderWallLength = borderWallLength,
            diffuseThreshold = diffuseThreshold,
            startIndex = startIndex,
            endIndex = endIndex,
            tileStates = tileStates,
            nearWall = nearWall,
            civIndexOfAntOnTiles = antOnTileCivIndexesNA,
            size4Colors = newColorsNA
        };

        JobHandle sheduleJobDependency = new JobHandle();
        JobHandle jobHandle = job.Schedule(sheduleJobDependency);

        jobHandle.Complete();

        spriteManager.ModifyColorArrayDirectly(newColorsA);

        baseColors.Dispose();
        activeIndexesArray.Dispose();
        newColorsSharedArray.Dispose();
    }

    public void AddPher(Vector2Int gridPos, int civIndex, int pheromoneIndex, float amount)
    {
        int arrayIndex = Cord.TileIndexFromPos(gridPos);

        AddPheromone(arrayIndex, civIndex, pheromoneIndex, amount);
    }

    public void AddPheromone(int arrayIndex, int civIndex, int pheromoneIndex, float amount)
    {
        Vector3 pher = pheromonesA[civIndex * arraySize + arrayIndex];
        if (pheromoneIndex == PATH_PHEROMONE)
        {
            pher.x += amount;
        }
        else if (pheromoneIndex == FOOD_PHEROMONE)
        {
            pher.y += amount;
        }
        else
        {
            pher.z += amount;
        }

        pheromonesA[civIndex * arraySize + arrayIndex] = pher;
    }

    public void UpdateAllPheromonesSpritesManually(int civIndex)
    {
        for (int i = 0; i < arraySize; i++)
        {
            UpdatePheromoneSpriteManuallyAssumeRightCivIndex(i, civIndex);
        }
    }

    public void UpdatePheromoneSpriteManuallyAssumeRightCivIndex(int arrayIndex, int civIndex)
    {
        Vector3 pher = GetPheromoneAt(arrayIndex, civIndex);

        float pathPheromone = pher.x;
        float foodPheromone = pher.y;
        float depletedFoodPheromone = pher.z;

        if (tileStatesA[arrayIndex] == TileState.Pheromone)
        {
            Color color = baseGrassTextures[arrayIndex];

            if (depletedFoodPheromone > 0)
            {
                color = Color.Lerp(color, MapGenerator.depletedFoodPherColor, 0.5f + depletedFoodPheromone / 40f);
            }
            else if (foodPheromone > 0)
            {
                if (pathPheromone > 0)
                {
                    color = Color.Lerp(color, Color.Lerp(MapGenerator.pathPherColor, MapGenerator.foodPherColor, foodPheromone / (pathPheromone + foodPheromone)), (pathPheromone + foodPheromone) / 20f);
                }
                else
                {
                    color = Color.Lerp(color, MapGenerator.foodPherColor, foodPheromone / 20f);
                }
            }
            else if (pathPheromone > 0)
            {
                color = Color.Lerp(color, MapGenerator.pathPherColor, pathPheromone / 20f);
            }

            smSprites[arrayIndex].SetColor(color);
        }
    }

    public void UpdatePheromoneSpriteManually(int arrayIndex, int civIndex)
    {
        Vector3 pher = GetPheromoneAt(arrayIndex, civIndex);

        float pathPheromone = pher.x;
        float foodPheromone = pher.y;
        float depletedFoodPheromone = pher.z;

        if (civIndex >= 0)
        {
            Color color = baseGrassTextures[arrayIndex];

            if (depletedFoodPheromone > 0)
            {
                color = Color.Lerp(color, MapGenerator.depletedFoodPherColor, 0.5f + depletedFoodPheromone / 40f);
            }
            else if (foodPheromone > 0)
            {
                if (pathPheromone > 0)
                {
                    color = Color.Lerp(color, Color.Lerp(MapGenerator.pathPherColor, MapGenerator.foodPherColor, foodPheromone / (pathPheromone + foodPheromone)), (pathPheromone + foodPheromone) / 20f);
                }
                else
                {
                    color = Color.Lerp(color, MapGenerator.foodPherColor, foodPheromone / 20f);
                }
            }
            else if (pathPheromone > 0)
            {
                color = Color.Lerp(color, MapGenerator.pathPherColor, pathPheromone / 20f);
            }

            smSprites[arrayIndex].SetColor(color);
        }
    }

    public void SetPheromone(int arrayIndex, int civIndex, int pheromoneIndex, float amount)
    {
        int index = civIndex * arraySize + arrayIndex;
        Vector3 pher = pheromonesA[index];

        if (pheromoneIndex == PATH_PHEROMONE)
        {
            pher.x = amount;
        }
        else if (pheromoneIndex == FOOD_PHEROMONE)
        {
            pher.y = amount;
        }
        else
        {
            pher.z = amount;
        }

        pheromonesA[index] = pher;
    }

    public void ResetPheromone(int arrayIndex)
    {
        for (int i = 0; i < 4; i++)
        {
            pheromonesA[i * arraySize + arrayIndex] = new Vector3(0, 0, 0);
        }
    }

    public Vector3 GetPheromoneAt(int arrayIndex, int civIndex)
    {
        return pheromonesA[civIndex * arraySize + arrayIndex];
    }

    public float GetPheromoneAt(int arrayIndex, int civIndex, int pheromoneIndex)
    {
        if (pheromoneIndex == PATH_PHEROMONE)
        {
            return pheromonesA[civIndex * arraySize + arrayIndex].x;
        }
        else if (pheromoneIndex == FOOD_PHEROMONE)
        {
            return pheromonesA[civIndex * arraySize + arrayIndex].y;
        }
        else
        {
            return pheromonesA[civIndex * arraySize + arrayIndex].z;
        }
    }

    public void MultiplyFoodPheromonePheromones(int arrayIndex, float mult, int civIndex)
    {
        pheromonesA[civIndex * arraySize + arrayIndex].y *= mult;
    }

    public void MultiplyPheromones(int arrayIndex, float mult, int civIndex)
    {
        pheromonesA[civIndex * arraySize + arrayIndex] *= mult;
    }

    public Vector3 GetPheromonesAt(int arrayIndex, int civIndex)
    {
        return pheromonesA[civIndex * arraySize + arrayIndex];
    }

    public void DisposeOfArrays()
    {
        pheromones.Dispose();

        neighborables.Dispose();       
    }
}
