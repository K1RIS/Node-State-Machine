namespace StateMachine
{
    public abstract class Action 
    {
        public virtual void OnStart() { }
        public virtual void OnUpdate() { }
        public virtual void OnEnd() { }
    }
}