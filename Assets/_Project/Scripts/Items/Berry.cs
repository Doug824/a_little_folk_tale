using UnityEngine;
using ALittleFolkTale.Characters;

namespace ALittleFolkTale.Items
{
    public class Berry : Item
    {
        [Header("Berry Settings")]
        [SerializeField] private int healAmount = 25;
        public int HealAmount => healAmount;

        protected override void Start()
        {
            // Create default ItemData if none assigned
            if (itemData == null)
            {
                itemData = ScriptableObject.CreateInstance<ItemData>();
                itemData.itemName = "Healing Berry";
                itemData.description = "A sweet berry that restores health when consumed.";
                itemData.itemType = ItemType.Consumable;
                itemData.maxStackSize = 10;
                itemData.isConsumable = true;
            }

            base.Start();
            
            // Create berry visual if none exists
            if (GetComponent<Renderer>() == null)
            {
                CreateBerryVisual();
            }
        }

        private void CreateBerryVisual()
        {
            // Create berry using a scaled sphere
            GameObject berryVisual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            berryVisual.name = "BerryVisual";
            berryVisual.transform.SetParent(transform);
            berryVisual.transform.localPosition = Vector3.zero;
            berryVisual.transform.localScale = Vector3.one * 0.4f;
            
            // Color it red/pink
            Renderer renderer = berryVisual.GetComponent<Renderer>();
            renderer.material.color = new Color(0.9f, 0.2f, 0.3f, 1f);
            
            // Remove the visual collider (parent has the trigger collider)
            Destroy(berryVisual.GetComponent<Collider>());
            
            // Add our own collider for pickup
            if (GetComponent<Collider>() == null)
            {
                SphereCollider col = gameObject.AddComponent<SphereCollider>();
                col.radius = 0.3f;
                col.isTrigger = true;
            }
        }
        
        // Static method to create berries easily
        public static GameObject CreateBerry(Vector3 position, int healAmount = 25)
        {
            GameObject berry = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            berry.name = "Healing Berry";
            berry.transform.position = position;
            berry.transform.localScale = Vector3.one * 0.4f;
            
            // Add Berry component
            Berry berryComponent = berry.AddComponent<Berry>();
            berryComponent.healAmount = healAmount;
            
            return berry;
        }
    }
}