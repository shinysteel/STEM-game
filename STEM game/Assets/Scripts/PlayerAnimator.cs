using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public Player player;
    private Animator ac;
    private void Start()
    {
        ac = transform.GetChild(0).GetComponent<Animator>();
    }
    private void Update()
    {
        if (!player.DoUpdate())
        {
            ac.speed = 0f;
        }
        else
        {
            ac.speed = 2f;
        }
    }
    public void PlayAnimation(string id)
    {
        ac.Play(id);
    }
}
