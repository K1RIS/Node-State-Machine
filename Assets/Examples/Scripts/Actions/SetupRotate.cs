using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/Action/SetupRotate")]
public class SetupRotate : StateMachine.Action
{
    [SerializeField] private TransformRuntimeVariable transform = null;
    [SerializeField] private QuaternionRuntimeVariable startRot = null;
    [SerializeField] private QuaternionRuntimeVariable endRot = null;
    [SerializeField] private FloatRuntimeVariable rotationInput = null;
    [SerializeField] private FloatRuntimeVariable time = null;

    public override void Execute()
    {
        startRot.Value = transform.Value.rotation;

        endRot.Value = transform.Value.rotation * Quaternion.Euler(0f, Mathf.Sign(rotationInput.Value) * 90f, 0f);

        time.Value = 0f;
    }
}