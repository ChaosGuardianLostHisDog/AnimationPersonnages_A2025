using UnityEngine;

public class AstuceScreen : MonoBehaviour
{
    public GameObject UIscreenMain;
    public GameObject UIscreenAstuce;
    public bool isActive = false;

    // Fonction pour afficher l'écran d'astuce
    public void AfficherAstuce()
    {
        isActive = true;
        UpdateUI();
    }

    // Fonction pour revenir à l'écran principal
    public void MasquerAstuce()
    {
        isActive = false;
        UpdateUI();
    }

    // Fonction interne qui met à jour les UI
    private void UpdateUI()
    {
        UIscreenMain.SetActive(!isActive);
        UIscreenAstuce.SetActive(isActive);
    }
}
