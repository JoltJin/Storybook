using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAIBase : MonoBehaviour
{
    //basic controller info
    [SerializeField] private float walkSpeed = 2;
    [SerializeField] private float chaseSpeed = 4;
    [SerializeField] Animator flipAnim;
    [SerializeField] Animator spriteAnim;
    private NavMeshAgent agent;

    //random location info
    [SerializeField] Transform[] waypoints = new Transform[1];
    [SerializeField] private float waypointCounter = 1.5f; // cooldown to picking new location
    private int waypointNum = 0;

    [SerializeField]private int minAnimLoops = 1, maxAnimLoops = 2;
    private float animCounter = 0;
    private bool chooseNextPos = true;
    private Coroutine nextLocation;
    private bool isChasing = false;

    private bool facingRight = false;

    private Transform target;

    // Start is called before the first frame update
    void Start()
    {
        nextLocation = StartCoroutine(SetNextLocation());
        agent = GetComponent<NavMeshAgent>();

        if(minAnimLoops > maxAnimLoops)
        {
            int holder = minAnimLoops;
            minAnimLoops = maxAnimLoops;
            maxAnimLoops = holder;
        }
    }

    public void DecreaseAnimCounter()
    {
        animCounter--;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isChasing && nextLocation == null && animCounter <= 0)
        {
            nextLocation = StartCoroutine(SetNextLocation());
        }
        else if(isChasing)
        {
            agent.speed = chaseSpeed;
            agent.SetDestination(target.position);
        }
        else
        {
            agent.speed = walkSpeed;
        }

        if(facingRight && agent.destination.x < transform.position.x || !facingRight && agent.destination.x > transform.position.x)
        {
            FlipAnimation();
        }

        if(agent.destination.x != transform.position.x || agent.destination.z != transform.position.z)
        {
            SpriteAnimation(true);
        }
        else
        {
            SpriteAnimation(false);
        }
    }

    IEnumerator SetNextLocation()
    {
        animCounter = Random.Range(minAnimLoops, maxAnimLoops + 1);
        waypointNum = Random.Range(0, waypoints.Length + 1);
        yield return new WaitForSeconds(.001f);

        if (waypointNum == waypoints.Length)
        {
            agent.SetDestination(transform.position);
        }
        else
        {
            agent.SetDestination(waypoints[waypointNum].position);
        }

        yield return new WaitForSeconds(waypointCounter);
        agent.SetDestination(transform.position);
        nextLocation = null;
            //= StartCoroutine(SetNextLocation());
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.tag == "Player")
        //{
        //    StopAllCoroutines();
        //    nextLocation = null;
        //    isChasing = true;
        //    target = other.transform;
        //}
    }

    public void EnterZone(Collider other)
    {
        if (other.tag == "Player")
        {
            if (facingRight && other.transform.position.x > transform.position.x || !facingRight && other.transform.position.x < transform.position.x)
            {
                StopAllCoroutines();
                nextLocation = null;
                isChasing = true;
                target = other.transform;
            }
        }
    }
    public void LeaveZone(Collider other)
    {
        if (other.tag == "Player")
        {
            nextLocation = StartCoroutine(SetNextLocation());
            isChasing = false;
            target = null;
        }
    }

    

    public void StartBattle(BattleSceneTypes scene)
    {

        Debug.Log("Start Battle");
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if(collision.gameObject.GetComponent<PlayerController>())
    //    {
    //    }
    //}

    private void FlipAnimation()
    {
        if (!facingRight)
        {
            flipAnim.SetBool("TurningLeft", false);
            flipAnim.SetTrigger("Flip");
        }
        else if (facingRight)
        {
            flipAnim.SetBool("TurningLeft", true);
            flipAnim.SetTrigger("Flip");
        }

        facingRight = !facingRight;
    }
    private void SpriteAnimation(bool isWalking)
    {

        spriteAnim.SetBool("isWalking", isWalking);
    }
}
