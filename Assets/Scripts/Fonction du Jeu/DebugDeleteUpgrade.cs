using UnityEngine;
using UnityEngine.UI;

public class DebugDeleteUpgrade : MonoBehaviour
{
    void Awake()
    {
        PlayerPrefs.DeleteKey("HpUpgradeLevel");
        PlayerPrefs.DeleteKey("MoveUpgradeLevel");
        PlayerPrefs.DeleteKey("DamageUpgradeLevel");
        PlayerPrefs.DeleteKey("EnduranceUpgradeLevel");
    }
}
