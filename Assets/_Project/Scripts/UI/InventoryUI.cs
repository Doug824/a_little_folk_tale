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
            
            FindPlayerInventory();
            SetupInventorySlots();
            CloseInventory(); // Start closed
            
            // Quick-use panel should always be visible
            if (quickUsePanel != null)
            {
                quickUsePanel.SetActive(true);
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
            
            // Create main inventory panel
            CreateInventoryPanel();
            CreateCharacterPreviewPanel();
            CreateEquipmentPanel();
            CreateQuickUsePanel();
        }

        private void CreateInventoryPanel()
        {
            inventoryPanel = new GameObject("InventoryPanel");
            inventoryPanel.transform.SetParent(inventoryCanvas.transform, false);
            
            RectTransform panelRect = inventoryPanel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.3f, 0.5f);
            panelRect.anchorMax = new Vector2(0.3f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(300, 400);
            
            Image panelImage = inventoryPanel.AddComponent<Image>();
            panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
            
            // Create title
            GameObject title = new GameObject("InventoryTitle");
            title.transform.SetParent(inventoryPanel.transform, false);
            
            RectTransform titleRect = title.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 1);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.pivot = new Vector2(0.5f, 1);
            titleRect.anchoredPosition = new Vector2(0, 0);
            titleRect.sizeDelta = new Vector2(0, 40);
            
            Text titleText = title.AddComponent<Text>();
            titleText.text = "Bag";
            titleText.fontSize = 24;
            titleText.color = Color.white;
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.fontStyle = FontStyle.Bold;
            titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            
            // Create inventory grid
            GameObject gridObj = new GameObject("InventoryGrid");
            gridObj.transform.SetParent(inventoryPanel.transform, false);
            
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
            
            inventoryGrid = gridRect;
        }

        private void CreateCharacterPreviewPanel()
        {
            characterPreviewPanel = new GameObject("CharacterPreviewPanel");
            characterPreviewPanel.transform.SetParent(inventoryCanvas.transform, false);
            
            RectTransform previewRect = characterPreviewPanel.AddComponent<RectTransform>();
            previewRect.anchorMin = new Vector2(0.7f, 0.5f);
            previewRect.anchorMax = new Vector2(0.7f, 0.5f);
            previewRect.pivot = new Vector2(0.5f, 0.5f);
            previewRect.anchoredPosition = Vector2.zero;
            previewRect.sizeDelta = new Vector2(400, 400);
            
            Image previewBg = characterPreviewPanel.AddComponent<Image>();
            previewBg.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
            
            // Character preview title
            GameObject previewTitle = new GameObject("PreviewTitle");
            previewTitle.transform.SetParent(characterPreviewPanel.transform, false);
            
            RectTransform titleRect = previewTitle.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 1);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.pivot = new Vector2(0.5f, 1);
            titleRect.anchoredPosition = new Vector2(0, 0);
            titleRect.sizeDelta = new Vector2(0, 40);
            
            Text titleText = previewTitle.AddComponent<Text>();
            titleText.text = "Equipment";
            titleText.fontSize = 24;
            titleText.color = Color.white;
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.fontStyle = FontStyle.Bold;
            titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            
            // Character preview window
            GameObject previewWindow = new GameObject("CharacterPreview");
            previewWindow.transform.SetParent(characterPreviewPanel.transform, false);
            
            RectTransform windowRect = previewWindow.AddComponent<RectTransform>();
            windowRect.anchorMin = new Vector2(0.5f, 0.5f);
            windowRect.anchorMax = new Vector2(0.5f, 0.5f);
            windowRect.pivot = new Vector2(0.5f, 0.5f);
            windowRect.anchoredPosition = new Vector2(0, 0);
            windowRect.sizeDelta = new Vector2(200, 200);
            
            characterPreviewImage = previewWindow.AddComponent<RawImage>();
            characterPreviewImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        }
        
        private void CreateEquipmentPanel()
        {
            // Equipment slots are now part of the character preview panel
            equipmentPanel = characterPreviewPanel.transform as RectTransform;
        }

        private void CreateQuickUsePanel()
        {
            // Quick-use panel in top-right corner of screen (always visible)
            quickUsePanel = new GameObject("QuickUsePanel");
            quickUsePanel.transform.SetParent(inventoryCanvas.transform, false);
            
            RectTransform quickRect = quickUsePanel.AddComponent<RectTransform>();
            quickRect.anchorMin = new Vector2(1f, 1f);
            quickRect.anchorMax = new Vector2(1f, 1f);
            quickRect.pivot = new Vector2(1f, 1f);
            quickRect.anchoredPosition = new Vector2(-20, -20);
            quickRect.sizeDelta = new Vector2(150, 80);
            
            // Y/Q button slot
            GameObject ySlot = CreateQuickUseSlot("Y/Q", new Vector2(-80, -40));
            quickUseYIcon = ySlot.transform.Find("Icon").GetComponent<Image>();
            quickUseYText = ySlot.transform.Find("ButtonText").GetComponent<Text>();
            
            // X/E button slot
            GameObject xSlot = CreateQuickUseSlot("X/E", new Vector2(-20, -40));
            quickUseXIcon = xSlot.transform.Find("Icon").GetComponent<Image>();
            quickUseXText = xSlot.transform.Find("ButtonText").GetComponent<Text>();
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
            
            RefreshInventoryDisplay();
            RefreshEquipmentDisplay();
            RefreshQuickUseDisplay();
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
            
            // Create quantity text
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
            }
            
            InventorySlotUI slotUI = slotObj.AddComponent<InventorySlotUI>();
            slotUI.Initialize(slotIndex, iconImage, quantityText, slotButton, isHotbar);
            
            return slotUI;
        }

        private void CreateEquipmentSlots()
        {
            // Equipment slots positioned around character preview
            // Weapon slot (left side)
            weaponSlot = CreateEquipmentSlot("Weapon", new Vector2(-150, 0), EquipmentSlot.MainHand);
            
            // Armor slot (top)
            armorSlot = CreateEquipmentSlot("Armor", new Vector2(0, 150), EquipmentSlot.Armor);
            
            // Accessory slot (right side)
            accessorySlot = CreateEquipmentSlot("Accessory", new Vector2(150, 0), EquipmentSlot.Accessory);
        }

        private EquipmentSlotUI CreateEquipmentSlot(string slotName, Vector2 position, EquipmentSlot slotType)
        {
            GameObject slotObj = new GameObject($"{slotName}Slot");
            slotObj.transform.SetParent(equipmentPanel, false);
            
            RectTransform slotRect = slotObj.AddComponent<RectTransform>();
            slotRect.anchoredPosition = position;
            slotRect.sizeDelta = new Vector2(60, 60);
            
            Image slotImage = slotObj.AddComponent<Image>();
            slotImage.color = new Color(0.2f, 0.4f, 0.2f, 0.8f);
            
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
            
            // Create label
            GameObject labelObj = new GameObject("SlotLabel");
            labelObj.transform.SetParent(slotObj.transform, false);
            
            RectTransform labelRect = labelObj.AddComponent<RectTransform>();
            labelRect.anchoredPosition = new Vector2(0, -35);
            labelRect.sizeDelta = new Vector2(80, 20);
            
            Text labelText = labelObj.AddComponent<Text>();
            labelText.text = slotName;
            labelText.fontSize = 10;
            labelText.color = Color.white;
            labelText.alignment = TextAnchor.MiddleCenter;
            labelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            
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
            
            // Pause game
            Time.timeScale = 0f;
            
            RefreshInventoryDisplay();
            RefreshEquipmentDisplay();
            RefreshQuickUseDisplay();
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
            
            // Update Y/Q slot
            Item itemY = playerInventory.GetQuickUseItemY();
            if (itemY != null && itemY.Data.icon != null)
            {
                quickUseYIcon.sprite = itemY.Data.icon;
                quickUseYIcon.color = Color.white;
            }
            else
            {
                quickUseYIcon.sprite = null;
                quickUseYIcon.color = Color.clear;
            }
            
            // Update X/E slot
            Item itemX = playerInventory.GetQuickUseItemX();
            if (itemX != null && itemX.Data.icon != null)
            {
                quickUseXIcon.sprite = itemX.Data.icon;
                quickUseXIcon.color = Color.white;
            }
            else
            {
                quickUseXIcon.sprite = null;
                quickUseXIcon.color = Color.clear;
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
            
            if (playerInventory.MoveItem(fromSlot, toSlot))
            {
                Debug.Log($"Moved/swapped items between slot {fromSlot} and slot {toSlot}");
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
    }
}