using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class temp_listener_disable : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            AudioListener.pause = !AudioListener.pause;
        }
    }
}
