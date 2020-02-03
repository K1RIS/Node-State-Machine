using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private FloatRuntimeVariable horizontalInput = null;
    [SerializeField] private FloatRuntimeVariable verticalInput = null;
    [SerializeField] private FloatRuntimeVariable rotationInput = null;

    private void Update()
    {
        horizontalInput.Value = Input.GetAxis("Horizontal");
        verticalInput.Value = Input.GetAxis("Vertical");
        rotationInput.Value = Input.GetAxis("Mouse X");
    }
}