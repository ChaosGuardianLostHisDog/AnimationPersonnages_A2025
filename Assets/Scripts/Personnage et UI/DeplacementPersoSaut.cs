using System.Collections;
using System.Collections.Generic;

using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeplacementPersoSaut : MonoBehaviour
{
   [Header("Composants")]
   [SerializeField] private GameObject explosionDeath;
   [SerializeField] private Image HPBar;
   [SerializeField] private Image ScreenBloodOverlay;
   [SerializeField] private Image staminaBar;
   [SerializeField] private GameObject ProjectilePrefab;
   [SerializeField] private Transform Firepoint;
   [SerializeField] private GameObject DiplayStatsJoueurUI;
   private bool auSol; // est-ce que le personnage a les pieds au sol
   private TirScript_CreationBalle TirScript;
   private Rigidbody rb;
   private Animator anim;
   public AudioSource audioSource;
   public AudioClip[] SonDegat;
   Collider col;

   [Header("UI")]
   [SerializeField] private StatsJoueur statsJoueur;

   [Header("Stats Joueur")]
   public float VieJoueurMax;
   public float VieJoueur;
   public float DegatJoueurMelee;
   public float DegatJoueurDistance;

   [Header("Mouvement")]
   public float vitesseDeplace = 6f;
   public float multiplicateurSprint = 2f;
   private float vitesseDeplaceBase;
   public float vitesseTourne;
   public float forceSaut;

   [Header("Stamina")]
   public float BarreStaminaMax;
   public float BarreStamina;
   public float staminaPerteParSeconde = 20f;   // Vitesse de perte
   public float staminaRegenParSeconde;   // Vitesse de régénération
   public float sprintCooldown = 7.5f;          // Temps avant de pouvoir resprinter après épuisement (Ceci est un pénalité pour avoir consommer toute la stamina)
   [SerializeField] float sprintCooldownTimer = 0f;

   [Header("Out Of Bounds")]
   public float hauteurMaxAutorisee = 20f;
   public float delaiAvantOoB = 2f;     // délai avant que le vrai timer commence
   public float tempsMaxAuDessus = 5f;  // temps de survie après le délai
   private float timerGrace = 0f;
   private float timerOoB = 0f;
   private bool OoBActif = false;

   [Header("Pause")]
   private bool isPaused = false;
   public GameObject screenPaused;


   void Start()
   {
      rb = GetComponent<Rigidbody>();
      anim = GetComponent<Animator>();
      col = GetComponent<Collider>();
      audioSource = GetComponent<AudioSource>();
      TirScript = GetComponent<TirScript_CreationBalle>();


      vitesseDeplaceBase = vitesseDeplace;

      Cursor.lockState = CursorLockMode.Locked;
      Cursor.visible = false;

      SetBloodOverlayAlpha(0f);

      // Mise à jour des stats du joueur selon l'achat fait par le joueur
      int hpUpgradeLevel = PlayerPrefs.GetInt("HpUpgradeLevel", 0);
      int bonusHP = hpUpgradeLevel * 100;
      VieJoueurMax += bonusHP;
      VieJoueur = VieJoueurMax;
      UpdateHPBar();

      int EnduranceUpgradeLevel = PlayerPrefs.GetInt("EnduranceUpgradeLevel", 0);
      int bonusEndurance = EnduranceUpgradeLevel * 100;
      int bonusRegenEndurance = EnduranceUpgradeLevel * 5;
      BarreStaminaMax += bonusEndurance;
      BarreStamina = BarreStaminaMax;
      staminaRegenParSeconde += bonusRegenEndurance;
      UpdateStaminaBar();

      int MoveUpgradeLevel = PlayerPrefs.GetInt("MoveUpgradeLevel", 0);
      int bonusMove = MoveUpgradeLevel;
      vitesseDeplaceBase += bonusMove;
      vitesseDeplace = vitesseDeplaceBase;

      int DamageUpgradeLevel = PlayerPrefs.GetInt("DamageUpgradeLevel", 0);
      int bonusDamageMelee = DamageUpgradeLevel * 20;
      int bonusDamageDistance = DamageUpgradeLevel * 30;
      DegatJoueurMelee += bonusDamageMelee;
      DegatJoueurDistance += bonusDamageDistance;
   }

   // Update is called once per frame
   void Update()
   {
      if (Input.GetKeyDown(KeyCode.Alpha3))
      {
         TogglePause();
         return; // empêche le reste du Update pendant la pause
      }

      if (isPaused)
         return;

      // Permet de savoir si un objet se trouve aux pieds du personnage
      RaycastHit infosCollision;
      auSol = Physics.SphereCast(transform.position + new Vector3(0, 0.5f, 0), 0.2f, -Vector3.up, out infosCollision, 0.8f);

      // On ajuste la paramètre booléen en fonction du spherecast (animSaut = true si en l'air)
      anim.SetBool("animSaut", !auSol);

      // Récupération des axes
      float laVitesseV = Input.GetAxis("Vertical") * vitesseDeplace;
      float laVitesseH = Input.GetAxis("Horizontal") * vitesseDeplace;
      float velociteY = rb.linearVelocity.y;

      // Gestion de la course (Grandement inspiré de votre gestion de votre script d'essence durant les exercices d'hélico)
      bool veutSprinter = Input.GetKey(KeyCode.LeftShift) && sprintCooldownTimer <= 0f;

      // Gestion de la stamina
      if (veutSprinter && BarreStamina > 0f && (laVitesseV != 0 || laVitesseH != 0))
      {
         vitesseDeplace = vitesseDeplaceBase * multiplicateurSprint;

         BarreStamina -= staminaPerteParSeconde * Time.deltaTime;
         BarreStamina = Mathf.Clamp(BarreStamina, 0f, BarreStaminaMax);

         UpdateStaminaBar();

         if (BarreStamina <= 0f)
            sprintCooldownTimer = sprintCooldown;
      }
      else
      {
         vitesseDeplace = vitesseDeplaceBase;

         if (BarreStamina < BarreStaminaMax)
            BarreStamina += staminaRegenParSeconde * Time.deltaTime;

         BarreStamina = Mathf.Clamp(BarreStamina, 0f, BarreStaminaMax);
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
         velociteY += Physics.gravity.y * Time.deltaTime - 0.035f;
         rb.linearVelocity = transform.forward * vitesseTotale.z + transform.right * vitesseTotale.x + new Vector3(0f, velociteY, 0f);


         // Important : mettre les paramètres de déplacement à zéro en l'air pour que l'anim de saut puisse s'afficher
         anim.SetFloat("vitesseDevantDeplacement", 0f);
         anim.SetFloat("vitesseCoterDeplacement", 0f);
         anim.SetFloat("VitesseTotale", 0f);
      }

      transform.Rotate(0f, Input.GetAxis("Mouse X") * vitesseTourne, 0f);
      GestionHauteurMort();
   }

   private void UpdateStaminaBar()
   {
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

   // Le joueur prend des dégâts quand cette fonction est appeller
   public void MiseAJourDeLaVieDuJoueur(float degats)
   {
      AppliquerSonDamage();

      VieJoueur -= degats;
      VieJoueur = Mathf.Clamp(VieJoueur, 0f, VieJoueurMax);

      // Calculer alpha : 0 = full life, 1 = zero life
      // Mettre à jour la barre de vie (fillAmount de 0 à 1)
      float alpha = 1f - (VieJoueur / VieJoueurMax);
      alpha = Mathf.Clamp01(alpha);
      SetBloodOverlayAlpha(alpha);
      UpdateHPBar();

      if (VieJoueur <= 0)
      {
         DiplayStatsJoueurUI.SetActive(false);
         SetBloodOverlayAlpha(0f);
         anim.SetBool("animMort", true);
         explosionDeath.SetActive(true);
         StartCoroutine(ResetAnimMort());
      }
   }

   private void SetBloodOverlayAlpha(float alpha)
   {
      alpha = Mathf.Clamp01(alpha);
      // Le parent prend directement l’alpha donné
      Color parentColor = ScreenBloodOverlay.color;
      parentColor.a = alpha;
      ScreenBloodOverlay.color = parentColor;
      // Enfants deviennent opaques seulement à partir de 50%
      float childAlpha = 0f;
      if (alpha > 0.5f)
      {
         childAlpha = (alpha - 0.5f) / 0.5f;
      }
      var images = ScreenBloodOverlay.GetComponentsInChildren<UnityEngine.UI.Image>(includeInactive: true);
      foreach (var img in images)
      {
         if (img == ScreenBloodOverlay) continue; // éviter de modifier le parent 2x
         Color c = img.color;
         c.a = childAlpha;
         img.color = c;
      }
   }

   void GestionHauteurMort()
   {
      if (transform.position.y >= hauteurMaxAutorisee)
      {
         // Phase 1 : délai de grâce
         if (!OoBActif)
         {
            timerGrace += Time.deltaTime;

            if (timerGrace >= delaiAvantOoB)
            {
               OoBActif = true;
               timerOoB = tempsMaxAuDessus;
            }
         }
         // Phase 2 : vrai timer
         else
         {
            timerOoB -= Time.deltaTime;
            timerOoB = Mathf.Max(timerOoB, 0f);

            if (statsJoueur != null)
               statsJoueur.AfficherOoB(timerOoB);

            if (timerOoB <= 0f)
            {
               MiseAJourDeLaVieDuJoueur(VieJoueurMax);
               statsJoueur.CacherOoB();
            }
         }
      }
      else
      {
         // Reset si retour en zone
         if (timerGrace > 0f || OoBActif)
         {
            timerGrace = 0f;
            timerOoB = 0f;
            OoBActif = false;

            if (statsJoueur != null)
               statsJoueur.CacherOoB();
         }
      }
   }

   private void UpdateHPBar()
   {
      float fill = VieJoueur / VieJoueurMax;
      fill = Mathf.Clamp01(fill);
      HPBar.fillAmount = fill;

      if (fill > 0.75f)
         HPBar.color = Color.green;
      else if (fill > 0.3f)
         HPBar.color = Color.yellow;
      else
         HPBar.color = Color.red;
   }
   private IEnumerator ResetAnimMort()
   {
      yield return new WaitForSeconds(0.05f);
      anim.SetBool("animMort", false);
      TirScript.enabled = false;
      Cursor.lockState = CursorLockMode.None;
      Cursor.visible = true;
      // Quand le joueur meurt, il se fait éjecter par derrière lui-même 
      rb.linearVelocity = -transform.forward * 50f + Vector3.up * 2f;

      // désactiver le script de déplacement
      this.enabled = false;
      // Activer la caméra de mort via le CameraManager
      CameraManager cameraManager = FindFirstObjectByType<CameraManager>();
      cameraManager.ActivateDeathCamera();

      // recharger la scène après 5 secondes
      yield return new WaitForSeconds(3f);
      SceneManager.LoadScene(2);
   }

   public void AppliquerSonDamage()
   {
      if (SonDegat.Length > 0)
      {
         int index = Random.Range(0, SonDegat.Length);
         audioSource.pitch = 1f;
         audioSource.PlayOneShot(SonDegat[index]);
      }
   }

   void TogglePause()
   {
      isPaused = !isPaused;

      if (isPaused)
      {
         Time.timeScale = 0f;
         Cursor.lockState = CursorLockMode.None;
         Cursor.visible = true;
         screenPaused.SetActive(true);
         TirScript.enabled = false;
      }
      else
      {
         Time.timeScale = 1f;
         Cursor.lockState = CursorLockMode.Locked;
         Cursor.visible = false;
         screenPaused.SetActive(false);
         TirScript.enabled = true;
      }
   }

}
