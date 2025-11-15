using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class GameManagerBehavior : MonoBehaviour
{

    public static Text goldLabel;
    private static int gold;
    public static MoneyOnKill m;
    public static int Gold
    {
        get
        {
            return gold;
        }
        set
        {
            gold = value;
            m.addMoney();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Gold = 1000;
        int g = Gold;
        Debug.Log(g);
    }
    // so pretty much this script just shows the total amount of gold that the player has and gives them their startng gold for the game.
    // Update is called once per frame
    void Update()
    {
        Gold = 15000;
    }
}
