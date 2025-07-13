using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using ALittleFolkTale.Characters;
using ALittleFolkTale.Items;

namespace ALittleFolkTale.UI
{
    public class InventoryUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Canvas inventoryCanvas;
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private Transform inventoryGrid;
        [SerializeField] private Transform equipmentPanel;
        [SerializeField] private Transform hotbarPanel;
        
        [Header("Prefabs")]
        [SerializeField] private GameObject slotPrefab;
        
        [Header("Settings")]
        [SerializeField] private float slotSize = 80f;
        [SerializeField] private float slotSpacing = 10f;
        [SerializeField] private int inventoryColumns = 3; // Link's Awakening style 3x3 grid
        
        private List<InventorySlotUI> inventorySlots;
        private List<InventorySlotUI> hotbarSlots;
        private EquipmentSlotUI weaponSlot;
        private EquipmentSlotUI armorSlot;
        private EquipmentSlotUI accessorySlot;
        
        // Character preview
        [Header("Character Preview")]
        [SerializeField] private GameObject characterPreviewPanel;
        [SerializeField] private RawImage characterPreviewImage;
        [SerializeField] private Camera characterPreviewCamera;
        
        // Quick-use UI elements
        [Header("Quick Use UI")]
        [SerializeField] private GameObject quickUsePanel;
        [SerializeField] private Image quickUseYIcon;
        [SerializeField] private Image quickUseXIcon;
        [SerializeField] private Text quickUseYText;
        [SerializeField] private Text quickUseXText;
        
        [Header("Hotkey Panel")]
        [SerializeField] private GameObject hotkeyPanel;
        [SerializeField] private InventorySlotUI hotkeySlotQ;
        [SerializeField] private InventorySlotUI hotkeySlotE;
        
        private Inventory playerInventory;
        private bool isInventoryOpen = false;
        private int selectedSlotIndex = -1;
        
        // Singleton access
        private static InventoryUI instance;
        public static InventoryUI Instance
        {
            get
            {
                if (instance == null)
                    instance = FindFirstObjectByType<InventoryUI>();
                return instance;
            }
        }

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            if (inventoryCanvas == null)
            {
                CreateInventoryUI();
            }
            
            // Ensure EventSystem exists for UI events
            EnsureEventSystem();
            
            FindPlayerInventory();
            SetupInventorySlots();
            CloseInventory(); // Start closed
            
            // Quick-use panel should always be visible
            if (quickUsePanel != null)
            {
                quickUsePanel.SetActive(true);
            }
        }
        
        private void EnsureEventSystem()
        {
            if (EventSystem.current == null)
            {
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystemObj.AddComponent<EventSystem>();
                eventSystemObj.AddComponent<StandaloneInputModule>();
                Debug.Log("Created EventSystem for UI events");
            }
            else
            {
                Debug.Log($"EventSystem found: {EventSystem.current.name}");
            }
        }

        private void Update()
        {
            // Toggle inventory with Tab key
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                ToggleInventory();
            }
            
            // Quick-use item keys (only when inventory is closed)
            if (!isInventoryOpen)
            {
                if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Y))
                {
                    UseQuickItemY();
                }
                
                if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.X))
                {
                    UseQuickItemX();
                }
            }
            
            // Handle keyboard navigation when inventory is open
            HandleKeyboardNavigation();
        }

        private void CreateInventoryUI()
        {
            // Create main inventory canvas
            GameObject canvasObj = new GameObject("InventoryCanvas");
            inventoryCanvas = canvasObj.AddComponent<Canvas>();
            inventoryCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            inventoryCanvas.sortingOrder = 20; // Above other UI
            
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // Create Link's Awakening style layout
            CreateMainInventoryPanel();
            CreateHotkeyPanel();
            CreateCharacterPreviewPanel();
            CreateEquipmentPanel();
            CreateQuickUsePanel();
        }

        private void CreateMainInventoryPanel()
        {
            // Main background panel covering most of the screen
            inventoryPanel = new GameObject("MainInventoryPanel");
            inventoryPanel.transform.SetParent(inventoryCanvas.transform, false);
            
            RectTransform panelRect = inventoryPanel.AddComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
            
            Image panelImage = inventoryPanel.AddComponent<Image>();
            panelImage.color = new Color(0.05f, 0.1f, 0.05f, 0.95f); // Dark green tint like Link's Awakening
            
            // Create the items grid section (left side)
            GameObject itemsSection = new GameObject("ItemsSection");
            itemsSection.transform.SetParent(inventoryPanel.transform, false);
            
            RectTransform itemsRect = itemsSection.AddComponent<RectTransform>();
            itemsRect.anchorMin = new Vector2(0.1f, 0.3f);
            itemsRect.anchorMax = new Vector2(0.5f, 0.9f);
            itemsRect.offsetMin = Vector2.zero;
            itemsRect.offsetMax = Vector2.zero;
            
            Image itemsImage = itemsSection.AddComponent<Image>();
            itemsImage.color = new Color(0.2f, 0.25f, 0.2f, 0.8f);
            
            // Items title
            GameObject itemsTitle = new GameObject("ItemsTitle");
            itemsTitle.transform.SetParent(itemsSection.transform, false);
            
            RectTransform titleRect = itemsTitle.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 1);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.pivot = new Vector2(0.5f, 1);
            titleRect.anchoredPosition = new Vector2(0, 0);
            titleRect.sizeDelta = new Vector2(0, 40);
            
            Text titleText = itemsTitle.AddComponent<Text>();
            titleText.text = "ITEMS";
            titleText.fontSize = 20;
            titleText.color = new Color(0.9f, 0.9f, 0.7f); // Cream color like Link's Awakening
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.fontStyle = FontStyle.Bold;
            titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            
            // Create inventory grid
            GameObject gridObj = new GameObject("InventoryGrid");
            gridObj.transform.SetParent(itemsSection.transform, false);
            
            RectTransform gridRect = gridObj.AddComponent<RectTransform>();
            gridRect.anchorMin = new Vector2(0, 0);
            gridRect.anchorMax = new Vector2(1, 1);
            gridRect.pivot = new Vector2(0.5f, 0.5f);
            gridRect.offsetMin = new Vector2(20, 20);
            gridRect.offsetMax = new Vector2(-20, -50);
            
            GridLayoutGroup grid = gridObj.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(slotSize, slotSize);
            grid.spacing = new Vector2(slotSpacing, slotSpacing);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = inventoryColumns;
            grid.childAlignment = TextAnchor.UpperCenter;
            
            inventoryGrid = gridRect;
        }
        
        private void CreateHotkeyPanel()
        {
            // Hotkey assignment area (bottom center)
            hotkeyPanel = new GameObject("HotkeyPanel");
            hotkeyPanel.transform.SetParent(inventoryPanel.transform, false);
            
            RectTransform hotkeyRect = hotkeyPanel.AddComponent<RectTransform>();
            hotkeyRect.anchorMin = new Vector2(0.1f, 0.05f);
            hotkeyRect.anchorMax = new Vector2(0.9f, 0.25f);
            hotkeyRect.offsetMin = Vector2.zero;
            hotkeyRect.offsetMax = Vector2.zero;
            
            Image hotkeyImage = hotkeyPanel.AddComponent<Image>();
            hotkeyImage.color = new Color(0.15f, 0.2f, 0.15f, 0.8f);
            
            // Hotkey title
            GameObject hotkeyTitle = new GameObject("HotkeyTitle");
            hotkeyTitle.transform.SetParent(hotkeyPanel.transform, false);
            
            RectTransform titleRect = hotkeyTitle.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 1);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.pivot = new Vector2(0.5f, 1);
            titleRect.anchoredPosition = new Vector2(0, 0);
            titleRect.sizeDelta = new Vector2(0, 30);
            
            Text titleText = hotkeyTitle.AddComponent<Text>();
            titleText.text = "SELECT WITH Q AND E";
            titleText.fontSize = 16;
            titleText.color = new Color(0.9f, 0.9f, 0.7f);
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.fontStyle = FontStyle.Bold;
            titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            
            // Create Q slot
            GameObject qSlotContainer = new GameObject("QSlotContainer");
            qSlotContainer.transform.SetParent(hotkeyPanel.transform, false);
            
            RectTransform qContainerRect = qSlotContainer.AddComponent<RectTransform>();
            qContainerRect.anchorMin = new Vector2(0.2f, 0.2f);
            qContainerRect.anchorMax = new Vector2(0.2f, 0.2f);
            qContainerRect.pivot = new Vector2(0.5f, 0.5f);
            qContainerRect.anchoredPosition = Vector2.zero;
            qContainerRect.sizeDelta = new Vector2(100, 80);
            
            // Q button label
            GameObject qLabel = new GameObject("QLabel");
            qLabel.transform.SetParent(qSlotContainer.transform, false);
            
            RectTransform qLabelRect = qLabel.AddComponent<RectTransform>();
            qLabelRect.anchorMin = new Vector2(0, 0);
            qLabelRect.anchorMax = new Vector2(1, 0);
            qLabelRect.pivot = new Vector2(0.5f, 0);
            qLabelRect.anchoredPosition = new Vector2(0, -10);
            qLabelRect.sizeDelta = new Vector2(0, 20);
            
            Text qLabelText = qLabel.AddComponent<Text>();
            qLabelText.text = "Q";
            qLabelText.fontSize = 14;
            qLabelText.color = new Color(1f, 1f, 0.5f); // Yellow
            qLabelText.alignment = TextAnchor.MiddleCenter;
            qLabelText.fontStyle = FontStyle.Bold;
            qLabelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            
            // Create E slot
            GameObject eSlotContainer = new GameObject("ESlotContainer");
            eSlotContainer.transform.SetParent(hotkeyPanel.transform, false);
            
            RectTransform eContainerRect = eSlotContainer.AddComponent<RectTransform>();
            eContainerRect.anchorMin = new Vector2(0.8f, 0.2f);
            eContainerRect.anchorMax = new Vector2(0.8f, 0.2f);
            eContainerRect.pivot = new Vector2(0.5f, 0.5f);
            eContainerRect.anchoredPosition = Vector2.zero;
            eContainerRect.sizeDelta = new Vector2(100, 80);
            
            // E button label
            GameObject eLabel = new GameObject("ELabel");
            eLabel.transform.SetParent(eSlotContainer.transform, false);
            
            RectTransform eLabelRect = eLabel.AddComponent<RectTransform>();
            eLabelRect.anchorMin = new Vector2(0, 0);
            eLabelRect.anchorMax = new Vector2(1, 0);
            eLabelRect.pivot = new Vector2(0.5f, 0);
            eLabelRect.anchoredPosition = new Vector2(0, -10);
            eLabelRect.sizeDelta = new Vector2(0, 20);
            
            Text eLabelText = eLabel.AddComponent<Text>();
            eLabelText.text = "E";
            eLabelText.fontSize = 14;
            eLabelText.color = new Color(1f, 1f, 0.5f); // Yellow
            eLabelText.alignment = TextAnchor.MiddleCenter;
            eLabelText.fontStyle = FontStyle.Bold;
            eLabelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            
            // Create actual hotkey slots
            hotkeySlotQ = CreateHotkeySlot(qSlotContainer, "Q", -100);
            hotkeySlotE = CreateHotkeySlot(eSlotContainer, "E", -101);
        }

        private void CreateCharacterPreviewPanel()
        {
            // Character section (right side, like Link's Awakening)
            characterPreviewPanel = new GameObject("CharacterPreviewPanel");
            characterPreviewPanel.transform.SetParent(inventoryPanel.transform, false);
            
            RectTransform previewRect = characterPreviewPanel.AddComponent<RectTransform>();
            previewRect.anchorMin = new Vector2(0.55f, 0.3f);
            previewRect.anchorMax = new Vector2(0.9f, 0.9f);
            previewRect.offsetMin = Vector2.zero;
            previewRect.offsetMax = Vector2.zero;
            
            Image previewBg = characterPreviewPanel.AddComponent<Image>();
            previewBg.color = new Color(0.2f, 0.25f, 0.2f, 0.8f);
            
            // Character preview title
            GameObject previewTitle = new GameObject("CharacterTitle");
            previewTitle.transform.SetParent(characterPreviewPanel.transform, false);
            
            RectTransform titleRect = previewTitle.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 1);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.pivot = new Vector2(0.5f, 1);
            titleRect.anchoredPosition = new Vector2(0, 0);
            titleRect.sizeDelta = new Vector2(0, 40);
            
            Text titleText = previewTitle.AddComponent<Text>();
            titleText.text = "MOSS";
            titleText.fontSize = 20;
            titleText.color = new Color(0.9f, 0.9f, 0.7f);
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.fontStyle = FontStyle.Bold;
            titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            
            // Character preview window (larger, positioned right)
            GameObject previewWindow = new GameObject("CharacterPreview");
            previewWindow.transform.SetParent(characterPreviewPanel.transform, false);
            
            RectTransform windowRect = previewWindow.AddComponent<RectTransform>();
            windowRect.anchorMin = new Vector2(0.4f, 0.15f);
            windowRect.anchorMax = new Vector2(0.9f, 0.85f);
            windowRect.offsetMin = Vector2.zero;
            windowRect.offsetMax = Vector2.zero;
            
            characterPreviewImage = previewWindow.AddComponent<RawImage>();
            characterPreviewImage.color = new Color(0.3f, 0.4f, 0.3f, 1f); // Placeholder color
            
            // Add simple character placeholder
            GameObject placeholder = new GameObject("CharacterPlaceholder");
            placeholder.transform.SetParent(previewWindow.transform, false);
            
            RectTransform placeholderRect = placeholder.AddComponent<RectTransform>();
            placeholderRect.anchorMin = Vector2.zero;
            placeholderRect.anchorMax = Vector2.one;
            placeholderRect.offsetMin = Vector2.zero;
            placeholderRect.offsetMax = Vector2.zero;
            
            Text placeholderText = placeholder.AddComponent<Text>();
            placeholderText.text = "♦";
            placeholderText.fontSize = 48;
            placeholderText.color = new Color(0.9f, 0.9f, 0.7f);
            placeholderText.alignment = TextAnchor.MiddleCenter;
            placeholderText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }
        
        private void CreateEquipmentPanel()
        {
            // Equipment slots are now part of the character preview panel
            equipmentPanel = characterPreviewPanel.transform as RectTransform;
        }

        private void CreateQuickUsePanel()
        {
            // Create separate canvas for quick-use panel (always on top)
            GameObject quickUseCanvasObj = new GameObject("QuickUseCanvas");
            Canvas quickUseCanvas = quickUseCanvasObj.AddComponent<Canvas>();
            quickUseCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            quickUseCanvas.sortingOrder = 30; // Higher than main inventory canvas
            
            CanvasScaler quickScaler = quickUseCanvasObj.AddComponent<CanvasScaler>();
            quickScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            quickScaler.referenceResolution = new Vector2(1920, 1080);
            
            quickUseCanvasObj.AddComponent<GraphicRaycaster>();
            
            // Quick-use panel in top-right corner of screen (always visible)
            quickUsePanel = new GameObject("QuickUsePanel");
            quickUsePanel.transform.SetParent(quickUseCanvas.transform, false);
            
            RectTransform quickRect = quickUsePanel.AddComponent<RectTransform>();
            quickRect.anchorMin = new Vector2(1f, 1f);
            quickRect.anchorMax = new Vector2(1f, 1f);
            quickRect.pivot = new Vector2(1f, 1f);
            quickRect.anchoredPosition = new Vector2(-20, -20);
            quickRect.sizeDelta = new Vector2(150, 80);
            
            // Y/Q button slot
            GameObject ySlot = CreateQuickUseSlot("Y/Q", new Vector2(-80, -40));
            quickUseYIcon = ySlot.transform.Find("Icon").GetComponent<Image>();
            quickUseYText = ySlot.transform.Find("QuantityText").GetComponent<Text>();
            
            // X/E button slot
            GameObject xSlot = CreateQuickUseSlot("X/E", new Vector2(-20, -40));
            quickUseXIcon = xSlot.transform.Find("Icon").GetComponent<Image>();
            quickUseXText = xSlot.transform.Find("QuantityText").GetComponent<Text>();
        }
        
        private InventorySlotUI CreateHotkeySlot(GameObject parent, string keyName, int slotIndex)
        {
            GameObject slotObj = new GameObject($"HotkeySlot_{keyName}");
            slotObj.transform.SetParent(parent.transform, false);
            
            RectTransform slotRect = slotObj.AddComponent<RectTransform>();
            slotRect.anchorMin = new Vector2(0.5f, 0.5f);
            slotRect.anchorMax = new Vector2(0.5f, 0.5f);
            slotRect.pivot = new Vector2(0.5f, 0.5f);
            slotRect.anchoredPosition = new Vector2(0, 10);
            slotRect.sizeDelta = new Vector2(60, 60);
            
            Image slotImage = slotObj.AddComponent<Image>();
            slotImage.color = new Color(0.4f, 0.4f, 0.3f, 0.9f);
            slotImage.raycastTarget = true;
            
            Button slotButton = slotObj.AddComponent<Button>();
            
            // Create item icon
            GameObject iconObj = new GameObject("ItemIcon");
            iconObj.transform.SetParent(slotObj.transform, false);
            
            RectTransform iconRect = iconObj.AddComponent<RectTransform>();
            iconRect.anchorMin = Vector2.zero;
            iconRect.anchorMax = Vector2.one;
            iconRect.offsetMin = new Vector2(5, 5);
            iconRect.offsetMax = new Vector2(-5, -5);
            
            Image iconImage = iconObj.AddComponent<Image>();
            iconImage.color = Color.clear;
            iconImage.raycastTarget = false;
            
            // Create quantity text (though hotkeys typically don't show quantity)
            GameObject quantityObj = new GameObject("QuantityText");
            quantityObj.transform.SetParent(slotObj.transform, false);
            
            RectTransform quantityRect = quantityObj.AddComponent<RectTransform>();
            quantityRect.anchorMin = new Vector2(1, 0);
            quantityRect.anchorMax = new Vector2(1, 0);
            quantityRect.pivot = new Vector2(1, 0);
            quantityRect.anchoredPosition = new Vector2(-5, 5);
            quantityRect.sizeDelta = new Vector2(20, 15);
            
            Text quantityText = quantityObj.AddComponent<Text>();
            quantityText.fontSize = 10;
            quantityText.color = Color.white;
            quantityText.alignment = TextAnchor.MiddleRight;
            quantityText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            quantityText.raycastTarget = false;
            
            InventorySlotUI slotUI = slotObj.AddComponent<InventorySlotUI>();
            slotUI.Initialize(slotIndex, iconImage, quantityText, slotButton, false);
            
            return slotUI;
        }
        
        private GameObject CreateQuickUseSlot(string buttonText, Vector2 position)
        {
            GameObject slot = new GameObject($"QuickUseSlot_{buttonText}");
            slot.transform.SetParent(quickUsePanel.transform, false);
            
            RectTransform slotRect = slot.AddComponent<RectTransform>();
            slotRect.anchoredPosition = position;
            slotRect.sizeDelta = new Vector2(50, 50);
            
            Image slotBg = slot.AddComponent<Image>();
            slotBg.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            
            // Item icon
            GameObject icon = new GameObject("Icon");
            icon.transform.SetParent(slot.transform, false);
            
            RectTransform iconRect = icon.AddComponent<RectTransform>();
            iconRect.anchorMin = Vector2.zero;
            iconRect.anchorMax = Vector2.one;
            iconRect.offsetMin = new Vector2(5, 5);
            iconRect.offsetMax = new Vector2(-5, -5);
            
            Image iconImage = icon.AddComponent<Image>();
            iconImage.color = Color.clear;
            
            // Button text
            GameObject text = new GameObject("ButtonText");
            text.transform.SetParent(slot.transform, false);
            
            RectTransform textRect = text.AddComponent<RectTransform>();
            textRect.anchoredPosition = new Vector2(0, -30);
            textRect.sizeDelta = new Vector2(40, 20);
            
            Text buttonLabel = text.AddComponent<Text>();
            buttonLabel.text = buttonText;
            buttonLabel.fontSize = 12;
            buttonLabel.color = Color.yellow;
            buttonLabel.alignment = TextAnchor.MiddleCenter;
            buttonLabel.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            
            // Add quantity text in bottom-right corner
            GameObject quantityObj = new GameObject("QuantityText");
            quantityObj.transform.SetParent(slot.transform, false);
            
            RectTransform quantityRect = quantityObj.AddComponent<RectTransform>();
            quantityRect.anchorMin = new Vector2(1, 0);
            quantityRect.anchorMax = new Vector2(1, 0);
            quantityRect.pivot = new Vector2(1, 0);
            quantityRect.anchoredPosition = new Vector2(-3, 3);
            quantityRect.sizeDelta = new Vector2(15, 12);
            
            Text quantityText = quantityObj.AddComponent<Text>();
            quantityText.fontSize = 10;
            quantityText.color = Color.white;
            quantityText.alignment = TextAnchor.MiddleRight;
            quantityText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            quantityText.raycastTarget = false;
            
            return slot;
        }

        private void SetupInventorySlots()
        {
            if (playerInventory == null) return;
            
            inventorySlots = new List<InventorySlotUI>();
            
            // Create inventory slots (3x3 grid)
            for (int i = 0; i < playerInventory.GetInventorySize(); i++)
            {
                InventorySlotUI slot = CreateInventorySlot(i, inventoryGrid);
                inventorySlots.Add(slot);
            }
            
            // Create equipment slots
            CreateEquipmentSlots();
            
            // Subscribe to inventory changes
            playerInventory.OnInventoryChanged += RefreshInventoryDisplay;
            playerInventory.OnEquipmentChanged += RefreshEquipmentDisplay;
            playerInventory.OnQuickUseChanged += RefreshQuickUseDisplay;
            playerInventory.OnInventoryChanged += RefreshHotkeyDisplay;
            playerInventory.OnQuickUseChanged += RefreshHotkeyDisplay;
            
            RefreshInventoryDisplay();
            RefreshEquipmentDisplay();
            RefreshQuickUseDisplay();
            RefreshHotkeyDisplay();
        }

        private InventorySlotUI CreateInventorySlot(int slotIndex, Transform parent, bool isHotbar = false)
        {
            GameObject slotObj = new GameObject($"Slot_{slotIndex}");
            slotObj.transform.SetParent(parent, false);
            
            RectTransform slotRect = slotObj.AddComponent<RectTransform>();
            slotRect.sizeDelta = new Vector2(slotSize, slotSize);
            
            Image slotImage = slotObj.AddComponent<Image>();
            slotImage.color = new Color(0.3f, 0.3f, 0.3f, 0.8f);
            slotImage.raycastTarget = true; // Ensure it can receive drag events
            
            Button slotButton = slotObj.AddComponent<Button>();
            
            // Create item icon
            GameObject iconObj = new GameObject("ItemIcon");
            iconObj.transform.SetParent(slotObj.transform, false);
            
            RectTransform iconRect = iconObj.AddComponent<RectTransform>();
            iconRect.anchorMin = Vector2.zero;
            iconRect.anchorMax = Vector2.one;
            iconRect.offsetMin = Vector2.zero;
            iconRect.offsetMax = Vector2.zero;
            
            Image iconImage = iconObj.AddComponent<Image>();
            iconImage.color = Color.clear;
            iconImage.raycastTarget = false; // Don't block drag events
            
            // Create quantity text
            GameObject quantityObj = new GameObject("QuantityText");
            quantityObj.transform.SetParent(slotObj.transform, false);
            
            RectTransform quantityRect = quantityObj.AddComponent<RectTransform>();
            quantityRect.anchorMin = new Vector2(1, 0);
            quantityRect.anchorMax = new Vector2(1, 0);
            quantityRect.pivot = new Vector2(1, 0);
            quantityRect.anchoredPosition = new Vector2(-3, 3);
            quantityRect.sizeDelta = new Vector2(25, 20);
            
            Text quantityText = quantityObj.AddComponent<Text>();
            quantityText.fontSize = 14;
            quantityText.color = Color.yellow;
            quantityText.alignment = TextAnchor.MiddleCenter;
            quantityText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            quantityText.fontStyle = FontStyle.Bold;
            quantityText.raycastTarget = false; // Don't block drag events
            
            // Add outline for better visibility
            Outline outline = quantityObj.AddComponent<Outline>();
            outline.effectColor = Color.black;
            outline.effectDistance = new Vector2(1, 1);
            
            // Add hotbar number for hotbar slots
            if (isHotbar)
            {
                GameObject numberObj = new GameObject("HotbarNumber");
                numberObj.transform.SetParent(slotObj.transform, false);
                
                RectTransform numberRect = numberObj.AddComponent<RectTransform>();
                numberRect.anchorMin = new Vector2(0, 1);
                numberRect.anchorMax = new Vector2(0, 1);
                numberRect.pivot = new Vector2(0, 1);
                numberRect.anchoredPosition = new Vector2(5, -5);
                numberRect.sizeDelta = new Vector2(15, 15);
                
                Text numberText = numberObj.AddComponent<Text>();
                numberText.text = (slotIndex + 1).ToString();
                numberText.fontSize = 10;
                numberText.color = Color.yellow;
                numberText.alignment = TextAnchor.MiddleCenter;
                numberText.fontStyle = FontStyle.Bold;
                numberText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                numberText.raycastTarget = false; // Don't block drag events
            }
            
            InventorySlotUI slotUI = slotObj.AddComponent<InventorySlotUI>();
            slotUI.Initialize(slotIndex, iconImage, quantityText, slotButton, isHotbar);
            
            return slotUI;
        }

        private void CreateEquipmentSlots()
        {
            // Equipment slots positioned to the left of character preview
            // Weapon slot (top left)
            weaponSlot = CreateEquipmentSlot("Sword", new Vector2(-120, 40), EquipmentSlot.MainHand);
            
            // Shield slot (middle left) 
            armorSlot = CreateEquipmentSlot("Shield", new Vector2(-120, 0), EquipmentSlot.Armor);
            
            // Accessory slot (bottom left)
            accessorySlot = CreateEquipmentSlot("Item", new Vector2(-120, -40), EquipmentSlot.Accessory);
        }

        private EquipmentSlotUI CreateEquipmentSlot(string slotName, Vector2 position, EquipmentSlot slotType)
        {
            GameObject slotObj = new GameObject($"{slotName}Slot");
            slotObj.transform.SetParent(characterPreviewPanel.transform, false);
            
            RectTransform slotRect = slotObj.AddComponent<RectTransform>();
            slotRect.anchorMin = new Vector2(0.5f, 0.5f);
            slotRect.anchorMax = new Vector2(0.5f, 0.5f);
            slotRect.pivot = new Vector2(0.5f, 0.5f);
            slotRect.anchoredPosition = position;
            slotRect.sizeDelta = new Vector2(50, 50);
            
            Image slotImage = slotObj.AddComponent<Image>();
            slotImage.color = new Color(0.4f, 0.4f, 0.3f, 0.9f); // Link's Awakening style color
            
            Button slotButton = slotObj.AddComponent<Button>();
            
            // Create item icon
            GameObject iconObj = new GameObject("ItemIcon");
            iconObj.transform.SetParent(slotObj.transform, false);
            
            RectTransform iconRect = iconObj.AddComponent<RectTransform>();
            iconRect.anchorMin = Vector2.zero;
            iconRect.anchorMax = Vector2.one;
            iconRect.offsetMin = new Vector2(3, 3);
            iconRect.offsetMax = new Vector2(-3, -3);
            
            Image iconImage = iconObj.AddComponent<Image>();
            iconImage.color = Color.clear;
            iconImage.raycastTarget = false;
            
            // Create label below slot
            GameObject labelObj = new GameObject("SlotLabel");
            labelObj.transform.SetParent(slotObj.transform, false);
            
            RectTransform labelRect = labelObj.AddComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0, 0);
            labelRect.anchorMax = new Vector2(1, 0);
            labelRect.pivot = new Vector2(0.5f, 1);
            labelRect.anchoredPosition = new Vector2(0, -5);
            labelRect.sizeDelta = new Vector2(0, 15);
            
            Text labelText = labelObj.AddComponent<Text>();
            labelText.text = slotName.ToUpper();
            labelText.fontSize = 8;
            labelText.color = new Color(0.9f, 0.9f, 0.7f);
            labelText.alignment = TextAnchor.MiddleCenter;
            labelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            labelText.raycastTarget = false;
            
            EquipmentSlotUI slotUI = slotObj.AddComponent<EquipmentSlotUI>();
            slotUI.Initialize(slotType, iconImage, slotButton);
            
            return slotUI;
        }

        private void FindPlayerInventory()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerInventory = player.GetComponent<Inventory>();
            }
            
            if (playerInventory == null)
            {
                playerInventory = FindFirstObjectByType<Inventory>();
            }
        }

        public void ToggleInventory()
        {
            if (isInventoryOpen)
            {
                CloseInventory();
            }
            else
            {
                OpenInventory();
            }
        }

        public void OpenInventory()
        {
            isInventoryOpen = true;
            inventoryPanel.SetActive(true);
            characterPreviewPanel.SetActive(true);
            
            // Ensure quick-use panel stays visible
            if (quickUsePanel != null)
            {
                quickUsePanel.SetActive(true);
            }
            
            // Pause game
            Time.timeScale = 0f;
            
            RefreshInventoryDisplay();
            RefreshEquipmentDisplay();
            RefreshQuickUseDisplay();
            RefreshHotkeyDisplay();
        }

        public void CloseInventory()
        {
            isInventoryOpen = false;
            inventoryPanel.SetActive(false);
            characterPreviewPanel.SetActive(false);
            
            // Resume game
            Time.timeScale = 1f;
        }

        private void UseQuickItemY()
        {
            if (playerInventory != null)
            {
                playerInventory.UseQuickItemY();
                RefreshInventoryDisplay();
                RefreshQuickUseDisplay();
            }
        }
        
        private void UseQuickItemX()
        {
            if (playerInventory != null)
            {
                playerInventory.UseQuickItemX();
                RefreshInventoryDisplay();
                RefreshQuickUseDisplay();
            }
        }

        private void RefreshInventoryDisplay()
        {
            if (playerInventory == null || inventorySlots == null) return;
            
            var slots = playerInventory.GetInventorySlots();
            
            for (int i = 0; i < inventorySlots.Count && i < slots.Count; i++)
            {
                inventorySlots[i].UpdateDisplay(slots[i]);
            }
        }

        private void RefreshEquipmentDisplay()
        {
            if (playerInventory == null) return;
            
            var equipment = playerInventory.GetEquipment();
            
            weaponSlot?.UpdateDisplay(equipment.weapon);
            armorSlot?.UpdateDisplay(equipment.armor);
            accessorySlot?.UpdateDisplay(equipment.accessory);
        }
        
        private void RefreshQuickUseDisplay()
        {
            if (playerInventory == null) return;
            
            // Update Y/Q slot - get from hotkey slot Q
            var hotkeySlotQ = playerInventory.GetHotkeySlotQ();
            if (!hotkeySlotQ.IsEmpty && hotkeySlotQ.item.Data.icon != null)
            {
                quickUseYIcon.sprite = hotkeySlotQ.item.Data.icon;
                quickUseYIcon.color = Color.white;
                
                // Show quantity if more than 1
                if (hotkeySlotQ.quantity > 1)
                {
                    quickUseYText.text = hotkeySlotQ.quantity.ToString();
                }
                else
                {
                    quickUseYText.text = "";
                }
            }
            else
            {
                quickUseYIcon.sprite = null;
                quickUseYIcon.color = Color.clear;
                quickUseYText.text = "";
            }
            
            // Update X/E slot - get from hotkey slot E
            var hotkeySlotE = playerInventory.GetHotkeySlotE();
            if (!hotkeySlotE.IsEmpty && hotkeySlotE.item.Data.icon != null)
            {
                quickUseXIcon.sprite = hotkeySlotE.item.Data.icon;
                quickUseXIcon.color = Color.white;
                
                // Show quantity if more than 1
                if (hotkeySlotE.quantity > 1)
                {
                    quickUseXText.text = hotkeySlotE.quantity.ToString();
                }
                else
                {
                    quickUseXText.text = "";
                }
            }
            else
            {
                quickUseXIcon.sprite = null;
                quickUseXIcon.color = Color.clear;
                quickUseXText.text = "";
            }
        }
        
        public void OnSlotSelected(int slotIndex)
        {
            selectedSlotIndex = slotIndex;
            Debug.Log($"Selected slot {slotIndex}");
        }
        
        public void OnInventorySlotClicked(int slotIndex)
        {
            // This is now handled by the slot's selection system
            // Keep for backward compatibility
        }
        
        public void OnInventorySlotRightClicked(int slotIndex)
        {
            if (playerInventory == null) return;
            
            Item item = playerInventory.GetItem(slotIndex);
            if (item == null) return;
            
            // Right-click context menu options
            if (item.Data.itemType == ItemType.Weapon || item.Data.itemType == ItemType.Equipment)
            {
                // Equip the item
                playerInventory.EquipItem(slotIndex);
            }
            else if (item.Data.isConsumable)
            {
                // Show quick-use assignment options
                ShowQuickUseMenu(slotIndex);
            }
        }
        
        private void ShowQuickUseMenu(int slotIndex)
        {
            Item item = playerInventory.GetItem(slotIndex);
            if (item == null) return;
            
            // For now, cycle between Y and X slots
            if (playerInventory.GetQuickUseItemY() == item)
            {
                playerInventory.SetQuickUseItemX(item);
                Debug.Log($"Assigned {item.Data.itemName} to X/E slot");
            }
            else
            {
                playerInventory.SetQuickUseItemY(item);
                Debug.Log($"Assigned {item.Data.itemName} to Y/Q slot");
            }
        }
        
        public void SwapItems(int fromSlot, int toSlot)
        {
            if (playerInventory == null) return;
            
            // Handle hotkey slot assignments
            if (toSlot == -100) // Q hotkey slot
            {
                AssignToHotkeyQ(fromSlot);
                return;
            }
            else if (toSlot == -101) // E hotkey slot
            {
                AssignToHotkeyE(fromSlot);
                return;
            }
            
            // Handle regular inventory swapping
            if (playerInventory.MoveItem(fromSlot, toSlot))
            {
                Debug.Log($"Moved/swapped items between slot {fromSlot} and slot {toSlot}");
            }
        }
        
        private void AssignToHotkeyQ(int inventorySlotIndex)
        {
            if (playerInventory == null) return;
            
            var item = playerInventory.GetItem(inventorySlotIndex);
            if (item != null && item.Data.isConsumable)
            {
                playerInventory.SetHotkeyQ(inventorySlotIndex);
                Debug.Log($"Assigned {item.Data.itemName} to Q hotkey");
            }
            else
            {
                Debug.Log("Cannot assign non-consumable item to hotkey");
            }
        }
        
        private void AssignToHotkeyE(int inventorySlotIndex)
        {
            if (playerInventory == null) return;
            
            var item = playerInventory.GetItem(inventorySlotIndex);
            if (item != null && item.Data.isConsumable)
            {
                playerInventory.SetHotkeyE(inventorySlotIndex);
                Debug.Log($"Assigned {item.Data.itemName} to E hotkey");
            }
            else
            {
                Debug.Log("Cannot assign non-consumable item to hotkey");
            }
        }
        
        // Keyboard navigation support
        private void HandleKeyboardNavigation()
        {
            if (!isInventoryOpen) return;
            
            // Arrow key navigation
            int currentIndex = selectedSlotIndex;
            if (currentIndex == -1) currentIndex = 0;
            
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                int newIndex = currentIndex - inventoryColumns;
                if (newIndex >= 0)
                {
                    inventorySlots[newIndex].GetComponent<InventorySlotUI>()?.OnPointerClick(new PointerEventData(EventSystem.current) { button = PointerEventData.InputButton.Left });
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                int newIndex = currentIndex + inventoryColumns;
                if (newIndex < inventorySlots.Count)
                {
                    inventorySlots[newIndex].GetComponent<InventorySlotUI>()?.OnPointerClick(new PointerEventData(EventSystem.current) { button = PointerEventData.InputButton.Left });
                }
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                int newIndex = currentIndex - 1;
                if (newIndex >= 0 && currentIndex % inventoryColumns != 0)
                {
                    inventorySlots[newIndex].GetComponent<InventorySlotUI>()?.OnPointerClick(new PointerEventData(EventSystem.current) { button = PointerEventData.InputButton.Left });
                }
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                int newIndex = currentIndex + 1;
                if (newIndex < inventorySlots.Count && currentIndex % inventoryColumns != inventoryColumns - 1)
                {
                    inventorySlots[newIndex].GetComponent<InventorySlotUI>()?.OnPointerClick(new PointerEventData(EventSystem.current) { button = PointerEventData.InputButton.Left });
                }
            }
            
            // Enter/Space to use/equip selected item
            if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)) && selectedSlotIndex >= 0)
            {
                Item item = playerInventory.GetItem(selectedSlotIndex);
                if (item != null)
                {
                    if (item.Data.itemType == ItemType.Weapon || item.Data.itemType == ItemType.Equipment)
                    {
                        playerInventory.EquipItem(selectedSlotIndex);
                    }
                    else if (item.Data.isConsumable)
                    {
                        playerInventory.UseConsumable(selectedSlotIndex);
                    }
                }
            }
        }
        
        private void RefreshHotkeyDisplay()
        {
            if (playerInventory == null) return;
            
            // Update Q hotkey slot
            if (hotkeySlotQ != null)
            {
                hotkeySlotQ.UpdateDisplay(playerInventory.GetHotkeySlotQ());
            }
            
            // Update E hotkey slot
            if (hotkeySlotE != null)
            {
                hotkeySlotE.UpdateDisplay(playerInventory.GetHotkeySlotE());
            }
        }
    }
}