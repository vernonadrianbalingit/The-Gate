using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentAdditionTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TowerAiming ta = gameObject.AddComponent<TowerAiming>() as TowerAiming;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
