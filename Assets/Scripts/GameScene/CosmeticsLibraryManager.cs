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
        if(string.IsNullOrEmpty(id)) return null;
        
        if (cosmeticLibrary.ContainsKey(id))
        {
            return cosmeticLibrary[id];
        }
        Debug.LogWarning("Косметика не найдена, id предмета: " + id);
        return null;
    }
}