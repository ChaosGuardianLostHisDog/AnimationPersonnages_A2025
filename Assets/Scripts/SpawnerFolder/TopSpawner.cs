using UnityEngine;

public class TopSpawner : MonoBehaviour
{
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

        // Le monstre apparait sur le position du spawner
        Vector3 SpawnPosition = transform.position;

        Instantiate(prefabToSpawn[randomIndex], SpawnPosition, Quaternion.identity);
    }
}
