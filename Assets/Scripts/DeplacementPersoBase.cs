using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeplacementPersoBase : MonoBehaviour
{
   public float vitesseDeplace;
   public float vitesseTourne;

   void Start()
   {
      Cursor.lockState = CursorLockMode.Locked;
   }

   // Update is called once per frame
   void Update()
   {

      float laVitesse = Input.GetAxis("Vertical") * vitesseDeplace;
      float velociteY = GetComponent<Rigidbody>().linearVelocity.y;

      if(Input.GetKey(KeyCode.LeftShift))
      {
         laVitesse *= 2;
      }

      GetComponent<Rigidbody>().linearVelocity = transform.forward * laVitesse + new Vector3(0f, velociteY, 0f);

      GetComponent<Animator>().SetFloat("vitesseDeplacement", Mathf.Abs(laVitesse));
      
      transform.Rotate(0f,Input.GetAxis("Mouse X")*vitesseTourne,0f);

    }

   
} 
