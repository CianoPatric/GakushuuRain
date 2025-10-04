using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CosmeticsCollection", menuName = "GakushuuRain/CosmeticsCollection", order = 0)]
public class CosmeticsCollection : ScriptableObject
{
    public List<CosmeticItem> allCosmeticItems;
}