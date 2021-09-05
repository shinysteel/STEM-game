using UnityEngine;

public interface IPlayerMoveable
{
    Player player { get; set; }
    Rigidbody2D RB { get; set; }
    void Move();
    void MoveEvent();
    IPlayerMoveable UpdateMovementType();
}