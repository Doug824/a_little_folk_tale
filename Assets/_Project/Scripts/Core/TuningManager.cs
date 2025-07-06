using UnityEngine;
using ALittleFolkTale.Characters;

namespace ALittleFolkTale.Core
{
    public class TuningManager : MonoBehaviour
    {
        [Header("Movement Tuning")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private float rollSpeed = 8f;
        [SerializeField] private float rollDuration = 0.5f;

        [Header("Camera Tuning")]
        [SerializeField] private float cameraHeight = 12f;
        [SerializeField] private float cameraDistance = 8f;
        [SerializeField] private float cameraAngle = 45f;
        [SerializeField] private float cameraSmoothing = 5f;

        [Header("Combat Tuning")]
        [SerializeField] private float attackCooldown = 0.5f;
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private float attackStaminaCost = 5f;

        [Header("Real-Time Tuning")]
        [SerializeField] private bool enableRealTimeTuning = true;
        [SerializeField] private KeyCode tuningToggleKey = KeyCode.F2;

        private PlayerController playerController;
        private CameraController cameraController;
        private bool tuningVisible = false;

        private void Start()
        {
            FindComponents();
            ApplySettings();
        }

        private void Update()
        {
            if (enableRealTimeTuning)
            {
                ApplySettings();
                
                if (Input.GetKeyDown(tuningToggleKey))
                {
                    tuningVisible = !tuningVisible;
                }
            }
        }

        private void FindComponents()
        {
            if (playerController == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerController = player.GetComponent<PlayerController>();
                }
            }

            if (cameraController == null)
            {
                cameraController = Camera.main?.GetComponent<CameraController>();
            }
        }

        private void ApplySettings()
        {
            if (playerController != null)
            {
                // Apply movement settings via reflection or public setters
                // Note: PlayerController needs public setters for real-time tuning
                ApplyPlayerSettings();
            }

            if (cameraController != null)
            {
                ApplyCameraSettings();
            }
        }

        private void ApplyPlayerSettings()
        {
            // We'll need to add public setters to PlayerController for this to work
            // For now, just store the values for manual adjustment
        }

        private void ApplyCameraSettings()
        {
            // Apply camera settings if CameraController has public setters
        }

        private void OnGUI()
        {
            if (!enableRealTimeTuning || !tuningVisible) return;

            GUILayout.BeginArea(new Rect(10, 200, 300, 400));
            GUILayout.Box("Real-Time Tuning (F2 to toggle)");
            
            GUILayout.Label("Movement");
            moveSpeed = GUILayout.HorizontalSlider(moveSpeed, 1f, 15f);
            GUILayout.Label($"Move Speed: {moveSpeed:F1}");
            
            rotationSpeed = GUILayout.HorizontalSlider(rotationSpeed, 1f, 25f);
            GUILayout.Label($"Rotation Speed: {rotationSpeed:F1}");
            
            rollSpeed = GUILayout.HorizontalSlider(rollSpeed, 5f, 20f);
            GUILayout.Label($"Roll Speed: {rollSpeed:F1}");
            
            GUILayout.Space(10);
            GUILayout.Label("Camera");
            cameraHeight = GUILayout.HorizontalSlider(cameraHeight, 5f, 25f);
            GUILayout.Label($"Camera Height: {cameraHeight:F1}");
            
            cameraDistance = GUILayout.HorizontalSlider(cameraDistance, 3f, 15f);
            GUILayout.Label($"Camera Distance: {cameraDistance:F1}");
            
            cameraAngle = GUILayout.HorizontalSlider(cameraAngle, 20f, 70f);
            GUILayout.Label($"Camera Angle: {cameraAngle:F1}");
            
            GUILayout.Space(10);
            GUILayout.Label("Combat");
            attackCooldown = GUILayout.HorizontalSlider(attackCooldown, 0.1f, 2f);
            GUILayout.Label($"Attack Cooldown: {attackCooldown:F2}");
            
            attackRange = GUILayout.HorizontalSlider(attackRange, 0.5f, 3f);
            GUILayout.Label($"Attack Range: {attackRange:F1}");
            
            GUILayout.EndArea();
        }

        // Helper method to save current settings
        [ContextMenu("Save Current Settings")]
        public void SaveCurrentSettings()
        {
            Debug.Log($"Current Settings:");
            Debug.Log($"Move Speed: {moveSpeed}");
            Debug.Log($"Rotation Speed: {rotationSpeed}");
            Debug.Log($"Roll Speed: {rollSpeed}");
            Debug.Log($"Camera Height: {cameraHeight}");
            Debug.Log($"Camera Distance: {cameraDistance}");
            Debug.Log($"Camera Angle: {cameraAngle}");
            Debug.Log($"Attack Cooldown: {attackCooldown}");
            Debug.Log($"Attack Range: {attackRange}");
        }
    }
}