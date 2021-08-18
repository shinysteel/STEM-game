using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Player player;

    private Text timeText; private Text depthText;
    private Text weightText; private Text temperatureText;
    private Text analysisTitle; private Text analysisDescription;
    private Image darknessOverlay;

    private Inventory inventory;
    private Image[] visualInventorySlotImages;
    private Text[] visualInventorySlotNameTexts;
    private Text[] visualInventorySlotQuantityTexts;
    private const int STARTING_INVENTORY_SIZE = 4;

    private void Start()
    {
        GC.BuildUIImage("Left Panel", new Vector2(-400f, 0f), new Vector2(150, 450), "sprite:builtin:background", new Color(1f, 1f, 1f, 0.25f));
        GC.BuildUIText("Dive Data Title", new Vector2(-363f, 210f), new Vector2(75, 25), "Dive Data", 14, TextAnchor.MiddleCenter, "font:arial", FontStyle.Bold, Color.black, 5);
        timeText = GC.BuildUIText("Dive Data Time Text", new Vector2(-363f, 190f), new Vector2(75, 15), "Time: 1300", 8, TextAnchor.MiddleLeft, "font:arial", FontStyle.Normal, Color.black, 5);
        depthText = GC.BuildUIText("Dive Data Depth Text", new Vector2(-363f, 180f), new Vector2(75, 15), "Depth: 060m", 8, TextAnchor.MiddleLeft, "font:arial", FontStyle.Normal, Color.black, 5);
        weightText = GC.BuildUIText("Dive Data Weight Text", new Vector2(-363f, 170f), new Vector2(75, 15), "Weight: 67kg", 8, TextAnchor.MiddleLeft, "font:arial", FontStyle.Normal, Color.black, 5);
        temperatureText = GC.BuildUIText("Dive Data Temperature Text", new Vector2(-363f, 160f), new Vector2(75, 15), "Temperature: -24°C", 8, TextAnchor.MiddleLeft, "font:arial", FontStyle.Normal, Color.black, 5);
        GC.BuildUIImage("Right Panel Top Half", new Vector2(400f, 0f), new Vector2(150, 450), "sprite:builtin:background", new Color(1f, 1f, 1f, 0.25f));
        GC.BuildUIImage("Analysis Display Backdrop", new Vector2(363f, 187f), new Vector2(75, 75), "sprite:builtin:knob", Color.white);
        analysisTitle = GC.BuildUIText("Analysis Display Title", new Vector2(363f, 140f), new Vector2(75, 25), "Fish Name", 14, TextAnchor.MiddleCenter, "font:arial", FontStyle.Bold, Color.black, 5);
        analysisDescription = GC.BuildUIText("Analysis Display Description", new Vector2(363f, 63f), new Vector2(75, 130), "This is just sample text I am just filling the empty space to see what it looks like.", 8, TextAnchor.UpperLeft, "font:arial", FontStyle.Normal, Color.black, 5);
        GC.BuildUIImage("Right Panel Bottom Half", new Vector2(400f, -113f), new Vector2(150, 225), "sprite:builtin:background", new Color(0.5f, 0.5f, 0.5f, 1f));

        inventory = new Inventory(STARTING_INVENTORY_SIZE);
        GC.BuildUIImage("Inventory Slot 1 Backdrop", new Vector2(345f, -23f), new Vector2(30, 30), "sprite:builtin:background", Color.grey);
        GC.BuildUIImage("Inventory Slot 2 Backdrop", new Vector2(380f, -23f), new Vector2(30, 30), "sprite:builtin:background", Color.grey);
        GC.BuildUIImage("Inventory Slot 3 Backdrop", new Vector2(345f, -58f), new Vector2(30, 30), "sprite:builtin:background", Color.grey);
        GC.BuildUIImage("Inventory Slot 4 Backdrop", new Vector2(380f, -58f), new Vector2(30, 30), "sprite:builtin:background", Color.grey);
        visualInventorySlotImages = new Image[STARTING_INVENTORY_SIZE];
        visualInventorySlotImages[0] = GC.BuildUIImage("Inventory Slot 1 Image", new Vector2(345f, -23f), new Vector2(30, 30), "", Color.white);
        visualInventorySlotImages[1] = GC.BuildUIImage("Inventory Slot 2 Image", new Vector2(380f, -23f), new Vector2(30, 30), "", Color.white);
        visualInventorySlotImages[2] = GC.BuildUIImage("Inventory Slot 3 Image", new Vector2(345f, -58f), new Vector2(30, 30), "", Color.white);
        visualInventorySlotImages[3] = GC.BuildUIImage("Inventory Slot 4 Image", new Vector2(385f, -58f), new Vector2(30, 30), "", Color.white);
        visualInventorySlotNameTexts = new Text[STARTING_INVENTORY_SIZE];
        visualInventorySlotNameTexts[0] = GC.BuildUIText("Inventory Slot 1 Name", new Vector2(345f, -13f), new Vector2(25, 25), "N/A", 5, TextAnchor.MiddleLeft, "font:arial", FontStyle.Bold, Color.black, 5);
        visualInventorySlotNameTexts[1] = GC.BuildUIText("Inventory Slot 2 Name", new Vector2(380f, -13f), new Vector2(25, 25), "N/A", 5, TextAnchor.MiddleLeft, "font:arial", FontStyle.Bold, Color.black, 5);
        visualInventorySlotNameTexts[2] = GC.BuildUIText("Inventory Slot 3 Name", new Vector2(345f, -48f), new Vector2(25, 25), "N/A", 5, TextAnchor.MiddleLeft, "font:arial", FontStyle.Bold, Color.black, 5);
        visualInventorySlotNameTexts[3] = GC.BuildUIText("Inventory Slot 4 Name", new Vector2(380f, -48f), new Vector2(25, 25), "N/A", 5, TextAnchor.MiddleLeft, "font:arial", FontStyle.Bold, Color.black, 5);
        visualInventorySlotQuantityTexts = new Text[STARTING_INVENTORY_SIZE];
        visualInventorySlotQuantityTexts[0] = GC.BuildUIText("Inventory Slot 1 Quantity", new Vector2(355f, -32f), new Vector2(15, 15), "0", 10, TextAnchor.MiddleCenter, "font:arial", FontStyle.Bold, Color.black, 5);
        visualInventorySlotQuantityTexts[1] = GC.BuildUIText("Inventory Slot 2 Quantity", new Vector2(390f, -32f), new Vector2(15, 15), "0", 10, TextAnchor.MiddleCenter, "font:arial", FontStyle.Bold, Color.black, 5);
        visualInventorySlotQuantityTexts[2] = GC.BuildUIText("Inventory Slot 3 Quantity", new Vector2(355f, -67f), new Vector2(15, 15), "0", 10, TextAnchor.MiddleCenter, "font:arial", FontStyle.Bold, Color.black, 5);
        visualInventorySlotQuantityTexts[3] = GC.BuildUIText("Inventory Slot 4 Quantity", new Vector2(390f, -67f), new Vector2(15, 15), "0", 10, TextAnchor.MiddleCenter, "font:arial", FontStyle.Bold, Color.black, 5);
        UpdateInventoryDisplay();

        darknessOverlay = GC.BuildUIImage("Darkness Overlay", Vector2.zero, new Vector2(900f, 500f), "sprite:builtin:background", new Color(0f, 0f, 0f, 1f));
    }
    
    public void SetDarkness(float newOpacity) { darknessOverlay.color = new Color(darknessOverlay.color.r, darknessOverlay.color.g, darknessOverlay.color.b, newOpacity); }
    public void SetDepth(int newDepth) { depthText.text = $"Depth: {newDepth}m"; }

    public void ObtainItems(Item item, int quantity) { inventory.AddItems(item, quantity); UpdateInventoryDisplay(); }
    private void UpdateInventoryDisplay()
    {
        for (int i = 0; i < inventory.GetSize(); i++)
        {
            Slot slot = inventory.GetSlot(i);
            if (slot.IsOccupied())
            {
                Item item = slot.Item;
                visualInventorySlotImages[i].sprite = item.Sprite;
                visualInventorySlotImages[i].gameObject.SetActive(true);
                visualInventorySlotNameTexts[i].text = item.Name;
                visualInventorySlotNameTexts[i].gameObject.SetActive(true);
                visualInventorySlotQuantityTexts[i].text = slot.Quantity.ToString();
                visualInventorySlotQuantityTexts[i].gameObject.SetActive(true);
            }
            else
            {
                visualInventorySlotImages[i].gameObject.SetActive(false);
                visualInventorySlotNameTexts[i].gameObject.SetActive(false);
                visualInventorySlotQuantityTexts[i].gameObject.SetActive(false);
            }
        }
    }
}
