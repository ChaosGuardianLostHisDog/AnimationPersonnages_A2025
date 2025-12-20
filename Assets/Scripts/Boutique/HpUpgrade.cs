using UnityEngine;
using UnityEngine.UI;

public class HpUpgrade : MonoBehaviour
{
    // Code grandement inspiré de https://youtu.be/-gP2Pw-PtOU?si=CeqEei50vRF2brqQ
    [Header("UI")]
    [SerializeField] private Image fillAmountNiveauAchat;

    [Header("Achat")]
    [SerializeField] private int coutParAchat = 100;
    [SerializeField] private int achatMax = 3;
    private int niveauActuel;
    public AudioSource audioSource;
    public AudioClip[] AchatEffectuer;
    public AudioClip[] AchatRefuser;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        niveauActuel = PlayerPrefs.GetInt("HpUpgradeLevel", 0); // Ce truc est basiquement un "cookie"
        UpdateUI();
    }

    public void AcheterHP()
    {
        if (niveauActuel >= achatMax || ScoreController.Score < coutParAchat)
        {   // Peut pas, car Level Max ou t'es pauvre
            int indexRefus = Random.Range(0, AchatRefuser.Length);
            audioSource.pitch = 1f;
            audioSource.PlayOneShot(AchatRefuser[indexRefus]);
            return;
        }

        // Retirer l'argent
        ScoreController.AddPoints(-coutParAchat);

        int index = Random.Range(0, AchatEffectuer.Length);
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(AchatEffectuer[index]);

        // Ajouter un niveau
        niveauActuel++;
        PlayerPrefs.SetInt("HpUpgradeLevel", niveauActuel);
        PlayerPrefs.Save();

        UpdateUI();
    }

    void UpdateUI()
    {
        fillAmountNiveauAchat.fillAmount =
            (float)niveauActuel / achatMax;
    }
}
