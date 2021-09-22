using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ShinyOwl.Utils;
using System;
using Random = UnityEngine.Random;


public class Fish : CreatureBase
{
    private string spriteID; public string SpriteID { get { return spriteID; } }
    private float swimForce; public float SwimForce { get { return swimForce; } }
    private float idleTime; public float IdleTime { get { return idleTime; } }
    private float wanderTendency; public float WanderTendency { get { return wanderTendency; } }
    private float senseRange; public float SenseRange { get { return senseRange; } }
    private float startleTime; public float StartleTime { get { return startleTime; } }
    public Fish(string _SpriteID, float _SwimForce, float _IdleTime, float _WanderTendency, float _SenseRange, float _StartleTime, string _Name, string _Description, string _ResearchItemID, GameObject _GO, int _InstanceID, string _ReferenceID) : base (_Name, _Description, _ResearchItemID, _GO, _InstanceID, _ReferenceID)
    {
        spriteID = _SpriteID;
        swimForce = _SwimForce;
        idleTime = _IdleTime;
        wanderTendency = _WanderTendency;
        senseRange = _SenseRange;
        startleTime = _StartleTime;
    }
    public override object DeepClone(float x, float y)
    {
        Fish fish = new Fish(spriteID, swimForce, idleTime, wanderTendency, senseRange, startleTime, Name, Description, ResearchItemID, GC.CreatePrefab("prefab:fish", x, y), GC.GetNewInstanceID(), ReferenceID);
        GC.InitBehaviour<FishBehaviour>(fish.GO, fish);
        return fish;
    }

    protected float velocityX;
    protected float velocityY;
    public bool isIdle = true;
    protected Vector3 targetPos;
    protected Rigidbody2D rb;
    protected SpriteRenderer sr;
    protected string currentFTimerName;
    protected string currentFUpdaterName;

    public void Start()
    {
        targetPos = GO.transform.position;
        rb = GO.GetComponent<Rigidbody2D>();
        sr = GO.GetComponent<SpriteRenderer>();
    }
    public void FixedUpdate()
    {
        if (GO.transform.position.y <= Player.TOP_OF_MAP)
            rb.MovePosition(new Vector3(GO.transform.position.x + velocityX, GO.transform.position.y + velocityY));
        velocityX = GC.ApproachValue(velocityX, 0f, 0.005f);
        velocityY = GC.ApproachValue(velocityY, 0f, 0.005f);
    }
    public void Update()
    {
        rb.gravityScale = GO.transform.position.y > Player.TOP_OF_MAP ? 1f : 0f;
    }

    public void Interrupt()
    {
        isIdle = true;
        if (currentFTimerName != null)
            FunctionTimer.DestroyTimerWithName(currentFTimerName);
        if (currentFUpdaterName != null)
            FunctionTimer.DestroyTimerWithName(currentFUpdaterName);
        currentFTimerName = null;
        currentFUpdaterName = null;
    }
    public void Wait(float waitTime, Action onFinishedWaiting)
    {
        isIdle = false;
        currentFTimerName = $"{InstanceID} Timer";
        FunctionTimer.Create(() => 
        {
            onFinishedWaiting();
            isIdle = true;
            currentFTimerName = null;
        }, waitTime, currentFTimerName);
    }
    public void MoveTo(Vector3 pos, float stopDist, Action onArrivedAtPos, float maxTime = 999f, float speedMod = 1f)
    {
        isIdle = false;
        currentFUpdaterName = $"{InstanceID} Updater";
        targetPos = pos;
        float timer = 0f;
        sr.flipX = targetPos.x > GO.transform.position.x;
        FunctionUpdater.Create(() =>
        {
            timer += Time.deltaTime;
            Vector3 dist = targetPos - GO.transform.position;
            Vector3 dir = dist.normalized;
            if (timer < maxTime && dist.magnitude > stopDist)
            {
                GC.GetVelocityToPos(GO.transform.position, targetPos, swimForce * speedMod, out velocityX, out velocityY);
                return false;
            }
            else
            {
                onArrivedAtPos();
                isIdle = true;
                currentFUpdaterName = null;
                return true;
            }
        }, currentFUpdaterName);
    }
}
