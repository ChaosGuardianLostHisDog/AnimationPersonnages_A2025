using UnityEngine;
using TMPro;

public class StatsJoueur : MonoBehaviour
{
    [Header("Stats Joueur")]
    public float nombreMunition = 1000;
    public float nombreMunitionMax = 111150;

    [Header("Composants UI")]
    [SerializeField] private TMP_Text MunitionAfficheur;
    [SerializeField] private TMP_Text MunitionAfficheurMax;

    void Start()
    {
        MettreAJourAffichageMunition();
    }

    public void MettreAJourAffichageMunition()
    {
        if (MunitionAfficheur != null)
            MunitionAfficheur.text = nombreMunition.ToString();

        if (MunitionAfficheurMax != null)
            MunitionAfficheurMax.text = "/" + nombreMunitionMax.ToString();
    }
}

