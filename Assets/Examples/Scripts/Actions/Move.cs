using System;
using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/Action/Move")]
public class Move : StateMachine.Action
{
    [SerializeField] private TransformRuntimeVariable transform = null;
    [SerializeField] private FloatStandardVariable duration = null;
    [SerializeField] private FloatRuntimeVariable time = null;
    [SerializeField] private FloatRuntimeVariable horizontalInput = null;
    [SerializeField] private FloatRuntimeVariable verticalInput = null;

    [NonSerialized] private Vector3 startPos;
    [NonSerialized] private Vector3 endPos;

    public override void OnStart()
    {
        startPos = transform.Value.position;

        endPos = transform.Value.position + (transform.Value.rotation * new Vector3Int(RoundAwayFromZero(horizontalInput.Value), 0, RoundAwayFromZero(verticalInput.Value)));

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

    public override void OnUpdate()
    {
        time.Value += Time.deltaTime;
        transform.Value.position = Vector3.Lerp(startPos, endPos, time.Value / duration.Value);
    }

    public override void OnEnd()
    {
        transform.Value.position = endPos;
    }
}