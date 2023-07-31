using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCommandController : MonoBehaviour
{
    [SerializeField] private GameObject greenLight;
    [SerializeField] private GameObject yellowLight;
    [SerializeField] private GameObject RedLight;
    [SerializeField] private GameObject successLight;
    [SerializeField] private GameObject failureLight;
    [SerializeField] private Animator actionIcon;
    [SerializeField] private BattleController battleController;

    [SerializeField] private GameObject chargeBody;

    private float firstCheck = .5f, secondCheck = 1f, thirdCheck = 1.5f, finalCheck = 2;

    private bool isActive;
    private GameObject child;
    private float counter;

    private bool endAction = false;
    internal bool chainAttack;

    public void HoldLeft()
    {
        Debug.Log("Hold Left");
        GetComponent<Animator>().SetBool("DisplayAction", true);
        chargeBody.SetActive(true);
        child.SetActive(true);
        actionIcon.SetTrigger("Left Press");
        isActive = true;
    }

    public void TimedA()
    {
        Debug.Log("timed A");
        GetComponent<Animator>().SetBool("DisplayAction", true);
        child.SetActive(true);
        actionIcon.SetTrigger("A Press");
        isActive = true;
    }

    public void ActivatePressed()
    {
        actionIcon.SetBool("Is Pressed", true);
    }

    public void DeactivatePressed()
    {
        actionIcon.SetBool("Is Pressed", false);
    }

    // Start is called before the first frame update
    void Start()
    {
        actionIcon.SetTrigger("Left Press");
        ResetObjects();
        child = transform.GetChild(0).gameObject;
        DeactivateActionCommand();
    }

    public void ActivateActionCommand(float xPos)
    {
        transform.position = new Vector3(xPos, transform.position.y, transform.position.z);
    }

    public void DeactivateActionCommand()
    {
        GetComponent<Animator>().SetBool("DisplayAction", false);
        isActive = false;
        chargeBody.SetActive(false);
        child.SetActive(false);
    }

    public void TurnOffObject()
    {
        ResetObjects();
        child.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive)
        {
            return;
        }

        if (Input.GetAxisRaw("Vertical") < 0)
        {
            ResetObjects();
        }

        HoldLeftBaseLighter();

        if(chainAttack)
        {
            TapButtonCommand();

        }
    }

    private void TapButtonCommand()
    {
        if(Input.GetButtonDown("Action1"))
        {
            battleController.PlayChainAttack();
        }
    }

    private void HoldLeftBaseLighter()
    {
        
        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            counter += Time.deltaTime;

            if (!actionIcon.GetBool("Is Pressed"))
            {
                actionIcon.SetBool("Is Pressed", true);
            }
        }
        else if (Input.GetAxisRaw("Horizontal") >= 0 && actionIcon.GetBool("Is Pressed"))
        {
            actionIcon.SetBool("Is Pressed", false);
            actionIcon.SetTrigger("Reset");
            counter = 0;
        }

        if (counter >= firstCheck)
        {
            greenLight.SetActive(true);
        }
        if (counter >= secondCheck)
        {
            yellowLight.SetActive(true);
        }
        if (counter >= thirdCheck)
        {
            RedLight.SetActive(true);
        }
        if (counter >= finalCheck)
        {
            successLight.SetActive(true);
            endAction = true;
        }
        else if (counter < firstCheck && greenLight.activeInHierarchy && !successLight.activeInHierarchy)
        {
            failureLight.SetActive(true);
            endAction = true;
        }

        if (endAction && Input.GetAxisRaw("Horizontal") >= 0)
        {
            TriggerBattle(successLight.activeInHierarchy);
            //if ()
            //{
            //    TriggerBattle(2);
            //}
            //else
            //{
            //    TriggerBattle(1);
            //}
        }
    }

    private void TriggerBattle(bool bonus)
    {
        battleController.TriggerBattleAction(bonus);
        DeactivateActionCommand();
        endAction = false;
    }

    private void ResetObjects()
    {
        greenLight.SetActive(false);
        yellowLight.SetActive(false);
        RedLight.SetActive(false);
        successLight.SetActive(false);
        failureLight.SetActive(false);
        //actionIcon.gameObject.SetActive(false);
    }
}