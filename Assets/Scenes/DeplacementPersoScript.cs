using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeplacementPersoScript : MonoBehaviour
{

   /*#################################################
    -- variables publiques à définir dans l'inspecteur
   #################################################*/
    public GameObject cameraPerso; //la caméra qui doit suivre le perso. À définir dans l'inspecteur
    public Vector3 distanceCamera; // la distance à laquelle la caméra doit suivre le perso.
    public float vitesseDeplacementPerso; // vitesse de déplacement du personnage
    public float vitesseRotationPerso;// vitesse de rotation du personnage lorsque la souris se déplace horizontalement
    public bool curseurLock; // On vérouille ou non le curseur.

   

    void Start()
    {
        // Active le verrouillage du curseur seulement si l'option est cochée. Utilie seulement avec la caméra simple "rotate".
        if (curseurLock) Cursor.lockState = CursorLockMode.Locked;
        
        
    }


    /*
     * Fonction FixeUpdate pour le déplacement du perso, la gestion des animations du perso et l'ajustement de la 
     * position et de la rotation de la caméra
     */
    void FixedUpdate()
    {
        /* ### déplacement du perso ###
        On commence par récupérer les valeurs de l'axe vertical et de l'axe horizontal. 
        GetAxisRaw renvoie une valeur soit de -1, 0 ou 1. Aucune progression comme avec GetAxis.*/
        float axeH = Input.GetAxisRaw("Horizontal");
        float axeV = Input.GetAxisRaw("Vertical");
        /*
         **** déplacement du personnage --> partie à compléter ****
         *
        On modifie la vélocité du personnage en lui donnant un nouveau vector 3 composé de la valeur des axes vertical et
        horizontal. Ce vecteur doit être normalisé (pour éviter que le personnage se déplace plus vite en diagonale.
        On multiplie ce vecteur par la variable vitesseDeplacementPerso pour pouvoir ajuste la vitesse de déplacement.*/

        Vector3 vecteurDeplacement = new Vector3(axeH, 0f, axeV).normalized * vitesseDeplacementPerso;
        GetComponent<Rigidbody>().linearVelocity = vecteurDeplacement;
        //----------------------------------------------------------------------------------------------

        /* ### rotation du personnage simple ###
         * on tourne le personnage en fonctione du déplacement horizontal de la souris. On mutliplie par la variable
         * vitesseRotationPerso pour pouvoir contrôler la vitesse de rotation*/
        float tourne = Input.GetAxis("Mouse X") * vitesseRotationPerso;
        transform.Rotate(0f, tourne, 0f);

        /* ### rotation du personnage complexe, mais plus précise pour le tir. Activer cette fonction pour qu'elle s'exécute
         * et mettre en commentaire la rotation simple.*/
        //TournePersonnage();

        //----------------------------------------------------------------------------------------------

        /* 
         **** gestion des animations --> partie à compléter ****
         *
         * Activation de l'animation de marche si la magnitude de la vélocité est plus grande que 0. Si ce n'est pas le cas
         * on active l'animation de repos. GetComponent<Rigidbody>().velocity.magnitude...
         * 
        /* 


        //----------------------------------------------------------------------------------------------

        /* positionnement de la caméra qui suit le joueur. On place la caméra à la position actuelle du joueur en ajoutant
         * une distance (variable distanceCamera). On fait aussi un LookAt pour s'assurer que la caméra regarde vers le joueur*/
        cameraPerso.transform.position = transform.position + distanceCamera;
        cameraPerso.transform.LookAt(transform.position);

        //----------------------------------------------------------------------------------------------
    }

    /*
     * Fonction TournePersonnage qui permet de faire pivoter le personnage en fonction de la position de la caméra et du curseur
     * de la souris.
     */
    void TournePersonnage()
    {
        /*crée un rayon à partir de la caméra vers l’avant à la position de la souris. Le rayon est mémorisé dans la variable
         * locale camRay (variable de type Ray)*/
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        // variable locale infoCollision : contiendra les infos retournées par le Raycast sur l’objet touché 
        RaycastHit infoCollision;

        /* lance un rayon de 5000 unités à partir du rayon crée précédemment, vérifie seulement la collision avec le plancher en
         * spécifiant un LayerMask. Le plancher doit avoir un layerMask (exemple:“Plancher”) assigné dans l’inspecteur.
         * La commande RayCast renvoie True ou False (true si le plancher est touché par le rayon dans ce cas). Il est donc possible
         * de l'utiliser dans un if.
         * 
         * Dans l'ordre, les paramètres du RayCast sont :
         * 1- le point d'origine du rayon
         * 2- la direction dans lequel le rayon doit être tracé.
         * 3- la variable qui récoltera les informations s'il y a un contact du rayon. Ne pas oublier le mot clé "out".
         * 4- la longueur du rayon tracé
         * 5- le layerMask qui permet de tenir compte seulement des objets qui sont sur ce layer.*/

        if (Physics.Raycast(camRay.origin, camRay.direction, out infoCollision, 5000, LayerMask.GetMask("plancher")))
        {
            // si le rayon frappe le plancher...
            // le personnage regarde vers le point de contact (là ou le rayon à touché le plancher)
            transform.LookAt(infoCollision.point);

            /* Ici, on s'assure que le personnage tourne seulement sur son Axe Y en mettant ses rotations X et Z à 0. Pour changer
             * ces valeurs par programmation, il faut changer la propriété localEuleurAngles.*/
            Vector3 rotationActuelle = transform.localEulerAngles;
            rotationActuelle.x = 0f;
            rotationActuelle.z = 0f;
            transform.localEulerAngles = rotationActuelle;
        }
        //outil de déboggage pour visualiser le rayon dans l'onglet scene
        Debug.DrawRay(camRay.origin, camRay.direction * 100, Color.yellow);   
    }
}
