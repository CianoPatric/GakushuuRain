using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public static class LocalSaveManager
{
    private const string PROFILE_FILE_PREFIX = "player_profile_";
    private const string AUTH_CACHE_FILE_NAME = "auth_cache.json";

    public static void SaveProfile(PlayerData data, int slotIndex)
    {
        string fileName = $"{PROFILE_FILE_PREFIX}{slotIndex}.json";
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        string path = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllText(path, json);
        Debug.Log($"Профиль для слота {slotIndex} сохранён локально по пути {path}");
    }

    public static PlayerData LoadProfile(int slotIndex)
    {
        string fileName = $"{PROFILE_FILE_PREFIX}{slotIndex}.json";
        string path = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<PlayerData>(json);
        }
        return null;
    }

    public static void DeleteProfile(int slotIndex)
    {
        string fileName = $"{PROFILE_FILE_PREFIX}{slotIndex}.json";
        string path = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"Локальный профиль для слота {slotIndex} удалён.");
        }
    }

    public static void SaveAuthCache(AuthCache cache)
    {
        string json = JsonUtility.ToJson(cache, true);
        string path = Path.Combine(Application.persistentDataPath, AUTH_CACHE_FILE_NAME);
        File.WriteAllText(path, json);
    }

    public static AuthCache LoadAuthCache()
    {
        string path = Path.Combine(Application.persistentDataPath, AUTH_CACHE_FILE_NAME);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<AuthCache>(json);
        }
        return null;
    }
}