using UnityEngine;

public class Rotate : StateMachine.Action
{
    [SerializeField] private TransformRuntimeVariable transform = null;
    [SerializeField] private FloatRuntimeVariable rotationInput = null;

    private Quaternion startRot;
    private Quaternion endRot;

    public override void OnStart()
    {
        startRot = transform.Value.rotation;

        endRot = transform.Value.rotation * Quaternion.Euler(0f, Mathf.Sign(rotationInput.Value) * 90f, 0f);
    }

    public override void OnUpdate(float stateTime, float stateDuration)
    {
        transform.Value.rotation = Quaternion.Slerp(startRot, endRot, stateTime / stateDuration);
    }

    public override void OnEnd()
    {
        transform.Value.rotation = endRot;
    }
}