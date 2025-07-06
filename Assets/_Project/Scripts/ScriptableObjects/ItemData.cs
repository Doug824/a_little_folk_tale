using UnityEngine;

namespace ALittleFolkTale.Items
{
    [CreateAssetMenu(fileName = "New Item", menuName = "A Little Folk Tale/Items/Item Data")]
    public class ItemData : ScriptableObject
    {
        [Header("Basic Info")]
        public string itemName;
        [TextArea(3, 5)]
        public string description;
        public Sprite icon;
        public GameObject worldPrefab;

        [Header("Item Type")]
        public ItemType itemType;
        public EquipmentSlot equipmentSlot;

        [Header("Stack Settings")]
        public bool isStackable = false;
        public int maxStackSize = 1;

        [Header("Stats")]
        public int value = 0;
        public ItemStats stats;

        [Header("Use Effects")]
        public bool isConsumable = false;
        public ItemEffect[] useEffects;

        public virtual void Use(Characters.PlayerController player)
        {
            if (!isConsumable) return;

            foreach (var effect in useEffects)
            {
                effect.ApplyEffect(player);
            }

            Debug.Log($"Used {itemName}");
        }
    }

    public enum ItemType
    {
        Weapon,
        Tool,
        Consumable,
        KeyItem,
        Material,
        Equipment
    }

    public enum EquipmentSlot
    {
        None,
        MainHand,
        OffHand,
        Armor,
        Accessory
    }

    [System.Serializable]
    public class ItemStats
    {
        public int damage;
        public int defense;
        public float attackSpeed;
        public float moveSpeedBonus;
        public int healthBonus;
        public float staminaBonus;
    }

    [System.Serializable]
    public abstract class ItemEffect : ScriptableObject
    {
        public abstract void ApplyEffect(Characters.PlayerController player);
    }
}