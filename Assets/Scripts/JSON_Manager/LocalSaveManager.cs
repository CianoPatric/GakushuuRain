using System.IO;
using UnityEngine;

public static class LocalSaveManager
{
    private const string PROFILE_FILE_NAME = "player_profile.json";
    private const string AUTH_CACHE_FILE_NAME = "auth_cache.json";

    public static void SaveProfile(PlayerData data)
    {
        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(Application.persistentDataPath, PROFILE_FILE_NAME);
        File.WriteAllText(path, json);
        Debug.Log($"Профиль сохранён локально по пути {path}");
    }

    public static PlayerData LoadProfile()
    {
        string path = Path.Combine(Application.persistentDataPath, PROFILE_FILE_NAME);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<PlayerData>(json);
        }
        return null;
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