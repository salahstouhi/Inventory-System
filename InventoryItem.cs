using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "InventoryItem")]
public class InventoryItem : ScriptableObject

{
    public string Name;
    public int Level = 1;
    public ItemReference CreateReference()
    {
        var itemRef = new ItemReference
        {
            Name = Name,
            Level = Level.ToString(),
        };
        return itemRef;
    }
    public InventoryItem InstantiateItem(ItemReference itemRef)
    {
        var item = Instantiate(this);
        item.Name = itemRef.Name;
        item.Level = int.Parse(itemRef.Level);
        return item;
    }
    public void UpgradeItem(InventoryItem itemInstant, int newLevel)
    {
        itemInstant.Level = newLevel;
    }
}

