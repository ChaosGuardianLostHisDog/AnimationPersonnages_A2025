using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TirScript_CreationBalle : MonoBehaviour
{
    /*#################################################
    -- variables publiques à définir dans l'inspecteur
    #################################################*/
    [Header("Composant de tir")]
    public GameObject balle;
    public GameObject particuleBalle;
    public float vitesseBalle;

    [SerializeField] private Transform Firepoint;
    [SerializeField] private float spawnOffset = 5f;
    [SerializeField] private AudioClip gunShotAudioSource;
    [SerializeField] private StatsJoueur statsJoueur;

    private bool peutTirer;

    [Header("Composant de Melee")]
    public GameObject meleeWeapon;
    public Animator joueurAnimator;
    public float dureeAttaqueMelee = 1f;
    private bool peutAttaquer = true;

    private Transform lastSpawnBase;   // pour le debug gizmos

    //----------------------------------------------------------------------------------------------
    void Start()
    {
        peutTirer = true;
    }
    //----------------------------------------------------------------------------------------------

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && peutTirer && peutAttaquer && statsJoueur.nombreMunition > 0)
        {
            Tir();
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && peutTirer && peutAttaquer)
        {
            StartCoroutine(MeleeAttack());
        }
    }
    //----------------------------------------------------------------------------------------------

    void Tir()
    {
        peutTirer = false;
        Invoke("ActiveTir", 0.1f);

        if (CameraManager.isFirstPersonCamera == true)
        {
            particuleBalle.SetActive(true);
            GetComponent<AudioSource>().PlayOneShot(gunShotAudioSource);

            Transform spawnBase = Firepoint != null ? Firepoint : (Camera.main != null ? Camera.main.transform : transform);
            lastSpawnBase = spawnBase; // pour afficher les gizmos

            Vector3 direction = spawnBase.forward;
            Vector3 spawnPos = spawnBase.position + direction * spawnOffset;
            Quaternion spawnRot = spawnBase.rotation;
            GameObject balleCopie = Instantiate(balle, spawnPos, spawnRot);

            balleCopie.SetActive(true);
            balleCopie.GetComponent<Rigidbody>().linearVelocity = direction * vitesseBalle;
        }
        else
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

    void ActiveTir()
    {
        peutTirer = true;
        if (particuleBalle != null)
            particuleBalle.SetActive(false);
    }

    IEnumerator MeleeAttack()
    {
        peutAttaquer = false;
        joueurAnimator.SetTrigger("MeleeTrigger");

        meleeWeapon.SetActive(true);
        yield return new WaitForSeconds(1f);

        meleeWeapon.SetActive(false);
        peutAttaquer = true;
    }
}
     
