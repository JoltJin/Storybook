using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteract : MonoBehaviour, IInterface
{
    [SerializeField] private string text = "";
    [SerializeField] private textEnder ender;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// 
    /// 
    /// </summary>
    /// <param name="trans">Position to determine facing direction</param>
    public void Interact(Transform trans)
    {
        TextboxController.Instance.SetPosition(transform, 1, GetComponentInParent<CharacterAnimator>());
        TextboxController.Instance.SetText(text, ender);
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
