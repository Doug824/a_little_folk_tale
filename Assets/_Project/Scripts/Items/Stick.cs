using UnityEngine;
using ALittleFolkTale.Characters;

namespace ALittleFolkTale.Items
{
    public class Stick : Item
    {
        [Header("Weapon Stats")]
        [SerializeField] private int attackDamage = 15;
        [SerializeField] private float attackRange = 2f;
        [SerializeField] private float attackSpeed = 1.2f;

        public int AttackDamage => attackDamage;
        public float AttackRange => attackRange;
        public float AttackSpeed => attackSpeed;

        protected override void Start()
        {
            // Create default ItemData if none assigned
            if (itemData == null)
            {
                itemData = ScriptableObject.CreateInstance<ItemData>();
                itemData.itemName = "Wooden Stick";
                itemData.description = "A simple wooden stick. Better than bare hands!";
                itemData.itemType = ItemType.Weapon;
                itemData.equipmentSlot = EquipmentSlot.MainHand;
                itemData.maxStackSize = 1;
                itemData.isConsumable = false;
                
                // Set weapon stats
                itemData.stats.damage = attackDamage;
                itemData.stats.attackSpeed = attackSpeed;
            }

            base.Start();
            
            // Create visual if none exists
            if (GetComponent<Renderer>() == null)
            {
                CreateStickVisual();
            }
        }

        private void CreateStickVisual()
        {
            // Create stick using a scaled cylinder
            GameObject stickVisual = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            stickVisual.name = "StickVisual";
            stickVisual.transform.SetParent(transform);
            stickVisual.transform.localPosition = Vector3.zero;
            stickVisual.transform.localRotation = Quaternion.Euler(0, 0, 0);
            stickVisual.transform.localScale = new Vector3(0.1f, 0.8f, 0.1f);
            
            // Color it brown
            Renderer renderer = stickVisual.GetComponent<Renderer>();
            renderer.material.color = new Color(0.6f, 0.3f, 0.1f); // Brown color
            
            // Remove the visual collider (parent has the trigger collider)
            Destroy(stickVisual.GetComponent<Collider>());
            
            // Add our own collider for pickup
            if (GetComponent<Collider>() == null)
            {
                CapsuleCollider col = gameObject.AddComponent<CapsuleCollider>();
                col.height = 1.6f;
                col.radius = 0.2f;
                col.isTrigger = true;
            }
        }

        protected override void OnPickup(PlayerController player)
        {
            base.OnPickup(player);
            
            Debug.Log($"Picked up {itemData.itemName}! Damage: {attackDamage}, Range: {attackRange}");
        }

        // Static method to create sticks easily
        public static GameObject CreateStick(Vector3 position)
        {
            GameObject stick = new GameObject("Wooden Stick");
            stick.transform.position = position;
            
            Stick stickComponent = stick.AddComponent<Stick>();
            
            return stick;
        }

        // Method to get weapon stats for combat system
        public ItemStats GetWeaponStats()
        {
            return itemData.stats;
        }
    }
}