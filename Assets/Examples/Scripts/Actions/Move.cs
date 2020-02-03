using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/Action/Move")]
public class Move : StateMachine.Action
{
    [SerializeField] private TransformRuntimeVariable transform = null;
    [SerializeField] private Vector3RuntimeVariable startPos = null;
    [SerializeField] private Vector3RuntimeVariable endPos = null;
    [SerializeField] private FloatStandardVariable duration = null;
    [SerializeField] private FloatRuntimeVariable time = null;

    public override void Execute()
    {
        time.Value += Time.deltaTime;
        transform.Value.position = Vector3.Lerp(startPos.Value, endPos.Value, time.Value / duration.Value);
    }
}