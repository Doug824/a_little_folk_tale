using UnityEngine;
using UnityEngine.UI;
using System.Collections;
#if TMP_PRESENT
using TMPro;
#endif

namespace ALittleFolkTale.UI
{
    public class DamageNumbers : MonoBehaviour
    {
        [Header("Damage Number Settings")]
        [SerializeField] private float floatHeight = 2f;
        [SerializeField] private float floatDuration = 1.5f;
        [SerializeField] private float fadeStartTime = 1f;
        [SerializeField] private float randomSpread = 0.5f;
        
        [Header("Player Damage")]
        [SerializeField] private Color playerDamageColor = Color.red;
        [SerializeField] private float playerDamageSize = 24f;
        
        [Header("Enemy Damage")]
        [SerializeField] private Color enemyDamageColor = Color.yellow;
        [SerializeField] private float enemyDamageSize = 18f;
        
        [Header("Healing")]
        [SerializeField] private Color healingColor = Color.green;
        [SerializeField] private float healingSize = 20f;
        
        private static DamageNumbers instance;
        public static DamageNumbers Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<DamageNumbers>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("DamageNumbers");
                        instance = go.AddComponent<DamageNumbers>();
                    }
                }
                return instance;
            }
        }
        
        private Canvas worldCanvas;
        private Camera mainCamera;
        
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                CreateWorldCanvas();
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            mainCamera = Camera.main;
        }
        
        private void CreateWorldCanvas()
        {
            GameObject canvasObj = new GameObject("DamageNumbersCanvas");
            canvasObj.transform.SetParent(transform);
            
            worldCanvas = canvasObj.AddComponent<Canvas>();
            worldCanvas.renderMode = RenderMode.WorldSpace;
            worldCanvas.sortingOrder = 50;
            
            // Scale the canvas appropriately for world space
            worldCanvas.transform.localScale = Vector3.one * 0.01f;
        }
        
        public void ShowDamage(Vector3 worldPosition, int damageAmount, DamageType type = DamageType.Enemy)
        {
            if (worldCanvas == null) CreateWorldCanvas();
            
            StartCoroutine(CreateFloatingNumber(worldPosition, damageAmount, type));
        }
        
        public void ShowHealing(Vector3 worldPosition, int healAmount)
        {
            if (worldCanvas == null) CreateWorldCanvas();
            
            StartCoroutine(CreateFloatingNumber(worldPosition, healAmount, DamageType.Healing));
        }
        
        private IEnumerator CreateFloatingNumber(Vector3 worldPosition, int amount, DamageType type)
        {
            // Create the text object
            GameObject textObj = new GameObject("FloatingDamage");
            textObj.transform.SetParent(worldCanvas.transform);
            
            // Position in world space
            Vector3 randomOffset = new Vector3(
                Random.Range(-randomSpread, randomSpread),
                Random.Range(0, randomSpread * 0.5f),
                Random.Range(-randomSpread, randomSpread)
            );
            textObj.transform.position = worldPosition + randomOffset;
            
            // Add Text component (fallback for TMP issues)
            Text textMesh = textObj.AddComponent<Text>();
            
            // Configure text based on type
            switch (type)
            {
                case DamageType.Player:
                    textMesh.text = $"-{amount}";
                    textMesh.color = playerDamageColor;
                    textMesh.fontSize = (int)playerDamageSize;
                    break;
                case DamageType.Enemy:
                    textMesh.text = $"{amount}";
                    textMesh.color = enemyDamageColor;
                    textMesh.fontSize = (int)enemyDamageSize;
                    break;
                case DamageType.Healing:
                    textMesh.text = $"+{amount}";
                    textMesh.color = healingColor;
                    textMesh.fontSize = (int)healingSize;
                    break;
            }
            
            textMesh.alignment = TextAnchor.MiddleCenter;
            textMesh.fontStyle = FontStyle.Bold;
            textMesh.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            
            // Make it face the camera
            if (mainCamera != null)
            {
                textObj.transform.LookAt(mainCamera.transform);
                textObj.transform.Rotate(0, 180, 0); // Flip to face camera properly
            }
            
            // Animate the floating effect
            Vector3 startPos = textObj.transform.position;
            Vector3 endPos = startPos + Vector3.up * floatHeight;
            Color startColor = textMesh.color;
            
            float elapsed = 0f;
            
            while (elapsed < floatDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / floatDuration;
                
                // Float upward
                textObj.transform.position = Vector3.Lerp(startPos, endPos, progress);
                
                // Fade out after fadeStartTime
                if (elapsed >= fadeStartTime)
                {
                    float fadeProgress = (elapsed - fadeStartTime) / (floatDuration - fadeStartTime);
                    Color currentColor = startColor;
                    currentColor.a = Mathf.Lerp(1f, 0f, fadeProgress);
                    textMesh.color = currentColor;
                }
                
                // Scale effect (slight grow then shrink)
                float scaleMultiplier = 1f + Mathf.Sin(progress * Mathf.PI) * 0.2f;
                textObj.transform.localScale = Vector3.one * scaleMultiplier;
                
                // Keep facing camera
                if (mainCamera != null)
                {
                    textObj.transform.LookAt(mainCamera.transform);
                    textObj.transform.Rotate(0, 180, 0);
                }
                
                yield return null;
            }
            
            // Clean up
            Destroy(textObj);
        }
        
        public enum DamageType
        {
            Player,
            Enemy,
            Healing
        }
    }
}