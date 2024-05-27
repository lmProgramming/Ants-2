using UnityEngine;

public static class Cord
{
    // returns a random pos within tile, only supposed for transform pos
    public static Vector2 RandomPosInTile(Vector2 pos, float changeMult = 0.5f)
    {
        return new Vector2(pos.x + Random.Range(-changeMult, changeMult), pos.y + Random.Range(-changeMult, changeMult));
    }

    // returns a random pos within tile, only supposed for transform pos
    public static Vector3 RandomPosInTile(Vector3 pos)
    {
        return new Vector3(pos.x += Random.Range(-0.5f, 0.5f), pos.y += Random.Range(-0.5f, 0.5f), pos.z);
    }

    public static int TileIndexFromPos(Vector2 pos)
    {
        return (int)pos.x + (int)pos.y * MapGenerator.mapWidth;
    }

    public static int TileIndexFromPos(Vector2Int pos)
    {
        return pos.x + pos.y * MapGenerator.mapWidth;
    }

    public static Vector2Int ToVecInt(Vector2 pos)
    {
        return new Vector2Int((int)pos.x, (int)pos.y);
    }

    public static Vector3 ToVec3(Vector2 pos, float zPos = 0)
    {
        return new Vector3(pos.x, pos.y, zPos);
    }

    public static Vector3 ToVec3(Vector2Int pos, float zPos = 0)
    {
        return new Vector3(pos.x, pos.y, zPos);
    }

    public static Vector2 ToVec2(Vector3 pos)
    {
        return new Vector2(pos.x, pos.y);
    }
}
