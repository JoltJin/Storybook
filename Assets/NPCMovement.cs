using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.EventSystems.EventTrigger;

public class NPCMovement : MonoBehaviour
{
    //basic controller info
    [SerializeField] private float walkSpeed = 2;
    private NavMeshAgent agent;

    //random location info
    [SerializeField] private float waypointCounter = 1.5f; // cooldown to picking new location

    [SerializeField] private int minAnimLoops = 1, maxAnimLoops = 2;
    private float animCounter = 0;
    private Coroutine nextLocation;
    private bool watchingPlayer = false;
    private Vector3 homeBase;

    private Transform target;

    NPCInteract npcInteract;
    // Start is called before the first frame update
    void Start()
    {
        npcInteract = GetComponent<NPCInteract>();


        NextLocation();
        agent = GetComponent<NavMeshAgent>();
        homeBase = transform.position;
        if (minAnimLoops > maxAnimLoops)
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
        if(PlayerController.isBusy)
        {
            StopAllCoroutines();
            Debug.Log("Locked");
            return;
        }
        if (!watchingPlayer && nextLocation == null && animCounter <= 0)
        {
            NextLocation();
            //nextLocation = StartCoroutine(SetNextLocation());
        }
        else if (watchingPlayer)
        {
            agent.speed = 0;
            agent.SetDestination(transform.position);
        }
        else
        {
            agent.speed = walkSpeed;
        }



        if (MathF.Abs(agent.destination.x - transform.position.x) > 0.01f || MathF.Abs(agent.destination.z - transform.position.z) > 0.01f)
        {
            npcInteract.BasicAnimations(agent.destination.x - transform.position.x, agent.destination.z - transform.position.z);
        }
    }

    private void NextLocation()
    {
        if (Vector3.Distance(transform.position, homeBase) > 1)
        {
            //StartCoroutine(SetNextLocation(homeBase));
            nextLocation = StartCoroutine(SetNextLocation(homeBase));
        }
        else
        {
            Vector2 waypointSet = new Vector2(UnityEngine.Random.Range(-1, 1f), UnityEngine.Random.Range(-1, 1f));
            //StartCoroutine(SetNextLocation(new Vector3(transform.position.x + waypointSet.x, transform.position.y, transform.position.z + waypointSet.y)));
            nextLocation = StartCoroutine(SetNextLocation(new Vector3(transform.position.x + waypointSet.x, transform.position.y, transform.position.z + waypointSet.y)));
        }
    }
    IEnumerator SetNextLocation(Vector3 targetLocation)
    {
        animCounter = 0; //= UnityEngine.Random.Range(minAnimLoops, maxAnimLoops + 1);
        //Vector2 waypointSet = new Vector2(UnityEngine.Random.Range(-1, 1f), UnityEngine.Random.Range(-1, 1f));
        yield return new WaitForSeconds(.001f);

        //if (Vector3.Distance(transform.position, homeBase) > 3)
        //{
        //    agent.SetDestination(homeBase);
        //    Debug.Log(Vector3.Distance(transform.position, homeBase));
        //    //agent.SetDestination(transform.position);
        //}
        //else
        //{
        //    agent.SetDestination(new Vector3(transform.position.x + waypointSet.x, transform.position.y, transform.position.z + waypointSet.y));
        //}
        agent.SetDestination(targetLocation);

        yield return new WaitForSeconds(waypointCounter);
        agent.SetDestination(transform.position);
        yield return new WaitForSeconds(waypointCounter);
        nextLocation = null;
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log("NPC ");
    //    if (collision.transform.tag == "NPC" && collision.transform.gameObject != gameObject)
    //    {
    //        StopAllCoroutines();
    //        Vector3 newLocation = transform.position - collision.transform.transform.position;
    //        Debug.Log("NPC " + collision.transform.name + " " + newLocation.normalized);
    //        nextLocation = StartCoroutine(SetNextLocation(new Vector3(transform.position.x + newLocation.normalized.x * 5, transform.position.y, transform.position.z + newLocation.normalized.z * 5)));
    //    }
    //}
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Player");
            StopAllCoroutines();
            agent.SetDestination(transform.position);
            nextLocation = null;
            watchingPlayer = true;
        }
        else if (other.tag == "NPC" && other.gameObject != gameObject)
        {
            StopAllCoroutines();
            Vector3 newLocation = transform.position - other.transform.position;
            Debug.Log("NPC " + other.name + " " + newLocation.normalized);
            nextLocation = StartCoroutine(SetNextLocation(new Vector3(transform.position.x + newLocation.normalized.x * 5, transform.position.y, transform.position.z + newLocation.normalized.z * 1)));
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            //nextLocation = StartCoroutine(SetNextLocation());
            watchingPlayer = false;
        }
    }
}
