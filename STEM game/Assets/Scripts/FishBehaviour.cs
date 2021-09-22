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

    [SerializeField] private Vector3 moveDir;
    [SerializeField] private float velocityX;
    [SerializeField] private float velocityY;
    [SerializeField] private float moveTimer;
    [SerializeField] private float idleTimer;
    private const float VELOCITY_DECELERATION_RATE = 0.005f;

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private enum State { Idle, Roaming, Startled, Fleeing }
    private State state;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        state = State.Idle;
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
                    fish.Wait(fish.StartleTime, () => state = State.Fleeing);
                }
                break;
            case State.Fleeing:
                if (fish.isIdle)
                {
                    GC.PlaySound("sound:fish_dart1", 0.7f, 1f);
                    Vector3 dirAwayFromPlayer = (transform.position - GC.PlayerT.position).normalized;
                    Vector3 targetPos = transform.position + dirAwayFromPlayer;
                    fish.MoveTo(targetPos, 0.05f, () => state = State.Idle, 0.1f, 2.5f);
                }
                break;
            default:
                break;
        }
    }
    /*
    private void Update()
    {
        if (!DoUpdate()) return;
        switch (state)
        {
            case State.Idle:
                if (idleTimer > 0f) {
                    velocityX = GC.ApproachValue(velocityX, 0f, VELOCITY_DECELERATION_RATE);
                    velocityY = GC.ApproachValue(velocityY, 0f, VELOCITY_DECELERATION_RATE);
                    idleTimer -= Time.deltaTime;
                } else {
                    state = State.Roaming;
                    UpdateState();
                }
                break;
            case State.Roaming:
                if (moveTimer > 0f) {
                    moveTimer -= Time.deltaTime;
                } else {
                    state = State.Idle;
                    UpdateState();
                }
                break;
            default:
                break;
        }
        rb.MovePosition(new Vector3(transform.position.x + velocityX, transform.position.y + velocityY));
    }

    private void UpdateState()
    {
        switch (state)
        {
            case State.Idle:
                idleTimer = Random.Range(1f, 2f);
                break;
            case State.Roaming:
                moveDir = Random.insideUnitCircle.normalized;
                float radians = Mathf.Atan2(moveDir.y, moveDir.x);
                velocityX = Mathf.Cos(radians) * fish.SwimForce;
                velocityY = Mathf.Sin(radians) * fish.SwimForce;
                moveTimer = Random.Range(1f, 2f);
                sr.flipX = velocityX >= 0f;
                //transform.rotation = Quaternion.LookRotation(Vector3.forward, moveDir);
                break;
            default:
                break;
            
        }
    }
    */
}
