using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ShinyOwl.Utils;
using System;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public Player player;

    public TextMeshProUGUI timeText; public TextMeshProUGUI depthText;
    public TextMeshProUGUI weightText; public TextMeshProUGUI temperatureText;
    public TextMeshProUGUI balanceText;
    public TextMeshProUGUI analysisTitle; public TextMeshProUGUI analysisDescription; public Image analysisDisplay;
    public Animator analysisAC;
    public Transform playerInventoryT;
    public RectTransform codexEntriesMainOffsetT;
    public Slider codexEntriesSlider;
    public float codexSliderAmplifier; // offset amplifier scaling to amount of species.
    public TextMeshProUGUI codexSpeciesNameText;
    public TextMeshProUGUI codexSpeciesDescriptionText;
    public Transform hotbarReference;
    public GameObject fillBarPrefab;
    private static int hotbarBarCount = 0;
    private const float HOTBAR_Y_SPACING = 23.5f;
    [SerializeField] private RectTransform lightPointer;
    [SerializeField] private RectTransform torchLight;
    //[SerializeField] private RectTransform pressurePointer;
    [SerializeField] private RectTransform pressurePointerOrigin;
    private Image darknessOverlay;

    public Inventory playerInventory;
    private InventoryDisplay playerInventoryDisplay;

    private void Start()
    {
        //GC.BuildUIImage("Left Panel", null, new Vector2(-400f, 0f), new Vector2(150, 450), "sprite:builtin:background", new Color(1f, 1f, 1f, 0.25f));
        //GC.BuildUIText("Dive Data Title", null, new Vector2(-363f, 210f), new Vector2(75, 25), "Dive Data", 14, TextAnchor.MiddleCenter, "font:arial", FontStyle.Bold, Color.black, 5);
        //timeText = GC.BuildUIText("Dive Data Time Text", null, new Vector2(-363f, 190f), new Vector2(75, 15), "Time: 1300", 8, TextAnchor.MiddleLeft, "font:arial", FontStyle.Normal, Color.black, 5);
        //depthText = GC.BuildUIText("Dive Data Depth Text", null, new Vector2(-363f, 180f), new Vector2(75, 15), "Depth: 060m", 8, TextAnchor.MiddleLeft, "font:arial", FontStyle.Normal, Color.black, 5);
        //weightText = GC.BuildUIText("Dive Data Weight Text", null, new Vector2(-363f, 170f), new Vector2(75, 15), "Weight: 67kg", 8, TextAnchor.MiddleLeft, "font:arial", FontStyle.Normal, Color.black, 5);
        //temperatureText = GC.BuildUIText("Dive Data Temperature Text", null, new Vector2(-363f, 160f), new Vector2(75, 15), "Temperature: -24°C", 8, TextAnchor.MiddleLeft, "font:arial", FontStyle.Normal, Color.black, 5);
        //balanceText = GC.BuildUIText("Dive Data Balance Text", null, new Vector2(-363f, 150f), new Vector2(75, 15), "Balance: $0", 8, TextAnchor.MiddleLeft, "font:arial", FontStyle.Normal, Color.black, 5);
        //GC.BuildUIImage("Dive Data Pressure Wheel", null, new Vector2(-363f, 110f), new Vector2(75, 75), "sprite:builtin:knob", Color.white);
        //pressurePointer = GC.BuildUIEmpty("Dive Data Pressure Pointer Origin", null, new Vector2(-363f, 110f));
        //GC.BuildUIImage("Dive Data Pressure Pointer", pressurePointer.transform, new Vector2(0f, 15f), new Vector2(5, 30), "sprite:builtin:background", Color.black);
        //GC.BuildUIImage("Right Panel Top Half", null, new Vector2(400f, 0f), new Vector2(150, 450), "sprite:builtin:background", new Color(1f, 1f, 1f, 0.25f));
        //GC.BuildUIImage("Analysis Display Backdrop", null, new Vector2(363f, 187f), new Vector2(75, 75), "sprite:builtin:knob", Color.white);
        //analysisDisplay = GC.BuildUIImage("Analysis Display", null, new Vector2(363f, 187f), new Vector2(75, 75), null, Color.white);
        //analysisTitle = GC.BuildUIText("Analysis Display Title", null, new Vector2(363f, 140f), new Vector2(75, 25), "Fish Name", 14, TextAnchor.MiddleCenter, "font:arial", FontStyle.Bold, Color.black, 5);
        //analysisDescription = GC.BuildUIText("Analysis Display Description", null, new Vector2(363f, 63f), new Vector2(75, 130), "This is just sample text I am just filling the empty space to see what it looks like.", 8, TextAnchor.UpperLeft, "font:arial", FontStyle.Normal, Color.black, 5);
        //GC.BuildUIImage("Right Panel Bottom Half", null, new Vector2(400f, -113f), new Vector2(150, 225), "sprite:builtin:background", new Color(0.5f, 0.5f, 0.5f, 1f));

        int inventoryWidth = 3; int inventoryHeight = 2;
        playerInventory = new Inventory(inventoryWidth * inventoryHeight);

        playerInventoryDisplay = new InventoryDisplay(inventoryWidth, inventoryHeight, 30f, 15f, new Vector3(0f, 0f));
        playerInventoryDisplay.Main.transform.SetParent(playerInventoryT);
        playerInventoryDisplay.Main.transform.localPosition = new Vector3(0f, 15f);
        UpdateInventoryDisplay(playerInventory, playerInventoryDisplay);

        string[] entryIDs = new string[] { "creature:squid", "creature:widemouth_bass", "creature:snapper", "creature:bluefish",
        "creature:atlantic_croaker", "creature:western_blue_groper", "creature:hammerhead_shark", "creature:orange_coral",
        "creature:yellow_coral", "creature:orange_cup_coral" };
        for (int i = 0; i < entryIDs.Length; i++)
        {
            //RectTransform entry = new GameObject(entryIDs[i]).AddComponent<RectTransform>();
            CreatureBase creature = GC.GetReference<CreatureBase>(entryIDs[i]);
            Vector2 start = new Vector2(-210f, 0f);
            Vector2 displacement = new Vector2(95f * i, 0f);
            Image entry = GC.BuildUIImage(entryIDs[i], codexEntriesMainOffsetT, start + displacement, new Vector2(75f, 75f), "sprite:builtin:background", Color.white);
            entry.sprite = creature.DisplaySprite;
            Button button = entry.gameObject.AddComponent<Button>();
            button.onClick.AddListener(() => 
            {
                CreatureBase species = GC.GetReference<CreatureBase>(creature.ReferenceID);
                codexSpeciesNameText.text = species.Name;
                codexSpeciesDescriptionText.text = species.Description;
            });     
            //entry.rectTransform.localPosition *= GC.GetUIScale();
        }
        codexSliderAmplifier = 435f;
        codexSpeciesNameText.text = "";
        codexSpeciesDescriptionText.text = "";

        ObtainItems(GC.GetReference<InventoryItem>("item:small_ballast_weight"), 12);

        darknessOverlay = GC.BuildUIImage("Darkness Overlay", null, Vector2.zero, new Vector2(1500f, 1500f), "sprite:builtin:background", new Color(0f, 0f, 0f, 1f), isCutout: true);
        darknessOverlay.transform.SetParent(torchLight.transform);

        hotbarBarCount = 0;
    }

    private void Update()
    {
        // Update pos of creature icons slider main go
        codexEntriesMainOffsetT.transform.localPosition = new Vector2(0f - codexEntriesSlider.value * codexSliderAmplifier, 0f);

        if (!player.DoUpdate()) return;
        Vector2 dir = new Vector2(Input.mousePosition.x - Screen.width * 0.5f, Input.mousePosition.y - Screen.height * 0.5f);
        lightPointer.transform.rotation = Quaternion.LookRotation(Vector3.forward, dir);
        pressurePointerOrigin.transform.eulerAngles = new Vector3(0f, 0f, Mathf.Clamp(232f - 283f * (player.darkness * 1.5f), -51f, 232f));

        if (Input.GetKey("1") && !Input.GetKey("3"))
        {
            if (Input.GetKeyDown("1"))
            {
                float pitchChange = 0f;
                for (int i = 0; i < 2; i++)
                {
                    GC.PlaySound("sound:buoyancy_adjust1", 0.45f, 0.8f + pitchChange, delay: Mathf.Abs(pitchChange) * 1.25f);
                    pitchChange -= 0.05f;
                }
            }
            player.BCInflation -= 2f * Time.deltaTime;
        }
        else if (Input.GetKey("3"))
        {
            if (Input.GetKeyDown("3"))
            {
                float pitchChange = 0f;
                for (int i = 0; i < 2; i++)
                {
                    GC.PlaySound("sound:buoyancy_adjust1", 0.45f, 1f + pitchChange, delay: Mathf.Abs(pitchChange) * 1.25f);
                    pitchChange += 0.05f;
                }
            }
            player.BCInflation += 2f * Time.deltaTime;
        }
    }

    public void SetDarkness(float newOpacity) { darknessOverlay.color = new Color(darknessOverlay.color.r, darknessOverlay.color.g, darknessOverlay.color.b, newOpacity); }
    public void SetWeight(float newWeight) { weightText.text = $"{newWeight}kg"; }
    public void SetDepth(int newDepth) { depthText.text = $"{newDepth}m"; }
    public void SetAnalysis(string title, string description, Sprite sprite)
    {
        analysisTitle.text = title;
        analysisDescription.text = description;
        analysisDisplay.sprite = sprite;
        analysisDisplay.SetNativeSize();
    }
    public void SetBalance(float newBalance) { balanceText.text = $"${newBalance}"; }

    public void CreateFillBar(Func<float> getValue, Func<bool> killCondition, string textValue, Color fillColor)
    {
        GameObject fillBar = Instantiate(fillBarPrefab, hotbarReference);
        fillBar.transform.localPosition += new Vector3(0f, HOTBAR_Y_SPACING * hotbarBarCount);
        hotbarBarCount++;
        Transform origin = fillBar.transform.GetChild(1);
        origin.transform.localScale = new Vector3(Mathf.Clamp(getValue(), 0f, 1f), 1f);
        origin.GetChild(0).GetComponent<Image>().color = fillColor;
        TextMeshProUGUI textUI = fillBar.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        textUI.text = textValue;
        FunctionUpdater.Create(() =>
        {
            if (killCondition() == true)
            {
                hotbarBarCount--;
                Destroy(fillBar);
                return true;
            }
            else
            {
                origin.transform.localScale = new Vector3(Mathf.Clamp(getValue(), 0f, 1f), 1f);
                return false;
            }
        });
    }

    public void ObtainItems(InventoryItem item, int quantity)
    {
        playerInventory.AddItems(item, quantity);
        UpdateInventoryDisplay(playerInventory, playerInventoryDisplay);
        UtilsClass.CreateWorldTextPopup($"+{quantity} {item.Name}", player.transform, Vector3.zero, new Vector3(0f, 2.5f), 1.5f, 4, Color.green);
    }
    private void UpdateInventoryDisplay(Inventory inventory, InventoryDisplay display)
    {
        for (int y = 0; y < display.Height; y++) {
            for (int x = 0; x < display.Width; x++) {
                int currentIndex = x + display.Width * y;
                InventorySlot slot = inventory.GetSlot(currentIndex);
                display.ModifyVisual(x, y, GC.GetReference<Sprite>(slot.GetContainedSpriteID()), slot.GetContainedItemName(), slot.Quantity);
            }
        }

    }
    public void SellAllPlayerItems()
    {
        bool hasItems = false;
        for (int i = 0; i < playerInventory.GetSize(); i++)
        {
            InventorySlot slot = playerInventory.GetSlot(i);
            if (!slot.IsOccupied()) continue;
            hasItems = true;
        }
        if (!hasItems)
        {
            UtilsClass.CreateWorldTextPopup("Inventory is empty!", player.transform, Vector3.zero, new Vector3(0f, 2.5f), 1.5f, 4, Color.red);
            return;
        }

        float totalCurrencyGain = 0f;
        float delayTimer = 0f;
        float delayIncrement = 0.15f;
        for (int i = 0; i < playerInventory.GetSize(); i++)
        {
            InventorySlot slot = playerInventory.GetSlot(i);
            if (!slot.IsOccupied()) continue;
            float currencyGain = slot.Item.CurrencyValue;
            totalCurrencyGain += currencyGain * slot.Quantity;
            for (int j = 0; j < slot.Quantity; j++)
            {
                FunctionTimer.Create(
                    () =>
                    {
                        UtilsClass.CreateWorldTextPopup($"+${currencyGain}", player.transform, Vector3.zero, new Vector3(0f, 2.5f), 1.5f, 4, Color.green);
                        GC.PlaySound("sound:earn_money1", 0.6f, pitchRandomness: 0f);
                    },
                    delayTimer);
                delayTimer += delayIncrement;
            }
            slot.RemoveItems(slot.Quantity);
        }
        player.AddBalance(totalCurrencyGain);
        UpdateInventoryDisplay(playerInventory, playerInventoryDisplay);
    }
    public float GetPlayerWeight()
    {
        float weight = Player.HUMAN_WEIGHT;
        for (int i = 0; i < playerInventory.GetSize(); i++)
        {
            InventorySlot slot = playerInventory.GetSlot(i);
            InventoryItem item = playerInventory.GetSlot(i).Item;
            if (item != null) weight += playerInventory.GetSlot(i).Item.Weight * slot.Quantity;
        }
        return Mathf.Round(weight * 10.0f) * 0.1f;
    }

    public class InventoryDisplay
    {
        private RectTransform main; public RectTransform Main { get { return main; } }
        private int width; public int Width { get { return width; } }
        private int height; public int Height { get { return height; } }
        private SlotVisual[,] slots;
        public InventoryDisplay(int _Width, int _Height, float _SlotSize, float _SlotSpacing, Vector3 _Pos)
        {
            main = new GameObject("Inventory Display").AddComponent<RectTransform>();
            main.SetParent(GC.CanvasT);
            main.anchorMin = new Vector2(0.5f, 1f); main.anchorMax = new Vector2(0.5f, 1f);
            width = _Width; height = _Height;
            slots = new SlotVisual[_Width, _Height];
            for (int y = 0; y < _Height; y++)
            {
                for (int x = 0; x < _Width; x++)
                {
                    int currentIndex = x + _Width * y;
                    slots[x, y] = new SlotVisual(
                        new Vector2((_SlotSize + _SlotSpacing) * (x - 1), -(_SlotSize + _SlotSpacing) * y),
                        GC.BuildUIImage($"Inventory Slot {currentIndex + 1} Image", null, Vector2.zero, new Vector2(_SlotSize * 0.8f, _SlotSize * 0.8f), "sprite:builtin:background", Color.white),
                        GC.BuildUIText($"Inventory Slot {currentIndex + 1} Name", null, Vector2.zero, new Vector2(_SlotSize, _SlotSize), "N/A", 5, TextAnchor.UpperLeft, "font:arial", FontStyle.Bold, Color.black, 5),
                        GC.BuildUIText($"Inventory Slot {currentIndex + 1} Quantity", null, Vector2.zero, new Vector2(_SlotSize, _SlotSize), "0", 10, TextAnchor.MiddleCenter, "font:arial", FontStyle.Bold, Color.black, 5),
                        GC.BuildUIImage($"Inventory Slot {currentIndex + 1} Backdrop", null, Vector2.zero, new Vector2(_SlotSize, _SlotSize), "sprite:builtin:background", Color.white),
                        this.main);
                }
            }
        }
        public void ModifyVisual(int x, int y, Sprite newImage, string newName, int newQuantity)
        {
            slots[x, y].ApplyNewVisuals(newImage, newName, newQuantity);
        }
    }
    public class SlotVisual
    {
        private RectTransform main; public RectTransform Main { get { return main; } }
        private Image image; public Image Image { get { return image; } }
        private Text name; public Text Name { get { return name; } }
        private Text quantity; public Text Quantity { get { return quantity; } }

        public SlotVisual(Vector2 _Pos, Image _Image, Text _Name, Text _Quantity, Image _Backdrop, Transform _Main)
        {
            main = new GameObject("Slot Visual").AddComponent<RectTransform>();
            main.transform.SetParent(_Main);
            main.anchorMin = new Vector2(0.5f, 1f); main.anchorMax = new Vector2(0.5f, 1f);
            main.transform.localPosition = new Vector2(_Pos.x, _Pos.y) * GC.GetUIScale(); ;
            _Backdrop.transform.SetParent(main); _Backdrop.transform.localPosition = Vector2.zero;
            image = _Image; image.transform.SetParent(main); image.transform.localPosition = new Vector2(0f, 0f);
            name = _Name; name.transform.SetParent(main); name.transform.localPosition = new Vector2(0f, 0f);
            quantity = _Quantity; quantity.transform.SetParent(main); quantity.transform.localPosition = new Vector2(15f, -15f);

        }
        public void ApplyNewVisuals(Sprite newImage, string newName, int newQuantity)
        {
            image.sprite = newImage;
            name.text = newName;
            quantity.text = newQuantity.ToString();
        }
    }
    public class InteractiveUI
    {
        public InteractiveUI(Vector3 _Pos, Vector3 _SizeDelta, string _SpriteID, UnityAction _Action)
        {
            Image backdrop = GC.BuildUIImage("Interactive UI", null, _Pos, _SizeDelta, "sprite:builtin:background", Color.white);
            Image icon = GC.BuildUIImage("Icon", null, _Pos, _SizeDelta, _SpriteID, Color.white);
            backdrop.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            icon.transform.SetParent(backdrop.transform);
            icon.transform.localPosition = Vector3.zero;
            Button button = icon.gameObject.AddComponent<Button>();
            button.onClick.AddListener(_Action);
        }
    }
}
