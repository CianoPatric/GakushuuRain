using System.Collections.Generic;
using UnityEngine;

public class CosmeticsLibraryManager: MonoBehaviour
{
    public List<CosmeticItem> AllCosmeticItems {get; private set;}
    private Dictionary<string, CosmeticItem> _cosmeticLibrary = new();

    void Awake()
    {
        var collection = Resources.Load<CosmeticsCollection>("PlayerCosmetics");
        if (collection == null)
        {
            Debug.LogError("CosmeticsCollection not found");
            AllCosmeticItems = new List<CosmeticItem>();
            return;
        }

        AllCosmeticItems = collection.allCosmeticItems;
        foreach (var item in AllCosmeticItems)
        {
            if (item != null && !_cosmeticLibrary.ContainsKey(item.id))
            {
                _cosmeticLibrary.Add(item.id, item);
            }
        }
    }

    public CosmeticItem GetCosmeticItem(string id)
    {
        if(string.IsNullOrEmpty(id)) return null;
        
        if (_cosmeticLibrary.ContainsKey(id))
        {
            return _cosmeticLibrary[id];
        }
        Debug.LogWarning("Косметика не найдена, id предмета: " + id);
        return null;
    }
}