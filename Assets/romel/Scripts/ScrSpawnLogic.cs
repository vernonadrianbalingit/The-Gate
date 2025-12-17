using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnLogic : MonoBehaviour
{
    public int currentRound = 1;
    private GameObject[] spawnPoints;
    private bool isSpawning = false; // Track if currently spawning

    public GameObject WolfPrefab;
    public GameObject BeePrefab;
    public GameObject GuardPrefab;
    public float spawnDelay = 2f; // Delay between each spawn in seconds

    // Start is called before the first frame update
    void Start()
    {
        spawnPoints = FindAllSpawnPoints();
    }

    // Update is called once per frame
    void Update()
    {
        IncrementRound();
        
    }
    // find all spwan points in the scene
    private GameObject[] FindAllSpawnPoints()
    {
        GameObject[] landSpawners = GameObject.FindGameObjectsWithTag("LandSpawner");
        GameObject[] airSpawners = GameObject.FindGameObjectsWithTag("AirSpawner");
        GameObject[] spawnPoints = landSpawners.Concat(airSpawners).ToArray();
        return spawnPoints;
    }

    private void IncrementRound()
    {
        if (CreaturesInPlay() || isSpawning)
        {
            return;
        }
        StartCoroutine(SpawnCreaturesForCurrentRound());
        currentRound++;
    }

    private bool CreaturesInPlay()
    {
        // check if there are any creatures in play
        GameObject[] creatures = GameObject.FindGameObjectsWithTag("Enemy");
        return creatures.Length > 0;
    }

    private IEnumerator SpawnCreaturesForCurrentRound()
    {
        isSpawning = true;
        int creaturesToSpawn = currentRound * 2; // increase the number of creatures each round
        for (int i = 0; i < creaturesToSpawn; i++)
        {
            SpawnCreatureAtRandomPoint();
            yield return new WaitForSeconds(spawnDelay); // Wait between spawns
        }
        isSpawning = false;
    }

    private void SpawnCreatureAtRandomPoint()
    {
        // Select a random spawn point
        int randomIndex = Random.Range(0, spawnPoints.Length);
        GameObject spawnPoint = spawnPoints[randomIndex];

        // Determine creature type based on spawn point tag
        GameObject creaturePrefab = null;
        if (spawnPoint.CompareTag("LandSpawner"))
        {
            if (Random.Range(0, 2) == 0)
            {
                creaturePrefab = WolfPrefab; // Land creature type 1
            }
            else
            {
                creaturePrefab = GuardPrefab; // Land creature type 2
            }
        }
        else if (spawnPoint.CompareTag("AirSpawner"))
        {
            creaturePrefab = BeePrefab; // Air creature
        }

        // Instantiate the creature at the spawn point
        if (creaturePrefab != null)
        {
            Instantiate(creaturePrefab, spawnPoint.transform.position, Quaternion.identity);
        }

    }
}
