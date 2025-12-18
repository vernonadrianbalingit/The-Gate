using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfAnimationController : MonoBehaviour
{
    private Animator animator;
    private bool isLocked = false;
    private Vector3 lockedPosition;

    public Transform target;

    private FollowPath followPathScript;

    private bool isAttacking = false;

    public float attackDistance = 5.0f;

    public GameObject CityHealthObject;
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindWithTag("Wall").transform;
        animator = GetComponent<Animator>();
        followPathScript = GetComponent<FollowPath>();
        CityHealthObject = GameObject.FindGameObjectWithTag("CityHealth");
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
            animator.SetBool("Run", false);
            animator.SetBool("Bite Attack", true);

            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            animator.SetBool("Run", true);
            animator.SetBool("Bite Attack", false);
        }

        if (isAttacking)
        {
            // Additional logic for attacking can be added here
            animator.SetBool("Run", false);
            animator.SetBool("Bite Attack", true);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
            ScrCityHealth CityHealth = CityHealthObject.GetComponent<ScrCityHealth>();
            //CityHealth.cityHealth -= 100;
        }
    }
}
