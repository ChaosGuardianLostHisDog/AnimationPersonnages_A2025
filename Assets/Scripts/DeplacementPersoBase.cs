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
      float laVitesse = Input.GetAxis("Vertical") * vitesseDeplace;
     // Récupération de la velocité Y du personnage qu'on garde en mémoire
      float velociteY = GetComponent<Rigidbody>().linearVelocity.y;

      // Si majuscule-gauche enfoncée, on augment la vitesse de déplacement (pour la course)
      if (Input.GetKey(KeyCode.LeftShift))
      {
         laVitesse *= 2;
      }

      // On deplace le personnage en modifiant sa vélocité. On corrige la vélocité Y
      // en redonnant la valeur mémorisée plus haut
      GetComponent<Rigidbody>().linearVelocity = transform.forward * laVitesse + new Vector3(0f,velociteY,0f);

   // On change la paramètre de l'animator pour que l'animation de marche puisse joueur
      GetComponent<Animator>().SetFloat("vitesseDeplacement", Mathf.Abs(laVitesse));
      
      //Rotation du personnage en fonction du déplacement horizontal de la souris
      transform.Rotate(0f,Input.GetAxis("Mouse X")*vitesseTourne,0f);

    }

   
} 
