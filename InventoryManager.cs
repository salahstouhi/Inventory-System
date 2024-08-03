using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance = null;
    private void Awake()
    {
        if (!instance)
            instance = this;
        else Destroy(gameObject);
    }
    /// <summary>
    ///List of items that are at the start of the game. Used to create Item References. 
    /// </summary>
    [SerializeField] private List<InventoryItem> BaseItems;
    /// <summary>
    /// The List that will be used by the game to access the inventory. Contains the items that are instantiated according 
    /// to the references found in the json.
    /// </summary>
    [SerializeField] private List<InventoryItem> InventoryItems = new();
    private void Start()
    {
        InitInventory();
    }
    /// <summary>
    /// Initialize the Inventory by Creating references, if there isnt any, from the base items,
    /// or by instantiating the ones found in the references.
    /// </summary>
    private void InitInventory()
    {
        var reflist = ItemReferenceManager.instance.LoadData();
        if (reflist.Count == 0)
        {
            foreach (var item in BaseItems)
            {
                var itemref = ItemReferenceManager.instance.CreateItemRef(item);
                InstantiateItem(item, itemref);
            }
        }
        else
            foreach (var itemRef in reflist)
            {
                foreach (var baseItem in BaseItems)
                {
                    if (itemRef.Name == baseItem.Name)
                        InstantiateItem(baseItem, itemRef);
                }
            }
    }
    private void InstantiateItem(InventoryItem baseItem, ItemReference itemRef)
    {
        var itemInstant = baseItem.InstantiateItem(itemRef);
        InventoryItems.Add(itemInstant);
    }
    public void UpgradeItem(InventoryItem item)
    {
        ItemReferenceManager.instance.UpgradeItemRef(item, item.Level + 1);
        item.UpgradeItem(item, item.Level + 1);
    }
    public void DeleteItem(InventoryItem item)
    {
        InventoryItems.Remove(item);
        ItemReferenceManager.instance.DeleteItem(item);
    }
}

