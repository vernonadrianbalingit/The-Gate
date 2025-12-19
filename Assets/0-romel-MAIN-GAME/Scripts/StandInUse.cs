using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
- Simple script to track if a turret stand is in use
- avoids overlapping turrets on the same stand
*/

public class StandInUse : MonoBehaviour
{
    private bool inUse = false;

    public bool IsInUse()
    {
        return inUse;
    }

    public void SetInUse(bool value)
    {
        inUse = value;
    }
}
