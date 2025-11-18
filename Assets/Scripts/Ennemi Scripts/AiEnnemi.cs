using System;
using UnityEngine;
using UnityEngine.AI;

public class AiEnnemi : MonoBehaviour
{
    public GameObject AttackPointLeft;
    public GameObject AttackPointRight;

    public float lookRadius = 10f;
    Transform target;
    NavMeshAgent agent;
    Animator anim;
    bool isAttacking = false;
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

            anim.SetBool("isRunning", true);
        }
        else
        {
            anim.SetBool("isRunning", false);
        }
        if (distance <= agent.stoppingDistance && isAttacking == false)
            {
                // Attaquer le joueur
                // Regarder le joueur
            FaceTarget();
            AttackTarget();
            isAttacking = true;

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
        // le zombie ne peux marcher quand il attaque
        anim.SetTrigger("attackTrigger");
        agent.speed = 0f;

        //Activer le collider de l'attaque Point après l'execution de l'animation

        Invoke("ActiveAttackPoint", 0.6f);

        Invoke("ResetAgentSpeed", 1.5f);
        
    }

    void ActiveAttackPoint()
    {
        AttackPointRight.gameObject.SetActive(true);
    }

    void ResetAgentSpeed()
    {
        agent.speed = 3.5f; // Remettre la vitesse par défaut de l'agent
        isAttacking = false;
        AttackPointRight.gameObject.SetActive(false);
    }

        void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
