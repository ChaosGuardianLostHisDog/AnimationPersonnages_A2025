using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;

public class AiVehicule: MonoBehaviour
{
    [Header("Info Attack")]
    public GameObject ExplosionParticule;

    [Header("Ennemi Stats")]
    public float VieEnnemi;
    public bool isDestroyed = false;
    Rigidbody rb;
    MeshRenderer mesh;

    [Header("Info Explosion Mort")]
    public float AoEDistance = 7.5f;         // rayon de l'explosion
    public float DegatExplosionMax = 250f;    // dégâts max au centre
    public LayerMask explosionLayerMask = ~0;  // qui peut être touché (assignable dans l'inspector)

    [Header("Audio du Monstre")]
    public AudioSource audioSource;
    public AudioSource audioSource4Explosion;
    public AudioClip[] SonHit;
    public AudioClip[] SonExplosion;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("ProjectilePlayer"))
        {
            VieEnnemi -= 50;
            rb.linearVelocity = -transform.forward * 0.1f + Vector3.up * 2f;
            damageSound();
            if (VieEnnemi <= 0 && isDestroyed == false)
            {
                Animation2Mort();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("MeleePlayer") || other.gameObject.CompareTag("DeathPoint"))
        {

            VieEnnemi -= 20;
            damageSound();
            if (VieEnnemi <= 0 && isDestroyed == false)
            {
                Animation2Mort();
            }

        }
    }

    void Animation2Mort()
    {
        ExplosionParticule.SetActive(true);
        BOOM();

        DarkenOnDeath(0.4f); // Assombrit la texture (0.4 = 60% plus sombre)

        isDestroyed = true;
        this.enabled = false;
        Destroy(gameObject, 5f);
    }

    public void damageSound()
    {
        int index = UnityEngine.Random.Range(0, SonHit.Length);
        audioSource.volume = 1;
        audioSource.spatialBlend = 0.3f;
        audioSource.pitch = UnityEngine.Random.Range(0.5f, 1f);
        audioSource.PlayOneShot(SonHit[index]);
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

    void DarkenOnDeath(float factor = 0.4f)
    {
        if (mesh == null || mesh.material == null) return;

        // Instancie un matériel unique pour cet objet (évite d'assombrir TOUS les ennemis)
        Material mat = mesh.material;

        if (mat.HasProperty("_Color"))
        {
            Color c = mat.GetColor("_Color");
            c *= factor;
            c.a = 1f;
            mat.SetColor("_Color", c);
        }
        else if (mat.HasProperty("_BaseColor"))
        {
            Color c = mat.GetColor("_BaseColor");
            c *= factor;
            c.a = 1f;
            mat.SetColor("_BaseColor", c);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AoEDistance);
    }
}