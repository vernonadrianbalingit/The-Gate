using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DirectedAgent : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject enemy;

    public void getAgent()
    {
        enemy =  GameObject.FindGameObjectWithTag("enemy");
        agent = enemy.GetComponent<NavMeshAgent>();

    }

    public void MoveToLocation(Vector3 targetPoint)
    {
        agent.destination = targetPoint;
        agent.isStopped = false;
    }

    
}
