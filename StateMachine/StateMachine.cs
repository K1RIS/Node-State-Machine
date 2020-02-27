using UnityEngine;

namespace StateMachine
{
    public class StateMachine : MonoBehaviour
    {
        [SerializeField] private StateMachineController controller = null;

        private void Awake()
        {
            controller = Instantiate(controller);
        }

        private void Start()
        {
            controller.OnStart();
        }

        private void Update()
        {
            controller.OnUpdate(Time.deltaTime);
        }

        private void OnDestroy()
        {
            controller.OnEnd();
        }
    }
}