using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryMenu : MonoBehaviour
{
    public KeyCode inventoryKey = KeyCode.I;
    public GameObject inventoryUI;
    public Transform weaponListParent;
    public GameObject weaponButtonPrefab;
    public GameObject contextMenuPrefab;

    public Inventory playerInventory; // Assign via Inspector
    public ItemManager itemManager; // Assign via Inspector

    private GameObject contextMenuInstance;
    private bool isOpen = false;

    private int selectedItemID = -1; // Track selected item ID
    private Button selectedWeaponButton;

    void Start()
    {
        if (playerInventory == null)
        {
            playerInventory = FindObjectOfType<Inventory>();
        }

        if (playerInventory == null)
        {
            //Debug.LogError("No Inventory found in the scene. Please assign playerInventory.");
            return;
        }

        if (itemManager == null)
        {
            //Debug.LogError("No ItemManager assigned in the Inspector.");
            return;
        }

        if (inventoryUI == null)
        {
            //Debug.LogError("inventoryUI is not assigned in the Inspector.");
            return;
        }

        if (weaponListParent == null)
        {
            //Debug.LogError("weaponListParent is not assigned in the Inspector.");
            return;
        }

        if (weaponButtonPrefab == null)
        {
            //Debug.LogError("weaponButtonPrefab is not assigned in the Inspector.");
            return;
        }

        if (contextMenuPrefab == null)
        {
            //Debug.LogError("contextMenuPrefab is not assigned in the Inspector.");
            return;
        }

        inventoryUI.SetActive(false);

        // Instantiate context menu
        contextMenuInstance = Instantiate(contextMenuPrefab, inventoryUI.transform);
        contextMenuInstance.SetActive(false);
    }

    void Update()
    {
        if (playerInventory == null) return;

        if (Input.GetKeyDown(inventoryKey))
        {
            ToggleInventory();
        }
    }

    void ToggleInventory()
    {
        isOpen = !isOpen;
        inventoryUI.SetActive(isOpen);
        if (isOpen)
        {
            RefreshInventoryUI();
        }
        else
        {
            CloseContextMenu();
        }
    }

    public void RefreshInventoryUI()
    {
        if (playerInventory == null || weaponButtonPrefab == null || weaponListParent == null)
        {
            //Debug.LogWarning("Missing references.");
            return;
        }

        // Clear all old buttons under weaponListParent
        foreach (Transform child in weaponListParent)
        {
            Destroy(child.gameObject);
        }

        List<int> itemIDs = playerInventory.itemIDs;

        // Instantiate a button for each item
        for (int i = 0; i < itemIDs.Count; i++)
        {
            int itemID = itemIDs[i];
            if (itemID == -1) continue;

            // Create the button under weaponListParent
            GameObject btnObj = Instantiate(weaponButtonPrefab, weaponListParent);

            Button btn = btnObj.GetComponent<Button>();
            TMP_Text txt = btnObj.GetComponentInChildren<TMP_Text>();
            if (txt != null)
            {
                // Optionally, get the item name from ItemManager if available
                txt.text = "Item ID: " + itemID;
            }

            int index = i;
            btn.onClick.AddListener(() => OnWeaponButtonClicked(itemID, btn, index));
        }
    }

    void OnWeaponButtonClicked(int itemID, Button weaponButton, int index)
    {
        if (itemID == -1 || weaponButton == null)
            return;

        selectedItemID = itemID;
        selectedWeaponButton = weaponButton;

        ShowContextMenuAtButton(weaponButton, itemID);
    }

    public void ShowContextMenuForSlot(InventorySlot slot)
    {
        if (slot == null || slot.currentItemID == -1) return;

        selectedItemID = slot.currentItemID;

        contextMenuInstance.SetActive(true);
        contextMenuInstance.transform.position = slot.transform.position;

        Button equipBtn = contextMenuInstance.transform.Find("EquipButton").GetComponent<Button>();
        Button dropBtn = contextMenuInstance.transform.Find("DropButton").GetComponent<Button>();

        equipBtn.onClick.RemoveAllListeners();
        equipBtn.onClick.AddListener(() => {
            EquipItem(slot);
            contextMenuInstance.SetActive(false);
        });

        dropBtn.onClick.RemoveAllListeners();
        dropBtn.onClick.AddListener(() => {
            DropItemFromSlot(slot);
            contextMenuInstance.SetActive(false);
        });
    }

    void EquipItem(InventorySlot slot)
    {
        if (slot.currentItemID == -1) return;

        int itemID = slot.currentItemID;

        // Implement your equip logic here based on itemID
        //Debug.Log("Equipped item ID: " + itemID);

        CloseContextMenu();
    }

    void DropItemFromSlot(InventorySlot slot)
    {
        if (slot.currentItemID == -1) return;

        int itemID = slot.currentItemID;

        // Remove item from inventory
        playerInventory.RemoveItem(itemID);

        // Optionally, spawn the item in the world
        // Example: SpawnItemInWorld(itemID, player.transform.position + player.forward)

        // Refresh UI
        RefreshInventoryUI();
    }

    void ShowContextMenuAtButton(Button weaponButton, int itemID)
    {
        if (contextMenuInstance == null || weaponButton == null) return;

        RectTransform buttonRect = weaponButton.GetComponent<RectTransform>();
        RectTransform menuRect = contextMenuInstance.GetComponent<RectTransform>();
        if (buttonRect == null || menuRect == null) return;

        Vector3 buttonPos = buttonRect.transform.position;
        menuRect.transform.position = buttonPos;

        contextMenuInstance.SetActive(true);

        SetupContextMenuButtons();
    }

    void SetupContextMenuButtons()
    {
        Button equipBtn = contextMenuInstance.transform.Find("EquipButton")?.GetComponent<Button>();
        Button dropBtn = contextMenuInstance.transform.Find("DropButton")?.GetComponent<Button>();

        if (equipBtn == null || dropBtn == null) return;

        equipBtn.onClick.RemoveAllListeners();
        dropBtn.onClick.RemoveAllListeners();

        equipBtn.onClick.AddListener(() => OnEquipSelectedWeapon());
        dropBtn.onClick.AddListener(() => OnDropSelectedWeapon());
    }

    void OnEquipSelectedWeapon()
    {
        if (selectedItemID != -1)
        {
            // Implement equip logic based on selectedItemID
            //Debug.Log("Equipped item ID: " + selectedItemID);
        }
        CloseContextMenu();
    }

    void OnDropSelectedWeapon()
    {
        if (selectedItemID != -1 && playerInventory != null)
        {
            DropWeapon(selectedItemID);
        }
        CloseContextMenu();
    }

    public void DropWeapon(int itemID)
    {
        if (playerInventory == null) return;
        playerInventory.RemoveItem(itemID);
        // Spawn item in world based on itemID
        // This requires a method to spawn GameObject based on itemID
        RefreshInventoryUI();
    }

    void CloseContextMenu()
    {
        if (contextMenuInstance != null)
            contextMenuInstance.SetActive(false);
        selectedItemID = -1;
        selectedWeaponButton = null;
    }
}
