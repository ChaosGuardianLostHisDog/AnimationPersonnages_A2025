using UnityEngine;
using UnityEngine.SceneManagement;
public class ChangerScene : MonoBehaviour
{
    public void Jouer(int numeroScene)
    {
        SceneManager.LoadScene(numeroScene);
        Time.timeScale = 1f;
    }

}
