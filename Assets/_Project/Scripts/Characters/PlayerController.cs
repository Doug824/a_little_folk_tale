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
        [SerializeField] private float rollSpeed = 8f;
        [SerializeField] private float rollDuration = 0.5f;
        [SerializeField] private float rollStaminaCost = 10f;

        [Header("Combat Settings")]
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private int attackDamage = 10;
        [SerializeField] private float attackCooldown = 0.5f;
        [SerializeField] private float attackStaminaCost = 5f;

        [Header("Stats")]
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private int currentHealth;
        [SerializeField] private float maxStamina = 100f;
        [SerializeField] private float currentStamina;
        [SerializeField] private float staminaRegenRate = 10f;

        private CharacterController characterController;
        private Animator animator;
        private Camera mainCamera;

        private Vector2 movementInput;
        private Vector3 movementDirection;
        private bool isRolling = false;
        private float rollTimer = 0f;
        private float attackTimer = 0f;

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

            SetupInputActions();
        }

        private void Start()
        {
            currentHealth = maxHealth;
            currentStamina = maxStamina;
        }

        private void SetupInputActions()
        {
            moveAction = playerInput.actions["Move"];
            attackAction = playerInput.actions["Attack"];
            rollAction = playerInput.actions["Roll"];
            interactAction = playerInput.actions["Interact"];

            attackAction.performed += OnAttack;
            rollAction.performed += OnRoll;
            interactAction.performed += OnInteract;
        }

        private void Update()
        {
            HandleMovementInput();
            HandleMovement();
            HandleRotation();
            UpdateTimers();
            RegenerateStamina();
        }

        private void HandleMovementInput()
        {
            movementInput = moveAction.ReadValue<Vector2>();
            
            Vector3 forward = mainCamera.transform.forward;
            Vector3 right = mainCamera.transform.right;
            
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            movementDirection = forward * movementInput.y + right * movementInput.x;
        }

        private void HandleMovement()
        {
            if (isRolling)
            {
                characterController.Move(transform.forward * rollSpeed * Time.deltaTime);
            }
            else if (movementDirection.magnitude > 0.1f)
            {
                characterController.Move(movementDirection * moveSpeed * Time.deltaTime);
                
                if (animator != null)
                {
                    animator.SetFloat("Speed", movementDirection.magnitude);
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
                    if (animator != null)
                    {
                        animator.SetBool("IsRolling", false);
                    }
                }
            }

            if (attackTimer > 0f)
            {
                attackTimer -= Time.deltaTime;
            }
        }

        private void RegenerateStamina()
        {
            if (currentStamina < maxStamina && !isRolling)
            {
                currentStamina = Mathf.Min(maxStamina, currentStamina + staminaRegenRate * Time.deltaTime);
            }
        }

        private void OnAttack(InputAction.CallbackContext context)
        {
            if (attackTimer <= 0f && currentStamina >= attackStaminaCost && !isRolling)
            {
                PerformAttack();
                currentStamina -= attackStaminaCost;
                attackTimer = attackCooldown;
            }
        }

        private void OnRoll(InputAction.CallbackContext context)
        {
            if (!isRolling && currentStamina >= rollStaminaCost && movementDirection.magnitude > 0.1f)
            {
                isRolling = true;
                rollTimer = rollDuration;
                currentStamina -= rollStaminaCost;
                
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

            Collider[] hitColliders = Physics.OverlapSphere(transform.position + transform.forward * attackRange * 0.5f, attackRange);
            foreach (Collider col in hitColliders)
            {
                if (col.CompareTag("Enemy"))
                {
                    Debug.Log($"Hit enemy: {col.name} for {attackDamage} damage");
                }
            }
        }

        public void TakeDamage(int damage)
        {
            if (isRolling) return;

            currentHealth -= damage;
            
            if (currentHealth <= 0)
            {
                Die();
            }
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

        private void OnDestroy()
        {
            if (attackAction != null) attackAction.performed -= OnAttack;
            if (rollAction != null) rollAction.performed -= OnRoll;
            if (interactAction != null) interactAction.performed -= OnInteract;
        }
    }

    public interface IInteractable
    {
        void Interact(PlayerController player);
    }
}