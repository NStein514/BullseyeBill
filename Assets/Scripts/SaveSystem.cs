using UnityEngine;
using System.IO;
using UnityEditor.Overlays;

public class SaveSystem : MonoBehaviour
{
    private static string saveFileName = "saveData.json";

    public static void SaveGame(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        string fullPath = Path.Combine(Application.persistentDataPath, saveFileName);
        File.WriteAllText(fullPath, json);
        Debug.Log("Game saved to: " + fullPath);
    }

    public static SaveData LoadGame()
    {
        string fullPath = Path.Combine(Application.persistentDataPath, saveFileName);

        if (File.Exists(fullPath))
        {
            string json = File.ReadAllText(fullPath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("Game loaded from: " + fullPath);
            return data;
        }
        else
        {
            Debug.LogWarning("Save file not found. Returning new SaveData.");
            return new SaveData();
        }
    }

    public static void DeleteSave()
    {
        string fullPath = Path.Combine(Application.persistentDataPath, saveFileName);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            Debug.Log("Save file deleted."); 
        }
    }
}
