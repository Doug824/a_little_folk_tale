using UnityEngine;

namespace ALittleFolkTale.Characters
{
    public class Enemy : MonoBehaviour
    {
        [Header("Enemy Stats")]
        [SerializeField] private int maxHealth = 30;
        [SerializeField] private int currentHealth;
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private int attackDamage = 5;
        [SerializeField] private float attackRange = 1.2f;
        [SerializeField] private float attackCooldown = 1.5f;
        
        private PlayerController target;
        private float attackTimer = 0f;
        private bool isDead = false;
        private Renderer enemyRenderer;
        private Color originalColor;
        
        private void Start()
        {
            currentHealth = maxHealth;
            target = FindFirstObjectByType<PlayerController>();
            enemyRenderer = GetComponent<Renderer>();
            if (enemyRenderer != null)
            {
                originalColor = enemyRenderer.material.color;
            }
        }
        
        private void Update()
        {
            if (isDead) return;
            
            UpdateTimers();
            
            if (target != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, target.transform.position);
                
                if (distanceToPlayer <= attackRange)
                {
                    AttemptAttack();
                }
                else if (distanceToPlayer <= 8f) // Chase range
                {
                    ChasePlayer();
                }
            }
        }
        
        private void ChasePlayer()
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            direction.y = 0f; // Keep movement on ground plane
            
            transform.position += direction * moveSpeed * Time.deltaTime;
            transform.LookAt(target.transform.position);
        }
        
        private void AttemptAttack()
        {
            if (attackTimer <= 0f)
            {
                PerformAttack();
                attackTimer = attackCooldown;
            }
        }
        
        private void PerformAttack()
        {
            if (target != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, target.transform.position);
                if (distanceToPlayer <= attackRange)
                {
                    target.TakeDamage(attackDamage);
                    Debug.Log($"{gameObject.name} attacked player for {attackDamage} damage!");
                    
                    // Visual feedback for attack
                    if (enemyRenderer != null)
                    {
                        StartCoroutine(FlashColor(Color.yellow, 0.2f));
                    }
                }
            }
        }
        
        private void UpdateTimers()
        {
            if (attackTimer > 0f)
            {
                attackTimer -= Time.deltaTime;
            }
        }
        
        public void TakeDamage(int damage)
        {
            if (isDead) return;
            
            currentHealth -= damage;
            Debug.Log($"{gameObject.name} took {damage} damage! Health: {currentHealth}/{maxHealth}");
            
            // Flash red when hit
            if (enemyRenderer != null)
            {
                StartCoroutine(FlashColor(Color.red, 0.3f));
            }
            
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        
        private void Die()
        {
            isDead = true;
            Debug.Log($"{gameObject.name} died!");
            
            // Simple death effect
            if (enemyRenderer != null)
            {
                enemyRenderer.material.color = Color.gray;
            }
            
            // Disable movement and collision
            GetComponent<Collider>().enabled = false;
            
            // Remove after delay
            Destroy(gameObject, 2f);
        }
        
        private System.Collections.IEnumerator FlashColor(Color flashColor, float duration)
        {
            if (enemyRenderer != null)
            {
                enemyRenderer.material.color = flashColor;
                yield return new WaitForSeconds(duration);
                enemyRenderer.material.color = originalColor;
            }
        }
        
        public bool IsDead()
        {
            return isDead;
        }
        
        public float GetHealthPercentage()
        {
            return (float)currentHealth / maxHealth;
        }
    }
}