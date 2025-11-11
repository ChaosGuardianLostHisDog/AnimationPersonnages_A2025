using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private GameObject FirstPersonCamera;
    [SerializeField] private GameObject ThirdPersonCamera;
    [SerializeField] private GameObject DeathAngleCamera;
    [SerializeField] private GameObject PlayerBody;
    public static bool isFirstPersonCamera = true;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeCamera(FirstPersonCamera);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeCamera(ThirdPersonCamera);
    }

    public void ChangeCamera(GameObject laCamera)
    {
        // Switcher entre les 2 caméras actuelle et activer la nouvelle

        // Si FirstPersonCamera est activée, le corp du perso doit être désactivé
        if (laCamera == FirstPersonCamera)
        {
            ThirdPersonCamera.SetActive(false);
            PlayerBody.SetActive(false);
             isFirstPersonCamera = true;
        }
        else if (laCamera == ThirdPersonCamera)
        {
            FirstPersonCamera.SetActive(false);
            PlayerBody.SetActive(true);
            isFirstPersonCamera = false;
        }
        laCamera.SetActive(true);
    }

    public void ActivateDeathCamera()
    {
        // Activer la caméra de mort
        FirstPersonCamera.SetActive(false);
        ThirdPersonCamera.SetActive(false);
        DeathAngleCamera.SetActive(true);
        PlayerBody.SetActive(true);

        // Désactiver ce script pour éviter tout conflit
        this.enabled = false;
    }
}
