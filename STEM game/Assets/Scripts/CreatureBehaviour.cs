using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CreatureBehaviour : MonoBehaviour, IClassSetable
{
    public abstract void SetClass(object classReference);
    new public abstract int GetInstanceID();
    public abstract string GetResearchItemID();
    public virtual Sprite GetSpriteIcon() { return GetComponent<SpriteRenderer>().sprite; }
}
