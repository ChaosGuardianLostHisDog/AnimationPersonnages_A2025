using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeplacementPersoBase : MonoBehaviour
{
   [SerializeField]private float vitesseDeplace;
   [SerializeField]private float vitesseTourne;

   void Start()
   {
      // On lock la souris au centre et ont la masque
      Cursor.lockState = CursorLockMode.Locked;
   }

   // Gestion du déplacement du personnage
   void Update()
   {

      // Récupération de l'axe vertical (-1 et 1) qu'on multiplie par une vitesse de déplacement
      float laVitesseY = Input.GetAxis("Vertical") * vitesseDeplace;
        float laVitesseH = Input.GetAxis("Horizontal") * vitesseDeplace;
        // Récupération de la velocité Y et la Vélocité H du personnage qu'on garde en mémoire
        float velociteY = GetComponent<Rigidbody>().linearVelocity.y;
        float velociteH = GetComponent<Rigidbody>().linearVelocity.x;

        // Si majuscule-gauche enfoncée, on augment la vitesse de déplacement (pour la course)
        if (Input.GetKey(KeyCode.LeftShift))
      {
         laVitesseY *= 2;
            laVitesseH *= 1.5f;
        }

      // On deplace le personnage en modifiant sa vélocité. On corrige la vélocité Y
      // en redonnant la valeur mémorisée plus haut
      GetComponent<Rigidbody>().linearVelocity = transform.forward * laVitesseY + new Vector3(0f,velociteY,0f);
      // On deplace le personnage en modifiant sa vélocité. On corrige la vélocité X
      // en redonnant la valeur mémorisée plus haut
      GetComponent<Rigidbody>().linearVelocity += transform.right * laVitesseH + new Vector3(velociteH, 0f, 0f);



        // On change la paramètre de l'animator pour que l'animation de marche puisse joueur
        GetComponent<Animator>().SetFloat("vitesseDeplacement", Mathf.Abs(laVitesseY));
      
      //Rotation du personnage en fonction du déplacement horizontal de la souris
      transform.Rotate(0f,Input.GetAxis("Mouse X")*vitesseTourne,0f);

    }

   
} 
