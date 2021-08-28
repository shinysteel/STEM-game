public interface IPlayerMoveable
{
    Player player { get; set; }
    void Move();
    IPlayerMoveable UpdateMovementType();
}
