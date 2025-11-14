using UnityEngine;
using UnityEngine.AI;

public class AiEnnemi : MonoBehaviour
{

    public float lookRadius = 10f;
    Transform target;
    NavMeshAgent agent;
    Animator anim;
    bool isWalking = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        target = PlayerManager.instance.player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(target.position, transform.position);

        if (distance <= lookRadius)
        {
            agent.SetDestination(target.position);
            // Jouer l'animation du monstre qui cours ver le joueur
            if (!isWalking)
            {
                anim.SetBool("isRunning", true);
            }else
            {
                anim.SetBool("isRunning", false);
            }

            if (distance <= agent.stoppingDistance)
            {
                // Attaquer le joueur
                // Regarder le joueur
                FaceTarget();
                anim.SetTrigger("attackTrigger");

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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
