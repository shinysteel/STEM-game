using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shark : Fish
{
    public Shark(string _SpriteID, float _SwimForce, float _IdleTime, float _WanderTendency, float _SenseRange, float _StartleTime, string _Name, string _Description, string _ResearchItemID, Sprite _DisplaySprite, GameObject _GO, int _InstanceID, string _ReferenceID) : base(_SpriteID, _SwimForce, _IdleTime, _WanderTendency, _SenseRange, _StartleTime, _Name, _Description, _ResearchItemID, _DisplaySprite, _GO, _InstanceID, _ReferenceID)
    {
    }
    public override object DeepClone(float x, float y)
    {
        Shark shark = new Shark(SpriteID, SwimForce, IdleTime, WanderTendency, SenseRange, StartleTime, Name, Description, ResearchItemID, DisplaySprite, GC.CreatePrefab("prefab:fish", x, y), GC.GetNewInstanceID(), ReferenceID);
        GC.InitBehaviour<FishBehaviour>(shark.GO, shark);
        return shark;
    }

    public override void React(Action onFinished)
    {
        GC.PlaySound("sound:creature_scream1", 0.6f, 1f, pitchRandomness: 0f);
        Vector3 dirToPlayer = (GC.PlayerT.position - GO.transform.position).normalized;
        Vector3 targetPos = GO.transform.position + dirToPlayer;
        MoveTo(targetPos, 0.01f, () =>
        {
            Debug.Log("I have finished chasing the player.");
            onFinished();
        }, 2.5f, 10f);
    }
}
