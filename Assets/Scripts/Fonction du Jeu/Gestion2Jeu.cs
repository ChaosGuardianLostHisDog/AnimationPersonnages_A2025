using UnityEngine;
using TMPro;
using System.Collections;

public class Gestion2Jeu : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private Transform Spawners;
    [SerializeField] private GameObject BackgroundNotice;
    [SerializeField] private TMP_Text NoticeActive;
    [SerializeField] private TMP_Text NoticeDesactive;
    [SerializeField] private TMP_Text Temps2grace;

    [Header("Difficulté")]
    [SerializeField] private float reductionSpawnRate = 1f;
    [SerializeField] private float spawnRateMinimum = 0.5f;
    private TopSpawner[] EnfantSpawners;
    private Coroutine countdownCoroutine;
    public AudioSource audioSource;
    public AudioClip[] sonEvent;

    void Start()
    {
        // Récupère tous les spawners enfants
        EnfantSpawners = Spawners.GetComponentsInChildren<TopSpawner>(true);

        // Sécurité UI
        BackgroundNotice.SetActive(true);
        NoticeActive.gameObject.SetActive(false);
        NoticeDesactive.gameObject.SetActive(false);

        // Rebours initial 10 → 0
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);   
        }
        countdownCoroutine = StartCoroutine(ReboursGrace(10));
        

        // Activer spawners après le rebours
        Invoke("ActiverSpawners", 11.5f);
    }

    // ==========================
    // REBOURS AVANT VAGUE
    // ==========================
    IEnumerator ReboursGrace(int duree)
    {
        for (int i = duree; i >= 0; i--)
        {
            Temps2grace.text = "Temps Restant: " + i.ToString() + "s";
            yield return new WaitForSeconds(1f);
        }
        Temps2grace.text = "";
        Temps2grace.gameObject.SetActive(false);
    }

    // ==========================
    // ACTIVER SPAWNERS
    // ==========================
    void ActiverSpawners()
    {
        Spawners.gameObject.SetActive(true);
        JouerSonEvent();
        StartCoroutine(AfficherNotice(NoticeActive));
        // Prochaine augmentation de difficulté
        Invoke("AugmenterDifficulte", 60f);
    }

    // ==========================
    // AUGMENTER DIFFICULTÉ
    // ==========================
    void AugmenterDifficulte()
    {
        foreach (TopSpawner spawner in EnfantSpawners)
        {
            spawner.spawnRate -= reductionSpawnRate;
            spawner.spawnRate = Mathf.Max(spawner.spawnRate, spawnRateMinimum);
        }

        Debug.Log("Difficulté augmentée : spawn plus rapide !");
        DesactiverSpawner();
    }

    // ==========================
    // DÉSACTIVER SPAWNERS
    // ==========================
    void DesactiverSpawner()
    {
        Spawners.gameObject.SetActive(false);
        JouerSonEvent();
        StartCoroutine(AfficherNotice(NoticeDesactive));
        Invoke("ActiverSpawners", 25f);
    }

    // ==========================
    // AFFICHAGE UI (3s)
    // ==========================
    IEnumerator AfficherNotice(TMP_Text notice)
    {
        BackgroundNotice.SetActive(true);
        NoticeActive.gameObject.SetActive(false);
        NoticeDesactive.gameObject.SetActive(false);

        notice.gameObject.SetActive(true);

        yield return new WaitForSeconds(3f);

        notice.gameObject.SetActive(false);
        BackgroundNotice.SetActive(false);
    }

    void JouerSonEvent()
    {
        int index = Random.Range(0, sonEvent.Length);
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(sonEvent[index]);
    }
}

