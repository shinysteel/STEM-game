using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : CreatureBase
{
    private string spriteID; public string SpriteID { get { return spriteID; } }
    private float swimForce;
    public float SwimForce { get { return swimForce; } }
    public Fish(string _SpriteID, float _SwimForce, string _Name, string _Description, string _ResearchItemID, GameObject _GO, int _InstanceID, string _ReferenceID) : base (_Name, _Description, _ResearchItemID, _GO, _InstanceID, _ReferenceID)
    {
        spriteID = _SpriteID;
        swimForce = _SwimForce;
    }
    public override object DeepClone(float x, float y)
    {
        Fish fish = new Fish(spriteID, swimForce, Name, Description, ResearchItemID, GC.CreatePrefab("prefab:fish", x, y), GC.GetNewInstanceID(), ReferenceID);
        GC.InitBehaviour<FishBehaviour>(fish.GO, fish);
        return fish;
    }
}
