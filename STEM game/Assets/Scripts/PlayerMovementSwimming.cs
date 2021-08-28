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
    private const float VELOCITY_DECELERATION_RATE = 0.005f;
    private const float FALL_ACCELERATION_RATE = 0.005f;

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

        RB.MovePosition(new Vector2(transform.position.x + velocityX, transform.position.y + velocityY - currentFallSpeed));
        velocityX = GC.ApproachValue(velocityX, 0f, VELOCITY_DECELERATION_RATE);
        velocityY = GC.ApproachValue(velocityY, 0f, VELOCITY_DECELERATION_RATE);
        currentFallSpeed = GC.ApproachValue(currentFallSpeed, targetFallSpeed, FALL_ACCELERATION_RATE);
    }
    public void Move()
    {
        if (Input.GetMouseButton(0))
        {
            GC.GetVelocityToMouse(transform.position, swimForce, out velocityX, out velocityY);
            currentFallSpeed = 0f;
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
