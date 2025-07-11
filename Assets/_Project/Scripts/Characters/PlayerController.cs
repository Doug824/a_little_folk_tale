using UnityEngine;
using UnityEngine.InputSystem;

namespace ALittleFolkTale.Characters
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private float rollSpeed = 12f;
        [SerializeField] private float rollDuration = 0.4f;
        [SerializeField] private float rollStaminaCost = 15f;
        [SerializeField] private float rollCooldown = 0.3f;

        [Header("Combat Settings")]
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private int attackDamage = 10;
        [SerializeField] private float attackCooldown = 0.3f;
        [SerializeField] private float attackDuration = 0.3f;
        [SerializeField] private float attackStaminaCost = 8f;
        [SerializeField] private float attackMovementReduction = 0.3f;
        
        [Header("Debug Visualization")]
        [SerializeField] private bool showAttackRangeDebug = false;
        [SerializeField] private bool showDirectionArrow = true;

        [Header("Stats")]
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private int currentHealth;
        [SerializeField] private float maxStamina = 100f;
        [SerializeField] private float currentStamina;
        [SerializeField] private float staminaRegenRate = 15f;
        [SerializeField] private float staminaRegenDelay = 1f;

        private CharacterController characterController;
        private Animator animator;
        private Camera mainCamera;

        private Vector2 movementInput;
        private Vector3 movementDirection;
        private Vector3 rollDirection;
        private bool isRolling = false;
        private bool isAttacking = false;
        private float rollTimer = 0f;
        private float rollCooldownTimer = 0f;
        private float attackTimer = 0f;
        private float attackDurationTimer = 0f;
        private float staminaRegenTimer = 0f;
        
        private GameObject persistentDirectionArrow;

        private PlayerInput playerInput;
        private InputAction moveAction;
        private InputAction attackAction;
        private InputAction rollAction;
        private InputAction interactAction;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            animator = GetComponentInChildren<Animator>();
            mainCamera = Camera.main;
            
            // Create PlayerInput component if it doesn't exist
            if (GetComponent<PlayerInput>() == null)
            {
                playerInput = gameObject.AddComponent<PlayerInput>();
            }
            else
            {
                playerInput = GetComponent<PlayerInput>();
            }

            // Don't setup input actions here - wait for Start()
        }

        private void Start()
        {
            currentHealth = maxHealth;
            currentStamina = maxStamina;
            
            // Setup input actions after everything is initialized
            SetupInputActions();
            
            // Create persistent direction arrow
            if (showDirectionArrow)
            {
                CreatePersistentDirectionArrow();
            }
        }

        private void SetupInputActions()
        {
            Debug.Log("Setting up input actions...");
            
            if (playerInput == null)
            {
                Debug.LogError("PlayerInput component is null!");
                return;
            }
            
            if (playerInput.actions == null)
            {
                Debug.LogError("PlayerInput.actions is null!");
                return;
            }
            
            Debug.Log($"PlayerInput found with {playerInput.actions.actionMaps.Count} action maps");
            
            try
            {
                moveAction = playerInput.actions["Move"];
                attackAction = playerInput.actions["Attack"];
                rollAction = playerInput.actions["Roll"];
                interactAction = playerInput.actions["Interact"];
                
                Debug.Log("All input actions found successfully!");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error setting up input actions: {e.Message}");
                return;
            }

            // Use lambda expressions to avoid method resolution issues
            attackAction.performed += ctx => OnAttack(ctx);
            rollAction.performed += ctx => OnRoll(ctx);
            interactAction.performed += ctx => OnInteract(ctx);
            
            Debug.Log("Input action callbacks subscribed successfully");
        }

        private void Update()
        {
            HandleMovementInput();
            HandleDirectInput(); // Add direct keyboard input fallback
            HandleMovement();
            HandleRotation();
            UpdateTimers();
            RegenerateStamina();
            UpdatePersistentDirectionArrow();
        }

        private void HandleMovementInput()
        {
            // Use keyboard input directly (WASD)
            Vector2 keyboardInput = Vector2.zero;
            if (Input.GetKey(KeyCode.W)) keyboardInput.y += 1f;
            if (Input.GetKey(KeyCode.S)) keyboardInput.y -= 1f;
            if (Input.GetKey(KeyCode.A)) keyboardInput.x -= 1f;
            if (Input.GetKey(KeyCode.D)) keyboardInput.x += 1f;
            
            movementInput = keyboardInput;
            
            // Try Input System if available and no keyboard input
            if (movementInput.magnitude <= 0.1f && moveAction != null)
            {
                movementInput = moveAction.ReadValue<Vector2>();
            }
            
            if (mainCamera == null)
            {
                movementDirection = Vector3.zero;
                return;
            }
            
            // Calculate movement direction relative to camera
            Vector3 forward = mainCamera.transform.forward;
            Vector3 right = mainCamera.transform.right;
            
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            movementDirection = forward * movementInput.y + right * movementInput.x;
        }
        
        private void HandleDirectInput()
        {
            // Direct keyboard input fallback for combat actions
            if (Input.GetMouseButtonDown(0)) // Left mouse button
            {
                TryAttack();
            }
            
            if (Input.GetKeyDown(KeyCode.Space)) // Space key
            {
                TryRoll();
            }
            
            if (Input.GetKeyDown(KeyCode.E)) // E key
            {
                TryInteract();
            }
        }
        
        private void TryAttack()
        {
            if (attackTimer <= 0f && currentStamina >= attackStaminaCost && !isRolling && !isAttacking)
            {
                Debug.Log("🗡️ Player attacked!");
                PerformAttack();
                currentStamina -= attackStaminaCost;
                attackTimer = attackCooldown;
                isAttacking = true;
                attackDurationTimer = attackDuration;
                staminaRegenTimer = staminaRegenDelay;
                
                if (animator != null)
                {
                    animator.SetBool("IsAttacking", true);
                }
            }
            else
            {
                Debug.Log($"❌ Attack blocked - Timer:{attackTimer:F1} Stamina:{currentStamina:F0}/{attackStaminaCost} Rolling:{isRolling} Attacking:{isAttacking}");
            }
        }
        
        private void TryRoll()
        {
            if (!isRolling && rollCooldownTimer <= 0f && currentStamina >= rollStaminaCost && movementDirection.magnitude > 0.1f)
            {
                Debug.Log("🎯 Player rolled!");
                isRolling = true;
                rollTimer = rollDuration;
                rollDirection = movementDirection.normalized;
                currentStamina -= rollStaminaCost;
                staminaRegenTimer = staminaRegenDelay;
                
                // Play roll sound
                PlaySoundPlaceholder("Player Roll", 0.4f);
                
                if (animator != null)
                {
                    animator.SetBool("IsRolling", true);
                }
            }
            else
            {
                Debug.Log($"❌ Roll blocked - Rolling:{isRolling} Cooldown:{rollCooldownTimer:F1} Stamina:{currentStamina:F0}/{rollStaminaCost} Movement:{movementDirection.magnitude:F1}");
            }
        }
        
        private void TryInteract()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 2f);
            foreach (Collider col in colliders)
            {
                IInteractable interactable = col.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact(this);
                    break;
                }
            }
        }

        private void HandleMovement()
        {
            if (characterController == null) return;
            
            float currentMoveSpeed = moveSpeed;
            
            // Reduce movement during attack
            if (isAttacking)
            {
                currentMoveSpeed *= attackMovementReduction;
            }
            
            if (isRolling)
            {
                characterController.Move(rollDirection * rollSpeed * Time.deltaTime);
            }
            else if (movementDirection.magnitude > 0.1f)
            {
                Vector3 movement = movementDirection * currentMoveSpeed * Time.deltaTime;
                characterController.Move(movement);
                
                if (animator != null)
                {
                    animator.SetFloat("Speed", movementDirection.magnitude * (isAttacking ? 0.5f : 1f));
                }
            }
            else
            {
                if (animator != null)
                {
                    animator.SetFloat("Speed", 0f);
                }
            }
        }

        private void HandleRotation()
        {
            if (!isRolling && movementDirection.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        private void UpdateTimers()
        {
            if (isRolling)
            {
                rollTimer -= Time.deltaTime;
                if (rollTimer <= 0f)
                {
                    isRolling = false;
                    rollCooldownTimer = rollCooldown;
                    if (animator != null)
                    {
                        animator.SetBool("IsRolling", false);
                    }
                }
            }

            if (isAttacking)
            {
                attackDurationTimer -= Time.deltaTime;
                if (attackDurationTimer <= 0f)
                {
                    isAttacking = false;
                    if (animator != null)
                    {
                        animator.SetBool("IsAttacking", false);
                    }
                }
            }

            if (attackTimer > 0f)
            {
                attackTimer -= Time.deltaTime;
            }
            
            if (rollCooldownTimer > 0f)
            {
                rollCooldownTimer -= Time.deltaTime;
            }
            
            if (staminaRegenTimer > 0f)
            {
                staminaRegenTimer -= Time.deltaTime;
            }
        }

        private void RegenerateStamina()
        {
            if (currentStamina < maxStamina && !isRolling && !isAttacking && staminaRegenTimer <= 0f)
            {
                currentStamina = Mathf.Min(maxStamina, currentStamina + staminaRegenRate * Time.deltaTime);
            }
        }

        private void OnAttack(InputAction.CallbackContext context)
        {
            if (attackTimer <= 0f && currentStamina >= attackStaminaCost && !isRolling && !isAttacking)
            {
                PerformAttack();
                currentStamina -= attackStaminaCost;
                attackTimer = attackCooldown;
                isAttacking = true;
                attackDurationTimer = attackDuration;
                staminaRegenTimer = staminaRegenDelay;
                
                if (animator != null)
                {
                    animator.SetBool("IsAttacking", true);
                }
            }
        }

        private void OnRoll(InputAction.CallbackContext context)
        {
            if (!isRolling && rollCooldownTimer <= 0f && currentStamina >= rollStaminaCost && movementDirection.magnitude > 0.1f)
            {
                isRolling = true;
                rollTimer = rollDuration;
                rollDirection = movementDirection.normalized;
                currentStamina -= rollStaminaCost;
                staminaRegenTimer = staminaRegenDelay;
                
                // Play roll sound
                PlaySoundPlaceholder("Player Roll", 0.4f);
                
                if (animator != null)
                {
                    animator.SetBool("IsRolling", true);
                }
            }
        }

        private void OnInteract(InputAction.CallbackContext context)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 2f);
            foreach (Collider col in colliders)
            {
                IInteractable interactable = col.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact(this);
                    break;
                }
            }
        }

        private void PerformAttack()
        {
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
            
            // Play attack sound placeholder
            PlaySoundPlaceholder("Attack Swing", 0.3f);

            // Create attack area in front of player
            Vector3 attackPosition = transform.position + transform.forward * attackRange * 0.5f;
            
            // Show attack range visualization
            CreateAttackRangeEffect(attackPosition);
            
            Collider[] hitColliders = Physics.OverlapSphere(attackPosition, attackRange);
            
            bool hitSomething = false;
            foreach (Collider col in hitColliders)
            {
                // Check for Enemy component instead of relying on tags
                var enemy = col.GetComponent<Enemy>();
                if (enemy != null || col.name.Contains("Enemy"))
                {
                    hitSomething = true;
                    
                    // Add impact effect
                    CreateHitEffect(col.transform.position);
                    
                    // Play hit sound
                    PlaySoundPlaceholder("Attack Hit", 0.5f);
                    
                    // Apply damage
                    if (enemy != null)
                    {
                        enemy.TakeDamage(attackDamage);
                        Debug.Log($"Hit {col.name} for {attackDamage} damage!");
                    }
                    else
                    {
                        Debug.Log($"Hit enemy: {col.name} for {attackDamage} damage");
                    }
                    
                    // Add knockback
                    Vector3 knockbackDirection = (col.transform.position - transform.position).normalized;
                    var rb = col.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.AddForce(knockbackDirection * 5f, ForceMode.Impulse);
                    }
                }
            }
            
            // Play whiff sound if no hit
            if (!hitSomething)
            {
                PlaySoundPlaceholder("Attack Whiff", 0.2f);
            }
        }
        
        private void CreateAttackRangeEffect(Vector3 attackPosition)
        {
            // Create attack cone visualization
            GameObject rangeEffect = CreateAttackCone(attackPosition);
            
            // Animate the range effect
            StartCoroutine(AnimateAttackRange(rangeEffect));
            
            // Don't create direction arrow on attack anymore - it's persistent now
        }
        
        private GameObject CreateAttackCone(Vector3 attackPosition)
        {
            // Create parent object for the cone effect
            GameObject coneParent = new GameObject("AttackConeEffect");
            coneParent.transform.position = transform.position;
            coneParent.transform.rotation = transform.rotation;
            
            // Create cone using a cylinder primitive stretched forward
            GameObject cone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cone.transform.SetParent(coneParent.transform);
            cone.name = "AttackCone";
            
            // Position and scale the cone
            cone.transform.localPosition = Vector3.forward * attackRange * 0.5f;
            cone.transform.localRotation = Quaternion.Euler(90f, 0f, 0f); // Rotate to point forward
            cone.transform.localScale = new Vector3(attackRange * 1.5f, attackRange * 0.7f, attackRange * 1.5f);
            
            Renderer renderer = cone.GetComponent<Renderer>();
            renderer.material.color = new Color(1f, 0.3f, 0f, 0.2f); // Orange, more transparent
            
            // Make it transparent
            renderer.material.SetFloat("_Mode", 3); // Transparent mode
            renderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            renderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            renderer.material.SetInt("_ZWrite", 0);
            renderer.material.DisableKeyword("_ALPHATEST_ON");
            renderer.material.EnableKeyword("_ALPHABLEND_ON");
            renderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            renderer.material.renderQueue = 3000;
            
            Destroy(cone.GetComponent<Collider>());
            
            return coneParent;
        }
        
        private void CreatePersistentDirectionArrow()
        {
            // Create persistent direction arrow
            persistentDirectionArrow = new GameObject("PersistentDirectionArrow");
            persistentDirectionArrow.transform.position = transform.position + Vector3.up * 2.5f;
            persistentDirectionArrow.transform.rotation = transform.rotation;
            
            // Arrow shaft
            GameObject shaft = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            shaft.transform.SetParent(persistentDirectionArrow.transform);
            shaft.transform.localPosition = Vector3.forward * 0.3f;
            shaft.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            shaft.transform.localScale = new Vector3(0.08f, 0.25f, 0.08f);
            
            // Arrow head (using cube as pyramid doesn't exist)
            GameObject head = GameObject.CreatePrimitive(PrimitiveType.Cube);
            head.transform.SetParent(persistentDirectionArrow.transform);
            head.transform.localPosition = Vector3.forward * 0.5f;
            head.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            head.transform.localScale = new Vector3(0.15f, 0.08f, 0.25f);
            
            // Color the arrow cyan for better visibility against green ground
            shaft.GetComponent<Renderer>().material.color = Color.cyan;
            head.GetComponent<Renderer>().material.color = Color.cyan;
            
            // Clean up colliders
            Destroy(shaft.GetComponent<Collider>());
            Destroy(head.GetComponent<Collider>());
        }
        
        private void UpdatePersistentDirectionArrow()
        {
            if (persistentDirectionArrow != null && showDirectionArrow)
            {
                // Update position and rotation to follow player
                persistentDirectionArrow.transform.position = transform.position + Vector3.up * 2.5f;
                persistentDirectionArrow.transform.rotation = transform.rotation;
            }
            else if (persistentDirectionArrow == null && showDirectionArrow)
            {
                // Create arrow if it doesn't exist but should
                CreatePersistentDirectionArrow();
            }
            else if (persistentDirectionArrow != null && !showDirectionArrow)
            {
                // Destroy arrow if it exists but shouldn't
                Destroy(persistentDirectionArrow);
                persistentDirectionArrow = null;
            }
        }
        
        
        private System.Collections.IEnumerator AnimateAttackRange(GameObject effect)
        {
            float duration = 0.3f;
            float elapsed = 0f;
            
            // Get the cone renderer (child object)
            Renderer coneRenderer = effect.GetComponentInChildren<Renderer>();
            if (coneRenderer != null)
            {
                Color startColor = coneRenderer.material.color;
                
                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    float progress = elapsed / duration;
                    
                    // Fade out
                    Color color = startColor;
                    color.a = startColor.a * (1f - progress);
                    coneRenderer.material.color = color;
                    
                    yield return null;
                }
            }
            
            Destroy(effect);
        }
        
        
        private void CreateHitEffect(Vector3 position)
        {
            // Create hit impact visual effect
            GameObject hitEffect = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            hitEffect.name = "HitEffect";
            hitEffect.transform.position = position + Vector3.up * 0.5f;
            hitEffect.transform.localScale = Vector3.one * 0.2f;
            
            Renderer renderer = hitEffect.GetComponent<Renderer>();
            renderer.material.color = Color.yellow;
            
            // Animate the effect
            StartCoroutine(AnimateHitEffect(hitEffect));
            
            Destroy(hitEffect.GetComponent<Collider>());
        }
        
        private System.Collections.IEnumerator AnimateHitEffect(GameObject effect)
        {
            float duration = 0.3f;
            float elapsed = 0f;
            Vector3 startScale = effect.transform.localScale;
            Color startColor = effect.GetComponent<Renderer>().material.color;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
                
                // Scale up then down
                float scale = Mathf.Sin(progress * Mathf.PI) * 2f;
                effect.transform.localScale = startScale * (1f + scale);
                
                // Fade out
                Color color = startColor;
                color.a = 1f - progress;
                effect.GetComponent<Renderer>().material.color = color;
                
                yield return null;
            }
            
            Destroy(effect);
        }
        
        private void PlaySoundPlaceholder(string soundName, float volume)
        {
            Debug.Log($"🔊 SOUND: {soundName} (Volume: {volume:F1})");
            
            // TODO: Replace with actual AudioManager.Instance.PlaySFX(soundName, volume);
            // For now, we'll create a simple audio placeholder system
        }

        public void TakeDamage(int damage)
        {
            if (isRolling) return;

            currentHealth -= damage;
            
            // Play damage sound and effect
            PlaySoundPlaceholder("Player Hit", 0.6f);
            CreateDamageEffect();
            
            Debug.Log($"Player took {damage} damage! Health: {currentHealth}/{maxHealth}");
            
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        
        private void CreateDamageEffect()
        {
            // Create damage visual effect around player
            GameObject damageEffect = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            damageEffect.name = "DamageEffect";
            damageEffect.transform.position = transform.position + Vector3.up;
            damageEffect.transform.localScale = Vector3.one * 1.5f;
            
            Renderer renderer = damageEffect.GetComponent<Renderer>();
            renderer.material.color = Color.red;
            
            StartCoroutine(AnimateDamageEffect(damageEffect));
            
            Destroy(damageEffect.GetComponent<Collider>());
        }
        
        private System.Collections.IEnumerator AnimateDamageEffect(GameObject effect)
        {
            float duration = 0.4f;
            float elapsed = 0f;
            Renderer renderer = effect.GetComponent<Renderer>();
            Color startColor = renderer.material.color;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
                
                // Expand and fade
                effect.transform.localScale = Vector3.one * (1.5f + progress * 0.5f);
                
                Color color = startColor;
                color.a = 1f - progress;
                renderer.material.color = color;
                
                yield return null;
            }
            
            Destroy(effect);
        }

        private void Die()
        {
            Debug.Log("Player died!");
        }

        public float GetHealthPercentage()
        {
            return (float)currentHealth / maxHealth;
        }

        public float GetStaminaPercentage()
        {
            return currentStamina / maxStamina;
        }

        private void OnDrawGizmosSelected()
        {
            if (showAttackRangeDebug)
            {
                // Draw attack cone visualization
                Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
                
                // Draw cone outline
                Vector3 coneCenter = transform.position + transform.forward * attackRange * 0.5f;
                float coneRadius = attackRange * 0.75f;
                
                // Draw the cone base circle
                DrawWireCircle(coneCenter, coneRadius, transform.up);
                
                // Draw cone lines from player to circle edge
                Vector3 right = transform.right * coneRadius;
                Vector3 up = transform.up * coneRadius;
                
                Gizmos.DrawLine(transform.position, coneCenter + right);
                Gizmos.DrawLine(transform.position, coneCenter - right);
                Gizmos.DrawLine(transform.position, coneCenter + up);
                Gizmos.DrawLine(transform.position, coneCenter - up);
                
                // Draw center line
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, coneCenter);
                
                // Draw direction arrow
                Gizmos.color = Color.cyan;
                Vector3 arrowPos = transform.position + Vector3.up * 2.5f;
                Vector3 arrowEnd = arrowPos + transform.forward * 0.8f;
                Gizmos.DrawLine(arrowPos, arrowEnd);
                
                // Arrow head
                Vector3 arrowLeft = arrowEnd - transform.forward * 0.2f + transform.right * 0.1f;
                Vector3 arrowRight = arrowEnd - transform.forward * 0.2f - transform.right * 0.1f;
                Gizmos.DrawLine(arrowEnd, arrowLeft);
                Gizmos.DrawLine(arrowEnd, arrowRight);
            }
        }
        
        private void DrawWireCircle(Vector3 center, float radius, Vector3 normal)
        {
            Vector3 right = Vector3.Cross(normal, Vector3.forward).normalized;
            if (right == Vector3.zero) right = Vector3.Cross(normal, Vector3.up).normalized;
            Vector3 forward = Vector3.Cross(right, normal).normalized;
            
            int segments = 20;
            Vector3 prevPoint = center + right * radius;
            
            for (int i = 1; i <= segments; i++)
            {
                float angle = (float)i / segments * Mathf.PI * 2f;
                Vector3 point = center + (right * Mathf.Cos(angle) + forward * Mathf.Sin(angle)) * radius;
                Gizmos.DrawLine(prevPoint, point);
                prevPoint = point;
            }
        }

        private void OnDestroy()
        {
            if (attackAction != null) attackAction.performed -= OnAttack;
            if (rollAction != null) rollAction.performed -= OnRoll;
            if (interactAction != null) interactAction.performed -= OnInteract;
            
            // Clean up persistent direction arrow
            if (persistentDirectionArrow != null)
            {
                Destroy(persistentDirectionArrow);
            }
        }
    }

    public interface IInteractable
    {
        void Interact(PlayerController player);
    }
}