using UnityEngine;

public class ProjectilePlayer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void OnCollisionEnter(Collision other)
    {
        // Détruire le projectile lorsqu'il entre en collision avec un objet autre que le joueur
        if (other.gameObject.tag != "Player" && other.gameObject.tag != "DeathNDestroy")
        {
            Destroy(gameObject);
        }else if (other.gameObject.CompareTag("DeathNDestroy"))
        {
            //Le projectile s'auto détruit en entrant en collision avec les objets ayant le tag DeathNDestroy,
            //Détruit aussi l'objet avec lequel il entre en collision
            Destroy(other.gameObject);
        }        
    }
}
