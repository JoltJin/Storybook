using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BaseFlippingAnimator : MonoBehaviour
{
    [SerializeField] private float minAnimationTime, maxAnimationTime, minWobbleForward, maxWobbleForward;

    [SerializeField] private bool reverse = false;

    private Vector3 origin, parentOrigin;
    // Start is called before the first frame update

    private void OnEnable()
    {
        origin = transform.GetChild(0).rotation.eulerAngles;
        parentOrigin = transform.position;

        minWobbleForward = -minWobbleForward;
        maxWobbleForward = -maxWobbleForward;

        int rotation = 91;

        if (reverse )
        {
            rotation = -rotation;
        }
        transform.rotation = Quaternion.Euler(parentOrigin.x + rotation, parentOrigin.y, parentOrigin.z);

        FlipUp();
    }

    public void SceneTransitionStart()
    {
        //origin = transform.GetChild(0).rotation.eulerAngles;

        //minWobbleForward = -minWobbleForward;
        //maxWobbleForward = -maxWobbleForward;

        //int rotation = 91;

        //if (reverse)
        //{
        //    rotation = -rotation;
        //}
        //transform.rotation = Quaternion.Euler(parentOrigin.x + rotation, parentOrigin.y, parentOrigin.z);



        //StopAllCoroutines();
        //FlipUp();
    }

    public void FlipUp()
    {
        StartCoroutine(RotateObject());
    }

    IEnumerator RotateObject()
    {
        if(GetComponentInChildren<SpriteBillboard>())
        {
            GetComponentInChildren<SpriteBillboard>().applyBillboard = false;
        }
        //transform.rotation = Quaternion.Euler(transform.rotation.x + 91, transform.rotation.y, transform.rotation.z);

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

        if (GetComponentInChildren<SpriteBillboard>())
        {
            GetComponentInChildren<SpriteBillboard>().applyBillboard = true;
        }
    }
}