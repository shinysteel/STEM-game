using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CreatureBase : InstanceBase
{
    private string researchItemID; public string ResearchItemID { get { return researchItemID; } }
    public CreatureBase(string _ResearchItemID, int _InstanceID) : base(_InstanceID) { researchItemID = _ResearchItemID; }
    public string GetResearchItemID() { return researchItemID; }
}
