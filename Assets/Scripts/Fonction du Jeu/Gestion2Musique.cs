using System.Collections;
using UnityEngine;

public class Gestion2Musique : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] Musique;

    void Start()
    {
        if (Musique.Length > 0)
        {
            StartCoroutine(BoucleMusique());
        }
    }

    IEnumerator BoucleMusique()
    {
        while (true)
        {
            AudioClip clip = Musique[Random.Range(0, Musique.Length)];

            audioSource.clip = clip;
            audioSource.Play();

            // Attendre la fin exacte du clip
            yield return new WaitForSeconds(clip.length);
        }
    }
}
