using System.Collections;
using System.Collections.Generic;
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

      float laVitesse = Input.GetAxis("Vertical") * vitesseDeplace;
      float velociteY = GetComponent<Rigidbody>().linearVelocity.y;

      if (Input.GetKey(KeyCode.LeftShift))
      {
         laVitesse *= 2;
      }

      // Gestion du saut. Si la touche espace est enfoncée et que le personnage a les
   // pieds au sol, on applique une force verticale vers le haut
      if (Input.GetKeyDown(KeyCode.Space) && auSol)
      {
         velociteY += forceSaut;
      }

      // Déplacement du personnage, seulement s'il a les pieds au sol
      if (auSol)
      {
         GetComponent<Rigidbody>().linearVelocity = transform.forward * laVitesse + new Vector3(0f, velociteY, 0f);
      }



      GetComponent<Animator>().SetFloat("vitesseDeplacement", Mathf.Abs(laVitesse));

      transform.Rotate(0f, Input.GetAxis("Mouse X") * vitesseTourne, 0f);




   }


}
