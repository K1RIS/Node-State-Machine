using UnityEngine;

public class IsAxisInputDetected : StateMachine.Condition
{
    [SerializeField] private FloatRuntimeVariable[] axis = new FloatRuntimeVariable[0];

    public override bool Check()
    {
        for (int i = 0; i < axis.Length; i++)
        {
            if (axis[i].Value != 0f)
                return true;
        }

        return false;
    }
}