using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Inventory playerInventory;
    public ItemManager itemManager;
    public InventorySlot[] slots;
    public RectTransform inventoryPanelRect; // Sørg for, at denne er tildelt i Inspector

    void Start()
    {
        if (itemManager != null)
        {
            itemManager.Initialize();
        }
        else
        {
            //Debug.LogError("ItemManager is not assigned in InventoryUI.");
        }

        if (playerInventory == null)
        {
            //Debug.LogError("PlayerInventory is not assigned in InventoryUI.");
        }

        if (slots == null || slots.Length == 0)
        {
            //Debug.LogError("InventorySlots are not assigned in InventoryUI.");
        }

        // Assign slotIndex to each slot
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null)
            {
                slots[i].slotIndex = i;
                //Debug.Log($"Assigned slotIndex {i} to {slots[i].gameObject.name}");
            }
            else
            {
                //Debug.LogError($"slots[{i}] is null in InventoryUI.");
            }
        }

        // Force set all slots to be empty initially
        foreach (var slot in slots)
        {
            if (slot != null)
            {
                slot.SetItem(-1, null);
            }
        }

        RefreshInventoryUI();

        // Listen to inventory changes
        if (playerInventory != null)
            playerInventory.OnInventoryChanged += RefreshInventoryUI;
    }

    public void RefreshInventoryUI()
    {
        if (playerInventory == null)
        {
            //Debug.LogError("playerInventory is null in RefreshInventoryUI.");
            return;
        }
        if (itemManager == null)
        {
            //Debug.LogError("itemManager is null in RefreshInventoryUI.");
            return;
        }
        if (slots == null || slots.Length == 0)
        {
            //Debug.LogError("slots are not assigned or empty in RefreshInventoryUI.");
            return;
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
            {
                Debug.LogError($"slots[{i}] is null in RefreshInventoryUI.");
                continue;
            }

            int itemID = playerInventory.GetItemAtIndex(i);
            if (itemID != -1)
            {
                Sprite spr = itemManager.GetSpriteForItem(itemID);
                if (spr == null)
                {
                    Debug.LogWarning($"No sprite found for itemID: {itemID}");
                }
                slots[i].SetItem(itemID, spr);
                //Debug.Log($"Slot {i} set to itemID {itemID} with sprite {spr?.name ?? "null"}");
            }
            else
            {
                slots[i].SetItem(-1, null);
                //Debug.Log($"Slot {i} cleared.");
            }
        }
        //Debug.Log("InventoryUI refreshed.");
    }
}