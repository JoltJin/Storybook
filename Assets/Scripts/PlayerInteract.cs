using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private GameObject indicator;

    private float interactRange = .25f;

    IInterface interactable = null;
    // Update is called once per frame
    void Update()
    {
            FindInteractableObject();
            if (Input.GetKeyDown(KeyCode.X) && !PlayerController.isBusy && interactable != null)
            {
            //Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);

            //foreach (Collider collider in colliderArray)
            //{
            //    if (collider.TryGetComponent(out IInterface interact))
            //    {
            //        interact.Interact(transform);
            //    }
            //}

            interactable.Interact(transform);
        }
    }

    private void ShowIndicator(Transform trans, float indicatorHeight)
    {
        indicator.transform.position = trans.position + Vector3.up * indicatorHeight;
        indicator.SetActive(true);
    }

    private void HideIndicator()
    {
        indicator.SetActive(false);
        interactable = null;
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

        if (closestInteract != null && !PlayerController.isBusy)
        {
            ShowIndicator(closestInteract.GetTransform(), closestInteract.GetIndicatorHeight());
            interactable = closestInteract;
        }
        else
        {
            HideIndicator();
        }
    }
}
