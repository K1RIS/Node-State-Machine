using System;
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

        public void SetBool(string name, bool value) => controller.boolValues[Array.IndexOf(controller.boolNames, name)] = value;
        public bool GetBool(string name) => controller.boolValues[Array.IndexOf(controller.boolNames, name)];
        public void SetFloat(string name, float value) => controller.floatValues[Array.IndexOf(controller.floatNames, name)] = value;
        public float GetFloat(string name) => controller.floatValues[Array.IndexOf(controller.floatNames, name)];
        public void SetInteger(string name, int value) => controller.intValues[Array.IndexOf(controller.intNames, name)] = value;
        public int GetInteger(string name) => controller.intValues[Array.IndexOf(controller.intNames, name)];
    }
}