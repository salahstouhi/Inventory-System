using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
/// <summary>
/// Handles the process of storing the references of inventory items in json
/// </summary>
public class ItemReferenceManager : MonoBehaviour
{
    public static ItemReferenceManager instance = null;
    private void Awake()
    {
        if (!instance)
            instance = this;
        else Destroy(gameObject);
    }
    private string jsonPath = "item.json";
    /// <summary>
    /// Creates an Item Reference from a given inventory item and assigns its Name and Level
    /// </summary>
    public ItemReference CreateItemRef(InventoryItem item)
    {
        var itemRef = item.CreateReference();
        SaveData(item, itemRef);
        return itemRef;
    }
    /// <summary>
    /// Saves the Item Reference into its respective json file
    /// </summary>
    private void SaveData(InventoryItem item, ItemReference itemRef)
    {
        var path = Path.Combine(Application.persistentDataPath, jsonPath);

        string jsonData = JsonUtility.ToJson(itemRef);
        string appendedJsonData = jsonData + "\n";

        if (File.Exists(path))
        {
            string existingData = File.ReadAllText(path);
            string updatedData = existingData + appendedJsonData;

            File.WriteAllText(path, updatedData);
        }
        else
        {
            Debug.Log("saved json");
            File.WriteAllText(path, appendedJsonData);
        }
    }
    /// <summary>
    /// Returns a list of Item References from a json file depending on the given type. If the file does not exist, then 
    /// it returns a new list of Item References
    /// </summary>
    public List<ItemReference> LoadData()
    {
        var path = Path.Combine(Application.persistentDataPath, jsonPath);
        if (File.Exists(path))
        {
            string jsonData = File.ReadAllText(path);
            var jsonArray = jsonData.Split("\n");
            List<ItemReference> dataList = new();
            foreach (var json in jsonArray)
            {
                if (json.Length != 0)
                {
                    var loadedData = new ItemReference { };
                    JsonUtility.FromJsonOverwrite(json, loadedData);
                    dataList.Add(loadedData);
                }
            }
            return dataList;
        }
        else
        {
            Debug.LogWarning("Save file does not exist at path: " + path);
            return new List<ItemReference>();
        }

    }
    /// <summary>
    /// Updates the reference to an Inventory Item in json. 
    /// Checks if the reference is in the file and updates the value of its level.
    /// </summary>
    public void UpgradeItemRef(InventoryItem item, int newLevel)
    {
        var refslist = LoadData();
        for (int i = 0; i < refslist.Count; i++)
        {
            if (IsInRefList(refslist[i], item))
            {
                refslist[i].Level = newLevel.ToString();
                UpdateItemInJson(refslist);
                break;
            }
        }

    }
    /// <summary>
    /// Updates the json file after receiving new info
    /// </summary>
    private void UpdateItemInJson(List<ItemReference> list)
    {
        var path = Path.Combine(Application.persistentDataPath, jsonPath);
        string jsonData = string.Join("\n", list.Select(item => JsonUtility.ToJson(item))) + "\n";
        File.WriteAllText(path, jsonData);
    }
    /// <summary>
    /// Deletes an Item Reference from the json file after checking if it exists.
    /// </summary>
    /// <param name="item"></param>
    public void DeleteItem(InventoryItem item)
    {
        var refslist = LoadData();
        for (int i = 0; i < refslist.Count; i++)
        {
            if (IsInRefList(refslist[i], item))
            {
                refslist.RemoveAt(i);
                UpdateItemInJson(refslist);
                break;
            }
        }
    }
    /// <summary>
    /// Checks whether a reference is in the list of references found in the json file by 
    /// comparing the name and level
    /// </summary>
    public bool IsInRefList(ItemReference reference, InventoryItem item)
    {
        bool sameName = reference.Name == item.Name;
        bool sameLevel = int.Parse(reference.Level) == item.Level;
        return sameName && sameLevel; ;
    }
}
/// <summary>
/// Class used to store the info of an inventory item in a json file
/// </summary>
[System.Serializable]
public class ItemReference
{
    public string Name;
    public string Level;

}