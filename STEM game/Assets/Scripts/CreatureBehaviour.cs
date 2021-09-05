using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public abstract class CreatureBehaviour : MonoBehaviour, IClassSetable
{
    private CreatureBase creature; public CreatureBase Creature { get { return creature; } }
    public virtual void SetClass(object classReference)
    {
        creature = (CreatureBase)classReference;
    }
    public virtual Sprite GetSpriteIcon()
    {
        return GetComponent<SpriteRenderer>().sprite;
    }
    new public int GetInstanceID()
    {
        return creature.InstanceID;
    }
    private bool paused;

    protected virtual void Start()
    {
        paused = GC.paused;
        GC.OnPause += Creature_UpdatePauseState;
    }
    protected bool DoUpdate()
    {
        return !paused;
    }
    private void Creature_UpdatePauseState(object sender, EventArgs e)
    {
        paused = !paused;
    }
}
