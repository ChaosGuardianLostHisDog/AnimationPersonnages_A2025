using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeplacementPersoSaut : MonoBehaviour
{
   public float vitesseDeplace;
   public float vitesseTourne;
   public float forceSaut;

   bool saute;
   bool auSol;

   void Start()
   {
      Cursor.lockState = CursorLockMode.Locked;
   }

   // Update is called once per frame
   void Update()
   {
      RaycastHit infosCollision;
      bool auSol = Physics.SphereCast(transform.position + new Vector3(0, 0.5f, 0), 0.2f, -Vector3.up, out infosCollision, 0.8f);
      GetComponent<Animator>().SetBool("animSaut", !auSol);

      float laVitesse = Input.GetAxis("Vertical") * vitesseDeplace;
      float velociteY = GetComponent<Rigidbody>().linearVelocity.y;

      if (Input.GetKey(KeyCode.LeftShift))
      {
         laVitesse *= 2;
      }

      if (Input.GetKeyDown(KeyCode.Space) && auSol)
      {
         velociteY += forceSaut;
      }

      if (auSol)
      {
         GetComponent<Rigidbody>().linearVelocity = transform.forward * laVitesse + new Vector3(0f, velociteY, 0f);
      }



      GetComponent<Animator>().SetFloat("vitesseDeplacement", Mathf.Abs(laVitesse));

      transform.Rotate(0f, Input.GetAxis("Mouse X") * vitesseTourne, 0f);




   }


}
