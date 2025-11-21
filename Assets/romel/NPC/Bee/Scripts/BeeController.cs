using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeController : MonoBehaviour
{

    /*
    - Finds closest tower and goes to it to attack
    - if no tower, it attacks the castle
    - handles animation states
    */

    private Animator animator;

    private Transform target;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        target = FindClosestTower();
    }

    void FixedUpdate()
    {
        if (target != null && Vector3.Distance(transform.position, target.position) >= 5f)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * Time.fixedDeltaTime * 2f; // Move towards the target

            animator.SetBool("Fly Forward", true);
        }
        else
        {
            animator.SetBool("Fly Forward", false);
        }

        if (Vector3.Distance(transform.position, target.position) < 5f)
        {
            // Attack logic here
            animator.SetBool("Sting Attack", true);
            animator.SetBool("Fly Forward", false);
        }
        else
        {
            animator.SetBool("Sting Attack", false);
            if (target != null)
            {
                animator.SetBool("Fly Forward", true);
            }
        }
    }
    void Update()
    {
        // constantly look at target
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }
    private Transform FindClosestTower()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");
        float closestDistance = Mathf.Infinity;
        Transform closestTower = null;

        foreach (GameObject tower in towers)
        {
            float distance = Vector3.Distance(transform.position, tower.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTower = tower.transform;
            }
        }

        return closestTower;
    }
}
