﻿using System;
using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/Action/Rotate")]
public class Rotate : StateMachine.Action
{
    [SerializeField] private TransformRuntimeVariable transform = null;
    [SerializeField] private FloatStandardVariable duration = null;
    [SerializeField] private FloatRuntimeVariable time = null;
    [SerializeField] private FloatRuntimeVariable rotationInput = null;

    [NonSerialized] private Quaternion startRot;
    [NonSerialized] private Quaternion endRot;

    public override void OnStart()
    {
        startRot = transform.Value.rotation;

        endRot = transform.Value.rotation * Quaternion.Euler(0f, Mathf.Sign(rotationInput.Value) * 90f, 0f);

        time.Value = 0f;
    }

    public override void OnUpdate()
    {
        time.Value += Time.deltaTime;
        transform.Value.rotation = Quaternion.Slerp(startRot, endRot, time.Value / duration.Value);
    }

    public override void OnEnd()
    {
        transform.Value.rotation = endRot;
    }
}