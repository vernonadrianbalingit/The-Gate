using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RupeeManager : MonoBehaviour
{

    public static Text rupeeLabel;
    private static int rupee;
    public static MoneyOnKill k;
    public static int Rupee
    {
        get
        {
            return rupee;
        }
        set
        {
            rupee = value;
            k.addMoney();
            rupeeLabel.GetComponent<Text>().text = "Rupee: " + rupee;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Rupee = 100;
        int r = Rupee;
        Debug.Log(r);
    }
    // so pretty much this script just shows the total amount of Rupees that the player has and gives them their startng rupee for the game.
    // Update is called once per frame
    void Update()
    {
        rupee = 150;
    }
}
