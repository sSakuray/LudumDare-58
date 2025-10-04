using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float sens = 2f;
    public float maxYAngle = 90f;

    private float rotationX = 0f;

    public void Update()
    {
        CameraRotation();
    }
    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void CameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        transform.parent.Rotate(Vector3.up * mouseX * sens);

        rotationX -= mouseY * sens;
        rotationX = Mathf.Clamp(rotationX, -maxYAngle, maxYAngle);
        transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
    }
}
