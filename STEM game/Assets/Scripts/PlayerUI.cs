using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ShinyOwl.Utils;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Player player;

    private Text timeText; private Text depthText;
    private Text weightText; private Text temperatureText;
    private Text balanceText;
    private Text analysisTitle; private Text analysisDescription; private Image analysisDisplay;
    [SerializeField] private RectTransform lightPointer;
    [SerializeField] private RectTransform torchLight;
    private Image darknessOverlay;

    public Inventory playerInventory;
    private InventoryDisplay playerInventoryDisplay;

    private void Start()
    {
        GC.BuildUIImage("Left Panel", new Vector2(-400f, 0f), new Vector2(150, 450), "sprite:builtin:background", new Color(1f, 1f, 1f, 0.25f));
        GC.BuildUIText("Dive Data Title", new Vector2(-363f, 210f), new Vector2(75, 25), "Dive Data", 14, TextAnchor.MiddleCenter, "font:arial", FontStyle.Bold, Color.black, 5);
        timeText = GC.BuildUIText("Dive Data Time Text", new Vector2(-363f, 190f), new Vector2(75, 15), "Time: 1300", 8, TextAnchor.MiddleLeft, "font:arial", FontStyle.Normal, Color.black, 5);
        depthText = GC.BuildUIText("Dive Data Depth Text", new Vector2(-363f, 180f), new Vector2(75, 15), "Depth: 060m", 8, TextAnchor.MiddleLeft, "font:arial", FontStyle.Normal, Color.black, 5);
        weightText = GC.BuildUIText("Dive Data Weight Text", new Vector2(-363f, 170f), new Vector2(75, 15), "Weight: 67kg", 8, TextAnchor.MiddleLeft, "font:arial", FontStyle.Normal, Color.black, 5);
        temperatureText = GC.BuildUIText("Dive Data Temperature Text", new Vector2(-363f, 160f), new Vector2(75, 15), "Temperature: -24°C", 8, TextAnchor.MiddleLeft, "font:arial", FontStyle.Normal, Color.black, 5);
        balanceText = GC.BuildUIText("Dive Data Balance Text", new Vector2(-363f, 150f), new Vector2(75, 15), "Balance: $0", 8, TextAnchor.MiddleLeft, "font:arial", FontStyle.Normal, Color.black, 5);
        GC.BuildUIImage("Right Panel Top Half", new Vector2(400f, 0f), new Vector2(150, 450), "sprite:builtin:background", new Color(1f, 1f, 1f, 0.25f));
        GC.BuildUIImage("Analysis Display Backdrop", new Vector2(363f, 187f), new Vector2(75, 75), "sprite:builtin:knob", Color.white);
        analysisDisplay = GC.BuildUIImage("Analysis Display", new Vector2(363f, 187f), new Vector2(75, 75), null, Color.white);
        analysisTitle = GC.BuildUIText("Analysis Display Title", new Vector2(363f, 140f), new Vector2(75, 25), "Fish Name", 14, TextAnchor.MiddleCenter, "font:arial", FontStyle.Bold, Color.black, 5);
        analysisDescription = GC.BuildUIText("Analysis Display Description", new Vector2(363f, 63f), new Vector2(75, 130), "This is just sample text I am just filling the empty space to see what it looks like.", 8, TextAnchor.UpperLeft, "font:arial", FontStyle.Normal, Color.black, 5);
        GC.BuildUIImage("Right Panel Bottom Half", new Vector2(400f, -113f), new Vector2(150, 225), "sprite:builtin:background", new Color(0.5f, 0.5f, 0.5f, 1f));

        int inventoryWidth = 2; int inventoryHeight = 1;
        playerInventory = new Inventory(inventoryWidth * inventoryHeight);
        playerInventoryDisplay = new InventoryDisplay(inventoryWidth, inventoryHeight, 30f, 35f, new Vector3(330f, -23f));
        UpdateInventoryDisplay(playerInventory, playerInventoryDisplay);
        InteractiveUI pauseButton = new InteractiveUI(new Vector3(295f, -195f), new Vector3(50f, 50f), "sprite:gear_icon", EchoTest);
        InteractiveUI sellButton = new InteractiveUI(new Vector3(240f, -195f), new Vector3(50f, 50f), "sprite:dollar_icon", SellAllPlayerItems);

        darknessOverlay = GC.BuildUIImage("Darkness Overlay", Vector2.zero, new Vector2(1500f, 1500f), "sprite:builtin:background", new Color(0f, 0f, 0f, 1f), isCutout: true);
        darknessOverlay.transform.SetParent(torchLight.transform);
    }

    private void Update()
    {
        Vector2 dir = new Vector2(Input.mousePosition.x - Screen.width * 0.5f, Input.mousePosition.y - Screen.height * 0.5f);
        lightPointer.transform.rotation = Quaternion.LookRotation(Vector3.forward, dir);
    }

    public void SetDarkness(float newOpacity) { darknessOverlay.color = new Color(darknessOverlay.color.r, darknessOverlay.color.g, darknessOverlay.color.b, newOpacity); }
    public void SetDepth(int newDepth) { depthText.text = $"Depth: {newDepth}m"; }
    public void SetAnalysis(string title, string description, Sprite sprite)
    {
        analysisTitle.text = title;
        analysisDescription.text = description;
        analysisDisplay.sprite = sprite;
    }
    public void SetBalance(float newBalance) { balanceText.text = $"Balance: ${newBalance}"; }

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
                FunctionTimer.Create(() => UtilsClass.CreateWorldTextPopup($"+${currencyGain}", player.transform, Vector3.zero, new Vector3(0f, 2.5f), 1.5f, 4, Color.green),
                    delayTimer);
                delayTimer += delayIncrement;
            }
            slot.RemoveItems(slot.Quantity);
        }
        player.AddBalance(totalCurrencyGain);
        UpdateInventoryDisplay(playerInventory, playerInventoryDisplay);
    }
    public void EchoTest()
    {
        Debug.Log("xd");
    }

    public class InventoryDisplay
    {
        private Transform main;
        private int width; public int Width { get { return width; } }
        private int height; public int Height { get { return height; } }
        private SlotVisual[,] slots;
        public InventoryDisplay(int _Width, int _Height, float _SlotSize, float _SlotSpacing, Vector3 _Pos)
        {
            main = new GameObject("Inventory Display").transform;
            main.SetParent(GC.CanvasT);
            main.transform.localPosition = _Pos;
            width = _Width; height = _Height;
            slots = new SlotVisual[_Width, _Height];
            for (int y = 0; y < _Height; y++)
            {
                for (int x = 0; x < _Width; x++)
                {
                    int currentIndex = x + _Width * y;
                    slots[x, y] = new SlotVisual(
                        new Vector2((_SlotSize + _SlotSpacing) * x, -(_SlotSize + _SlotSpacing) * y),
                        GC.BuildUIImage($"Inventory Slot {currentIndex + 1} Image", Vector2.zero, new Vector2(_SlotSize * 0.8f, _SlotSize * 0.8f), "sprite:builtin:background", Color.white),
                        GC.BuildUIText($"Inventory Slot {currentIndex + 1} Name", Vector2.zero, new Vector2(_SlotSize, _SlotSize), "N/A", 5, TextAnchor.UpperLeft, "font:arial", FontStyle.Bold, Color.black, 5),
                        GC.BuildUIText($"Inventory Slot {currentIndex + 1} Quantity", Vector2.zero, new Vector2(_SlotSize, _SlotSize), "0", 10, TextAnchor.MiddleCenter, "font:arial", FontStyle.Bold, Color.black, 5),
                        GC.BuildUIImage($"Inventory Slot {currentIndex + 1} Backdrop", Vector2.zero, new Vector2(_SlotSize, _SlotSize), "sprite:builtin:background", Color.white),
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
        private Transform main; public Transform Main { get { return main; } }
        private Image image; public Image Image { get { return image; } }
        private Text name; public Text Name { get { return name; } }
        private Text quantity; public Text Quantity { get { return quantity; } }

        public SlotVisual(Vector2 _Pos, Image _Image, Text _Name, Text _Quantity, Image _Backdrop, Transform _Main)
        {
            main = new GameObject("Slot Visual").AddComponent<RectTransform>().transform;
            main.transform.SetParent(_Main);
            main.transform.localPosition = _Pos;
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
            Image backdrop = GC.BuildUIImage("Interactive UI", _Pos, _SizeDelta, "sprite:builtin:background", Color.white);
            Image icon = GC.BuildUIImage("Icon", _Pos, _SizeDelta, _SpriteID, Color.white);
            icon.transform.SetParent(backdrop.transform);
            icon.transform.localPosition = Vector3.zero;
            Button button = icon.gameObject.AddComponent<Button>();
            button.onClick.AddListener(_Action);
        }
    }
}
