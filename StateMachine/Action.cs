﻿namespace StateMachine
{
    public abstract class Action
    {
        public virtual void OnStart() { }
        public virtual void OnUpdate(float stateTime, float stateDuration) { }
        public virtual void OnEnd() { }
    }
}