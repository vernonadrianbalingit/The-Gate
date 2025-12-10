using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfAnimationController : MonoBehaviour
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
            animator.SetBool("Run", false);
            animator.SetBool("Bite Attack", true);
        }
        else
        {
            animator.SetBool("Run", true);
            animator.SetBool("Bite Attack", false);
        }
    }
}
