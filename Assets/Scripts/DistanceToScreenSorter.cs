using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(SpriteRenderer) || typeof(ParticleSystem)]
public class DistanceToScreenSorter : MonoBehaviour
{
    private Vector3 distance;
    private Renderer spriteNum;
    private float secToWait = .15f;
    
    // Start is called before the first frame update
    void OnEnable()
    {
        if(GetComponent<ParticleSystem>())
        {
            spriteNum = GetComponent<ParticleSystem>().GetComponent<Renderer>();
        }
        else if(GetComponent<Renderer>()) 
        {
            spriteNum = GetComponent<Renderer>();

        }
        //sprite = GetComponent<SpriteRenderer>();
        spriteNum.sortingOrder = -(int)(Vector3.Distance(transform.position, Camera.main.transform.position) *10); //(int)Vector3.Dot(transform.position, Camera.main.transform.position);
        StartCoroutine(CheckDistanceFromScreen());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator CheckDistanceFromScreen()
    {
        yield return new WaitForSeconds(secToWait);

        if(spriteNum.isVisible)
        {
            spriteNum.sortingOrder = -(int)(Vector3.Distance(transform.position, Camera.main.transform.position) * 10);
        }
        StartCoroutine(CheckDistanceFromScreen());
    }
}
