using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyManagerV2 : MonoBehaviour
{

    private GameManagerBehavior gameManager;
    
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManagerBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
