using UnityEngine;
public class MedicPack : MonoBehaviour
{
    [Header("Vitesse de rotation en degrés par seconde")]
    public float vitesseRotation = 30f;

    [Header("Force du soins")]
    [SerializeField] private StatsJoueur statsJoueur;

    [Header("Audio du Monstre")]
    public AudioSource audioSource;
    public AudioClip[] HealSound;

    void Update()
    {
        // Faire tourner la caméra autour de son axe Y (vertical)
        transform.Rotate(0f, vitesseRotation * Time.deltaTime, 0f, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            JouerSonAmmo();
            Destroy(gameObject);
        }
    }
    
    void JouerSonAmmo()
    {
        statsJoueur.nombreMunition += 5;
        statsJoueur.MettreAJourAffichageMunition();
        int index = Random.Range(0, HealSound.Length);
        AudioSource.PlayClipAtPoint(HealSound[index], transform.position, 1f);
    }

}