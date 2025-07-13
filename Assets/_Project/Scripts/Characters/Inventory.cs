using UnityEngine;
using System.Collections.Generic;
using ALittleFolkTale.Items;

namespace ALittleFolkTale.Characters
{
    [System.Serializable]
    public class InventorySlot
    {
        public Item item;
        public int quantity;
        
        public InventorySlot()
        {
            item = null;
            quantity = 0;
        }
        
        public InventorySlot(Item item, int quantity)
        {
            this.item = item;
            this.quantity = quantity;
        }
        
        public bool IsEmpty => item == null || quantity <= 0;
        
        public bool CanStack(Item newItem)
        {
            if (IsEmpty) return true;
            if (!newItem.Data.isStackable) return false;
            if (item.Data.itemName != newItem.Data.itemName) return false;
            if (quantity >= item.Data.maxStackSize) return false;
            return true;
        }
        
        public void Clear()
        {
            item = null;
            quantity = 0;
        }
    }

    [System.Serializable]
    public class EquipmentSlots
    {
        public Item weapon;
        public Item armor;
        public Item accessory;
        
        public void Clear()
        {
            weapon = null;
            armor = null;
            accessory = null;
        }
    }

    public class Inventory : MonoBehaviour
    {
        [Header("Inventory Settings")]
        [SerializeField] private int inventorySize = 9; // Link's Awakening style 3x3 grid
        [SerializeField] private int hotbarSize = 4;
        
        private List<InventorySlot> inventorySlots;
        private EquipmentSlots equipment;
        private PlayerController playerController;
        
        // Quick-use item slots (Y/X buttons, Q/E keys)
        private Item quickUseItemY; // Y button / Q key
        private Item quickUseItemX; // X button / E key
        
        // Hotkey slots for UI (these reference items in the main inventory)
        private int hotkeySlotQ = -1; // Index in main inventory, -1 if empty
        private int hotkeySlotE = -1; // Index in main inventory, -1 if empty
        
        // Events for UI updates
        public System.Action OnInventoryChanged;
        public System.Action OnEquipmentChanged;
        public System.Action OnQuickUseChanged;
        
        // Singleton-like access for UI
        private static Inventory instance;
        public static Inventory Instance
        {
            get
            {
                if (instance == null)
                    instance = FindFirstObjectByType<Inventory>();
                return instance;
            }
        }

        private void Awake()
        {
            instance = this;
            playerController = GetComponent<PlayerController>();
            
            // Initialize inventory
            inventorySlots = new List<InventorySlot>();
            for (int i = 0; i < inventorySize; i++)
            {
                inventorySlots.Add(new InventorySlot());
            }
            
            equipment = new EquipmentSlots();
        }

        public bool AddItem(Item item)
        {
            if (item == null) return false;
            
            // Try to stack first
            for (int i = 0; i < inventorySlots.Count; i++)
            {
                if (inventorySlots[i].CanStack(item))
                {
                    if (inventorySlots[i].IsEmpty)
                    {
                        // Create inventory instance of the item
                        Item inventoryItem = item.CreateInventoryInstance();
                        inventorySlots[i] = new InventorySlot(inventoryItem, item.Quantity);
                    }
                    else
                    {
                        int spaceLeft = inventorySlots[i].item.Data.maxStackSize - inventorySlots[i].quantity;
                        int amountToAdd = Mathf.Min(spaceLeft, item.Quantity);
                        inventorySlots[i].quantity += amountToAdd;
                    }
                    
                    OnInventoryChanged?.Invoke();
                    return true;
                }
            }
            
            return false; // Inventory full
        }

        public bool RemoveItem(int slotIndex, int quantity = 1)
        {
            if (slotIndex < 0 || slotIndex >= inventorySlots.Count) return false;
            if (inventorySlots[slotIndex].IsEmpty) return false;
            
            inventorySlots[slotIndex].quantity -= quantity;
            
            if (inventorySlots[slotIndex].quantity <= 0)
            {
                if (inventorySlots[slotIndex].item != null)
                {
                    Destroy(inventorySlots[slotIndex].item.gameObject);
                }
                inventorySlots[slotIndex].Clear();
            }
            
            OnInventoryChanged?.Invoke();
            return true;
        }

        public Item GetItem(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= inventorySlots.Count) return null;
            return inventorySlots[slotIndex].item;
        }

        public int GetItemQuantity(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= inventorySlots.Count) return 0;
            return inventorySlots[slotIndex].quantity;
        }

        public bool EquipItem(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= inventorySlots.Count) return false;
            
            Item item = inventorySlots[slotIndex].item;
            if (item == null) return false;
            
            switch (item.Data.equipmentSlot)
            {
                case EquipmentSlot.MainHand:
                    return EquipWeapon(item, slotIndex);
                case EquipmentSlot.Armor:
                    return EquipArmor(item, slotIndex);
                case EquipmentSlot.Accessory:
                    return EquipAccessory(item, slotIndex);
                default:
                    // Try by item type for backward compatibility
                    if (item.Data.itemType == ItemType.Weapon)
                        return EquipWeapon(item, slotIndex);
                    else if (item.Data.itemType == ItemType.Equipment)
                        return EquipArmor(item, slotIndex);
                    return false;
            }
        }

        private bool EquipWeapon(Item weapon, int slotIndex)
        {
            // Unequip current weapon if any
            if (equipment.weapon != null)
            {
                UnequipWeapon();
            }
            
            equipment.weapon = weapon;
            RemoveItem(slotIndex, 1);
            
            // Apply weapon stats to player
            if (weapon is Stick stick)
            {
                ApplyWeaponStats(stick.GetWeaponStats());
            }
            else if (weapon.Data.itemType == ItemType.Weapon)
            {
                ApplyWeaponStats(weapon.Data.stats);
            }
            
            OnEquipmentChanged?.Invoke();
            Debug.Log($"Equipped {weapon.Data.itemName}");
            return true;
        }

        private bool EquipArmor(Item armor, int slotIndex)
        {
            if (equipment.armor != null)
            {
                AddItemToInventory(equipment.armor);
            }
            
            equipment.armor = armor;
            RemoveItem(slotIndex, 1);
            OnEquipmentChanged?.Invoke();
            return true;
        }

        private bool EquipAccessory(Item accessory, int slotIndex)
        {
            if (equipment.accessory != null)
            {
                AddItemToInventory(equipment.accessory);
            }
            
            equipment.accessory = accessory;
            RemoveItem(slotIndex, 1);
            OnEquipmentChanged?.Invoke();
            return true;
        }

        public void UnequipWeapon()
        {
            if (equipment.weapon != null)
            {
                AddItemToInventory(equipment.weapon);
                equipment.weapon = null;
                
                // Reset weapon stats to default
                ResetWeaponStats();
                
                OnEquipmentChanged?.Invoke();
                Debug.Log("Unequipped weapon");
            }
        }

        public void UnequipArmor()
        {
            if (equipment.armor != null)
            {
                AddItemToInventory(equipment.armor);
                equipment.armor = null;
                OnEquipmentChanged?.Invoke();
            }
        }

        public void UnequipAccessory()
        {
            if (equipment.accessory != null)
            {
                AddItemToInventory(equipment.accessory);
                equipment.accessory = null;
                OnEquipmentChanged?.Invoke();
            }
        }

        private void AddItemToInventory(Item item)
        {
            // Find empty slot and add item back
            for (int i = 0; i < inventorySlots.Count; i++)
            {
                if (inventorySlots[i].IsEmpty)
                {
                    inventorySlots[i] = new InventorySlot(item, 1);
                    OnInventoryChanged?.Invoke();
                    return;
                }
            }
            
            // If no space, drop on ground
            // TODO: Implement drop system
            Debug.LogWarning("No space in inventory, item should be dropped");
        }

        private void ApplyWeaponStats(ItemStats stats)
        {
            if (playerController != null)
            {
                playerController.ApplyWeaponStats(stats);
            }
        }

        private void ResetWeaponStats()
        {
            if (playerController != null)
            {
                playerController.ResetWeaponStats();
            }
        }

        public bool UseConsumable(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= inventorySlots.Count) return false;
            
            Item item = inventorySlots[slotIndex].item;
            if (item == null || !item.Data.isConsumable) return false;
            
            // Handle specific consumables
            if (item is Berry berry)
            {
                // Use the berry's healing effect
                if (playerController != null)
                {
                    playerController.Heal(25); // Berry heal amount
                    RemoveItem(slotIndex, 1);
                    return true;
                }
            }
            
            return false;
        }

        // Quick-use item management
        public void SetQuickUseItemY(Item item)
        {
            quickUseItemY = item;
            OnQuickUseChanged?.Invoke();
        }
        
        public void SetQuickUseItemX(Item item)
        {
            quickUseItemX = item;
            OnQuickUseChanged?.Invoke();
        }
        
        public bool UseQuickItemY()
        {
            if (hotkeySlotQ >= 0 && hotkeySlotQ < inventorySlots.Count)
            {
                if (UseConsumable(hotkeySlotQ))
                {
                    // Update the hotkey reference
                    if (inventorySlots[hotkeySlotQ].IsEmpty)
                    {
                        ClearHotkeyQ();
                    }
                    else
                    {
                        quickUseItemY = inventorySlots[hotkeySlotQ].item;
                    }
                    return true;
                }
            }
            return false;
        }
        
        public bool UseQuickItemX()
        {
            if (hotkeySlotE >= 0 && hotkeySlotE < inventorySlots.Count)
            {
                if (UseConsumable(hotkeySlotE))
                {
                    // Update the hotkey reference
                    if (inventorySlots[hotkeySlotE].IsEmpty)
                    {
                        ClearHotkeyE();
                    }
                    else
                    {
                        quickUseItemX = inventorySlots[hotkeySlotE].item;
                    }
                    return true;
                }
            }
            return false;
        }
        
        // Item manipulation methods
        public bool SwapItems(int fromSlot, int toSlot)
        {
            if (fromSlot < 0 || fromSlot >= inventorySlots.Count ||
                toSlot < 0 || toSlot >= inventorySlots.Count)
                return false;
            
            // Swap the items
            var temp = inventorySlots[fromSlot];
            inventorySlots[fromSlot] = inventorySlots[toSlot];
            inventorySlots[toSlot] = temp;
            
            OnInventoryChanged?.Invoke();
            return true;
        }
        
        public bool MoveItem(int fromSlot, int toSlot)
        {
            if (fromSlot < 0 || fromSlot >= inventorySlots.Count ||
                toSlot < 0 || toSlot >= inventorySlots.Count)
                return false;
            
            if (inventorySlots[fromSlot].IsEmpty)
                return false;
            
            // If destination is empty, just move
            if (inventorySlots[toSlot].IsEmpty)
            {
                inventorySlots[toSlot] = inventorySlots[fromSlot];
                inventorySlots[fromSlot] = new InventorySlot();
                OnInventoryChanged?.Invoke();
                return true;
            }
            
            // If items can stack, try to stack them
            if (inventorySlots[toSlot].CanStack(inventorySlots[fromSlot].item))
            {
                int spaceLeft = inventorySlots[toSlot].item.Data.maxStackSize - inventorySlots[toSlot].quantity;
                int amountToMove = Mathf.Min(spaceLeft, inventorySlots[fromSlot].quantity);
                
                inventorySlots[toSlot].quantity += amountToMove;
                inventorySlots[fromSlot].quantity -= amountToMove;
                
                if (inventorySlots[fromSlot].quantity <= 0)
                {
                    inventorySlots[fromSlot].Clear();
                }
                
                OnInventoryChanged?.Invoke();
                return true;
            }
            
            // Otherwise, swap the items
            return SwapItems(fromSlot, toSlot);
        }
        
        // Hotkey slot management
        public void SetHotkeyQ(int inventorySlotIndex)
        {
            if (inventorySlotIndex >= 0 && inventorySlotIndex < inventorySlots.Count)
            {
                hotkeySlotQ = inventorySlotIndex;
                quickUseItemY = inventorySlots[inventorySlotIndex].item;
                OnQuickUseChanged?.Invoke();
            }
        }
        
        public void SetHotkeyE(int inventorySlotIndex)
        {
            if (inventorySlotIndex >= 0 && inventorySlotIndex < inventorySlots.Count)
            {
                hotkeySlotE = inventorySlotIndex;
                quickUseItemX = inventorySlots[inventorySlotIndex].item;
                OnQuickUseChanged?.Invoke();
            }
        }
        
        public void ClearHotkeyQ()
        {
            hotkeySlotQ = -1;
            quickUseItemY = null;
            OnQuickUseChanged?.Invoke();
        }
        
        public void ClearHotkeyE()
        {
            hotkeySlotE = -1;
            quickUseItemX = null;
            OnQuickUseChanged?.Invoke();
        }
        
        public InventorySlot GetHotkeySlotQ()
        {
            if (hotkeySlotQ >= 0 && hotkeySlotQ < inventorySlots.Count)
                return inventorySlots[hotkeySlotQ];
            return new InventorySlot(); // Empty slot
        }
        
        public InventorySlot GetHotkeySlotE()
        {
            if (hotkeySlotE >= 0 && hotkeySlotE < inventorySlots.Count)
                return inventorySlots[hotkeySlotE];
            return new InventorySlot(); // Empty slot
        }
        
        // Getters for UI
        public List<InventorySlot> GetInventorySlots() => inventorySlots;
        public EquipmentSlots GetEquipment() => equipment;
        public int GetInventorySize() => inventorySize;
        public int GetHotbarSize() => hotbarSize;
        public Item GetQuickUseItemY() => quickUseItemY;
        public Item GetQuickUseItemX() => quickUseItemX;
        public int GetHotkeySlotQIndex() => hotkeySlotQ;
        public int GetHotkeySlotEIndex() => hotkeySlotE;
    }
}