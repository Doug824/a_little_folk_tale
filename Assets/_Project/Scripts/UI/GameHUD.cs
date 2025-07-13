using UnityEngine;
using UnityEngine.UI;
using ALittleFolkTale.Characters;
#if TMP_PRESENT
using TMPro;
#endif

namespace ALittleFolkTale.UI
{
    public class GameHUD : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Canvas hudCanvas;
        [SerializeField] private Slider healthBar;
        [SerializeField] private Slider staminaBar;
        [SerializeField] private Text healthText;
        [SerializeField] private Text staminaText;
        
        [Header("Settings")]
        [SerializeField] private bool showNumbers = true;
        [SerializeField] private float barWidth = 200f;
        [SerializeField] private float barHeight = 20f;
        
        private PlayerController playerController;
        
        // Singleton instance
        private static GameHUD instance;
        public static GameHUD Instance
        {
            get
            {
                if (instance == null)
                    instance = FindFirstObjectByType<GameHUD>();
                return instance;
            }
        }
        
        private void Awake()
        {
            instance = this;
        }
        
        private void Start()
        {
            if (hudCanvas == null)
            {
                CreateGameHUD();
            }
            
            FindPlayerController();
        }
        
        private void CreateGameHUD()
        {
            // Create main HUD canvas
            GameObject canvasObj = new GameObject("GameHUDCanvas");
            hudCanvas = canvasObj.AddComponent<Canvas>();
            hudCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            hudCanvas.sortingOrder = 10; // Below debug UI
            
            // Add Canvas Scaler
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            
            // Add GraphicRaycaster
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // Create main HUD panel
            GameObject hudPanel = new GameObject("HUDPanel");
            hudPanel.transform.SetParent(hudCanvas.transform, false);
            
            RectTransform panelRect = hudPanel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 1);
            panelRect.anchorMax = new Vector2(1, 1);
            panelRect.pivot = new Vector2(0.5f, 1);
            panelRect.anchoredPosition = new Vector2(0, 0);
            panelRect.sizeDelta = new Vector2(0, 100);
            
            // Create health bar
            CreateHealthBar(hudPanel.transform);
            
            // Create stamina bar
            CreateStaminaBar(hudPanel.transform);
        }
        
        private void CreateHealthBar(Transform parent)
        {
            // Health bar background
            GameObject healthBG = new GameObject("HealthBarBG");
            healthBG.transform.SetParent(parent, false);
            
            RectTransform healthBGRect = healthBG.AddComponent<RectTransform>();
            healthBGRect.anchorMin = new Vector2(0, 1);
            healthBGRect.anchorMax = new Vector2(0, 1);
            healthBGRect.pivot = new Vector2(0, 1);
            healthBGRect.anchoredPosition = new Vector2(20, -20);
            healthBGRect.sizeDelta = new Vector2(barWidth, barHeight);
            
            Image healthBGImage = healthBG.AddComponent<Image>();
            healthBGImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            
            // Health bar slider
            GameObject healthSliderObj = new GameObject("HealthBar");
            healthSliderObj.transform.SetParent(healthBG.transform, false);
            
            healthBar = healthSliderObj.AddComponent<Slider>();
            healthBar.minValue = 0f;
            healthBar.maxValue = 1f;
            healthBar.value = 1f;
            
            RectTransform sliderRect = healthSliderObj.GetComponent<RectTransform>();
            sliderRect.anchorMin = Vector2.zero;
            sliderRect.anchorMax = Vector2.one;
            sliderRect.offsetMin = Vector2.zero;
            sliderRect.offsetMax = Vector2.zero;
            
            // Health bar fill area
            GameObject fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(healthSliderObj.transform, false);
            
            RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
            fillAreaRect.anchorMin = Vector2.zero;
            fillAreaRect.anchorMax = Vector2.one;
            fillAreaRect.offsetMin = Vector2.zero;
            fillAreaRect.offsetMax = Vector2.zero;
            
            // Health bar fill
            GameObject fill = new GameObject("Fill");
            fill.transform.SetParent(fillArea.transform, false);
            
            RectTransform fillRect = fill.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            
            Image fillImage = fill.AddComponent<Image>();
            fillImage.color = new Color(0.8f, 0.2f, 0.2f, 1f); // Red
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            
            healthBar.fillRect = fillRect;
            healthBar.targetGraphic = fillImage;
            
            // Health text
            if (showNumbers)
            {
                GameObject healthTextObj = new GameObject("HealthText");
                healthTextObj.transform.SetParent(healthBG.transform, false);
                
                RectTransform textRect = healthTextObj.AddComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.offsetMin = Vector2.zero;
                textRect.offsetMax = Vector2.zero;
                
                healthText = healthTextObj.AddComponent<Text>();
                healthText.text = "100/100";
                healthText.fontSize = 12;
                healthText.color = Color.white;
                healthText.alignment = TextAnchor.MiddleCenter;
                healthText.fontStyle = FontStyle.Bold;
                healthText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }
        }
        
        private void CreateStaminaBar(Transform parent)
        {
            // Stamina bar background
            GameObject staminaBG = new GameObject("StaminaBarBG");
            staminaBG.transform.SetParent(parent, false);
            
            RectTransform staminaBGRect = staminaBG.AddComponent<RectTransform>();
            staminaBGRect.anchorMin = new Vector2(0, 1);
            staminaBGRect.anchorMax = new Vector2(0, 1);
            staminaBGRect.pivot = new Vector2(0, 1);
            staminaBGRect.anchoredPosition = new Vector2(20, -50);
            staminaBGRect.sizeDelta = new Vector2(barWidth, barHeight);
            
            Image staminaBGImage = staminaBG.AddComponent<Image>();
            staminaBGImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            
            // Stamina bar slider
            GameObject staminaSliderObj = new GameObject("StaminaBar");
            staminaSliderObj.transform.SetParent(staminaBG.transform, false);
            
            staminaBar = staminaSliderObj.AddComponent<Slider>();
            staminaBar.minValue = 0f;
            staminaBar.maxValue = 1f;
            staminaBar.value = 1f;
            
            RectTransform sliderRect = staminaSliderObj.GetComponent<RectTransform>();
            sliderRect.anchorMin = Vector2.zero;
            sliderRect.anchorMax = Vector2.one;
            sliderRect.offsetMin = Vector2.zero;
            sliderRect.offsetMax = Vector2.zero;
            
            // Stamina bar fill area
            GameObject fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(staminaSliderObj.transform, false);
            
            RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
            fillAreaRect.anchorMin = Vector2.zero;
            fillAreaRect.anchorMax = Vector2.one;
            fillAreaRect.offsetMin = Vector2.zero;
            fillAreaRect.offsetMax = Vector2.zero;
            
            // Stamina bar fill
            GameObject fill = new GameObject("Fill");
            fill.transform.SetParent(fillArea.transform, false);
            
            RectTransform fillRect = fill.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            
            Image fillImage = fill.AddComponent<Image>();
            fillImage.color = new Color(0.2f, 0.8f, 0.8f, 1f); // Cyan
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            
            staminaBar.fillRect = fillRect;
            staminaBar.targetGraphic = fillImage;
            
            // Stamina text
            if (showNumbers)
            {
                GameObject staminaTextObj = new GameObject("StaminaText");
                staminaTextObj.transform.SetParent(staminaBG.transform, false);
                
                RectTransform textRect = staminaTextObj.AddComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.offsetMin = Vector2.zero;
                textRect.offsetMax = Vector2.zero;
                
                staminaText = staminaTextObj.AddComponent<Text>();
                staminaText.text = "100/100";
                staminaText.fontSize = 12;
                staminaText.color = Color.white;
                staminaText.alignment = TextAnchor.MiddleCenter;
                staminaText.fontStyle = FontStyle.Bold;
                staminaText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }
        }
        
        private void FindPlayerController()
        {
            // Try tag first
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerController = player.GetComponent<PlayerController>();
            }
            
            // Fallback to component search
            if (playerController == null)
            {
                playerController = FindFirstObjectByType<PlayerController>();
            }
        }
        
        private void Update()
        {
            if (playerController == null)
            {
                FindPlayerController();
                return;
            }
            
            UpdateHUD();
        }
        
        private void UpdateHUD()
        {
            // Update health
            if (healthBar != null)
            {
                float healthPercent = playerController.GetHealthPercentage();
                healthBar.value = healthPercent;
                
                if (healthText != null && showNumbers)
                {
                    int currentHealth = Mathf.RoundToInt(healthPercent * 100);
                    healthText.text = $"{currentHealth}/100";
                }
            }
            
            // Update stamina
            if (staminaBar != null)
            {
                float staminaPercent = playerController.GetStaminaPercentage();
                staminaBar.value = staminaPercent;
                
                if (staminaText != null && showNumbers)
                {
                    int currentStamina = Mathf.RoundToInt(staminaPercent * 100);
                    staminaText.text = $"{currentStamina}/100";
                }
            }
        }
        
        public void SetHealthBarVisible(bool visible)
        {
            if (healthBar != null)
                healthBar.transform.parent.gameObject.SetActive(visible);
        }
        
        public void SetStaminaBarVisible(bool visible)
        {
            if (staminaBar != null)
                staminaBar.transform.parent.gameObject.SetActive(visible);
        }
        
        public void SetHUDVisible(bool visible)
        {
            if (hudCanvas != null)
                hudCanvas.gameObject.SetActive(visible);
        }
    }
}