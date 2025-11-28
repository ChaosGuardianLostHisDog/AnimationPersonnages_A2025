using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalleScript : MonoBehaviour
{
    [Header("Impact")]
    public GameObject impactTir;     
    public GameObject personnage;    

    [Header("SphereCast Detection")]
    public float sphereRadius;        
    public float detectionDistance;     
    public LayerMask detectLayerMask;        
    public bool debugSphereCast = false;     

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        DetectWithSphereCast();
    }

    private void DetectWithSphereCast()
    {
        if (rb == null) return;

        // Direction de déplacement de la balle
        Vector3 direction = rb.linearVelocity.normalized;

        // DEBUG : dessine la sphère
        if (debugSphereCast)
        {
            Debug.DrawRay(transform.position, direction * detectionDistance, Color.yellow);
        }

        // Raycast
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, sphereRadius, direction, out hit, detectionDistance, detectLayerMask))
        {
            GameObject root = hit.collider.transform.root.gameObject;

            AiSkeletonWarrior warrior = root.GetComponent<AiSkeletonWarrior>();
            if (warrior != null)
            {
                // Appelle la fonction de blocage AVANT l’impact
                warrior.BlockProjectileJoueur();
            }
        }
    }

    private void OnCollisionEnter(Collision infoCollisions)
    {
        // Effets d’impact
        if (impactTir != null)
        {
            GameObject particulesCopie = Instantiate(impactTir);
            particulesCopie.transform.position = infoCollisions.contacts[0].point;
            particulesCopie.SetActive(true);

            if (personnage != null)
                particulesCopie.transform.LookAt(personnage.transform);

            particulesCopie.transform.Translate(0, 0, 0.2f, Space.Self);

            Destroy(particulesCopie, 1f);
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (!debugSphereCast) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, sphereRadius);
    }
}