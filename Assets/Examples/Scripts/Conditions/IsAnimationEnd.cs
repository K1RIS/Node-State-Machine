using UnityEngine;

public class IsAnimationEnd : StateMachine.Condition
{
    [SerializeField] private FloatStandardVariable duration = null;
    [SerializeField] private FloatRuntimeVariable time = null;

    public override bool Check()
    {
        return time.Value / duration.Value >= 1f;
    }
}