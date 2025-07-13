using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using ALittleFolkTale.Characters;
using ALittleFolkTale.Items;

namespace ALittleFolkTale.UI
{
    public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
    {
        private int slotIndex;
        private Image itemIcon;
        private Text quantityText;
        private Button slotButton;
        private bool isHotbarSlot;
        private InventorySlot currentSlot;

        public void Initialize(int index, Image icon, Text quantity, Button button, bool isHotbar = false)
        {
            slotIndex = index;
            itemIcon = icon;
            quantityText = quantity;
            slotButton = button;
            isHotbarSlot = isHotbar;
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
            if (currentSlot == null || currentSlot.IsEmpty) return;
            
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                InventoryUI.Instance?.OnInventorySlotClicked(slotIndex);
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                InventoryUI.Instance?.OnInventorySlotRightClicked(slotIndex);
            }
        }

    }
}