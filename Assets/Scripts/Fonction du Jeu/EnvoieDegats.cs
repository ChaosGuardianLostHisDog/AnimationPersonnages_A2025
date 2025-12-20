using UnityEngine;

public class EnvoieDegats : MonoBehaviour
{
    private IGestion2Degats monster; // Interface universelle pour tous les monstres

    void Start()
    {
        // Cherche dans le parent un composant qui implémente IGestion2Degats
        monster = GetComponentInParent<IGestion2Degats>();
    }

    // Détecte les collisions avec le joueur sans le Trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && monster != null)
        {
            DeplacementPersoSaut player = other.GetComponent<DeplacementPersoSaut>();
            player.MiseAJourDeLaVieDuJoueur(monster.GetDamage());
        }
    }
}