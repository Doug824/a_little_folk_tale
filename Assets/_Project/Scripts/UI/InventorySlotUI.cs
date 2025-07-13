using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using ALittleFolkTale.Characters;
using ALittleFolkTale.Items;

namespace ALittleFolkTale.UI
{
    public class InventorySlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private int slotIndex;
        private Image itemIcon;
        private Text quantityText;
        private Button slotButton;
        private bool isHotbarSlot;
        private InventorySlot currentSlot;
        
        // Selection and drag state
        private bool isSelected = false;
        private static InventorySlotUI selectedSlot;
        private static InventorySlotUI draggedSlot;
        private GameObject dragIcon;
        private Canvas dragCanvas;
        
        // Visual components
        private Image slotBackground;
        private Color normalColor = new Color(0.3f, 0.3f, 0.3f, 0.8f);
        private Color selectedColor = new Color(0.5f, 0.5f, 0.2f, 0.9f);
        private Color hoverColor = new Color(0.4f, 0.4f, 0.4f, 0.9f);

        public void Initialize(int index, Image icon, Text quantity, Button button, bool isHotbar = false)
        {
            slotIndex = index;
            itemIcon = icon;
            quantityText = quantity;
            slotButton = button;
            isHotbarSlot = isHotbar;
            
            // Get or add background image component
            slotBackground = GetComponent<Image>();
            if (slotBackground == null)
            {
                slotBackground = gameObject.AddComponent<Image>();
            }
            slotBackground.color = normalColor;
            
            // Find the drag canvas (should be the main inventory canvas)
            dragCanvas = GetComponentInParent<Canvas>();
        }

        public void UpdateDisplay(InventorySlot slot)
        {
            currentSlot = slot;
            
            if (slot.IsEmpty)
            {
                // Empty slot
                itemIcon.color = Color.clear;
                itemIcon.sprite = null;
                quantityText.text = "";
            }
            else
            {
                // Show item
                if (slot.item.Data.icon != null)
                {
                    // Use actual item icon
                    itemIcon.sprite = slot.item.Data.icon;
                    itemIcon.color = Color.white;
                }
                else
                {
                    // Fallback to color coding
                    itemIcon.sprite = null;
                    itemIcon.color = GetItemColor(slot.item);
                }
                
                // Show quantity if more than 1
                if (slot.quantity > 1)
                {
                    quantityText.text = slot.quantity.ToString();
                }
                else
                {
                    quantityText.text = "";
                }
            }
        }

        private Color GetItemColor(Item item)
        {
            switch (item.Data.itemType)
            {
                case ItemType.Weapon:
                    return new Color(1f, 0.8f, 0.6f); // Light orange
                case ItemType.Consumable:
                    return new Color(0.6f, 1f, 0.6f); // Light green
                case ItemType.Equipment:
                    return new Color(0.6f, 0.6f, 1f); // Light blue
                case ItemType.Tool:
                    return new Color(1f, 0.6f, 1f); // Light purple
                default:
                    return Color.white;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                // Toggle selection
                if (isSelected)
                {
                    Deselect();
                }
                else
                {
                    Select();
                }
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                // Right click for quick actions
                if (currentSlot != null && !currentSlot.IsEmpty)
                {
                    InventoryUI.Instance?.OnInventorySlotRightClicked(slotIndex);
                }
            }
        }
        
        private void Select()
        {
            // Deselect previous slot
            if (selectedSlot != null && selectedSlot != this)
            {
                selectedSlot.Deselect();
            }
            
            isSelected = true;
            selectedSlot = this;
            slotBackground.color = selectedColor;
            
            // Notify InventoryUI of selection
            InventoryUI.Instance?.OnSlotSelected(slotIndex);
        }
        
        private void Deselect()
        {
            isSelected = false;
            if (selectedSlot == this)
            {
                selectedSlot = null;
            }
            slotBackground.color = normalColor;
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!isSelected && draggedSlot == null)
            {
                slotBackground.color = hoverColor;
            }
            
            // Show tooltip if item exists
            if (currentSlot != null && !currentSlot.IsEmpty)
            {
                ShowTooltip();
            }
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            if (!isSelected)
            {
                slotBackground.color = normalColor;
            }
            
            // Hide tooltip
            HideTooltip();
        }
        
        private void ShowTooltip()
        {
            if (currentSlot?.item != null)
            {
                string tooltip = $"<b>{currentSlot.item.Data.itemName}</b>";
                if (!string.IsNullOrEmpty(currentSlot.item.Data.description))
                {
                    tooltip += $"\n{currentSlot.item.Data.description}";
                }
                if (currentSlot.quantity > 1)
                {
                    tooltip += $"\nQuantity: {currentSlot.quantity}";
                }
                
                // For now, just debug log. In a full game, you'd show a proper tooltip UI
                Debug.Log(tooltip);
            }
        }
        
        private void HideTooltip()
        {
            // Hide tooltip UI (placeholder for now)
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (currentSlot == null || currentSlot.IsEmpty) return;
            
            draggedSlot = this;
            
            // Create drag icon
            CreateDragIcon();
            
            // Make the drag icon follow the mouse
            if (dragIcon != null)
            {
                dragIcon.transform.SetAsLastSibling();
                UpdateDragIconPosition(eventData);
            }
            
            // Reduce opacity of original slot
            itemIcon.color = new Color(itemIcon.color.r, itemIcon.color.g, itemIcon.color.b, 0.5f);
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if (dragIcon != null)
            {
                UpdateDragIconPosition(eventData);
            }
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            // Restore original slot opacity
            if (currentSlot != null && !currentSlot.IsEmpty)
            {
                itemIcon.color = new Color(itemIcon.color.r, itemIcon.color.g, itemIcon.color.b, 1f);
            }
            
            // Destroy drag icon
            if (dragIcon != null)
            {
                Destroy(dragIcon);
                dragIcon = null;
            }
            
            draggedSlot = null;
        }
        
        public void OnDrop(PointerEventData eventData)
        {
            if (draggedSlot == null || draggedSlot == this) return;
            
            // Perform item swap
            InventoryUI.Instance?.SwapItems(draggedSlot.slotIndex, slotIndex);
        }
        
        private void CreateDragIcon()
        {
            if (dragCanvas == null) return;
            
            dragIcon = new GameObject("DragIcon");
            dragIcon.transform.SetParent(dragCanvas.transform, false);
            
            RectTransform rect = dragIcon.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(60, 60);
            
            Image iconImage = dragIcon.AddComponent<Image>();
            iconImage.sprite = itemIcon.sprite;
            iconImage.color = itemIcon.color;
            iconImage.raycastTarget = false;
            
            // Add semi-transparent background
            GameObject bg = new GameObject("Background");
            bg.transform.SetParent(dragIcon.transform, false);
            bg.transform.SetAsFirstSibling();
            
            RectTransform bgRect = bg.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            
            Image bgImage = bg.AddComponent<Image>();
            bgImage.color = new Color(0, 0, 0, 0.5f);
            bgImage.raycastTarget = false;
        }
        
        private void UpdateDragIconPosition(PointerEventData eventData)
        {
            if (dragIcon != null && dragCanvas != null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    dragCanvas.transform as RectTransform,
                    eventData.position,
                    eventData.pressEventCamera,
                    out Vector2 localPoint
                );
                
                dragIcon.transform.localPosition = localPoint;
            }
        }

    }
}