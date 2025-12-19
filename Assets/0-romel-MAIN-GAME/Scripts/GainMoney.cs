using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
- Manages money gain after round completion
- Sets bool when enemies are spawned
- Awards money when all enemies are defeated
- Increases money reward based on current round
*/

public class GainMoney : MonoBehaviour
{
    private int currentRound = 1;
    private bool enemiesSpawned = false;
    private GameCurrency gameCurrency;

    void Start()
    {
        gameCurrency = GetComponent<GameCurrency>();
    }


    void FixedUpdate()
    {
        int enemyCount = FindAllEnemies();
        if (enemyCount > 0 && !enemiesSpawned)
        {
            enemiesSpawned = true;
        }

        else if (enemyCount == 0 && enemiesSpawned)
        {
            if (currentRound > 0)
            {
                int moneyToAdd = currentRound + 2;
                gameCurrency.AddMoney(moneyToAdd);
            }
            
            currentRound++;
            enemiesSpawned = false; 
        }
    }

    public int FindAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Debug.Log("Enemies Remaining: " + enemies.Length);
        return enemies.Length;
    }
}
