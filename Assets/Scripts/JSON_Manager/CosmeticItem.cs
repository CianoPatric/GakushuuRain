using System;
using UnityEngine;

[Serializable]
public class CosmeticItem
{
    public string id;
    public string display_name;
    public CosmeticSlot slot;
    public Sprite sprite;
}

public enum CosmeticSlot
{
    Hat,
    Shirt,
    Pants,
    Accessory
}