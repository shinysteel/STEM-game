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
    public Fish(string _SpriteID, float _SwimForce, float _IdleTime, float _WanderTendency, float _SenseRange, float _StartleTime, string _Name, string _Description, string _ResearchItemID, Sprite _DisplaySprite, GameObject _GO, int _InstanceID, string _ReferenceID) : base (_Name, _Description, _ResearchItemID, _DisplaySprite, _GO, _InstanceID, _ReferenceID)
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
        Fish fish = new Fish(spriteID, swimForce, idleTime, wanderTendency, senseRange, startleTime, Name, Description, ResearchItemID, DisplaySprite, GC.CreatePrefab("prefab:fish", x, y), GC.GetNewInstanceID(), ReferenceID);
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
        sr.flipX = velocityX >= 0f ? true : false;
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
        pos = new Vector3(pos.x, Mathf.Clamp(pos.y, -Mathf.Infinity, Player.TOP_OF_MAP - 3f));
        targetPos = pos;
        float timer = 0f;
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
    public virtual void React(Action onFinished)
    {
        GC.PlaySound("sound:fish_dart1", 0.7f, 1f);
        Vector3 dirAwayFromPlayer = (GO.transform.position - GC.PlayerT.position).normalized;
        Vector3 targetPos = GO.transform.position + dirAwayFromPlayer;
        MoveTo(targetPos, 0.05f, onFinished, 0.1f, 2.5f);
    }

    /*
    public void Flock()
    {
        velocityX = swimForce * 0.5f;
        for (int i = 0; i < GC.InstanceParent.transform.childCount; i++)
        {
            FishBehaviour fb = GC.InstanceParent.transform.GetChild(i).GetComponent<FishBehaviour>();
            if (fb == null) continue;
            Vector3 v2f = fb.transform.position - GO.transform.position;
            Vector3 fv = velocityX >= 0f ? new Vector3(1f, 0f) : new Vector3(-1f, 0f);
            float angle = Vector2.Angle(v2f, fv);
            if (v2f.magnitude <= senseRange && angle <= 135f)
            {
                Debug.Log(v2f.magnitude);
                Debug.DrawLine(GO.transform.position, GO.transform.position + v2f, Color.red);
                // avoid nearby boids
                float dist = v2f.magnitude;
                float steerFactor = -4f * Mathf.Pow(Mathf.Clamp(dist, 0f, 0.5f), 2f) + 1;
                GO.transform.eulerAngles = new Vector3(GO.transform.eulerAngles.x, GO.transform.eulerAngles.y, GO.transform.eulerAngles.z + 1f * steerFactor);
            }
        }
    }
    */
}
