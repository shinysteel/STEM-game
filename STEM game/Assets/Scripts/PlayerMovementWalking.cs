using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementWalking : MonoBehaviour, IPlayerMoveable
{
    public Player player { get; set; }
    //private float leapForce = 6.5f;
    private float leapForce = 1.6f;
    private float moveSpeed = 6f;
    public Rigidbody2D RB { get; set; }
    private bool moving = false;

    private void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        RB.gravityScale = 1f;
        //GC.GetVelocityToMouse(transform.position, leapForce, out float velocityX, out float velocityY);
        //RB.velocity = new Vector3(velocityX, velocityY * 1.2f
        //float velocityBound = 6.5f;
        //RB.velocity = new Vector3(Mathf.Clamp(RB.velocity.x * leapForce * 1.5f, -velocityBound, velocityBound), Mathf.Clamp(RB.velocity.y * leapForce, 4f, velocityBound));
        float velocityBound = 6.5f;
        RB.velocity = new Vector3(Mathf.Clamp(RB.velocity.x * leapForce, -velocityBound, velocityBound), Mathf.Clamp(RB.velocity.y * leapForce, 0f, velocityBound));
        player.oxygen = Player.LUNG_CAPACITY;
    }
    private void FixedUpdate()
    {
        if (!player.DoUpdate()) return;
        int horizontal = 0;
        if (Input.GetKey("a")) horizontal--;
        if (Input.GetKey("d")) horizontal++;
        if (!moving)
        {
            player.visualT.rotation = Quaternion.Lerp(player.visualT.rotation, Quaternion.LookRotation(Vector3.forward, GC.GetWorldMousePos() - transform.position), 360f * Time.deltaTime);
        }
        else if (RB.velocity.y == 0f)
        {
            player.visualT.rotation = Quaternion.LookRotation(player.visualT.transform.forward, Vector3.up);
            RB.MovePosition(new Vector3(transform.position.x + horizontal * moveSpeed * Time.deltaTime, transform.position.y));
        }
    }
    public void Move()
    {
        Debug.DrawRay(transform.position + new Vector3(0.46f, -1f), Vector3.down * 0.1f, Color.red);
        Debug.DrawRay(transform.position + new Vector3(-0.46f, -1f), Vector3.down * 0.1f, Color.red);
        int groundsHit = 0;
        RaycastHit2D[] hit = new RaycastHit2D[100];
        ContactFilter2D filter = new ContactFilter2D();
        if (Physics2D.Raycast(transform.position + new Vector3(0.46f, -1f), Vector3.down, filter, hit, 0.1f) > 0)
        {
            if (hit[0].transform.tag == "Ground") groundsHit++;
        }
        if (Physics2D.Raycast(transform.position + new Vector3(-0.46f, -1f), Vector3.down, filter, hit, 0.1f) > 0)
        {
            if (hit[0].transform.tag == "Ground") groundsHit++;
        }
        moving = groundsHit > 0;
    }
    public void MoveEvent()
    {
        return;
    }
    public IPlayerMoveable UpdateMovementType()
    {
        if (player.transform.position.y >= Player.TOP_OF_MAP) return this;
        GC.PlaySound("sound:jump_splash1", 0.8f, 1f, 1);
        Destroy(GetComponent<PlayerMovementWalking>());
        PlayerMovementSwimming playerMovementSwimming = gameObject.AddComponent<PlayerMovementSwimming>();
        playerMovementSwimming.player = player;
        return playerMovementSwimming;
    }
}
