using UnityEngine;

public abstract class Variable<T> : ScriptableObject
{
    public abstract T Value
    {
        get;
        set;
    }
}