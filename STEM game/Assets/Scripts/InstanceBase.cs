using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InstanceBase : ReferenceBase
{
    private GameObject go; public GameObject GO { get { return go; } }
    private int instanceID; public int InstanceID { get { return instanceID; } }

    public InstanceBase(GameObject _GO, int _InstanceID, string _ReferenceID) : base(_ReferenceID)
    {
        go = _GO;
        instanceID = _InstanceID;
    }
}
