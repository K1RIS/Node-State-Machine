namespace StateMachine
{
    public abstract class Action 
    {
        public virtual void OnStart() { }
        public virtual void OnUpdate(float statePercent) { }
        public virtual void OnEnd() { }
    }
}