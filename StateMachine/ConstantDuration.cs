using UnityEngine;

namespace StateMachine
{
    public class ConstantDuration : Duration
    {
        [SerializeField] private float duration = Mathf.Infinity;

        public override float Get()
        {
            return duration;
        }
    }
}