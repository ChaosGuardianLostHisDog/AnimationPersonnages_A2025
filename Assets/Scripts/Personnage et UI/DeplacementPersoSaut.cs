using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;

using UnityEngine.UI;
using UnityEngine;

public class DeplacementPersoSaut : MonoBehaviour
{
   [Header("Composants")]
   [SerializeField] private GameObject explosionDeath;
   [SerializeField] private UnityEngine.UI.Image staminaBar;
   [SerializeField] private GameObject ProjectilePrefab;
   [SerializeField] private Transform Firepoint;

   [Header("Mouvement")]
    public float vitesseDeplace = 6f;
    public float vitesseTourne = 2f;
    public float forceSaut = 7f;
    
    [Header("Stamina")]
    public float BarreStaminaMax = 100f;
    public float BarreStamina = 100f;
    public float staminaPerteParSeconde = 20f;   // Vitesse de perte
    public float staminaRegenParSeconde = 15f;   // Vitesse de régénération
    public float sprintCooldown = 7.5f;          // Temps avant de pouvoir resprinter après épuisement (Ceci est un pénalité pour avoir consommer toute la stamina)
    [SerializeField] float sprintCooldownTimer = 0f;
   private bool auSol; // est-ce que le personnage a les pieds au sol
   private Rigidbody rb;
   private Animator anim;

   void Start()
   {
      rb = GetComponent<Rigidbody>();
      anim = GetComponent<Animator>();

      Cursor.lockState = CursorLockMode.Locked;
   }

   // Update is called once per frame
   void Update()
   {
      // Permet de savoir si un objet se trouve aux pieds du personnage
      RaycastHit infosCollision;
      auSol = Physics.SphereCast(transform.position + new Vector3(0, 0.5f, 0), 0.2f, -Vector3.up, out infosCollision, 0.8f);

      // On ajuste la paramètre booléen en fonction du spherecast (animSaut = true si en l'air)
      anim.SetBool("animSaut", !auSol);

      // Récupération des axes
      float laVitesseV = Input.GetAxis("Vertical") * vitesseDeplace;
      float laVitesseH = Input.GetAxis("Horizontal") * vitesseDeplace;

      // Utiliser linearVelocity
      float velociteY = rb.linearVelocity.y;

      // Gestion de la course (Grandement inspiré de votre gestion de votre script d'essence durant les exercices d'hélico)
      bool veutSprinter = Input.GetKey(KeyCode.LeftShift) && sprintCooldownTimer <= 0f;

      // Gestion de la stamina
      if (veutSprinter && BarreStamina > 0f && (laVitesseV != 0 || laVitesseH != 0))
      {
         // Sprint actif
         vitesseDeplace = 12f;
         BarreStamina -= staminaPerteParSeconde * Time.deltaTime;
         BarreStamina = Mathf.Clamp(BarreStamina, 0f, BarreStaminaMax);
         // On appelle la fonction de mise à jour de la barre de stamina de l'UI
         UpdateStaminaBar();

         if (BarreStamina <= 0f)
            sprintCooldownTimer = sprintCooldown; // cooldown si vidé
      }
      else
      {
         // Marche + régénération
         vitesseDeplace = 6f;
         if (BarreStamina < BarreStaminaMax)
            BarreStamina += staminaRegenParSeconde * Time.deltaTime;
         BarreStamina = Mathf.Clamp(BarreStamina, 0f, BarreStaminaMax);
         // On appelle la fonction de mise à jour de la barre de stamina de l'UI
         UpdateStaminaBar();
      }
      // Gestion du cooldown
      if (sprintCooldownTimer > 0f)
         sprintCooldownTimer -= Time.deltaTime;


      // Gestion du saut. Si la touche espace est enfoncée et que le personnage a les pieds au sol
      if (Input.GetKeyDown(KeyCode.Space) && auSol)
      {
         velociteY += forceSaut;
      }

      Vector3 vitesseTotale = new Vector3(laVitesseH, 0f, laVitesseV);

      // Normalisation optionnelle (conserve la direction, applique la vitesse de base)
      if (vitesseTotale.sqrMagnitude > 1f)
         vitesseTotale = vitesseTotale.normalized * vitesseDeplace;

      // Déplacement du personnage, il faut donner la possibilité au personnage de controler son mouvement en l'air
      if (auSol)
      {
         // Remplacement de rb.velocity par rb.linearVelocity
         rb.linearVelocity = transform.forward * vitesseTotale.z + transform.right * vitesseTotale.x + new Vector3(0f, velociteY, 0f);

         anim.SetFloat("vitesseDevantDeplacement", Mathf.Abs(vitesseTotale.z));
         anim.SetFloat("vitesseCoterDeplacement", Mathf.Abs(vitesseTotale.x));
         anim.SetFloat("VitesseTotale", Mathf.Abs(vitesseTotale.magnitude));
      }
      else
      {
         // Amplification de la vitesse de chute si le personnage est en l'air
         velociteY += Physics.gravity.y * Time.deltaTime - 0.025f;
         rb.linearVelocity = new Vector3(rb.linearVelocity.x, velociteY, rb.linearVelocity.z);

         // Important : mettre les paramètres de déplacement à zéro en l'air pour que l'anim de saut puisse s'afficher
         anim.SetFloat("vitesseDevantDeplacement", 0f);
         anim.SetFloat("vitesseCoterDeplacement", 0f);
         anim.SetFloat("VitesseTotale", 0f);
      }

      transform.Rotate(0f, Input.GetAxis("Mouse X") * vitesseTourne, 0f);

      // Tire du joueur
   if (Input.GetButtonDown("Fire1") && auSol)
   {
      ShootProjectile();
   }
   }

   private void UpdateStaminaBar()
   {
      // Par défaut, la barre de stamina n'est pas display, si le joueur ne sprint pas et quelle est pleine à 100%
      if (BarreStamina >= BarreStaminaMax && sprintCooldownTimer <= 0f)
      {
         staminaBar.enabled = false;
         return;
      }
      staminaBar.enabled = true;

      staminaBar.fillAmount = BarreStamina / BarreStaminaMax;
      //Changement de couleur de la barre de stamina en fonction du pourcentage restant
      if (BarreStamina / BarreStaminaMax > 0.6f)
      {
         staminaBar.color = Color.white;
      }
      else if (BarreStamina / BarreStaminaMax > 0.25f)
      {
         staminaBar.color = Color.yellow;
      }
      else
      {
         staminaBar.color = Color.red;
      }

      // Si la barre est en cooldown (on l'a vidé), on la fait noircir
      if (sprintCooldownTimer > 0f)
      {
         staminaBar.color = Color.black;
      }
   }
   
   private void ShootProjectile()
   {
      // Instancier le projectile à la position du personnage à partir du Firepoint
      Vector3 spawnPosition = Firepoint.position;
      GameObject projectile = Instantiate(ProjectilePrefab, spawnPosition, Quaternion.identity);

      // Donner une vitesse au projectile dans la direction où la camera de la 1er personne regarde
      Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
      float projectileSpeed = 50f;
      projectileRb.linearVelocity = Firepoint.forward * projectileSpeed;
   }

   // Jouer l'animation de mort de personnage après avoir toucher un objet ayant le tag "DeathPoint"
   private void OnCollisionEnter(Collision other)
   {
      if (other.gameObject.CompareTag("DeathPoint") || other.gameObject.CompareTag("DeathNDestroy"))
      {
         anim.SetBool("animMort", true);
         // instancier l'explosion à la position du personnage
         explosionDeath.SetActive(true);
         // après deux secondes setbool animMort à false pour réinitialiser l'animation
         StartCoroutine(ResetAnimMort());
      }
   }

   private IEnumerator ResetAnimMort()
   {
      yield return new WaitForSeconds(0.05f);
      anim.SetBool("animMort", false);
      // Quand le joueur meurt, il se fait éjecter par derrière lui-même 
      rb.linearVelocity = -transform.forward * 50f + Vector3.up * 2f;

      // désactiver le script de déplacement
      this.enabled = false;

      // Activer la caméra de mort via le CameraManager
      CameraManager cameraManager = FindObjectOfType<CameraManager>();
      cameraManager.ActivateDeathCamera();

      // recharger la scène après 5 secondes
      yield return new WaitForSeconds(5f);
      UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
   }
}
