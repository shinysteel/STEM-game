using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : CreatureBase
{
    private Sprite sprite;
    public Sprite Sprite { get { return sprite; } }
    private float swimForce;
    public float SwimForce { get { return swimForce; } }
    public Fish(Sprite _Sprite, float _SwimForce, string _ResearchItemID, int _InstanceID) : base (_ResearchItemID, _InstanceID)
    {
        sprite = _Sprite;
        swimForce = _SwimForce;
    }
    public override object DeepClone(float x, float y)
    {
        Fish fish = new Fish(sprite, swimForce, ResearchItemID, GC.GetNewInstanceID());
        GC.CreatePrefab<FishBehaviour>("prefab:fish", fish, x, y);
        return fish;
    }
}
