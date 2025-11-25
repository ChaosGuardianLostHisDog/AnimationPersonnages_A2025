using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;

public class AiSkeletonUltraBoomer : MonoBehaviour, IGestion2Degats
{
    [Header("Info Attack")]
    public GameObject lightSuicide;
    public GameObject ExplosionParticule;
    public bool isAttacking = false;
    public int attackNumber;
    public float DelaiAttaque = 2f; // ajustable si besoin dépendant de la difficulté da vague
    public bool SuicideActive = false;
    public bool IsDead = false;

    [Header("Ennemi Stats")]
    public float VieEnnemi;
    public float AttaqueForte;
    private float currentDamage;

    // Interface : permet au script EnvoieDegats de récupérer les dégâts
    public float GetDamage() => currentDamage;

    [Header("Ennemi Death Value")]
    public int deathNumber;

    [Header("Look Radius et AI")]
    public float lookRadius = 10f;
    Transform target;
    NavMeshAgent agent;
    Animator anim;
    Rigidbody rb;
    Collider col;

    [Header("Info Explosion Mort")]
    public float AoEDistance = 7.5f;         // rayon de l'explosion
    public float DegatExplosionMax = 250f;    // dégâts max au centre
    public LayerMask explosionLayerMask = ~0;  // qui peut être touché (assignable dans l'inspector)

    [Header("Audio du Monstre")]
    public AudioSource audioSource;
    public AudioSource audioSource4Explosion;
    public int idleDelayRange; // temps aléatoire entre sons
    public AudioClip[] SonIdle;
    public AudioClip[] SonAttaque;
    public AudioClip[] SonIncomingAttack;
    public AudioClip[] SonCri2Suicide;
    public AudioClip[] SonExplosion;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        target = PlayerManager.instance.player.transform;

        StartCoroutine(IdleSoundBoucle());
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(target.position, transform.position);

        if (distance <= lookRadius && agent.enabled)
        {
            agent.SetDestination(target.position);
            // Jouer l'animation du monstre qui cours ver le joueur

            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }
        if (distance <= agent.stoppingDistance && isAttacking == false)
        {
            // Attaquer le joueur
            // Regarder le joueur
            FaceTarget();
            AttackTarget();
            IncomingAttackSound();
        }
    }

    void FaceTarget()
    {
        // Tourner vers le joueur, il doit le regarder avec le moins de delai possible
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void AttackTarget()
    {
        isAttacking = true;


        // Choisir un nombre aléatoire entre 1 et 3
        int attackNumber = UnityEngine.Random.Range(1, 3);
        string triggerName;

        if (SuicideActive == false && isAttacking == true)
        {
            switch (attackNumber)
            {
                case 1:
                    triggerName = "TriggerJabAttack";
                    currentDamage = AttaqueForte;
                    break;
                case 2:
                    triggerName = "TriggerSuicide";
                    anim.SetBool("Suicide", true);
                    SuicideActive = true;
                    agent.speed = 10f;
                    lightSuicide.SetActive(true);
                    Invoke("Animation2Mort", 5f);
                    break;
                default:
                    triggerName = "TriggerJabAttack";
                    currentDamage = AttaqueForte;
                    break;
            }
            // Déclencher le trigger correspondant
            anim.SetTrigger(triggerName);
        }
        else if (SuicideActive == true && isAttacking == true)
        {
            anim.SetTrigger("TriggerSuicide");
            Invoke("Animation2Mort", 0.75f);
        }

    }

    public void MouvementStop()
    {
        Debug.Log("Attaque Effectuer !");
        agent.speed = 0;
    }
    public void RetablirMouvement()
    {
        if (SuicideActive == false)
        {
            Debug.Log("Attaque Réinistialiser");
            isAttacking = false;
            agent.speed = 2f;
        }
        else if (SuicideActive == true)
        {
            Debug.Log("Attaque Réinistialiser");
            isAttacking = false;
            agent.speed = 13f;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("ProjectilePlayer"))
        {
            VieEnnemi -= 50;
            //Le monstre prend du knockback, il est repoussé en arrière lorsqu'il est touché par l'attaque au corps à corps du joueur
            rb.linearVelocity = -transform.forward * 10f + Vector3.up * 2f;
            if (VieEnnemi <= 0)
            {
                Animation2Mort();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("MeleePlayer"))
        {

            VieEnnemi -= 20;
            //Le monstre prend du knockback, il est repoussé en arrière lorsqu'il est touché par l'attaque au corps à corps du joueur
            rb.linearVelocity = -transform.forward * 10f + Vector3.up * 2f;

            if (VieEnnemi <= 0)
            {
                Animation2Mort();
            }

        }
    }

    void Animation2Mort()
    {
        if( IsDead == false)
        {
        int deathNumber = UnityEngine.Random.Range(1, 4); // 1, 2, ou 3
        string deathName;
        IsDead = true;

        switch (deathNumber)
        {
            case 1:
                deathName = "Death1";
                break;
            case 2:
                deathName = "Death2";
                break;
            case 3:
                deathName = "Death3";
                break;
            default:
                deathName = "Death1";
                break;
        }

        anim.SetTrigger(deathName);
        col.enabled = false;
        agent.enabled = false;
        ExplosionParticule.SetActive(true);
        BOOM();
        this.enabled = false;

        Destroy(gameObject, 30f);
        }
    }

    IEnumerator IdleSoundBoucle()
    {
        while (true)
        {
            int delay = UnityEngine.Random.Range(7, 10); // temps aléatoire entre 6 et 10 secondes
            yield return new WaitForSeconds(delay);

            if (!this.enabled) yield break;
            if (isAttacking) continue;

            int index = UnityEngine.Random.Range(0, SonIdle.Length);
            audioSource.pitch = UnityEngine.Random.Range(0.15f, 0.35f);
            audioSource.volume = 0.5f;
            audioSource.PlayOneShot(SonIdle[index]);
        }
    }

    public void IncomingAttackSound()
    {
        int index = UnityEngine.Random.Range(0, SonIncomingAttack.Length);
        audioSource4Explosion.pitch = UnityEngine.Random.Range(0.75f, 1.25f);
        audioSource4Explosion.volume = 1f;
        audioSource4Explosion.PlayOneShot(SonIncomingAttack[index]);
    }

    public void RandomSweepAudio()
    {
        int index = UnityEngine.Random.Range(0, SonAttaque.Length);
        audioSource.pitch = UnityEngine.Random.Range(0.5f, 0.7f);
        audioSource.volume = 0.25f;
        audioSource.PlayOneShot(SonAttaque[index]);
    }

    public void SonCri()
    {
        int index = UnityEngine.Random.Range(0, SonCri2Suicide.Length);
        audioSource.volume = 1;
        audioSource.pitch = UnityEngine.Random.Range(0.6f, 0.7f);
        audioSource.PlayOneShot(SonCri2Suicide[index]);
    }

    public void BOOM()
    {
        int index = UnityEngine.Random.Range(0, SonExplosion.Length);
        audioSource4Explosion.volume = 1;
        audioSource4Explosion.spatialBlend = 0.3f;
        audioSource4Explosion.pitch = UnityEngine.Random.Range(0.5f, 1f);
        audioSource4Explosion.PlayOneShot(SonExplosion[index]);
        AppliquerDegatExplosion();
    }

    void AppliquerDegatExplosion()
    {
        Vector3 centre = transform.position;
        Collider[] hits = Physics.OverlapSphere(centre, AoEDistance, explosionLayerMask);

        foreach (Collider hit in hits)
        {
            if (hit.transform.IsChildOf(transform) || hit.gameObject == gameObject) continue;

            float dist = Vector3.Distance(centre, hit.ClosestPoint(centre));
            float t = 1f - Mathf.Clamp01(dist / AoEDistance);
            float dmg = t * DegatExplosionMax;
            dmg = Mathf.Round(dmg);
            if (dmg <= 0f) continue;

            DeplacementPersoSaut player = hit.GetComponentInParent<DeplacementPersoSaut>();
            if (player != null)
            {
                player.MiseAJourDeLaVieDuJoueur(dmg);
                continue;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
        Gizmos.DrawWireSphere(transform.position, AoEDistance);
    }
}