using UnityEngine;

public class KillableCube : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {                       
        if (other.gameObject.CompareTag("ProjectilePlayer"))
        {
            //le monstre se détruit en entrant en collision avec les tires du joueurs
            Destroy(gameObject);
        }
    } 
}
