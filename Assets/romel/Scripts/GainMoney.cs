using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainMoney : MonoBehaviour
{
    private int currentRound = 1;
    private bool enemiesSpawned = false;
    private GameCurrency gameCurrency;
    // Start is called before the first frame update
    void Start()
    {
        gameCurrency = GetComponent<GameCurrency>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int enemyCount = FindAllEnemies();
        
        // Enemies are present, mark that the wave has started
        if (enemyCount > 0)
        {
            if (!enemiesSpawned)
            {
                enemiesSpawned = true;
                Debug.Log("Enemies detected. Round " + currentRound + " in progress.");
            }
        }
        // All enemies are dead and a wave was active
        else if (enemyCount == 0 && enemiesSpawned)
        {
            // Only give money after round 1
            if (currentRound > 0)
            {
                int moneyToAdd = currentRound + 2;
                gameCurrency.AddMoney(moneyToAdd);
                Debug.Log("Added " + moneyToAdd + " currency for completing round " + currentRound);
            }
            else
            {
                Debug.Log("Round " + currentRound + " completed. No money for first round.");
            }
            
            currentRound++;
            enemiesSpawned = false; // Reset for next wave
        }
    }

    public int FindAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Debug.Log("Enemies Remaining: " + enemies.Length);
        return enemies.Length;
        
    }
}
