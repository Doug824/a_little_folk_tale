using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ALittleFolkTale.Characters;

namespace ALittleFolkTale.UI
{
    public class DebugUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Canvas debugCanvas;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI staminaText;
        [SerializeField] private TextMeshProUGUI positionText;
        [SerializeField] private TextMeshProUGUI inputText;

        private PlayerController playerController;

        private void Start()
        {
            if (debugCanvas == null)
            {
                CreateDebugUI();
            }
            
            FindPlayerController();
        }

        private void CreateDebugUI()
        {
            // Create canvas
            GameObject canvasObj = new GameObject("DebugCanvas");
            debugCanvas = canvasObj.AddComponent<Canvas>();
            debugCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            debugCanvas.sortingOrder = 100;
            
            // Add Canvas Scaler
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            // Add GraphicRaycaster
            canvasObj.AddComponent<GraphicRaycaster>();

            // Create debug panel
            GameObject panel = new GameObject("DebugPanel");
            panel.transform.SetParent(debugCanvas.transform, false);
            
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 1);
            panelRect.anchorMax = new Vector2(0, 1);
            panelRect.pivot = new Vector2(0, 1);
            panelRect.anchoredPosition = new Vector2(10, -10);
            panelRect.sizeDelta = new Vector2(300, 200);
            
            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.7f);

            // Create text elements
            healthText = CreateDebugText("Health: 100/100", panel.transform, new Vector2(10, -10));
            staminaText = CreateDebugText("Stamina: 100/100", panel.transform, new Vector2(10, -40));
            positionText = CreateDebugText("Position: (0,0,0)", panel.transform, new Vector2(10, -70));
            inputText = CreateDebugText("Input: None", panel.transform, new Vector2(10, -100));
            
            // Create instructions
            TextMeshProUGUI instructions = CreateDebugText("WASD/Left Stick: Move\nSpace/B: Roll\nLeft Click/X: Attack", panel.transform, new Vector2(10, -130));
            instructions.fontSize = 12;
            instructions.color = Color.yellow;
        }

        private TextMeshProUGUI CreateDebugText(string text, Transform parent, Vector2 position)
        {
            GameObject textObj = new GameObject("DebugText");
            textObj.transform.SetParent(parent, false);
            
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0, 1);
            textRect.anchorMax = new Vector2(0, 1);
            textRect.pivot = new Vector2(0, 1);
            textRect.anchoredPosition = position;
            textRect.sizeDelta = new Vector2(280, 30);
            
            TextMeshProUGUI textMesh = textObj.AddComponent<TextMeshProUGUI>();
            textMesh.text = text;
            textMesh.fontSize = 14;
            textMesh.color = Color.white;
            textMesh.fontStyle = FontStyles.Bold;
            
            return textMesh;
        }

        private void FindPlayerController()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerController = player.GetComponent<PlayerController>();
            }
        }

        private void Update()
        {
            if (playerController == null)
            {
                FindPlayerController();
                return;
            }

            UpdateDebugInfo();
        }

        private void UpdateDebugInfo()
        {
            if (healthText != null)
            {
                float healthPercent = playerController.GetHealthPercentage();
                healthText.text = $"Health: {(int)(healthPercent * 100)}/100";
                healthText.color = healthPercent > 0.5f ? Color.green : (healthPercent > 0.25f ? Color.yellow : Color.red);
            }

            if (staminaText != null)
            {
                float staminaPercent = playerController.GetStaminaPercentage();
                staminaText.text = $"Stamina: {(int)(staminaPercent * 100)}/100";
                staminaText.color = staminaPercent > 0.5f ? Color.cyan : (staminaPercent > 0.25f ? Color.yellow : Color.red);
            }

            if (positionText != null)
            {
                Vector3 pos = playerController.transform.position;
                positionText.text = $"Position: ({pos.x:F1}, {pos.y:F1}, {pos.z:F1})";
            }

            if (inputText != null)
            {
                string inputInfo = "Input: ";
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
                {
                    inputInfo += "Move ";
                }
                if (Input.GetKey(KeyCode.Space))
                {
                    inputInfo += "Roll ";
                }
                if (Input.GetMouseButton(0))
                {
                    inputInfo += "Attack ";
                }
                if (inputInfo == "Input: ")
                {
                    inputInfo += "None";
                }
                inputText.text = inputInfo;
            }
        }

        // Toggle debug UI with F1
        private void OnGUI()
        {
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.F1)
            {
                if (debugCanvas != null)
                {
                    debugCanvas.gameObject.SetActive(!debugCanvas.gameObject.activeSelf);
                }
            }
        }
    }
}