using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
- Finds closest gate and goes to it to attack
- if no gate, it attacks the castle
- handles animation states
*/
public class GuardController : MonoBehaviour
{
    private Animator animator;
    private Transform target;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        target = FindClosestGate();
    }

    // Update is called once per frame
    void Update()
    {
        // face target
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }
    
    void FixedUpdate()
    {
        if (target != null && Vector3.Distance(transform.position, target.position) >= 3f)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * Time.fixedDeltaTime * 1.1f; // Move towards the target

            animator.SetBool("Walk Forward", true);
        }
        else
        {
            animator.SetBool("Walk Forward", false);
        }

        if (Vector3.Distance(transform.position, target.position) < 3f)
        {
            // Attack logic here
            animator.SetBool("Attack", true);
            animator.SetBool("Walk Forward", false);
        }
        else
        {
            animator.SetBool("Attack", false);
            if (target != null)
            {
                animator.SetBool("Walk Forward", true);
            }
        }
    }
    private Transform FindClosestGate()
    {
        GameObject[] gates = GameObject.FindGameObjectsWithTag("Gate");
        float closestDistance = Mathf.Infinity;
        Transform closestGate = null;

        foreach (GameObject gate in gates)
        {
            float distance = Vector3.Distance(transform.position, gate.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestGate = gate.transform;
            }
        }

        return closestGate;
    }
}