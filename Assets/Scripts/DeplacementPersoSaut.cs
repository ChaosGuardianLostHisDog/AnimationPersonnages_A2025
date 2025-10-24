using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DeplacementPersoSaut : MonoBehaviour
{
   public float vitesseDeplace;
   public float vitesseTourne;
   public float forceSaut;

   private bool saute; // est-ce que le personnage saute
   private bool auSol; // est-ce que le personnage a les pieds au sol

   void Start()
   {

      Cursor.lockState = CursorLockMode.Locked;
   }

   // Update is called once per frame
   void Update()
   {
      // Permet de savoir si un objet se trouve aux pieds du personnage
      RaycastHit infosCollision;
      bool auSol = Physics.SphereCast(transform.position + new Vector3(0, 0.5f, 0), 0.2f, -Vector3.up, out infosCollision, 0.8f);

      // On ajuste la paramètre booléen en fonction du spherecast (auSol ou non)
      GetComponent<Animator>().SetBool("animSaut", !auSol);

      // Récupération de l'axe vertical (-1 et 1) qu'on multiplie par une vitesse de déplacement
      float laVitesseV = Input.GetAxis("Vertical") * vitesseDeplace;
      float laVitesseH = Input.GetAxis("Horizontal") * vitesseDeplace;
      
      float velociteY = GetComponent<Rigidbody>().linearVelocity.y;
      // Récupération de l'axe Horizontal (-1 et 1) qu'on multiplie par une vitesse de déplacement
    
        if (Input.GetKey(KeyCode.LeftShift))
      {
         laVitesseV *= 2;
           laVitesseH *= 2;
        }

      // Gestion du saut. Si la touche espace est enfoncée et que le personnage a les
      // pieds au sol, on applique une force verticale vers le haut
      if (Input.GetKeyDown(KeyCode.Space) && auSol)
      {
         velociteY += forceSaut;
      }

        Vector3 vitesseTotale = new Vector3(laVitesseH, 0f, laVitesseV);

        vitesseTotale = vitesseTotale.normalized * vitesseDeplace;

        // Déplacement du personnage, seulement s'il a les pieds au sol
        if (auSol)
      {
          

            GetComponent<Rigidbody>().linearVelocity = transform.forward * vitesseTotale.z + transform.right * vitesseTotale.x + new Vector3(0f, velociteY, 0f);
        }
        // Emplification de la vitesse de chute si le personnage est en l'air
        else if (!auSol){
            velociteY += Physics.gravity.y * Time.deltaTime - 0.01f;
            GetComponent<Rigidbody>().linearVelocity = new Vector3(GetComponent<Rigidbody>().linearVelocity.x, velociteY, GetComponent<Rigidbody>().linearVelocity.z);
        }

       



        GetComponent<Animator>().SetFloat("vitesseDevantDeplacement", Mathf.Abs(vitesseTotale.z));
        GetComponent<Animator>().SetFloat("vitesseCoterDeplacement", Mathf.Abs(vitesseTotale.x));
        GetComponent<Animator>().SetFloat("VitesseTotale", Mathf.Abs(vitesseTotale.magnitude));
        
         transform.Rotate(0f, Input.GetAxis("Mouse X") * vitesseTourne, 0f);




   }


}
