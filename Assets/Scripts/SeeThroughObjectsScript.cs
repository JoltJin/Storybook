using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeeThroughObjectsScript : MonoBehaviour
{
    private GameObject player;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>().gameObject.GetComponentInChildren<AudioSource>().gameObject;
    }

    void Update()
    {
        float dist = Vector3.Distance(transform.position, player.transform.position);
        Debug.DrawRay(transform.position, transform.forward, Color.red, dist);
        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, dist);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.GetComponentInParent<ObjectTransparencyScript>())
            {
                hit.collider.GetComponentInParent<ObjectTransparencyScript>().FadeOut();
            }
        }
    }
}
