using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemManager", menuName = "Game/ItemManager")]
public class ItemManager : ScriptableObject
{
    [System.Serializable]
    public class ItemEntry
    {
        public int itemID;
        public string itemName;
        public Sprite itemSprite;
        public GameObject weaponPrefab; // Kun for v�ben
        // Tilf�j yderligere data efter behov
    }

    public List<ItemEntry> items = new List<ItemEntry>();

    private Dictionary<int, ItemEntry> itemDictionary;

    // Initialiser Dictionary til opslagsform�l
    public void Initialize()
    {
        itemDictionary = new Dictionary<int, ItemEntry>();
        foreach (var entry in items)
        {
            if (!itemDictionary.ContainsKey(entry.itemID))
            {
                itemDictionary.Add(entry.itemID, entry);
            }
        }
    }

    // Hent ItemEntry baseret p� itemID
    public ItemEntry GetItemEntry(int id)
    {
        if (itemDictionary == null)
        {
            Initialize();
        }

        if (itemDictionary.ContainsKey(id))
            return itemDictionary[id];
        return null;
    }

    // Hent Sprite baseret p� itemID
    public Sprite GetSpriteForItem(int id)
    {
        var entry = GetItemEntry(id);
        return entry != null ? entry.itemSprite : null;
    }

    // Hent WeaponPrefab baseret p� itemID
    public GameObject GetWeaponPrefabForItem(int id)
    {
        var entry = GetItemEntry(id);
        return entry != null ? entry.weaponPrefab : null;
    }
}
