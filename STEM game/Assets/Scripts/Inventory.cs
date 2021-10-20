using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private InventorySlot[] slots;
    public Inventory (int _Size)
    {
        slots = new InventorySlot[_Size];
        for (int i = 0; i < _Size; i++)
        {
            slots[i] = new InventorySlot();
        }
    }

    public int GetSize() { return slots.Length; }
    public InventorySlot GetSlot(int slotID) { return slots[slotID]; }
    public bool IsFull()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].IsFull())
            {
                return false;
            }
        }
        return true;
    }
    public bool HasSpaceforItem(InventoryItem item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            InventorySlot slot = slots[i];
            if (!slot.IsOccupied() || (slot.GetContainedItemID() == item.ID && slot.CanAddQuantity(out _)))
            {
                return true;
            }
        }
        return false;
    }
    public void AddItems(InventoryItem item, int quantity)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            string containedItemID = slots[i].GetContainedItemID();
            if (containedItemID == item.ID)
            {
                if (slots[i].CanAddQuantity(out int quantityOfSpace))
                {
                    int quantityToAdd = Mathf.Clamp(quantity, 0, quantityOfSpace);
                    slots[i].AddItems(item, quantityToAdd);
                    quantity -= quantityOfSpace;
                    if (quantity > 0) { AddItems(item, quantity); }
                    break;
                }
            }
            else if (containedItemID == null)
            {
                int quantityOfSpace = item.MaxStack;
                int quantityToAdd = Mathf.Clamp(quantity, 0, quantityOfSpace);
                slots[i].AddItems(item, quantityToAdd);
                quantity -= quantityOfSpace;
                if (quantity > 0) { AddItems(item, quantity); }
                break;
            }
        }
    }
    public void RemoveItems(int slotID, int quantity) { slots[slotID].RemoveItems(quantity); }
}

public class InventorySlot
{
    private InventoryItem item; public InventoryItem Item { get { return item; } }
    private int quantity; public int Quantity { get { return quantity; } }
    public bool IsOccupied() { return item != null; }
    public bool IsFull(bool validate = true)
    {
        if (validate) if (!IsOccupied()) return false;
        return quantity >= item.MaxStack;
    }
    public string GetContainedItemID() { if (IsOccupied()) { return item.ID; } else { return null; } }
    public string GetContainedItemName() { if (IsOccupied()) { return item.Name; } else { return null; } }
    public int GetContainedMaxStack() { if (IsOccupied()) { return item.MaxStack; } else { return -1; } }
    public string GetContainedSpriteID() { if (IsOccupied()) { return item.SpriteID; } else { return null; } }
    public bool CanAddQuantity(out int quantityOfSpace)
    {
        quantityOfSpace = GetContainedMaxStack() - this.quantity;
        return this.quantity < GetContainedMaxStack();
    }

    public InventorySlot() { item = null; quantity = 0; }

    public void AddItems(InventoryItem item, int quantity)
    {
        if (this.item == null) { this.item = item; this.quantity = Mathf.Clamp(quantity, 0, item.MaxStack); }
        else { this.quantity = Mathf.Clamp(this.quantity + quantity, 0, GetContainedMaxStack()); }
    }
    public void RemoveItems(int quantity)
    {
        this.quantity = Mathf.Clamp(this.quantity - quantity, 0, this.quantity);
        if (this.quantity <= 0) { item = null; }
    }
}

public class InventoryItem
{
    private string id; public string ID { get { return id; } }
    private string name; public string Name { get { return name; } }
    private float currencyValue; public float CurrencyValue { get { return currencyValue; } }
    private int maxStack; public int MaxStack { get { return maxStack; } }
    private float weight; public float Weight { get { return weight; } }
    private string spriteID; public string SpriteID { get { return spriteID; } }

    public InventoryItem(string _ID, string _Name, float _CurrencyValue, int _MaxStack, float _Weight, string _SpriteID)
    {
        id = _ID;
        name = _Name;
        currencyValue = _CurrencyValue;
        maxStack = _MaxStack;
        weight = _Weight;
        spriteID = _SpriteID;
    }
}

/* what does an inventory need?

- slots, GetItemInSlot(0), GetItemInSlot(1), etc
- items that can be exchanged for money. an item has a custom name, currency value 
and possible key to track completion of researching all species.
- an inventory has a set capacity of slots.
- slots are needed to count quanties of alike items. 


*/
