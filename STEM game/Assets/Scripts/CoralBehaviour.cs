using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoralBehaviour : CreatureBehaviour
{
    public Coral coral;
    public override void SetClass(object classReference) { coral = (Coral)classReference; }
    public override int GetInstanceID() { return coral.InstanceID; }
    public override Sprite GetSpriteIcon() { return coral.IdleSprites[0]; }

    private float flickerTimer = 0f;
    private float flickTime = 1f;
    private int nextSpriteID;

    private SpriteRenderer sr;
    private void Awake() { sr = GetComponent<SpriteRenderer>(); }
    public override string GetResearchItemID() { return coral.GetResearchItemID(); }

    private void Start()
    {
        gameObject.AddComponent<BoxCollider2D>();
        sr.sprite = coral.IdleSprites[0];
        nextSpriteID = 1;
    }
    private void Update()
    {
        flickerTimer += Time.deltaTime;
        if (flickerTimer >= flickTime)
        {
            flickerTimer = 0f;
            sr.sprite = coral.IdleSprites[nextSpriteID];
            nextSpriteID = nextSpriteID - (nextSpriteID * 2) + 1;
        }
    }
}
