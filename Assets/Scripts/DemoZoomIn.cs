using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class DemoZoomIn : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private GameObject secondLookAt;
    [SerializeField] private Animator bookAnim;
    //[SerializeField] private Animator glowAnim;
    // Start is called before the first frame update
    void Awake()
    {
        virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition = 0;
        //glowAnim.speed = 0;
        bookAnim.SetTrigger("BookClosedSide");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition += Time.deltaTime * moveSpeed;

        if(virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition >= 0.5f)
        {
            bookAnim.SetTrigger("BookClosedSideGlow");

            //glowAnim.speed = 1;
        }

        if(virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition > 2.5f)
        {
            virtualCamera.LookAt = secondLookAt.transform;
            virtualCamera.Follow = secondLookAt.transform;
        }
    }
}
