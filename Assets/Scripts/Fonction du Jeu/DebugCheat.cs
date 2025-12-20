using UnityEngine;

public class DebugCheat : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKey(KeyCode.O) && Input.GetKeyDown(KeyCode.P))
        {
            ScoreController.AddPoints(4000);
            Debug.Log("CHEAT ACTIVÉ : +4k argent");
        }

        if (Input.GetKey(KeyCode.L) && Input.GetKeyDown(KeyCode.K))
        {
            PlayerPrefs.DeleteKey("HpUpgradeLevel");
            PlayerPrefs.DeleteKey("MoveUpgradeLevel");
            PlayerPrefs.DeleteKey("DamageUpgradeLevel");
            PlayerPrefs.DeleteKey("EnduranceUpgradeLevel");
        }
    }
}
