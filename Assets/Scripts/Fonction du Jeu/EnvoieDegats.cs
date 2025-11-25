using UnityEngine;

public class EnvoieDegats : MonoBehaviour
{
    private IGestion2Degats monster; // Interface universelle pour tous les monstres

    void Start()
    {
        // Cherche dans le parent un composant qui implémente IDamageDealer
        monster = GetComponentInParent<IGestion2Degats>();
        if (monster == null)
        {
            Debug.LogWarning("EnvoieDegats: Aucun IDamageDealer trouvé sur le parent !");
        }
    }

    // Détecte les collisions avec le joueur sans le Trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && monster != null)
        {
            DeplacementPersoSaut player = other.GetComponent<DeplacementPersoSaut>();
            if (player != null)
            {
                player.MiseAJourDeLaVieDuJoueur(monster.GetDamage());
            }
        }
    }
}