using UnityEngine;

public class DirectionalSpawner : MonoBehaviour
{
    //CE SCRIPT EST GRANDEMENT INSPIRER DE CETTE VIDÉO - https://youtu.be/IbiwNnOv5So?si=_wMtpz6Megx8LchY
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject[] prefabToSpawn;
    public float spawnRate;
    void Start()
    {
        InvokeRepeating("SpawnPrefab", spawnRate, spawnRate);
    }


    // Update is called once per frame  
    void SpawnPrefab()
    {
                // les prefabs seront ejectés vers la direction selon l'orientation de ce spawner (Selon le Z+ du spawner)
        int randomIndex = Random.Range(0, prefabToSpawn.Length);

        GameObject spawnedPrefab = Instantiate(prefabToSpawn[randomIndex], transform.position, transform.rotation);
        // Les prefabs spawner avancent vers la direction selon l'orientation du spawner
        Rigidbody rb = spawnedPrefab.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = transform.forward * 1000f;
        }
    }
}
