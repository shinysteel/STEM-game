using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class PlayerMovementSwimming : MonoBehaviour, IPlayerMoveable
{
    public Player player { get; set; }
    public Rigidbody2D RB { get; set; }
    public float swimForce = 2.5f;
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
        player.playerUI.CreateFillBar(() => { return player.BCInflation / Player.BC_INFLATION_MAX; }, () => { return this == null; }, "Buoyancy", Color.magenta);
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
        velocityY = GC.ApproachValue(velocityY, GetDownwardsForce(), VELOCITY_DECELERATION_RATE);
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        player.visualT.rotation = Quaternion.LookRotation(Vector3.forward, mousePos - transform.position);
        RB.AddForce(new Vector3(velocityX, velocityY) * 1.5f, ForceMode2D.Impulse);
        RB.velocity = new Vector3(GC.ApproachValue(RB.velocity.x, 0f, VELOCITY_DECELERATION_RATE), GC.ApproachValue(RB.velocity.y, 0f, VELOCITY_DECELERATION_RATE));
        RB.velocity = new Vector3(Mathf.Clamp(RB.velocity.x, -MAX_SPEED_FORCE, MAX_SPEED_FORCE), Mathf.Clamp(RB.velocity.y, -MAX_SPEED_FORCE, MAX_SPEED_FORCE));
    }
    public float GetDownwardsForce()
    {
        float depth = player.depth;
        float buoyancy = player.BCInflation;
        float depthForce;
        if (depth <= 10f) depthForce = 10f;
        else depthForce = -GC.RoundUpNearestTen(depth) + 20f;
        depthForce *= 0.05f;
        // every 10meters increments depthForce by 0.5, starting at -0.5 at 0-10meters
        // every 2.5kg after 60kg applies -0.1 depthForce
        float excessWeight = Mathf.Clamp(player.weight - 60f, 0f, Mathf.Infinity);
        int timesToAdd = (int)Mathf.Floor(excessWeight / 2.5f);
        depthForce -= 0.1f * timesToAdd;
        float dif = depthForce + buoyancy;
        
        //Debug.Log($"depthForce: {depthForce} and buoyancy: {buoyancy} and difference: {dif}");
        if (Mathf.Abs(dif) <= 0.25f) return 0f;
        else return dif;
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
