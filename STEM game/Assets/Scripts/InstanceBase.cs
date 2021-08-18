using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InstanceBase : IDeepCloneable
{
    private int instanceID;
    public int InstanceID { get { return instanceID; } }

    public InstanceBase(int _InstanceID)
    {
        this.instanceID = _InstanceID;
    }
    public abstract object DeepClone(float x, float y);
}
