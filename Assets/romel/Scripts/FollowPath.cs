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
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError(name + ": FollowPath requires a NavMeshAgent component. Disabling script.");
            enabled = false;
            return;
        }

        if (wall == null)
        {
            Debug.LogWarning(name + ": 'wall' GameObject is not assigned in the inspector. Assign a target to follow.");
        }
        else
        {
            target = wall.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            // Nothing to follow this frame
            return;
        }

        // Keep the agent at the same Y as this object so it moves on the XZ plane
        position = new Vector3(target.position.x, transform.position.y, target.position.z);

        if (agent == null)
            return;

        // Only set destination when agent is available
        agent.SetDestination(position);
    }
}
