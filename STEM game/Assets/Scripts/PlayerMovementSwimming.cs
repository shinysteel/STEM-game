using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class PlayerMovementSwimming : MonoBehaviour, IPlayerMoveable
{
    public Player player { get; set; }
    public Rigidbody2D RB { get; set; }
    public float swimForce = 3.75f;
    [SerializeField] private float velocityX = 0f;
    [SerializeField] private float velocityY = 0f;
    private const float VELOCITY_DECELERATION_RATE = 0.08f;
    private const float MAX_SPEED_FORCE = 10f;

    private void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        RB.gravityScale = 0f;
        velocityY = -swimForce * 0.5f + player.BCInflation;
        player.playerUI.CreateFillBar(() => { return player.oxygen / Player.LUNG_CAPACITY; }, () => { return this == null; }, "Oxygen", new Color32(77, 168, 188, 255));
        player.playerUI.CreateFillBar(() => { return (Player.BC_INFLATION_BOUND + player.BCInflation) / (Player.BC_INFLATION_BOUND * 2f); }, () => { return this == null; }, "Buoyancy", Color.magenta);
    }
    private void Update()
    {
        if (!player.DoUpdate()) return;
        player.oxygen -= Time.deltaTime;
    }
    private void FixedUpdate()
    {
        if (!player.DoUpdate()) return;
        velocityX = GC.ApproachValue(velocityX, 0f, VELOCITY_DECELERATION_RATE);
        velocityY = GC.ApproachValue(velocityY, GetDepthForce() + player.BCInflation, VELOCITY_DECELERATION_RATE);
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        player.visualT.rotation = Quaternion.LookRotation(Vector3.forward, mousePos - transform.position);
        RB.AddForce(new Vector3(velocityX, velocityY) * 1.5f, ForceMode2D.Impulse);
        RB.velocity = new Vector3(GC.ApproachValue(RB.velocity.x, 0f, VELOCITY_DECELERATION_RATE), GC.ApproachValue(RB.velocity.y, 0f, VELOCITY_DECELERATION_RATE));
        RB.velocity = new Vector3(Mathf.Clamp(RB.velocity.x, -MAX_SPEED_FORCE, MAX_SPEED_FORCE), Mathf.Clamp(RB.velocity.y, -MAX_SPEED_FORCE, MAX_SPEED_FORCE));
    }
    public float GetDepthForce()
    {
        if (player.depth < 30f)
            return -0.00001f * Mathf.Pow(player.depth + 60f, 3f) + 4f;
        else
            return -0.002f * Mathf.Pow(player.depth - 20f, 2f) - 1.12f;
    }
    public void Move()
    {
        if (player.playerScanner.IsScanning()) return;
        if (Input.GetMouseButton(0))
        {
            player.playerAnimator.PlayAnimation("player_swim");
        }
        else
        {
            player.playerAnimator.PlayAnimation("player_idle");
        }
    }
    public void MoveEvent()
    {
        GC.GetVelocityToMouse(transform.position, swimForce, out velocityX, out velocityY);
    }
    public IPlayerMoveable UpdateMovementType()
    {
        if (player.transform.position.y < Player.TOP_OF_MAP + 0.5f) return this;
        player.playerAnimator.PlayAnimation("player_idle");
        GC.PlaySound("sound:splash1", 0.7f, 1f, startTime: 0.35f);
        Destroy(GetComponent<PlayerMovementSwimming>());
        PlayerMovementWalking playerMovementWalking = gameObject.AddComponent<PlayerMovementWalking>();
        playerMovementWalking.player = player;
        return playerMovementWalking;
    }
}
