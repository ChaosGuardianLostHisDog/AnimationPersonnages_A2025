
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AiRunner : MonoBehaviour, IGestion2Degats
{
    [Header("Info Attack")]
    public bool isAttacking = false;
    public int attackNumber;
    public float DelaiAttaque = 2f; // ajustable si besoin dépendant de la difficulté da vague

    [Header("Ennemi Stats")]
    public float VieEnnemi;
    public float AttaqueNormale;
    public float AttaqueForte;
    private float currentDamage;

    // Interface : permet au script EnvoieDegats de récupérer les dégâts
    public float GetDamage() => currentDamage;

    [Header("Ennemi Death Value")]
    [SerializeField] private StatsJoueur statsJoueur;
    public float points;
    public int deathNumber;
    public GameObject flame;

    [Header("Look Radius et AI")]
    public float lookRadius = 10f;
    Transform target;
    NavMeshAgent agent;
    Animator anim;
    Rigidbody rb;
    Collider col;

    [Header("Audio du Monstre")]
    public AudioSource audioSource;
    public int idleDelayRange; // temps aléatoire entre sons
    public AudioClip[] SonIdle;
    public AudioClip[] SonAttaque;
    public AudioClip[] SonAttaqueLourde;
    public AudioClip[] SonIncomingAttack;
    public AudioClip[] SonDeath;
    public AudioClip[] sonGetMunition;

    [Header("Detection Vehicule")]
    public float detecteurVehicule;
    public float detecteurJoueurSurVehicule;


    public bool isVehiculeAhead = false;
    public bool isPlayerNearby = false;
    // ---------------------------------------------------------------------

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        target = PlayerManager.instance.player.transform;
        StartCoroutine(VerifieDistanceJoueur());
        StartCoroutine(IdleSoundBoucle());
    }

    // Update is called once per frame
    private IEnumerator VerifieDistanceJoueur()
    {
        const float intervalle = 0.25f; // 4 fois par seconde

        while (true)
        {
            DetectionJoueurSurVehicule();
            float distance = Vector3.Distance(target.position, transform.position);

            if (distance <= lookRadius)
            {
                agent.SetDestination(target.position);
                // Jouer l'animation du monstre qui cours ver le joueur

                anim.SetBool("isRunning", true);
            }
            else
            {
                anim.SetBool("isRunning", false);
            }
            if (distance <= agent.stoppingDistance && isAttacking == false || isVehiculeAhead == true && isPlayerNearby == true && isAttacking == false)
            {
                // Attaquer le joueur
                // Regarder le joueur
                FaceTarget();
                AttackTarget();
                IncomingAttackSound();
            }

            yield return new WaitForSeconds(intervalle);
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
        // Tourner vers le joueur, il doit le regarder avec le moins de delai possible
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void AttackTarget()
    {
        isAttacking = true;

        // Choisir un nombre aléatoire entre 1 et 3
        int attackNumber = Random.Range(1, 4); // 1, 2 ou 3
        string triggerName;

        switch (attackNumber)
        {
            case 1:
                triggerName = "TriggerRightKick";
                currentDamage = AttaqueNormale;
                break;
            case 2:
                triggerName = "TriggerLeftKick";
                currentDamage = AttaqueNormale;
                break;
            case 3:
                triggerName = "TriggerSmash";
                currentDamage = AttaqueForte;
                break;
            default:
                triggerName = "TriggerRightJab";
                currentDamage = AttaqueNormale;
                break;
        }

        // Déclencher le trigger correspondant
        anim.SetTrigger(triggerName);
    }

    public void MouvementStop()
    {
        Debug.Log("Attaque Effectuer !");
        agent.speed = 0;
    }
    public void RetablirMouvement()
    {
        Debug.Log("Attaque Réinistialiser");
        isAttacking = false;
        agent.speed = 18f;
    }

    private void OnCollisionEnter(Collision other)
    {
        // On récupère le joueur
        DeplacementPersoSaut player = FindFirstObjectByType<DeplacementPersoSaut>();

        if (other.gameObject.CompareTag("ProjectilePlayer"))
        {
            VieEnnemi -= player.DegatJoueurDistance;
    
            // Knockback
            rb.linearVelocity = -transform.forward * 10f + Vector3.up * 2f;
    
            if (VieEnnemi <= 0)
            {
                Animation2Mort();
            }
    
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        DeplacementPersoSaut player = FindFirstObjectByType<DeplacementPersoSaut>();
        if (other.gameObject.CompareTag("MeleePlayer"))
        {
            VieEnnemi -= player.DegatJoueurMelee;
    

            rb.linearVelocity = -transform.forward * 6f + Vector3.up * 1.5f;
    
            if (VieEnnemi <= 0)
            {
                Animation2Mort();
            }
        }
    }
    void Animation2Mort()
    {
        if (Random.Range(0, 2) == 1)
        {
            statsJoueur.nombreMunition++;
            statsJoueur.MettreAJourAffichageMunition();
            SoundMunitionGet();
        }
        // Ajoute les points seulement si le ScoreController existe
        ScoreController.AddPoints((int)points);
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
        flame.SetActive(false);
        Destroy(gameObject, 30f);
    }

    IEnumerator IdleSoundBoucle()
    {
        while (true)
        {
            int delay = Random.Range(1, 3); // temps aléatoire entre 6 et 10 secondes
            yield return new WaitForSeconds(delay);

            if (!this.enabled) yield break;
            if (isAttacking) continue;

            int index = Random.Range(0, SonIdle.Length);
            audioSource.pitch = Random.Range(0.25f, 0.85f);
            audioSource.volume = 0.5f;
            audioSource.PlayOneShot(SonIdle[index]);
        }
    }

    public void DeathSound()
    {
        int index = Random.Range(0, SonDeath.Length);
        audioSource.pitch = Random.Range(0.5f, 1f);
        audioSource.volume = 1f;
        audioSource.PlayOneShot(SonDeath[index]);
    }

    public void IncomingAttackSound()
    {
        int index = Random.Range(0, SonIncomingAttack.Length);
        audioSource.pitch = Random.Range(0.75f, 1.25f);
        audioSource.volume = 0.75f;
        audioSource.PlayOneShot(SonIncomingAttack[index]);
    }

    public void RandomSweepAudio()
    {
        int index = Random.Range(0, SonAttaque.Length);
        audioSource.pitch = Random.Range(0.5f, 1f);
        audioSource.volume = 0.5f;
        audioSource.PlayOneShot(SonAttaque[index]);
    }

    public void RandomHeavyImpactAudio()
    {
        int index = Random.Range(0, SonAttaqueLourde.Length);
        audioSource.volume = 1;
        audioSource.pitch = Random.Range(0.5f, 1f);
        audioSource.PlayOneShot(SonAttaqueLourde[index]);
    }

    public void SoundMunitionGet()
    {
        int index = Random.Range(0, sonGetMunition.Length);
        audioSource.pitch = Random.Range(0.5f, 1f);
        audioSource.volume = 1;
        audioSource.spatialBlend = 0;
        audioSource.PlayOneShot(sonGetMunition[index]);
    }

}
