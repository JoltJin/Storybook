using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteBillboard : MonoBehaviour
{
    [SerializeField] bool freezeXAxis = true;
    [SerializeField] bool freezeYAxis = true;
    [SerializeField] bool freezeZAxis = true;
    [SerializeField] bool isFlipped = false;

    public bool applyBillboard = true;
    // Update is called once per frame
    void LateUpdate()
    {
        if (!applyBillboard)
        {
            return;
        }
        if (!freezeXAxis && !freezeYAxis && !freezeZAxis)
        {
            return;
        }
        Vector3 rotation;
        if(!freezeXAxis)
        {
            rotation.x = Camera.main.transform.rotation.eulerAngles.x;
        }
        else
        {
            rotation.x = 0f;
        }
        if (!freezeYAxis)
        {
            rotation.y = Camera.main.transform.rotation.eulerAngles.y;
        }
        else
        {
            rotation.y = 0f;
        }
        if(!freezeZAxis)
        {
            rotation.z = Camera.main.transform.rotation.eulerAngles.z;
        }
        else
        {
            rotation.z = 0f;
        }

        transform.rotation = Quaternion.Euler(rotation);

        //if (freezeXZAxis)
        //{
        //    transform.rotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);
        //}
        //else
        //{
        //    transform.rotation = Camera.main.transform.rotation;
        //}
    }

    public void FlipSprite()
    {
        if(GetComponentInChildren<SpriteRenderer>())
        {
            isFlipped = !isFlipped;
            GetComponentInChildren<SpriteRenderer>().flipX = isFlipped;
        }
    }
}
