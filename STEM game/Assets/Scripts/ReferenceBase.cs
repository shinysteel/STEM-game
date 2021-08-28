using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ReferenceBase : IDeepCloneable
{
    private string referenceID; public string ReferenceID { get { return referenceID; } }

    public ReferenceBase(string _ReferenceID)
    {
        referenceID = _ReferenceID;
    }
    public abstract object DeepClone(float x, float y);
}
