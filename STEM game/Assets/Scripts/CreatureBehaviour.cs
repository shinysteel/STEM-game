using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
