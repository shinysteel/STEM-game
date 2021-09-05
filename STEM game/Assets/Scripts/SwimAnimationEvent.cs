using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwimAnimationEvent : MonoBehaviour
{
    public void ForwardSwimEvent()
    {
        transform.parent.GetComponent<Player>().playerMovement.MoveEvent();
        GC.PlaySound("sound:swimming_loop1", 0.8f, 1f, startTime: Random.Range(0, 2) == 0 ? 1.6f : 4.5f, cutoff: 1.2f);
    }

    public void PrepareSwimEvent()
    {
        GC.PlaySound("sound:swimming_loop1", 0.6f, 1f, startTime: Random.Range(0, 2) == 0 ? 2.3f : 6.5f, cutoff: 0.5f);
    }
}
