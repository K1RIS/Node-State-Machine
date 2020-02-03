using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/Action/SetupMove")]
public class SetupMove : StateMachine.Action
{
    [SerializeField] private TransformRuntimeVariable transform = null;
    [SerializeField] private Vector3RuntimeVariable startPos = null;
    [SerializeField] private Vector3RuntimeVariable endPos = null;
    [SerializeField] private FloatRuntimeVariable time = null;

    public override void Execute()
    {
        startPos.Value = transform.Value.position;

        if (Input.GetKeyDown(KeyCode.W))
            endPos.Value = transform.Value.position + (transform.Value.rotation * Vector3.forward);
        else if (Input.GetKeyDown(KeyCode.S))
            endPos.Value = transform.Value.position + (transform.Value.rotation * Vector3.back);
        else if (Input.GetKeyDown(KeyCode.A))
            endPos.Value = transform.Value.position + (transform.Value.rotation * Vector3.left);
        else if (Input.GetKeyDown(KeyCode.D))
            endPos.Value = transform.Value.position + (transform.Value.rotation * Vector3.right);

        time.Value = 0f;
    }
}