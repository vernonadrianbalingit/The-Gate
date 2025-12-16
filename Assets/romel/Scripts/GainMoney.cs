using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainMoney : MonoBehaviour
{
    private int currentRound = 0;
    private GameCurrency gameCurrency;
    private float timer = 0f;
    private float delay = 5f;
    
    // Start is called before the first frame update
    void Start()
    {
        gameCurrency = GetComponent<GameCurrency>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        
        // Wait 5 seconds before checking for enemies
        if (timer < delay)
        {
            return;
        }
        
        int enemyCount = FindAllEnemies();
        if (enemyCount == 0 && currentRound == 0)
        {
            currentRound++;
        }
        else if (enemyCount == 0 && currentRound > 1){
            currentRound++;
            gameCurrency.AddMoney(currentRound);
        }
        else if (enemyCount == 0 && currentRound == 1)
        {
            currentRound++;
        }
    }

    public int FindAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Debug.Log("Enemies Remaining: " + enemies.Length);
        return enemies.Length;
    }
}
