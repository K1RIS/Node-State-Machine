public abstract class RuntimeVariable<T> : Variable<T>
{
    [System.NonSerialized] private T value;

    public override T Value
    {
        get => value;
        set => this.value = value;
    }
}