using System.Collections.Generic;
using UnityEngine;

// Since Unity Meshes take up to 65535 vertices, we need more meshes for a big map (bigger than 127x127)
public sealed class SpriteManagerCreator : MonoBehaviour
{
    public static SpriteManagerCreator Instance;

    public List<SpriteManager> spriteManagers;
    public Transform SpriteManagersHolder;
    public GameObject prefabSMManager;

    public SpriteManager antSpriteManager;
    // approximate buffer
    private int defaultBuffer = 16128;
    private int lastSpriteManagerRemainingBuffer = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        lastSpriteManagerRemainingBuffer = 0;
    }

    public SpriteManager AssignSpriteManager(int tileIndex, int vertices = 4)
    {
        lastSpriteManagerRemainingBuffer -= vertices;
        if (lastSpriteManagerRemainingBuffer < 0)
        {
            lastSpriteManagerRemainingBuffer = defaultBuffer - vertices;

            GameObject SMManagerNewObject = Instantiate(prefabSMManager, Vector3.zero, Quaternion.identity, SpriteManagersHolder);

            SpriteManager spriteManager = SMManagerNewObject.GetComponent<SpriteManager>();

            spriteManagers.Add(spriteManager);

            int remainingTiles = MapGenerator.mapHeight * MapGenerator.mapWidth - tileIndex;

            if (remainingTiles >= defaultBuffer / 4)
            {
                spriteManager.allocBlockSize = (defaultBuffer) / 4;
            }
            else
            {
                spriteManager.allocBlockSize = (remainingTiles);
            }

            spriteManagers[spriteManagers.Count - 1].indexMin = tileIndex;
        }

        spriteManagers[spriteManagers.Count - 1].indexMax = tileIndex;

        return spriteManagers[spriteManagers.Count - 1];
    }
}
