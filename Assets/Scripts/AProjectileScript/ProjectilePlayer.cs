using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class project : MonoBehaviour
{
    private void Update()
    {
    }

    // Script à associer à la balle
    /*#################################################
    -- variables publiques à définir dans l'inspecteur
    #################################################*/
    public GameObject impactTir;   // Référence au Prefab à instancier lorsque le tir frappe un objet.
    public GameObject personnage;  // Référence au personnage

    /*
     * Fonction OnCollisionEnter. Gère ce qui se passe lorsqu'une balle touche un objet.
     */
    private void OnCollisionEnter(Collision infoCollisions)
    {
        /*
         * Étapes :
         * 1. Création d'une copie du Prefab impactTir
         * 2. Placement au point de contact
         * 3. Activation de l'objet
         * 4. Orientation vers le personnage (LookAt)
         * 5. Correction de position
         * 6. Destruction des particules après 1 seconde
         * 7. Destruction immédiate de la balle
         */

        GameObject particulesCopie = Instantiate(impactTir);

        // 2. Position au point de contact
        particulesCopie.transform.position = infoCollisions.contacts[0].point;

        // 3. Activation
        particulesCopie.SetActive(true);

        // 4. Orientation vers le personnage
        particulesCopie.transform.LookAt(personnage.transform);

        // 5. Correction légère pour éviter d'être derrière l'objet
        particulesCopie.transform.Translate(0, 0, 0.2f);

        // 6. Détruire les particules après 1 seconde
        Destroy(particulesCopie, 1f);

        // 7. Détruire la balle
        Destroy(gameObject);
    }
}
