using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
