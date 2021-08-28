using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementSwimming : MonoBehaviour, IPlayerMoveable
{
    public Player player { get; set; }
    private Rigidbody2D RB;
    public float swimForce = 0.125f;
    [SerializeField] private float velocityX = 0f;
    [SerializeField] private float velocityY = 0f;
    [SerializeField] private float currentFallSpeed = 0f;
    [SerializeField] private float targetFallSpeed = 0f;
    private const float VELOCITY_DECELERATION_RATE = 0.015f;
    private const float FALL_ACCELERATION_RATE = 0.005f;
    private const float MAX_SPEED_FORCE = 3.5f;

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        RB.gravityScale = 0f;
        velocityY = -swimForce * 0.5f;
    }

    private void FixedUpdate()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        player.visualT.rotation = Quaternion.LookRotation(Vector3.forward, mousePos - transform.position);
        RB.AddForce(new Vector3(velocityX, velocityY) * 1.5f, ForceMode2D.Impulse);
        RB.velocity = new Vector3(GC.ApproachValue(RB.velocity.x, 0f, VELOCITY_DECELERATION_RATE), GC.ApproachValue(RB.velocity.y, 0f, VELOCITY_DECELERATION_RATE));
        RB.velocity = new Vector3(Mathf.Clamp(RB.velocity.x, -MAX_SPEED_FORCE, MAX_SPEED_FORCE), Mathf.Clamp(RB.velocity.y, -MAX_SPEED_FORCE, MAX_SPEED_FORCE));
    }
    public void Move()
    {
        if (Input.GetMouseButton(0))
        {
            GC.GetVelocityToMouse(transform.position, swimForce, out velocityX, out velocityY);
            currentFallSpeed = 0f;
        }
        else
        {
            velocityX = GC.ApproachValue(velocityX, 0f, VELOCITY_DECELERATION_RATE);
            velocityY = GC.ApproachValue(velocityY, 0f, VELOCITY_DECELERATION_RATE);
        }
    }

    public IPlayerMoveable UpdateMovementType()
    {
        if (player.transform.position.y < Player.TOP_OF_MAP + 0.5f) return this; 
        Destroy(GetComponent<PlayerMovementSwimming>());
        PlayerMovementWalking playerMovementWalking = gameObject.AddComponent<PlayerMovementWalking>();
        playerMovementWalking.player = player;
        return playerMovementWalking;
    }
}
