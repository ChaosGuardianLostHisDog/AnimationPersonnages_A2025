using UnityEngine;

public class MurInvisibleManager : MonoBehaviour
{
    [Header("Layers à configurer")]
    public string playerLayerName = "Player";
    public string enemyLayerName = "Ennemy";
    public string skeletonWarriorLayerName = "skeletonwarrior";
    public string wallLayerName = "MurInvisible";

    void Awake()
    {
        int playerLayer = LayerMask.NameToLayer(playerLayerName);
        int enemyLayer = LayerMask.NameToLayer(enemyLayerName);
        int skeletonWarriorLayer = LayerMask.NameToLayer(skeletonWarriorLayerName);
        int wallLayer = LayerMask.NameToLayer(wallLayerName);

        // Le mur bloque le joueur
        Physics.IgnoreLayerCollision(wallLayer, playerLayer, false);

        // Le mur NE bloque PAS les ennemis
        Physics.IgnoreLayerCollision(wallLayer, enemyLayer, true);
        Physics.IgnoreLayerCollision(wallLayer, skeletonWarriorLayer, true);

        Debug.Log("Layer collisions configurées : Mur bloque le joueur, ennemis passent à travers");
    }
}

