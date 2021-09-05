using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoralBehaviour : CreatureBehaviour
{
    public Coral coral;
    public override void SetClass(object classReference)
    {
        base.SetClass(classReference);
        coral = (Coral)classReference;
    }

    public override Sprite GetSpriteIcon()
    {
        return GC.GetReference<Sprite>(coral.IdleSpritesIDs[0]);
    }

    private float flickerTimer = 0f;
    private float flickTime = 1f;
    private int nextSpriteID;

    private SpriteRenderer sr;
    private void Awake() { sr = GetComponent<SpriteRenderer>(); }

    protected override void Start()
    {
        base.Start();
        gameObject.AddComponent<CapsuleCollider2D>();
        sr.sprite = GC.GetReference<Sprite>(coral.IdleSpritesIDs[0]);
        nextSpriteID = 1;
    }
    private void Update()
    {
        if (!DoUpdate()) return;
        flickerTimer += Time.deltaTime;
        if (flickerTimer >= flickTime)
        {
            flickerTimer = 0f;
            sr.sprite = GC.GetReference<Sprite>(coral.IdleSpritesIDs[nextSpriteID]);
            nextSpriteID = nextSpriteID - (nextSpriteID * 2) + 1;
        }
    }
}
