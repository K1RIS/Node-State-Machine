using System;
using UnityEngine;

public abstract class StandardVariable<T> : Variable<T>
{
    [SerializeField] private T value = default;

    [NonSerialized] private T runtimeValue = default;
    [NonSerialized] private bool inited = false;

    public override T Value
    {
        get
        {
            if (!inited)
            {
                runtimeValue = value;
                inited = true;
            }

            return runtimeValue;
        }
        set
        {
            if (!inited)
                inited = true;

            runtimeValue = value;
        }
    }
}