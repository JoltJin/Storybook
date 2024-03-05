using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private GameObject indicator;

    private float interactRange = .25f;
    // Update is called once per frame
    void Update()
    {
        FindInteractableObject();
        if (Input.GetKeyDown(KeyCode.X) && !PlayerController.isBusy)
        {
            Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);

            foreach(Collider collider in colliderArray) 
            {
                if(collider.TryGetComponent(out IInterface interact))
                {
                    interact.Interact(transform);
                }
            } 
        }
    }

    private void ShowIndicator(Transform trans)
    {
        indicator.transform.position = trans.position + Vector3.up * 1f;
        indicator.SetActive(true);
    }

    private void HideIndicator()
    {
        indicator.SetActive(false);
    }

    public void FindInteractableObject()
    {
        List<IInterface> interactList = new List<IInterface>();
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);

        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent(out IInterface interact))
            {
                interactList.Add(interact);
            }
        }

        IInterface closestInteract = null;

        foreach (IInterface interact in interactList)
        {
            if(closestInteract == null)
            {
                closestInteract = interact;
            }
            else
            {
                if(Vector3.Distance(transform.position, interact.GetTransform().position) < Vector3.Distance(transform.position, closestInteract.GetTransform().position))
                {
                    closestInteract = interact;
                }
            }
        }

        if(closestInteract != null)
        {
            ShowIndicator(closestInteract.GetTransform());
        }
        else
        {
            HideIndicator();
        }
    }
}
