using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/Condition/IsAxisInputDetected")]
public class IsAxisInputDetected : StateMachine.Condition
{
    [SerializeField] private FloatRuntimeVariable[] axis = null;

    public override bool Check()
    {
        for (int i = 0; i < axis.Length; i++)
            if (axis[i].Value != 0f)
                return true;

        Debug.Log("false");
        return false;
    }
}