using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Player player;
    private Rigidbody2D RB;
    public float swimForce = 0.125f;
    [SerializeField] private float velocityX = 0f;
    [SerializeField] private float velocityY = 0f;
    [SerializeField] private float currentFallSpeed = 0f;
    [SerializeField] private float targetFallSpeed = 0f;
    private const float VELOCITY_DECELERATION_RATE = 0.005f;
    private const float FALL_ACCELERATION_RATE = 0.005f;

    private void Awake() { RB = GetComponent<Rigidbody2D>(); }

    private void Update()
    {
        RB.MovePosition(new Vector2(transform.position.x + velocityX, transform.position.y + velocityY - currentFallSpeed));
        velocityX = GC.ApproachValue(velocityX, 0f, VELOCITY_DECELERATION_RATE);
        velocityY = GC.ApproachValue(velocityY, 0f, VELOCITY_DECELERATION_RATE);
        currentFallSpeed = GC.ApproachValue(currentFallSpeed, targetFallSpeed, FALL_ACCELERATION_RATE);
    }
    public void Move()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 dir = (mousePos - transform.position).normalized;
        float radians = Mathf.Atan2(dir.y, dir.x);
        velocityX = Mathf.Cos(radians) * swimForce;
        velocityY = Mathf.Sin(radians) * swimForce;
        currentFallSpeed = 0f;
    }
}
