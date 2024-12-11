using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public Image itemIcon;
    public int currentItemID = -1; // -1 indikerer ingen item
    public int slotIndex = -1; // Indeks i inventory listen
    private CanvasGroup canvasGroup;
    private GameObject dragIcon;
    private Canvas rootCanvas;
    private InventoryUI inventoryUI;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        rootCanvas = GetComponentInParent<Canvas>();
        inventoryUI = GetComponentInParent<InventoryUI>();

        if (inventoryUI == null)
        {
            //Debug.LogError($"InventoryUI not found in parent hierarchy for {gameObject.name}");
        }
    }

    // Sæt itemID og sprite i slotten
    public void SetItem(int itemID, Sprite sprite)
    {
        currentItemID = itemID;
        if (sprite != null)
        {
            itemIcon.sprite = sprite;
            itemIcon.color = Color.white; // Sørg for, at sprite er synlig
            //Debug.Log($"SetItem: itemID {itemID} med sprite {sprite.name} i slot {gameObject.name}");
        }
        else
        {
            itemIcon.sprite = null;
            itemIcon.color = new Color(1, 1, 1, 0); // Gør sprite usynlig
            //Debug.Log($"SetItem: itemID {itemID} ryddet i slot {gameObject.name}");
        }
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentItemID == -1) return;

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        dragIcon = new GameObject("DragIcon");
        dragIcon.transform.SetParent(rootCanvas.transform, false);
        dragIcon.transform.SetAsLastSibling();
        Image dragImage = dragIcon.AddComponent<Image>();
        dragImage.sprite = itemIcon.sprite;
        dragImage.raycastTarget = false;
        dragIcon.transform.position = eventData.position;

        //Debug.Log($"Begynner at dragge itemID {currentItemID} fra slot {gameObject.name}");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
        {
            dragIcon.transform.position = eventData.position;
            //Debug.Log($"Dragging itemID {currentItemID} til position {eventData.position}");
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
            Destroy(dragIcon);

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Definer layer mask for InventorySlots
        int inventorySlotLayer = LayerMask.NameToLayer("InventorySlots");
        if (inventorySlotLayer == -1)
        {
            //Debug.LogError("Layer 'InventorySlots' not found. Please ensure it is created and assigned correctly.");
            return;
        }

        // Brug raycasting til at finde InventorySlot under cursoren
        List<RaycastResult> results = new List<RaycastResult>();
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
        EventSystem.current.RaycastAll(pointerData, results);

        //Debug.Log($"Raycast hit count: {results.Count}");
        foreach (var result in results)
        {
            Debug.Log($"Raycast hit: {result.gameObject.name}, Layer: {result.gameObject.layer}");
        }

        InventorySlot targetSlot = null;
        foreach (var result in results)
        {
            // Tjek om GameObject er på InventorySlots layer
            if (result.gameObject.layer == inventorySlotLayer)
            {
                InventorySlot slot = result.gameObject.GetComponent<InventorySlot>();
                if (slot != null && slot != this)
                {
                    targetSlot = slot;
                    break;
                }
            }
        }

        if (targetSlot != null)
        {
            //Debug.Log($"Droppe over slot {targetSlot.gameObject.name} med slotIndex {targetSlot.slotIndex}");
            // Swap items i Inventory data
            inventoryUI.playerInventory.SwapItems(this.slotIndex, targetSlot.slotIndex);

            // UI vil blive opdateret automatisk via OnInventoryChanged event
            return;
        }
        else
        {
            //Debug.Log("Droppe ikke over en gyldig InventorySlot");
        }

        // Hvis ikke droppet over en slot, drop item
        if (!IsPointerOverInventoryUI())
        {
            DropCurrentItem();
        }
        else
        {
            // Item forbliver i inventory, opdater UI
            inventoryUI.RefreshInventoryUI();
        }
    }

    bool IsPointerOverInventoryUI()
    {
        if (inventoryUI == null || inventoryUI.inventoryPanelRect == null)
        {
            //Debug.LogWarning("InventoryUI or inventoryPanelRect is null in IsPointerOverInventoryUI.");
            return false;
        }

        bool isOver = RectTransformUtility.RectangleContainsScreenPoint(
            inventoryUI.inventoryPanelRect,
            Input.mousePosition,
            null // Hvis du bruger en kamera-baseret UI, kan du specificere kameraet her
        );

        //Debug.Log($"IsPointerOverInventoryUI: {isOver}");
        return isOver;
    }

    void DropCurrentItem()
    {
        if (currentItemID != -1)
        {
            //Debug.Log($"Dropping item ID: {currentItemID} fra slot {gameObject.name}");

            // Fjern item fra inventory
            Inventory playerInventory = inventoryUI.playerInventory;
            if (playerInventory != null)
            {
                playerInventory.RemoveItemAtIndex(slotIndex);
                //Debug.Log($"Removed item ID: {currentItemID} fra inventory.");
            }
            else
            {
                //Debug.LogError("playerInventory er null i DropCurrentItem.");
            }

            // Spawn item i verden
            SpawnItemInWorld(currentItemID, transform.position + transform.forward);

            // Ryd slotten
            SetItem(-1, null);
            Debug.Log($"Rydde slot med item ID: {currentItemID}");

            // Opdater UI
            inventoryUI.RefreshInventoryUI();
        }
    }

    void SpawnItemInWorld(int itemID, Vector3 position)
    {
        ItemManager itemManager = inventoryUI.itemManager;
        if (itemManager == null)
        {
            Debug.LogError("ItemManager er null i SpawnItemInWorld.");
            return;
        }

        GameObject weaponPrefab = itemManager.GetWeaponPrefabForItem(itemID);
        if (weaponPrefab != null)
        {
            Instantiate(weaponPrefab, position, Quaternion.identity);
            Debug.Log($"Spawned weapon prefab for item ID: {itemID} ved position {position}");
        }
        else
        {
            Debug.LogWarning($"Ingen weaponPrefab fundet for item ID: {itemID} i SpawnItemInWorld.");
        }
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        // Højreklik for at vise context menu
        if (eventData.button == PointerEventData.InputButton.Right && currentItemID != -1)
        {
            if (inventoryUI != null && inventoryUI.GetComponent<InventoryMenu>() != null)
            {
                InventoryMenu inventoryMenu = inventoryUI.GetComponent<InventoryMenu>();
                inventoryMenu.ShowContextMenuForSlot(this);
            }
        }
    }
}