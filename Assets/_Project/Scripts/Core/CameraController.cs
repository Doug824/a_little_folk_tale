using UnityEngine;

namespace ALittleFolkTale.Core
{
    public class CameraController : MonoBehaviour
    {
        [Header("Target Settings")]
        [SerializeField] private Transform target;
        [SerializeField] private string targetTag = "Player";

        [Header("Camera Settings")]
        [SerializeField] private float cameraHeight = 15f;
        [SerializeField] private float cameraDistance = 10f;
        [SerializeField] private float cameraAngle = 45f;
        [SerializeField] private float smoothSpeed = 5f;

        [Header("Bounds")]
        [SerializeField] private bool useBounds = false;
        [SerializeField] private Vector2 minBounds;
        [SerializeField] private Vector2 maxBounds;

        private Vector3 offset;
        private Vector3 velocity = Vector3.zero;

        private void Start()
        {
            if (target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag(targetTag);
                if (player != null)
                {
                    target = player.transform;
                }
            }

            CalculateOffset();
        }

        private void CalculateOffset()
        {
            float angleRad = cameraAngle * Mathf.Deg2Rad;
            float horizontalDistance = cameraDistance * Mathf.Cos(angleRad);
            
            offset = new Vector3(0, cameraHeight, -horizontalDistance);
            
            transform.rotation = Quaternion.Euler(cameraAngle, 0, 0);
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag(targetTag);
                if (player != null)
                {
                    target = player.transform;
                }
                else
                {
                    return;
                }
            }

            Vector3 desiredPosition = target.position + offset;

            if (useBounds)
            {
                desiredPosition.x = Mathf.Clamp(desiredPosition.x, minBounds.x, maxBounds.x);
                desiredPosition.z = Mathf.Clamp(desiredPosition.z, minBounds.y, maxBounds.y);
            }

            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, 1f / smoothSpeed);
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