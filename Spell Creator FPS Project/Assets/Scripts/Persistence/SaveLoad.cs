using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoad {

    private static readonly string SavePath = Application.persistentDataPath + "/savedGames.gd";

    private static List<GameSave> _games = new List<GameSave>();
    
    public static void Save(GameSave toBeSaved) {
        PersistFile(toBeSaved);
    }

    private static void PersistFile(GameSave toBeSaved) {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(SavePath);
        bf.Serialize(file, toBeSaved);
        file.Close();
    }

    public static GameSave ReadFromDisk() {
        if(File.Exists(SavePath)) {
            Debug.Log("Loading game from disk...");
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(SavePath, FileMode.Open);
            GameSave game = (GameSave)bf.Deserialize(file);
            file.Close();
            return game;
        }
        Debug.Log("Save file not found. Creating new game...");
        return new GameSave();
    }

    public static void ClearDisk() {
        if (File.Exists(SavePath)) {
            File.Delete(SavePath);
        }
    }
}
