using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
- Wolf animation contoller
- When moving, plays running animation
- When within attack range of target, plays attack animation
*/

public class WolfAnimationController : MonoBehaviour
{
    private Animator animator;

    public Transform target;

    private FollowPath followPathScript;

    private bool isAttacking = false;

    public float attackDistance = 5.0f;

    void Start()
    {
        target = GameObject.FindWithTag("Wall").transform;
        animator = GetComponent<Animator>();
        followPathScript = GetComponent<FollowPath>();
    }

    void FixedUpdate()
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget <= attackDistance)
        {
            animator.SetBool("Run", false);
            animator.SetBool("Bite Attack", true);;
        }
        else
        {
            animator.SetBool("Run", true);
            animator.SetBool("Bite Attack", false);
        }

        if (isAttacking)
        {
            animator.SetBool("Run", false);
            animator.SetBool("Bite Attack", true);
        }
    }
}
