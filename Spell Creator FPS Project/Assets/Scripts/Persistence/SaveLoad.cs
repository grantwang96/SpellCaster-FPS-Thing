using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoad {

    private static readonly string SavePath = Application.persistentDataPath + "/savedGames.gd";
    
    public static void Save(Game toBeSaved) {
        PersistFile(toBeSaved);
    }

    private static void PersistFile(Game toBeSaved) {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(SavePath);
        bf.Serialize(file, toBeSaved);
        file.Close();
    }

    public static Game ReadFromDisk() {
        if(File.Exists(SavePath)) {
            Debug.Log("Loading game from disk...");
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(SavePath, FileMode.Open);
            Game game = (Game)bf.Deserialize(file);
            file.Close();
            return game;
        }
        Debug.Log("Save file not found. Creating new game...");
        return new Game();
    }

    public static void ClearDisk() {
        if (File.Exists(SavePath)) {
            File.Delete(SavePath);
        }
    }
}
