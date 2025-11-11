using UnityEngine;

public class DefaultCube : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Plateforme"))
        {
            //le cube se détruit en entrant en collision avec la plateforme
            Destroy(gameObject);
        }
    } 
}
