using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatsJoueur : MonoBehaviour
{
    [Header("Stats Joueur")]
    public float nombreMunition;
    public float nombreMunitionMax = 50f; // MAX = 50

    [Header("Composants UI Munition")]
    [SerializeField] private TMP_Text MunitionAfficheur;
    [SerializeField] private TMP_Text MunitionAfficheurMax;

    [Header("Composants UI Out Of Bound")]
    [SerializeField] private TMP_Text AfficheurOoBText;
    [SerializeField] private Image AfficheurOoBScreenDisplay;

    [Header("Out Of Bound Settings")]
    [SerializeField] private float tempsMaxOoB = 5f;

    void Start()
    {
        nombreMunition = Mathf.Clamp(nombreMunition, 0f, nombreMunitionMax);
        MettreAJourAffichageMunition();
        CacherOoB();
    }

    public void MettreAJourAffichageMunition()
    {
        // Clamp SÉCURITÉ
        nombreMunition = Mathf.Clamp(nombreMunition, 0f, nombreMunitionMax);

        MunitionAfficheur.text = nombreMunition.ToString();
        MunitionAfficheurMax.text = "/" + nombreMunitionMax.ToString();
    }

    // ===== OUT OF BOUND UI =====

    public void AfficherOoB(float tempsRestant)
    {
        float ratio = 1f - Mathf.Clamp01(tempsRestant / tempsMaxOoB);

        if (AfficheurOoBText != null)
        {
            AfficheurOoBText.gameObject.SetActive(true);
            AfficheurOoBText.text = $"REVENIR EN ZONE : {tempsRestant:F1}s";
        }

        if (AfficheurOoBScreenDisplay != null)
        {
            AfficheurOoBScreenDisplay.gameObject.SetActive(true);

            Color c = AfficheurOoBScreenDisplay.color;
            c.r = 1f;
            c.g = 0f;
            c.b = 0f;
            c.a = ratio;
            AfficheurOoBScreenDisplay.color = c;
        }
    }

    public void CacherOoB()
    {
        if (AfficheurOoBText != null)
            AfficheurOoBText.gameObject.SetActive(false);

        if (AfficheurOoBScreenDisplay != null)
        {
            Color c = AfficheurOoBScreenDisplay.color;
            c.a = 0f;
            AfficheurOoBScreenDisplay.color = c;
            AfficheurOoBScreenDisplay.gameObject.SetActive(false);
        }
    }
}
