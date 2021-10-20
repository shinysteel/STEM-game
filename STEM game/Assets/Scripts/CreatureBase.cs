using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ShinyOwl.Utils;

public abstract class CreatureBase : InstanceBase
{
    private bool isResearched = false; public bool IsResearched { get { return isResearched; } }
    public bool TryDeclareResearched()
    {
        if (isResearched) return false;
        isResearched = true;
        GameObject icon = new GameObject("Icon", typeof(SpriteRenderer));
        SpriteRenderer sr = icon.GetComponent<SpriteRenderer>();
        sr.sprite = GC.GetReference<Sprite>("sprite:scanned_icon");
        sr.sortingOrder = 25;
        
        icon.transform.SetParent(GO.transform);
        icon.transform.localPosition = new Vector3(0f, 0f);
        return true;
    }

    private string name; public string Name { get { return name; } }
    private string description; public string Description { get { return description; } }
    private string researchItemID; public string ResearchItemID { get { return researchItemID; } }
    private Sprite displaySpirte; public Sprite DisplaySprite { get { return displaySpirte; } }
    public CreatureBase(string _Name, string _Description, string _ResearchItemID, Sprite _DisplaySprite, GameObject _GO, int _InstanceID, string _ReferenceID) : base(_GO, _InstanceID, _ReferenceID)
    {
        name = _Name;
        if (_Description == null) description = $"It's a {_Name}!";
        else description = _Description;
        researchItemID = _ResearchItemID;
        displaySpirte = _DisplaySprite;
    }
}
