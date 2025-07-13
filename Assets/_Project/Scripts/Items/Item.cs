using UnityEngine;
using ALittleFolkTale.Characters;

namespace ALittleFolkTale.Items
{
    public abstract class Item : MonoBehaviour, IInteractable
    {
        [Header("Item Base Settings")]
        [SerializeField] protected ItemData itemData;
        [SerializeField] protected int quantity = 1;
        [SerializeField] protected bool isOnGround = true;
        [SerializeField] protected float groundOffset = 0.1f;
        
        [Header("Ground Item Settings")]
        [SerializeField] protected float bobSpeed = 1f;
        [SerializeField] protected float bobHeight = 0.2f;
        [SerializeField] protected float rotationSpeed = 45f;
        
        protected Vector3 startPosition;
        protected bool isPickedUp = false;
        protected Collider itemCollider;
        protected Renderer itemRenderer;

        public ItemData Data => itemData;
        public int Quantity => quantity;
        public bool IsOnGround => isOnGround;

        protected virtual void Start()
        {
            if (isOnGround)
            {
                PositionOnGround();
                startPosition = transform.position;
            }
            
            itemCollider = GetComponent<Collider>();
            itemRenderer = GetComponent<Renderer>();
            
            // Make sure collider is trigger for pickup
            if (itemCollider != null)
            {
                itemCollider.isTrigger = true;
            }
        }

        protected virtual void Update()
        {
            if (isOnGround && !isPickedUp)
            {
                AnimateGroundItem();
            }
        }

        protected virtual void PositionOnGround()
        {
            // Raycast down to find ground
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up * 5f, Vector3.down, out hit, 10f))
            {
                transform.position = hit.point + Vector3.up * groundOffset;
            }
            else
            {
                // Fallback to current Y position with offset
                Vector3 pos = transform.position;
                pos.y = groundOffset;
                transform.position = pos;
            }
        }

        protected virtual void AnimateGroundItem()
        {
            // Bob up and down
            float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = new Vector3(startPosition.x, newY, startPosition.z);
            
            // Rotate slowly
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }

        public virtual void Interact(PlayerController player)
        {
            if (!isPickedUp)
            {
                PickupItem(player);
            }
        }

        protected virtual void PickupItem(PlayerController player)
        {
            if (isPickedUp) return;
            
            // Try to add to inventory
            var inventory = player.GetComponent<Inventory>();
            if (inventory != null)
            {
                bool success = inventory.AddItem(this);
                if (success)
                {
                    isPickedUp = true;
                    OnPickup(player);
                    
                    // Hide the ground item
                    gameObject.SetActive(false);
                    
                    // Destroy after a short delay to allow for any effects
                    Destroy(gameObject, 0.1f);
                }
                else
                {
                    ShowInventoryFullMessage();
                }
            }
        }

        protected virtual void OnPickup(PlayerController player)
        {
            // Override in derived classes for specific pickup effects
            CreatePickupEffect();
            
            // Play pickup sound
            // PlaySoundPlaceholder($"{itemData.itemName} Pickup", 0.4f);
        }

        protected virtual void CreatePickupEffect()
        {
            // Simple pickup effect
            GameObject effect = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            effect.name = "ItemPickupEffect";
            effect.transform.position = transform.position;
            effect.transform.localScale = Vector3.one * 0.3f;
            
            Renderer effectRenderer = effect.GetComponent<Renderer>();
            effectRenderer.material.color = new Color(1f, 1f, 0.8f, 0.7f);
            
            Destroy(effect.GetComponent<Collider>());
            
            StartCoroutine(AnimatePickupEffect(effect));
        }

        protected virtual System.Collections.IEnumerator AnimatePickupEffect(GameObject effect)
        {
            float duration = 0.5f;
            float elapsed = 0f;
            Vector3 startScale = effect.transform.localScale;
            Vector3 startPos = effect.transform.position;
            Renderer renderer = effect.GetComponent<Renderer>();
            Color startColor = renderer.material.color;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
                
                effect.transform.localScale = Vector3.Lerp(startScale, startScale * 2f, progress);
                effect.transform.position = Vector3.Lerp(startPos, startPos + Vector3.up * 1f, progress);
                
                Color color = startColor;
                color.a = Mathf.Lerp(startColor.a, 0f, progress);
                renderer.material.color = color;
                
                yield return null;
            }
            
            Destroy(effect);
        }

        protected virtual void ShowInventoryFullMessage()
        {
            // TODO: Show UI message that inventory is full
            Debug.Log("Inventory is full!");
        }

        // For creating items in inventory
        public virtual Item CreateInventoryInstance()
        {
            GameObject itemObj = new GameObject(itemData.itemName);
            Item newItem = itemObj.AddComponent(this.GetType()) as Item;
            newItem.itemData = this.itemData;
            newItem.quantity = this.quantity;
            newItem.isOnGround = false;
            return newItem;
        }

        private void OnDrawGizmosSelected()
        {
            if (isOnGround)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, 1f);
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 2f);
            }
        }
    }
}