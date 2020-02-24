using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private TransformRuntimeVariable playerTransform = null;

    private void Awake()
    {
        playerTransform.Value = transform;
    }
}