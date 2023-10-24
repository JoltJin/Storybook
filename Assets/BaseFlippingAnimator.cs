using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BaseFlippingAnimator : MonoBehaviour
{
    [SerializeField] private float minAnimationTime, maxAnimationTime, minWobbleForward, maxWobbleForward;

    private Vector3 origin;
    // Start is called before the first frame update

    private void Start()
    {
        origin = transform.GetChild(0).rotation.eulerAngles;

        minWobbleForward = -minWobbleForward;
        maxWobbleForward = -maxWobbleForward;

        FlipUp();
    }

    public void FlipUp()
    {
        StartCoroutine(RotateObject());
    }

    IEnumerator RotateObject()
    {
        transform.rotation = Quaternion.Euler(91, origin.y, origin.z);

        Quaternion target = Quaternion.Euler(origin);

        Quaternion wobbleForward = Quaternion.Euler( new Vector3(UnityEngine.Random.Range(minWobbleForward, maxWobbleForward), origin.y, origin.z));

        float duration = UnityEngine.Random.Range(minAnimationTime, maxAnimationTime);

        duration = 90 / duration;


        while (transform.GetChild(0).rotation != target)
        {
            transform.GetChild(0).rotation = Quaternion.RotateTowards(transform.GetChild(0).rotation, target, Time.deltaTime * duration);

            yield return new  WaitForEndOfFrame();
        }

        while (transform.GetChild(0).rotation != wobbleForward)
        {
            transform.GetChild(0).rotation = Quaternion.RotateTowards(transform.GetChild(0).rotation, wobbleForward, Time.deltaTime * duration*1.5f);

            yield return new WaitForEndOfFrame();
        }

        while (transform.GetChild(0).rotation != target)
        {
            transform.GetChild(0).rotation = Quaternion.RotateTowards(transform.GetChild(0).rotation, target, Time.deltaTime * duration*1.5f);

            yield return new WaitForEndOfFrame();
        }
    }
}