using UnityEngine;
public class AmmoPack : MonoBehaviour, IGestion2Degats
{
    [Header("Vitesse de rotation en degrés par seconde")]
    public float vitesseRotation = 30f;

    [Header("Force du soins")]
    public float currentDamage;
    public float GetDamage() => currentDamage;

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
            JouerSonHeal();
            Destroy(gameObject);
        }
    }
    
    void JouerSonHeal()
    {
        
        int index = Random.Range(0, HealSound.Length);
        AudioSource.PlayClipAtPoint(HealSound[index], transform.position, 1f);
    }

}