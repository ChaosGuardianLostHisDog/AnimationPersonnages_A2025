using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AiBrute : MonoBehaviour, IGestion2Degats
{
    [Header("Info Attack")]
    public bool isAttacking = false;
    public int attackNumber;
    public float DelaiAttaque = 2f;

    [Header("Ennemi Stats")]
    public float VieEnnemi;
    public float AttaqueFaible;
    public float AttaqueNormale;
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

    [Header("Audio du Monstre")]
    public AudioSource audioSource;
    public int idleDelayRange;
    public AudioClip[] SonIdle;
    public AudioClip[] SonAttaque;
    public AudioClip[] SonAttaqueLourde;
    public AudioClip[] SonIncomingAttack;
    public AudioClip[] SonDeath;

    [Header("Detection Vehicule")]
    public float detecteurVehicule; // 1 ou 2 comme demandé
    public float detecteurJoueurSurVehicule;

    // Flags publics pour lire l'état de détection depuis d'autres scripts / l'inspector
    public bool isVehiculeAhead = false;
    public bool isPlayerNearby = false;

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

    void Update()
    {
        DetectionJoueurSurVehicule();
        float distance = Vector3.Distance(target.position, transform.position);

        if (distance <= lookRadius)
        {
            agent.SetDestination(target.position);
            anim.SetBool("isRunning", true);
        }
        else
        {
            anim.SetBool("isRunning", false);
        }

        if (distance <= agent.stoppingDistance && isAttacking == false || isVehiculeAhead == true && isPlayerNearby == true && isAttacking == false)
        {
            FaceTarget();
            AttackTarget();
            IncomingAttackSound();
        }
    }

    void DetectionJoueurSurVehicule()
    {
        // Origine légèrement relevée pour éviter d'intersecter le sol
        Vector3 origine = transform.position + Vector3.up * 0.5f;
        Vector3 directionRay = transform.forward;

        // Raycast frontal pour détecter un MeshCollider tagué "vehicule"
        isVehiculeAhead = false;
        RaycastHit RayCollisionInfo;
        if (Physics.Raycast(origine, directionRay, out RayCollisionInfo, detecteurVehicule))
        {
            Collider ColliderDect = RayCollisionInfo.collider;
            if (ColliderDect.CompareTag("vehicule") && ColliderDect is MeshCollider)
            {
                // vérifier que le collider est bien un MeshCollider (ou contient un)
                isVehiculeAhead = true;
            }
        }
       isPlayerNearby = false;
        Collider[] overlaps = Physics.OverlapSphere(origine, detecteurJoueurSurVehicule);
        for (int i = 0; i < overlaps.Length; i++)
        {
            Collider playerCollider = overlaps[i];
            if (playerCollider.CompareTag("Player"))
            {
                isPlayerNearby = true;
                break;
            }
        }
    }
    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void AttackTarget()
    {
        isAttacking = true;

        attackNumber = Random.Range(1, 6);
        string triggerName;

        switch (attackNumber)
        {
            case 1:
                triggerName = "TriggerRightSweep";
                currentDamage = AttaqueNormale;
                break;
            case 2:
                triggerName = "TriggerLeftSweep";
                currentDamage = AttaqueNormale;
                break;
            case 3:
                triggerName = "TriggerKick";
                currentDamage = AttaqueFaible;
                break;
            case 4:
                triggerName = "TriggerCombo1";
                currentDamage = AttaqueForte;
                break;
            case 5:
                triggerName = "TriggerCombo2";
                currentDamage = AttaqueForte;
                break;
            default:
                triggerName = "TriggerRightSweep";
                currentDamage = AttaqueNormale;
                break;
        }

        anim.SetTrigger(triggerName);
    }

    public void MouvementStop()
    {
        agent.speed = 0;
    }

    public void RetablirMouvement()
    {
        isAttacking = false;
        agent.speed = 1f;
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
            rb.linearVelocity = -transform.forward * 10f + Vector3.up * 2f;

            if (VieEnnemi <= 0)
            {
                Animation2Mort();
            }
        }
    }

    void Animation2Mort()
    {
        int deathNumber = Random.Range(1, 4); // 1, 2, ou 3
        string deathName;
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
        //  Désactive les colliders du monstre
        col.enabled = false;
        //  Désactive le NavMeshAgent pour éviter warnings + arrêter mouvement
        agent.enabled = false;
        //  Désactive ce script d’IA pour empêcher toute logique
        this.enabled = false;
        // Détruire le corps après 15 secondes

        DeathSound();
        Destroy(gameObject, 30f);
    }

    IEnumerator IdleSoundBoucle()
    {
        while (true)
        {
            int delay = Random.Range(6, 10);
            yield return new WaitForSeconds(delay);

            if (!this.enabled) yield break;
            if (!isAttacking && SonIdle.Length > 0)
            {
                int index = Random.Range(0, SonIdle.Length);
                audioSource.pitch = Random.Range(0.25f, 0.85f);
                audioSource.PlayOneShot(SonIdle[index]);
            }
        }
    }

    public void DeathSound()
    {
            int index = Random.Range(0, SonDeath.Length);
            audioSource.pitch = 1f;
            audioSource.PlayOneShot(SonDeath[index]);
    }

    public void IncomingAttackSound()
    {
            int index = Random.Range(0, SonIncomingAttack.Length);
            audioSource.pitch = Random.Range(0.5f, 1f);
            audioSource.PlayOneShot(SonIncomingAttack[index]);
    }

    public void RandomSweepAudio()
    {
        int index = Random.Range(0, SonAttaque.Length);
        audioSource.pitch = Random.Range(0.75f, 1.25f);
        audioSource.volume = 0.25f;
        audioSource.PlayOneShot(SonAttaque[index]);
    }

    public void RandomHeavyImpactAudio()
    {
            int index = Random.Range(0, SonAttaqueLourde.Length);
            audioSource.pitch = Random.Range(0.5f, 1f);
            audioSource.PlayOneShot(SonAttaqueLourde[index]);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
