using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using Random = UnityEngine.Random;

public class FishBehaviour : CreatureBehaviour
{
    public Fish fish;
    public override void SetClass(object classReference)
    {
        base.SetClass(classReference);
        fish = (Fish)classReference;
    }

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private enum State { Idle, Roaming, Startled, Reacting }
    //private enum State { Flocking, Fleeing }
    private State state;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        state = State.Idle;
        //state = State.Flocking;
    }
    protected override void Start()
    {
        base.Start();   
        GetComponent<SpriteRenderer>().sprite = GC.GetReference<Sprite>(fish.SpriteID);
        rb.freezeRotation = true;
        gameObject.AddComponent<CapsuleCollider2D>();
        fish.Start();
    }
    private void FixedUpdate()
    {
        fish.FixedUpdate();
    }
    private void Update()
    {
        fish.Update();
        if ((int)state < 2 && Vector3.Distance(transform.position, GC.PlayerT.position) <= fish.SenseRange)
        {
            fish.Interrupt();
            state = State.Startled;
        }
        switch (state)
        {
            case State.Idle:
                if (fish.isIdle)
                {
                    fish.Wait(Random.Range(fish.IdleTime, fish.IdleTime + 1f), () => { state = State.Roaming; });
                }
                break;
            case State.Roaming:
                if (fish.isIdle)
                {
                    Vector3 targetPos = transform.position + new Vector3(Random.Range(-fish.WanderTendency, fish.WanderTendency), Random.Range(-fish.WanderTendency, fish.WanderTendency));
                    targetPos = new Vector3(targetPos.x, Mathf.Clamp(targetPos.y, targetPos.y, Player.TOP_OF_MAP));
                    fish.MoveTo(targetPos, 0.5f, () => state = State.Idle, 6.5f);
                }
                break;
            case State.Startled:
                if (fish.isIdle)
                {
                    GC.CreatePopupSprite("sprite:alert_icon", transform, Vector3.zero);
                    fish.Wait(fish.StartleTime, () => state = State.Reacting);
                }
                break;
            case State.Reacting:
                if (fish.isIdle)
                {
                    fish.React(() => state = State.Idle);
                }
                break;
            default:
                break;
        }

        /*
        Vector3 forwardVector = new Vector3(1f, 0f);
        if (!sr.flipX) forwardVector *= -1f;
        Debug.DrawLine(transform.position, transform.position + forwardVector, Color.white);
        for (int i = 0; i < GC.InstanceParent.transform.childCount; i++)
        {
            FishBehaviour fb = GC.InstanceParent.transform.GetChild(i).GetComponent<FishBehaviour>();
            if (fb == null) continue;
            Vector3 v = fb.transform.position - transform.position;
            if (v.magnitude <= fish.SenseRange)
                Debug.DrawLine(transform.position, transform.position + v, Color.red);
        }
        */
    }
}
