using UnityEngine;

namespace ALittleFolkTale.Core
{
    public class CameraController : MonoBehaviour
    {
        [Header("Target Settings")]
        [SerializeField] private Transform target;
        [SerializeField] private string targetTag = "Player";

        [Header("Camera Settings")]
        [SerializeField] private float cameraHeight = 12f;
        [SerializeField] private float cameraDistance = 8f;
        [SerializeField] private float cameraAngle = 40f;
        [SerializeField] private float smoothSpeed = 8f;
        [SerializeField] private float viewportOffset = -4f;
        
        [Header("Camera Shake")]
        [SerializeField] private float shakeDuration = 0f;
        [SerializeField] private float shakeAmount = 0.1f;
        [SerializeField] private float decreaseFactor = 1.0f;

        [Header("Bounds")]
        [SerializeField] private bool useBounds = false;
        [SerializeField] private Vector2 minBounds;
        [SerializeField] private Vector2 maxBounds;

        private Vector3 offset;
        private Vector3 velocity = Vector3.zero;
        private Vector3 originalPosition;
        
        // Singleton instance
        private static CameraController instance;
        public static CameraController Instance
        {
            get
            {
                if (instance == null)
                    instance = FindFirstObjectByType<CameraController>();
                return instance;
            }
        }

        private void Awake()
        {
            instance = this;
        }
        
        private void Start()
        {
            if (target == null)
            {
                FindTarget();
            }

            CalculateOffset();
            
            // Snap to initial position without smoothing
            if (target != null)
            {
                transform.position = target.position + offset;
            }
        }
        
        private void FindTarget()
        {
            // Try to find by tag first
            if (!string.IsNullOrEmpty(targetTag))
            {
                GameObject player = GameObject.FindGameObjectWithTag(targetTag);
                if (player != null)
                {
                    target = player.transform;
                    return;
                }
            }
            
            // Fallback to finding PlayerController
            var playerController = FindFirstObjectByType<Characters.PlayerController>();
            if (playerController != null)
            {
                target = playerController.transform;
            }
        }

        private void CalculateOffset()
        {
            float angleRad = cameraAngle * Mathf.Deg2Rad;
            float horizontalDistance = cameraDistance * Mathf.Cos(angleRad);
            
            // Adjust the offset to center player better in viewport
            // Move camera back on Z axis to center player (negative values move camera back)
            offset = new Vector3(0, cameraHeight, -horizontalDistance + viewportOffset);
            
            transform.rotation = Quaternion.Euler(cameraAngle, 0, 0);
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                FindTarget();
                return;
            }

            Vector3 desiredPosition = target.position + offset;

            if (useBounds)
            {
                desiredPosition.x = Mathf.Clamp(desiredPosition.x, minBounds.x, maxBounds.x);
                desiredPosition.z = Mathf.Clamp(desiredPosition.z, minBounds.y, maxBounds.y);
            }

            // Smooth camera movement
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, 1f / smoothSpeed);
            
            // Apply camera shake if active
            if (shakeDuration > 0)
            {
                transform.position += Random.insideUnitSphere * shakeAmount;
                shakeDuration -= Time.deltaTime * decreaseFactor;
            }
            else
            {
                shakeDuration = 0f;
            }
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        public void SetBounds(Vector2 min, Vector2 max)
        {
            minBounds = min;
            maxBounds = max;
            useBounds = true;
        }
        
        public void ShakeCamera(float duration = 0.5f, float amount = 0.3f)
        {
            shakeDuration = duration;
            shakeAmount = amount;
        }
        
        public void QuickShake()
        {
            ShakeCamera(0.1f, 0.05f);
        }
        
        public void HeavyShake()
        {
            ShakeCamera(0.2f, 0.1f);
        }
        
        public void UpdateCameraSettings(float? height = null, float? distance = null, float? angle = null, float? offset = null)
        {
            if (height.HasValue) cameraHeight = height.Value;
            if (distance.HasValue) cameraDistance = distance.Value;
            if (angle.HasValue) cameraAngle = angle.Value;
            if (offset.HasValue) viewportOffset = offset.Value;
            
            CalculateOffset();
        }

        private void OnDrawGizmosSelected()
        {
            if (useBounds)
            {
                Gizmos.color = Color.yellow;
                Vector3 center = new Vector3((minBounds.x + maxBounds.x) / 2f, transform.position.y, (minBounds.y + maxBounds.y) / 2f);
                Vector3 size = new Vector3(maxBounds.x - minBounds.x, 0.1f, maxBounds.y - minBounds.y);
                Gizmos.DrawWireCube(center, size);
            }
        }
    }
}