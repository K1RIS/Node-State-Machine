using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/Condition/IsInputDetected")]
public class IsInputDetected : StateMachine.Condition
{
    [SerializeField] private KeyCode[] keyCodes = new KeyCode[0];

    public override bool Check()
    {
        for (int i = 0; i < keyCodes.Length; i++)
            if (Input.GetKeyDown(keyCodes[i]))
                return true;

        return false;
    }
}