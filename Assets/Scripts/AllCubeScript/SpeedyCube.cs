using UnityEngine;

public class SpeedyCube : MonoBehaviour
{
    // Vitesse de la chute du cube est amplifiée
    [SerializeField] private float fallSpeedMultiplier = 2f;
    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity += Vector3.down * fallSpeedMultiplier * Time.fixedDeltaTime;
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Plateforme"))
        {
            //le cube se détruit en entrant en collision avec la plateforme
            Destroy(gameObject);
        }
    } 
}
