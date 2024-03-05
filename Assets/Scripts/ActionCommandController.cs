using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCommandController : MonoBehaviour
{
    [SerializeField] private GameObject greenLight;
    [SerializeField] private Sprite[] greenLights = new Sprite[2];
    [SerializeField] private GameObject yellowLight;
    [SerializeField] private Sprite[] yellowLights = new Sprite[2];
    [SerializeField] private GameObject RedLight;
    [SerializeField] private Sprite[] redLights = new Sprite[2];
    [SerializeField] private GameObject[] successLight = new GameObject[2];
    [SerializeField] private Sprite[] successLights = new Sprite[3];
    //[SerializeField] private GameObject failureLight;
    [SerializeField] private Animator actionIcon;
    [SerializeField] private BattleController battleController;

    [SerializeField] private GameObject chargeBody;

    private float firstCheck = .5f, secondCheck = 1f, thirdCheck = 1.5f, finalCheck = 2;

    private bool isActive;
    private GameObject child;
    private float counter;

    private bool endAction = false;
    internal bool chainAttack;
    private bool flashing;
    private bool properlyCharged = false;

    public void HoldLeft()
    {
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

    public void ActivateActionCommand(float xPos, float zPos)
    {
        transform.position = new Vector3(xPos, transform.position.y, zPos);
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
        //properlyCharged = false;

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
            //actionIcon.SetTrigger("Reset");
            counter = 0;
        }

        if (counter >= firstCheck)
        {
            //greenLight.SetActive(true);
            greenLight.GetComponent<SpriteRenderer>().sprite = greenLights[1];
        }
        if (counter >= secondCheck)
        {
            yellowLight.GetComponent<SpriteRenderer>().sprite = yellowLights[1];
        }
        if (counter >= thirdCheck)
        {
            RedLight.GetComponent<SpriteRenderer>().sprite = redLights[1];
        }
        if (counter >= finalCheck && !flashing)
        {
            //successLight[0].GetComponent<SpriteRenderer>().sprite = successLights[1];
            StartCoroutine(ChargeFlasher());
        }
        else if (counter < finalCheck)
        {
            //failureLight.SetActive(true);
        }

        if (greenLight.GetComponent<SpriteRenderer>().sprite == greenLights[1] && Input.GetAxisRaw("Horizontal") >= 0)
        {
            //Debug.Log("ended");
            TriggerBattle(properlyCharged);
            StopAllCoroutines();
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

    IEnumerator ChargeFlasher()
    {
        //Debug.Log("flashing started");
        flashing = true;
        properlyCharged = true;
        float flasher = .2f;
        float maxCharge = flasher * 50;

        for (int i = 0; i < maxCharge; i++)
        {
            if (successLight[0].GetComponent<SpriteRenderer>().sprite == successLights[1])
            {
                successLight[0].GetComponent<SpriteRenderer>().sprite = successLights[2];
                successLight[1].GetComponent<SpriteRenderer>().sprite = successLights[2];
            }
            else
            {
                successLight[0].GetComponent<SpriteRenderer>().sprite = successLights[1];
                successLight[1].GetComponent<SpriteRenderer>().sprite = successLights[1];
            }
            yield return new WaitForSeconds(flasher);
        }
        properlyCharged = false;
    }

    private void TriggerBattle(bool bonus)
    {
        battleController.TriggerBattleAction(bonus);
        DeactivateActionCommand();
    }

    private void ResetObjects()
    {
        greenLight.GetComponent<SpriteRenderer>().sprite = greenLights[0];
        yellowLight.GetComponent<SpriteRenderer>().sprite = yellowLights[0];
        RedLight.GetComponent<SpriteRenderer>().sprite = redLights[0];
        successLight[0].GetComponent<SpriteRenderer>().sprite = successLights[0];
        successLight[1].GetComponent<SpriteRenderer>().sprite = successLights[0];

        //greenLight.SetActive(false);
        //yellowLight.SetActive(false);
        //RedLight.SetActive(false);
        //successLight[0].SetActive(false);
        //successLight[1].SetActive(false);
        //failureLight.SetActive(false);
        flashing = false;
        properlyCharged= false;
        //actionIcon.gameObject.SetActive(false);
    }
}