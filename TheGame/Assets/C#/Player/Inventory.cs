using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int maxSize = 24; // Maksimum antal items i inventory
    public List<int> itemIDs = new List<int>();

    // Event for inventory ændringer
    //public event Action OnInventoryChanged;
  

    public delegate void OnInventoryChangedHandler();
    public event OnInventoryChangedHandler OnInventoryChanged;
    // Tilføj et item til inventory
    void Start()
    {
        // Initialiser inventory med tomme slots
        for (int i = 0; i < maxSize; i++)
        {
            itemIDs.Add(-1);
        }
    }
    public int ItemCount
    {
        get { return itemIDs.Count(id => id != -1); }
    }
    public bool AddItem(int itemID)
    {
        // Find den første tomme slot
        int emptySlotIndex = itemIDs.IndexOf(-1);
        if (emptySlotIndex != -1)
        {
            itemIDs[emptySlotIndex] = itemID;
            //Debug.Log($"ItemID {itemID} tilføjet til slot {emptySlotIndex}");
            OnInventoryChanged?.Invoke();
            return true;
        }
        else
        {
            //Debug.LogWarning("Inventory er fuld. Kan ikke tilføje item.");
            return false;
        }
    }

    // Fjern et item fra inventory
    public void RemoveItem(int itemID)
    {
        if (itemIDs.Contains(itemID))
        {
            itemIDs.Remove(itemID);
            //Debug.Log("Removed item ID: " + itemID);
            OnInventoryChanged?.Invoke();
        }
        else
        {
            //Debug.LogWarning("Attempted to remove an item that doesn't exist in inventory.");
        }
    }

    // Hent itemID ved index
    public int GetItemAtIndex(int index)
    {
        if (index >= 0 && index < itemIDs.Count)
        {
            return itemIDs[index];
        }
        else
        {
            //Debug.LogWarning($"Kan ikke få item fra slot {index}. Ugyldigt index.");
            return -1;
        }
    }

    // Hent alle våben i inventory
    public List<int> GetAllWeaponIDs()
    {
        // Antag at alle våben har et weaponPrefab i ItemManager
        ItemManager itemManager = FindObjectOfType<ItemManager>();
        if (itemManager == null)
        {
            //Debug.LogError("No ItemManager found in the scene.");
            return new List<int>();
        }

        List<int> weaponIDs = new List<int>();
        foreach (int id in itemIDs)
        {
            if (itemManager.GetWeaponPrefabForItem(id) != null)
            {
                weaponIDs.Add(id);
            }
        }
        return weaponIDs;
    }
    public void RemoveItemAtIndex(int index)
    {
        if (index >= 0 && index < itemIDs.Count && itemIDs[index] != -1)
        {
            //Debug.Log($"Fjerner itemID {itemIDs[index]} fra slot {index}");
            itemIDs[index] = -1;
            OnInventoryChanged?.Invoke();
        }
        else
        {
            //Debug.LogWarning($"Kan ikke fjerne item fra slot {index}. Ugyldigt index eller allerede tomt.");
        }
    }

    public bool AddItems(List<int> newItemIDs)
    {
        foreach (int itemID in newItemIDs)
        {
            bool added = AddItem(itemID);
            if (!added)
            {
                //Debug.LogWarning("Nogle items kunne ikke tilføjes, da inventory er fuld.");
                return false;
            }
        }
        return true;
    }
    public bool SetItemAtIndex(int index, int itemID)
    {
        if (index >= 0 && index < itemIDs.Count)
        {
            if (itemIDs[index] == -1 || itemIDs[index] != itemID)
            {
                itemIDs[index] = itemID;
                //Debug.Log($"Satte itemID {itemID} i slot {index}");
                OnInventoryChanged?.Invoke();
                return true;
            }
            else
            {
                //Debug.LogWarning($"Slot {index} allerede indeholder itemID {itemID}");
                return false;
            }
        }
        else
        {
            //Debug.LogWarning($"Ugyldigt index {index} til at sætte item");
            return false;
        }
    }

    public List<int> GetEmptySlots()
    {
        List<int> emptySlots = new List<int>();
        for (int i = 0; i < itemIDs.Count; i++)
        {
            if (itemIDs[i] == -1)
            {
                emptySlots.Add(i);
            }
        }
        return emptySlots;
    }
    public void SwapItems(int index1, int index2)
    {
        if (index1 >= 0 && index1 < itemIDs.Count && index2 >= 0 && index2 < itemIDs.Count)
        {
            int temp = itemIDs[index1];
            itemIDs[index1] = itemIDs[index2];
            itemIDs[index2] = temp;
            //Debug.Log($"Byttede itemID {temp} fra slot {index1} med itemID {itemIDs[index2]} fra slot {index2}");
            OnInventoryChanged?.Invoke();
        }
        else
        {
            //Debug.LogWarning($"Kan ikke bytte items mellem slot {index1} og slot {index2}. Ugyldigt index.");
        }
    }
}