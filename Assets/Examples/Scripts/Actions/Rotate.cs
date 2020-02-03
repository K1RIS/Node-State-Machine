using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/Action/Rotate")]
public class Rotate : StateMachine.Action
{
    [SerializeField] private TransformRuntimeVariable transform = null;
    [SerializeField] private QuaternionRuntimeVariable startRot = null;
    [SerializeField] private QuaternionRuntimeVariable endRot = null;
    [SerializeField] private FloatStandardVariable duration = null;
    [SerializeField] private FloatRuntimeVariable time = null;

    public override void Execute()
    {
        time.Value += Time.deltaTime;
        transform.Value.rotation = Quaternion.Slerp(startRot.Value, endRot.Value, time.Value / duration.Value);
    }
}