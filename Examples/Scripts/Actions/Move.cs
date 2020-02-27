using UnityEngine;

public class Move : StateMachine.Action
{
    [SerializeField] private TransformRuntimeVariable transform = null;
    [SerializeField] private FloatRuntimeVariable horizontalInput = null;
    [SerializeField] private FloatRuntimeVariable verticalInput = null;

    private Vector3 startPos;
    private Vector3 endPos;

    public override void OnStart()
    {
        startPos = transform.Value.position;

        endPos = transform.Value.position + (transform.Value.rotation * new Vector3Int(RoundAwayFromZero(horizontalInput.Value), 0, RoundAwayFromZero(verticalInput.Value)));
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

    public override void OnUpdate(float statePercent)
    {
        transform.Value.position = Vector3.Lerp(startPos, endPos, statePercent);
    }

    public override void OnEnd()
    {
        transform.Value.position = endPos;
    }
}