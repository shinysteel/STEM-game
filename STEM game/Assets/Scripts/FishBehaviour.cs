using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBehaviour : CreatureBehaviour
{
    public Fish fish;
    public override void SetClass(object classReference) { fish = (Fish)classReference; }
    public override int GetInstanceID() { return fish.InstanceID; }

    [SerializeField] private Vector3 moveDir;
    [SerializeField] private float velocityX;
    [SerializeField] private float velocityY;
    [SerializeField] private float moveTimer;
    [SerializeField] private float idleTimer;
    private const float VELOCITY_DECELERATION_RATE = 0.005f;

    private Rigidbody2D rb;
    private enum State { Idle, Roaming }
    private State state;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        state = State.Roaming;
    }
    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = fish.Sprite;
        rb.freezeRotation = true;
        gameObject.AddComponent<BoxCollider2D>();
        UpdateState();
    }
    public override string GetResearchItemID() { return fish.ResearchItemID; }

    private void Update()
    {
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
                transform.rotation = Quaternion.LookRotation(Vector3.forward, moveDir);
                break;
            default:
                break;
            
        }
    }
}
