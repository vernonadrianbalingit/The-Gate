using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SpawnLogic : MonoBehaviour
{
    public int currentRound = 1;
    private GameObject[] spawnPoints;
    private bool isSpawning = false; 

    public GameObject WolfPrefab;
    public GameObject BeePrefab;
    public GameObject GuardPrefab;

    public GameObject EnemyCountText;
    public float spawnDelay = 2f; 

    void Start()
    {
        spawnPoints = FindAllSpawnPoints();
    }

    void Update()
    {
        IncrementRound();
        UpdateEnemyCountUI();
    }
  
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
   
        GameObject[] creatures = GameObject.FindGameObjectsWithTag("Enemy");
        return creatures.Length > 0;
    }

    private IEnumerator SpawnCreaturesForCurrentRound()
    {
        isSpawning = true;
        int creaturesToSpawn = currentRound * 3; 
        for (int i = 0; i < creaturesToSpawn; i++)
        {
            SpawnCreatureAtRandomPoint();
            yield return new WaitForSeconds(spawnDelay); 
        }
        isSpawning = false;
    }

    private void SpawnCreatureAtRandomPoint()
    {
 
        int randomIndex = Random.Range(0, spawnPoints.Length);
        GameObject spawnPoint = spawnPoints[randomIndex];

   
        GameObject creaturePrefab = null;
        if (spawnPoint.CompareTag("LandSpawner"))
        {
            if (Random.Range(0, 2) == 0)
            {
                creaturePrefab = WolfPrefab; 
            }
            else
            {
                creaturePrefab = GuardPrefab; 
            }
        }
        else if (spawnPoint.CompareTag("AirSpawner"))
        {
            creaturePrefab = BeePrefab;
        }

        if (creaturePrefab != null)
        {
            Instantiate(creaturePrefab, spawnPoint.transform.position, Quaternion.identity);
        }

    }

    private void UpdateEnemyCountUI()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        EnemyCountText.GetComponent<TextMeshProUGUI>().text = "Enemies Remaining: " + enemies.Length;
    }
}
