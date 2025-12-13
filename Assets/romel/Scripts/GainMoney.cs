using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainMoney : MonoBehaviour
{
    private int currentRound = 0;
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
        if (enemyCount == 0)
        {
            currentRound++;
            gameCurrency.AddMoney(currentRound);
        }
    }

    public int FindAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Debug.Log("Enemies Remaining: " + enemies.Length);
        return enemies.Length;
        
    }
}
