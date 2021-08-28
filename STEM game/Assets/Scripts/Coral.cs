using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coral : CreatureBase
{
    private string[] idleSpritesIDs; public string[] IdleSpritesIDs { get { return idleSpritesIDs; } }
    public Coral(string[] _IdleSpritesIDs, string _Name, string _Description, string _ResearchItemID, GameObject _GO, int _InstanceID, string _ReferenceID) : base(_Name, _Description, _ResearchItemID, _GO, _InstanceID, _ReferenceID)
    {
        idleSpritesIDs = _IdleSpritesIDs;
    }
    public override object DeepClone(float x, float y)
    {
        Coral coral = new Coral(IdleSpritesIDs, Name, Description, ResearchItemID, GC.CreatePrefab("prefab:coral", x, y), GC.GetNewInstanceID(), ReferenceID);
        GC.InitBehaviour<CoralBehaviour>(coral.GO, coral);
        return coral;
    }
}
