using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PurchasableItems", menuName = "ScriptableObjects/PurchasableItems", order = 1)]
public class PurchasableItems : ScriptableObject
{
    public List<PurchasableItemInfo> items;
}