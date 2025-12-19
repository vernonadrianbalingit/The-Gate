using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeController : MonoBehaviour
{

    /*
    - Finds closest tower and goes to it to attack
    - if no tower, it attacks the castle
    - handles animation states
    - Corrects position when attacking tower
    */

    private Animator animator;

    private Transform target;
    private bool isLocked = false;
    private Vector3 lockedPosition;

    void Start()
    {
        animator = GetComponent<Animator>();
        target = FindClosestTower();
    }

    void FixedUpdate()
    {
        target = FindClosestTower();
        if (target == null) return;

        Vector3 targetPosition = target.position;
        Camera towerCamera = target.GetComponentInChildren<Camera>();
        if (towerCamera != null)
        {
            targetPosition = towerCamera.transform.position + Vector3.up * -2f;
        }
        else
        {
            targetPosition = target.position + Vector3.up * -2f;
        }

        if (target != null && Vector3.Distance(transform.position, targetPosition) >= 3f)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            
            if (!isLocked)
            {
                transform.position += direction * Time.fixedDeltaTime * 2f; // Move towards the target
            }

            animator.SetBool("Fly Forward", true);
        }
        else
        {
            animator.SetBool("Fly Forward", false);
        }

        if (Vector3.Distance(transform.position, targetPosition) < 3f)
        {
            // Lock position when in attack range
            if (!isLocked)
            {
                isLocked = true;
                lockedPosition = transform.position;
            }
            
            // Keep bee at locked position
            transform.position = lockedPosition;
            
            // Attack logic here
            animator.SetBool("Sting Attack", true);
            animator.SetBool("Fly Forward", false);
        }
        else
        {
            // Unlock when out of range
            isLocked = false;
            
            animator.SetBool("Sting Attack", false);
            if (target != null)
            {
                animator.SetBool("Fly Forward", true);
            }
        }
    }
    void Update()
    {
        if (target != null)
        {
            Vector3 targetPosition = target.position;
            Camera towerCamera = target.GetComponentInChildren<Camera>();
            if (towerCamera != null)
            {
                targetPosition = towerCamera.transform.position + Vector3.up * -2f;
            }
            else
            {
                targetPosition = target.position + Vector3.up * -2f;
            }
            
            Vector3 direction = (targetPosition - transform.position).normalized;
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
