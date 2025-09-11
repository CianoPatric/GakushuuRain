using System.Collections.Generic;
using UnityEngine;

public class CosmeticsLibraryManager: MonoBehaviour
{
    public static CosmeticsLibraryManager Instance {get; private set;}
    public List<CosmeticItem> allCosmeticItems;

    private Dictionary<string, CosmeticItem> cosmeticLibrary = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (var item in allCosmeticItems)
        {
            if (!cosmeticLibrary.ContainsKey(item.id))
            {
                cosmeticLibrary.Add(item.id, item);
            }
        }
    }

    public CosmeticItem GetCosmeticItem(string id)
    {
        if (cosmeticLibrary.ContainsKey(id))
        {
            return cosmeticLibrary[id];
        }
        Debug.Log("Cosmetics Library Not Found: " + id);
        return null;
    }
}