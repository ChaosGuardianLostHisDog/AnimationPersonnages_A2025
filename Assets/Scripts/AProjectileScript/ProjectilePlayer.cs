using UnityEngine;

public class ProjectilePlayer : MonoBehaviour
{

    void Start()
    {
        Destroy(gameObject, 5f); // Détruire le projectile après 5 secondes pour éviter qu'il reste indéfiniment dans la scène
                                 //Le raycastsphere apparait quand la balle est instanciée
        RaycastHit hit;
        print("start du projectile joueur");
        if (Physics.SphereCast(transform.position, 10f, transform.forward, out hit, 500f))
        {
            print("Ennemi touché par le projectile joueur"); 
            if (hit.collider.CompareTag("SkeletonWarrior"))
            {
                AiSkeletonWarrior ennemiSpecial = hit.collider.GetComponent<AiSkeletonWarrior>();
                ennemiSpecial.BlockProjectileJoueur();
            }
        }
    }

    void Update()
    {
       


    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void OnCollisionEnter(Collision other)
    {
        // Détruire le projectile lorsqu'il entre en collision avec un objet autre que le joueur
        if (other.gameObject.tag != "Player")
        {
            Destroy(gameObject);
        }        
    }
}
