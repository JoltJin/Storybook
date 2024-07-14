using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDistanceFromPlayer : MonoBehaviour
{
    private float distanceFromPlayer;
    private Transform player;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>().transform;
        distanceFromPlayer =player.position.z - transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, player.position.z - distanceFromPlayer);
    }
}
