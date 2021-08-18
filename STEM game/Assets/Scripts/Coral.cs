using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coral : CreatureBase
{
    private Sprite[] idleSprites; public Sprite[] IdleSprites { get { return idleSprites; } }
    public Coral(Sprite[] _IdleSprites, string _ResearchItemID, int _InstanceID) : base(_ResearchItemID, _InstanceID)
    {
        idleSprites = _IdleSprites;
    }
    public override object DeepClone(float x, float y)
    {
        Coral coral = new Coral(idleSprites, ResearchItemID, GC.GetNewInstanceID());
        GC.CreatePrefab<CoralBehaviour>("prefab:coral", coral, x, y);
        return coral;
    }
}
