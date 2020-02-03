using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/Action/SetupRotate")]
public class SetupRotate : StateMachine.Action
{
    [SerializeField] private TransformRuntimeVariable transform = null;
    [SerializeField] private QuaternionRuntimeVariable startRot = null;
    [SerializeField] private QuaternionRuntimeVariable endRot = null;
    [SerializeField] private FloatRuntimeVariable time = null;

    public override void Execute()
    {
        startRot.Value = transform.Value.rotation;

        if (Input.GetKeyDown(KeyCode.Q))
            endRot.Value = transform.Value.rotation * Quaternion.Euler(0f, -90f, 0f);
        else if (Input.GetKeyDown(KeyCode.E))
            endRot.Value = transform.Value.rotation * Quaternion.Euler(0f, 90f, 0f);

        time.Value = 0f;
    }
}