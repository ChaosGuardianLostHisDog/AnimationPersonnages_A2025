using Unity.VisualScripting;
using UnityEngine;

public class DeathAngleCamera : MonoBehaviour
{
    [SerializeField] private GameObject objetCiblee;
    [SerializeField] private Vector3 Distance;

    // Update is called once per frame
    void Update()
    {
        transform.position = objetCiblee.transform.position + Distance;
        transform.LookAt(objetCiblee.transform);
        
    }
}
