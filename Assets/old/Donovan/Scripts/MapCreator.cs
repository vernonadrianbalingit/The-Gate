using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreator : MonoBehaviour
{

    private int xSize;
    private int ySize;

    private int[,] map;

    // Start is called before the first frame update
    void Start()
    {
        xSize = ySize = 1;
        map = new int[xSize, ySize];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
