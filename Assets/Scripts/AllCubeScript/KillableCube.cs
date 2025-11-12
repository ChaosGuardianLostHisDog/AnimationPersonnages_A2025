using UnityEngine;

public class KillableCube : MonoBehaviour
{
    [SerializeField] private EnnemiStats HpMonstre;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("ProjectilePlayer"))
        {
            //le monstre se détruit en entrant en collision avec les tires du joueurs
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("MeleePlayer"))
        {

            HpMonstre.VieEnnemi -= 20;
            rb.linearVelocity = -transform.forward * 50f + Vector3.up * 2f;

            //Le monstre prend du knockback, il est repoussé en arrière lorsqu'il est touché par l'attaque au corps à corps du joueur
            
            rb.linearVelocity = -transform.forward * 50f + Vector3.up * 2f;
            if (HpMonstre.VieEnnemi <= 0)
            {
                Destroy(gameObject);
            }

        }
    }
}
