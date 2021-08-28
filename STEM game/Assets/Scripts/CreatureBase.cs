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
        UtilsClass.CreateWorldText("isResearched", GO.transform, new Vector3(0f, 0.75f), 5, Color.green, TextAnchor.MiddleCenter, TextAlignment.Center, 100, 5);
        return true;
    }

    private string name; public string Name { get { return name; } }
    private string description; public string Description { get { return description; } }
    private string researchItemID; public string ResearchItemID { get { return researchItemID; } }
    public CreatureBase(string _Name, string _Description, string _ResearchItemID, GameObject _GO, int _InstanceID, string _ReferenceID) : base(_GO, _InstanceID, _ReferenceID)
    {
        name = _Name;
        if (_Description == null) description = $"It's a {_Name}!";
        else description = _Description;
        researchItemID = _ResearchItemID;
    }
}
