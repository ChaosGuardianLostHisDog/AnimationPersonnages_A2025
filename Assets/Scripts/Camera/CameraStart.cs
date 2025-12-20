using UnityEngine;

public class CameraStart : MonoBehaviour
{
    [Header("Vitesse de rotation en degrés par seconde")]
    public float vitesseRotation = 30f;

    void Update()
    {
        // Faire tourner la caméra autour de son axe Y (vertical)
        transform.Rotate(Vector3.up, vitesseRotation * Time.deltaTime);
    }
}
