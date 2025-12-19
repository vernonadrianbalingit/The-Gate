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
    private bool isLocked = false;
    private Vector3 lockedPosition;
    private AudioManager audioManager;
    
    public Transform target;

    private FollowPath followPathScript;
    public float attackDistance = 20.0f;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindWithTag("Wall").transform;
        animator = GetComponent<Animator>();
        followPathScript = GetComponent<FollowPath>();
        audioManager = FindObjectOfType<AudioManager>();
        //audioManager.Play("MonsterWalk");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Lock position if triggered by wall
        if (isLocked)
        {
            transform.position = lockedPosition;
            return;
        }
        
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget <= attackDistance)
        {
            animator.SetBool("Walk Forward", false);
            animator.SetBool("Attack", true);
            
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 0, transform.rotation.eulerAngles.z);
        }
        else
        {
            animator.SetBool("Walk Forward", true);
            animator.SetBool("Attack", false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            //Destroy(gameObject);
        }
    }
}
