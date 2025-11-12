using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TirScript_CreationBalle : MonoBehaviour
{
    /*#################################################
   -- variables publiques à définir dans l'inspecteur
   #################################################*/
   [Header("Composant de tir")]
    public GameObject balle; // Référence au gameObject de la balle (préfab)
    public GameObject particuleBalle; // Référence au gameObject à activer lorsque le personnage tir
    public float vitesseBalle; // Vitesse de la balle

    [SerializeField] private Transform Firepoint;
    [SerializeField] private float spawnOffset = 0.5f; // distance devant la caméra/firepoint
    [SerializeField] private AudioClip gunShotAudioSource; // Référence à la source audio pour le son de tir
    [SerializeField] private StatsJoueur statsJoueur;
    /*#################################################
   -- variables privées
   #################################################*/
    private bool peutTirer; // Est-ce que le personnage peut tirer

    [Header("Composant de Melee")]
    public GameObject meleeWeapon; // Référence à l'arme de mêlée
    public Animator joueurAnimator;         // L’Animator du joueur
    public float dureeAttaqueMelee = 1f;  // Durée pendant laquelle l’arme est active
    private bool peutAttaquer = true;
    

    //----------------------------------------------------------------------------------------------
    void Start()
    {
        peutTirer = true; // Au départ, on veut que le personnage puisse tirer
    }
    //----------------------------------------------------------------------------------------------


    /*
     * Fonction Update. On appele la fonction Tir() lorsque la touche souris gauche est enfoncée et que 
     * le personnage peut tirer
     */
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && peutTirer == true && peutAttaquer == true && statsJoueur.nombreMunition > 0)
        {
            Tir();
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && peutTirer == true && peutAttaquer == true)
        {
            StartCoroutine(MeleeAttack());
        }
    }
    //----------------------------------------------------------------------------------------------


    /*
     * Fonction Tir. Gère le tir d'une nouvelle balle.
     */
    void Tir()
    {
        /* On désactive la capacité de tirer et on appelle la fonction ActiveTir() après
         un délai de 0.1 seconde */
        peutTirer = false;
        Invoke("ActiveTir", 0.1f);

        // Vérification si la caméra active est en vue à la première personne

        if (CameraManager.isFirstPersonCamera == true)
        {
            // Effets son et particule
            particuleBalle.SetActive(true);
            GetComponent<AudioSource>().PlayOneShot(gunShotAudioSource);

            // Choix du point d'apparition : Firepoint > Camera.main > this.transform
            Transform spawnBase = Firepoint != null ? Firepoint : (Camera.main != null ? Camera.main.transform : transform);
            Vector3 direction = spawnBase.forward;
            Vector3 spawnPos = spawnBase.position + direction * spawnOffset;
            Quaternion spawnRot = spawnBase.rotation;
            GameObject balleCopie = Instantiate(balle, spawnPos, spawnRot);

            balleCopie.SetActive(true);

            Rigidbody rb = balleCopie.GetComponent<Rigidbody>();
            rb.linearVelocity = direction * vitesseBalle;


        }
        else if (CameraManager.isFirstPersonCamera == false)
        {
            particuleBalle.SetActive(true);
            GetComponent<AudioSource>().PlayOneShot(gunShotAudioSource);
            GameObject balleCopie = Instantiate(balle);
            balleCopie.transform.position = balle.transform.position;
            balleCopie.transform.rotation = balle.transform.rotation;
            balleCopie.SetActive(true);
            balleCopie.GetComponent<Rigidbody>().linearVelocity = transform.forward * vitesseBalle;
        }


        statsJoueur.nombreMunition--;
        statsJoueur.MettreAJourAffichageMunition();
    }
    //----------------------------------------------------------------------------------------------


    /*
     * Fonction ActiveTir(). Réactive la capacité de tirer.
     */

    void ActiveTir()
    {
        peutTirer = true;
        if (particuleBalle != null)
            particuleBalle.SetActive(false);
    }

    IEnumerator MeleeAttack()
    {
        peutAttaquer = false;

        // Déclenche l'animation d'attaque
        joueurAnimator.SetTrigger("MeleeTrigger");

        // Active l’arme
        meleeWeapon.SetActive(true);

        // délai
        yield return new WaitForSeconds(1f);
        meleeWeapon.SetActive(false);
        peutAttaquer = true;
    }
}       
