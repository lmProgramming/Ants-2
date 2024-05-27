using System.IO;
using UnityEngine;

public static class SaveGame
{
    public static void Save(string filename, GameDataHolder data)
    {
        filename = AddExtensionIfNeeded(filename);

        FileManager.Save(filename, data);
    }

    public static GameDataHolder Load(string filename)
    {
        filename = AddExtensionIfNeeded(filename);

        return FileManager.Load<GameDataHolder>(filename);
    }

    public static void Clear(string filename)
    {
        filename = AddExtensionIfNeeded(filename);

        File.Delete(Path.Combine(Application.persistentDataPath, filename));
    }

    public static string AddExtensionIfNeeded(string filename)
    {
        if (filename.Length < 5 || filename.Substring(filename.Length - 5, 5) != ".json")
        {
            filename += ".json";
        }

        return filename;
    }
}