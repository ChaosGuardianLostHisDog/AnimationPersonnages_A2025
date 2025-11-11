using UnityEngine;

public class TopSpawner : MonoBehaviour
{
    //CE SCRIPT EST GRANDEMENT INSPIRER DE CETTE VIDÉO - https://youtu.be/IbiwNnOv5So?si=_wMtpz6Megx8LchY
    public GameObject[] prefabToSpawn;
    public float spawnRate;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating("SpawnPrefab", spawnRate, spawnRate);
    }


    // Update is called once per frame  
    void SpawnPrefab()
    {
        int randomIndex = Random.Range(0, prefabToSpawn.Length);

        Vector3 randomSpawnPosition = new Vector3(
            Random.Range(-20f, 20f),
            30f,
            Random.Range(-20f, 20f)
        );
        Instantiate(prefabToSpawn[randomIndex], randomSpawnPosition, Quaternion.identity);
    }
}
