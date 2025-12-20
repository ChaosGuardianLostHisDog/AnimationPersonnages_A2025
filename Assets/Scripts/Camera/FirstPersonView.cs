using UnityEngine;

public class FirstPersonView : MonoBehaviour
{
    [Header("Sensibilité")]
    public float mouseSensitivity = 100f;

    [Header("Référence joueur")]
    public Transform playerBody;

    private float xRotation = 0f;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");

        float rotation = mouseSensitivity * Time.deltaTime;

        // Rotation verticale (caméra)
        xRotation -= mouseY * rotation;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotation horizontale (corps)
        playerBody.Rotate(Vector3.up * mouseX * rotation);
    }
}
