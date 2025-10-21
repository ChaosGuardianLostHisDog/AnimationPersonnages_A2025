using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuiviFluide : MonoBehaviour {

    public GameObject Cible;
    public Vector3 Distance;

    public Vector3 AjustementFocus;
    public float Amortissement;
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        Vector3 PositionFinale = Cible.transform.TransformPoint(Distance);
        transform.position = Vector3.Lerp(transform.position, PositionFinale, Amortissement);
        
        
        transform.LookAt(Cible.transform.position + AjustementFocus);
    }
}
