using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
- Updates npc positon on nav mesh
*/

public class FollowPath : MonoBehaviour
{
    public GameObject wall;
    private UnityEngine.AI.NavMeshAgent agent;
    private Vector3 position;
    private Transform target;

    // Start is called before the first frame update
    void Start()
    {
        wall = GameObject.FindWithTag("Wall");
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        target = wall.transform;
    }

    void Update()
    {
        position = new Vector3(target.position.x, transform.position.y, target.position.z);
        agent.SetDestination(position);
    }
}
