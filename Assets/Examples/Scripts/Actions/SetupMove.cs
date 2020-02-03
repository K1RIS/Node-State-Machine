using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/Action/SetupMove")]
public class SetupMove : StateMachine.Action
{
    [SerializeField] private TransformRuntimeVariable transform = null;
    [SerializeField] private Vector3RuntimeVariable startPos = null;
    [SerializeField] private Vector3RuntimeVariable endPos = null;
    [SerializeField] private FloatRuntimeVariable horizontalInput = null;
    [SerializeField] private FloatRuntimeVariable verticalInput = null;
    [SerializeField] private FloatRuntimeVariable time = null;

    public override void Execute()
    {
        startPos.Value = transform.Value.position;

        endPos.Value = transform.Value.position + (transform.Value.rotation * new Vector3Int(RoundAwayFromZero(horizontalInput.Value), 0, RoundAwayFromZero(verticalInput.Value)));

        time.Value = 0f;
    }

    private int RoundAwayFromZero(float f)
    {
        if (f < 0f)
            return -1;
        else if (f > 0f)
            return 1;
        else
            return 0;
    }
}