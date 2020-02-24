using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private StateMachine.StateMachine playerController = null;
    [Space]
    [SerializeField] private TransformRuntimeVariable playerTransform = null;

    private void Awake()
    {
        playerTransform.Value = transform;
    }

    private void Start()
    {
        playerController.OnStart();
    }

    private void Update()
    {
        playerController.OnUpdate();
    }

    private void OnDestroy()
    {
        playerController.OnEnd();
    }
}