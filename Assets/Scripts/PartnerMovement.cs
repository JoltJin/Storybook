using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PartnerMovement : MonoBehaviour
{
    [SerializeField] private float baseSpeed = 2.0f;
    [SerializeField] private float maxSpeed = 4.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float distanceGoal = 1f;
    [SerializeField] Animator flipAnim;
    [SerializeField] Animator spriteAnim;

    private Transform playerBody;
    private PlayerController player;
    private Rigidbody rb;
    
    private NavMeshAgent agent;

    private bool facingRight = true;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        playerBody = player.gameObject.transform;

        //Physics.IgnoreCollision(GetComponent<Collider>(), player.GetComponent<Collider>());

        //rb = GetComponent<Rigidbody>();

        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        FollowCharacter();
    }

    private void FollowCharacter()
    {
        // default of being left behind player
        Vector3 aimSpot = Vector3.left * distanceGoal;

        // flips if player is facing left
        if (!player.FacingRight)
        {
            aimSpot *= -1;
        }

        //to have the character always try to be behind the player when passing by
        aimSpot += new Vector3(0, 0, .1f);

        float dist = Vector3.Distance(playerBody.position, transform.position);
        float actualSpeed = baseSpeed;

        if(dist > 2.5f)
        {
            actualSpeed = maxSpeed;
        }

        agent.speed = actualSpeed;

        if(player.IsMoving)
        {
            if (transform.position != playerBody.position + aimSpot * 1.5f)
            {
                //Vector3 moveTo = Vector3.MoveTowards(transform.position, playerBody.position + aimSpot, actualSpeed * Time.fixedDeltaTime);
                //Vector3 goalPos = playerBody.transform.position + aimSpot;

                agent.SetDestination(playerBody.position + aimSpot);
                //rb.MovePosition(moveTo);
                SpriteAnimation(true);
            }
        }
        else
        {
            if (player.FacingRight && transform.position.x > (playerBody.position.x + aimSpot.x) || !player.FacingRight && transform.position.x < (playerBody.position.x + aimSpot.x)
                || dist > 1.2f || dist < 0.8f)
            {
                agent.SetDestination(playerBody.position + aimSpot);
                SpriteAnimation(true);
            }
            else
            {
                agent.SetDestination(transform.position);
                SpriteAnimation(false);
            }

        }
        dist = Mathf.Abs(playerBody.position.x - transform.position.x);

        if (dist < 0.2f)
        {
            FlipAnimation(transform.position.x - playerBody.position.x);
        }
    }

    private void FlipAnimation(float direction)
    {
        if (direction > 0)
        {
            if (!facingRight)
            {
                flipAnim.SetBool("TurningLeft", false);
                flipAnim.SetTrigger("Flip");
            }
        }
        else if (direction < 0)
        {
            if (facingRight)
            {
                flipAnim.SetBool("TurningLeft", true);
                flipAnim.SetTrigger("Flip");
            }
        }
    }
    private void SpriteAnimation(bool isWalking)
    {

        spriteAnim.SetBool("isWalking", isWalking);
    }
}