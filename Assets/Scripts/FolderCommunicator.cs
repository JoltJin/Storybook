using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FolderCommunicator : MonoBehaviour
{
    private Animator anim;

    private Action action;
    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartAnimation(Action finishingAction)
    {
        anim.SetTrigger("Unfold");
        action = finishingAction;
    }

    public void FinishedUnfolding()
    {
        action();
    }
}
