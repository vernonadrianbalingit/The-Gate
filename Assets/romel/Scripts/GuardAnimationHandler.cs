using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
- Simple animation handler for Guard NPCs.
- Walking animation until within a certain distance from target, then switch to attack animation.
*/

public class GuardAnimationHandler : MonoBehaviour
{

    private Animator animator;
    public Transform target;
    public float attackDistance = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindWithTag("Wall").transform;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget <= attackDistance)
        {
            animator.SetBool("Walk Forward", false);
            animator.SetBool("Attack", true);
        }
        else
        {
            animator.SetBool("Walk Forward", true);
            animator.SetBool("Attack", false);
        }
    }
}
