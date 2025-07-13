using UnityEngine;
using UnityEngine.UI;
using ALittleFolkTale.Characters;
using ALittleFolkTale.Items;

namespace ALittleFolkTale.UI
{
    public class EquipmentSlotUI : MonoBehaviour
    {
        private EquipmentSlot slotType;
        private Image itemIcon;
        private Button slotButton;
        private Item currentItem;

        public void Initialize(EquipmentSlot type, Image icon, Button button)
        {
            slotType = type;
            itemIcon = icon;
            slotButton = button;
            
            // Setup button click events
            slotButton.onClick.AddListener(OnSlotClicked);
        }

        public void UpdateDisplay(Item equippedItem)
        {
            currentItem = equippedItem;
            
            if (equippedItem == null)
            {
                // Empty equipment slot
                itemIcon.color = Color.clear;
                itemIcon.sprite = null;
            }
            else
            {
                // Show equipped item
                if (equippedItem.Data.icon != null)
                {
                    // Use actual item icon
                    itemIcon.sprite = equippedItem.Data.icon;
                    itemIcon.color = Color.white;
                }
                else
                {
                    // Fallback to color coding
                    itemIcon.sprite = null;
                    itemIcon.color = GetItemColor(equippedItem);
                }
            }
        }

        private Color GetItemColor(Item item)
        {
            switch (item.Data.itemType)
            {
                case ItemType.Weapon:
                    return new Color(1f, 0.8f, 0.6f); // Light orange
                case ItemType.Equipment:
                    return new Color(0.6f, 0.6f, 1f); // Light blue
                case ItemType.Tool:
                    return new Color(1f, 0.6f, 1f); // Light purple
                default:
                    return Color.white;
            }
        }

        private void OnSlotClicked()
        {
            if (currentItem == null) return;
            
            var inventory = Inventory.Instance;
            if (inventory == null) return;
            
            // Unequip the item
            switch (slotType)
            {
                case EquipmentSlot.MainHand:
                    inventory.UnequipWeapon();
                    break;
                case EquipmentSlot.Armor:
                    inventory.UnequipArmor();
                    break;
                case EquipmentSlot.Accessory:
                    inventory.UnequipAccessory();
                    break;
            }
        }

        private void OnDestroy()
        {
            if (slotButton != null)
            {
                slotButton.onClick.RemoveListener(OnSlotClicked);
            }
        }
    }
}