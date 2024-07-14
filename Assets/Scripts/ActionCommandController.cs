using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

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
    private bool commandComplete = false;

    private Action flee;

    [SerializeField] public MagicActionCommandController magicAction;
    [System.Serializable]public class MagicActionCommandController
    {
        private enum direction
        {
            Up,
            Down,
            Left,
            Right,
        }

        [SerializeField] private GameObject[] arrows = new GameObject[4];

        //[SerializeField] private
        public GameObject MagicActionObject;
        [SerializeField] private GameObject chainBar;
        [SerializeField] private SpriteRenderer[] individualChain = new SpriteRenderer[5];
        private KeyCode[] inputs = new KeyCode[] { KeyCode.UpArrow, KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow };
        [SerializeField] private Sprite lightRune, darkRune, waterRune, fireRune, natureRune, windRune;
        private int chainPlacement = 0;
        private int inputsCorrect = 0;
        [SerializeField] private GameObject inputChoices;
        [SerializeField] private GameObject[] choice = new GameObject[4];

        private BattleController battleController;

        private bool isPressed = false;

        private Action<int, Status, int, int, int> finishAction;

        private Color fullFaded = new Color(.5f, .5f, .5f, .25f);
        private Color highlightFaded = new Color(.75f, .75f, .75f, .9f);
        private Color correct = Color.white; //new Color(.8f, 1f, .8f, 1f);
        private Color wrong = new Color(.25f, .25f, .25f, 1f);
        private Color invisible = new Color(.5f, .5f, .5f, 0f);


        private Color buttonPressed = new Color(.75f, .75f, .75f, .9f);
        public void Start()
        {
            MagicActionObject.SetActive(false);
            chainPlacement = 0;
            inputsCorrect = 0;

            battleController = FindObjectOfType<BattleController>();
        }

        private void PressButtonVisual(direction direction)
        {
            if(isPressed)
            {
                arrows[(int)direction].transform.GetChild(0).localScale = Vector3.one * 1.25f;
                arrows[(int)direction].GetComponent<SpriteRenderer>().color = buttonPressed;
                arrows[(int)direction].transform.GetChild(0).GetComponent<SpriteRenderer>().color = buttonPressed;

                if(arrows[(int)direction].transform.parent.localPosition.x != 0)
                {
                    if (arrows[(int)direction].transform.parent.localPosition.x < 0)
                    {
                        arrows[(int)direction].transform.localPosition = new Vector3(.1f, 0, 0);
                    }
                    else if (arrows[(int)direction].transform.parent.localPosition.x > 0)
                    {
                        arrows[(int)direction].transform.localPosition = new Vector3(-.1f, 0, 0);
                    }
                }
                else if(arrows[(int)direction].transform.parent.localPosition.y != 0)
                {
                    if (arrows[(int)direction].transform.parent.localPosition.y < 0)
                    {
                        arrows[(int)direction].transform.localPosition = new Vector3(0, .1f, 0);
                    }
                    else if (arrows[(int)direction].transform.parent.localPosition.y > 0)
                    {
                        arrows[(int)direction].transform.localPosition = new Vector3(0, -.1f, 0);
                    }
                }
            }
            else
            {
                arrows[(int)direction].transform.GetChild(0).localScale = Vector3.one;
                arrows[(int)direction].GetComponent<SpriteRenderer>().color = Color.white;
                arrows[(int)direction].transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;

                if (arrows[(int)direction].transform.parent.localPosition.x != 0)
                {
                    if (arrows[(int)direction].transform.parent.localPosition.x < 0)
                    {
                        arrows[(int)direction].transform.localPosition = new Vector3(0, 0, 0);
                    }
                    else if (arrows[(int)direction].transform.parent.localPosition.x > 0)
                    {
                        arrows[(int)direction].transform.localPosition = new Vector3(0, 0, 0);
                    }
                }
                else if (arrows[(int)direction].transform.parent.localPosition.y != 0)
                {
                    if (arrows[(int)direction].transform.parent.localPosition.y < 0)
                    {
                        arrows[(int)direction].transform.localPosition = new Vector3(0, 0, 0);
                    }
                    else if (arrows[(int)direction].transform.parent.localPosition.y > 0)
                    {
                        arrows[(int)direction].transform.localPosition = new Vector3(0, 0, 0);
                    }
                }
            }
            

        }

        public void PressDirection()
        {
            if(!isPressed)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    isPressed = true;
                    PressButtonVisual(direction.Up);
                    if (inputs[chainPlacement] == KeyCode.UpArrow)
                    {
                        inputsCorrect++;
                        individualChain[chainPlacement].color = correct;
                    }
                    else
                    {
                        individualChain[chainPlacement].color = wrong;
                    }
                    chainPlacement++;
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    isPressed = true;
                    PressButtonVisual(direction.Down);
                    if (inputs[chainPlacement] == KeyCode.DownArrow)
                    {
                        inputsCorrect++;
                        individualChain[chainPlacement].color = correct;
                    }
                    else
                    {
                        individualChain[chainPlacement].color = wrong;
                    }
                    chainPlacement++;
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    isPressed = true;
                    PressButtonVisual(direction.Left);
                    if (inputs[chainPlacement] == KeyCode.LeftArrow)
                    {
                        inputsCorrect++;
                        individualChain[chainPlacement].color = correct;
                    }
                    else
                    {
                        individualChain[chainPlacement].color = wrong;
                    }
                    chainPlacement++;
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    isPressed = true;
                    PressButtonVisual(direction.Right);
                    if (inputs[chainPlacement] == KeyCode.RightArrow)
                    {
                        inputsCorrect++;
                        individualChain[chainPlacement].color = correct;
                    }
                    else
                    {
                        individualChain[chainPlacement].color = wrong;
                    }
                    chainPlacement++;
                }

                if (chainPlacement == inputs.Length)
                {
                    //for (int i = 0; i < individualChain.Length; i++)
                    //{
                    //    individualChain[i].color = Color.grey;
                    //}
                    //commandComplete = true;
                    MagicActionObject.GetComponent<Animator>().SetTrigger("Closing");
                }
                else
                {
                    individualChain[chainPlacement].color = highlightFaded;
                }
            }
            else
            {
                if (Input.GetKeyUp(KeyCode.UpArrow))
                {
                    isPressed = false;
                    PressButtonVisual(direction.Up);
                }
                else if (Input.GetKeyUp(KeyCode.DownArrow))
                {
                    isPressed = false;
                    PressButtonVisual(direction.Down);
                }
                else if (Input.GetKeyUp(KeyCode.LeftArrow))
                {
                    isPressed = false;
                    PressButtonVisual(direction.Left);
                }
                else if (Input.GetKeyUp(KeyCode.RightArrow))
                {
                    isPressed = false;
                    PressButtonVisual(direction.Right);
                }
            }
            

        }

        public void MagicBarCloser()
        {
            isPressed = false;
            PressButtonVisual(direction.Up);
            PressButtonVisual(direction.Down);
            PressButtonVisual(direction.Left);
            PressButtonVisual(direction.Right);
        }

        public void MagicBarFinisher()
        {
            int damageIncrease = 0;

            if (inputsCorrect <= 1)
            {
                damageIncrease = 0;
            }
            else if (inputsCorrect <= 3)
            {
                damageIncrease = 1;
            }
            else if (inputsCorrect >= 5)
            {
                damageIncrease = 2;
            }

            finishAction(damageIncrease, Status.Poisoned, 3, 1, 0);
            FinishMagicCharge();
            chainPlacement = 0;

            inputsCorrect = 0;
        }

        public void FinishMagicCharge()
        {
            MagicActionObject.SetActive(false);
        }

        public void ShowMagicAction(Action<int, Status, int, int, int> finAction)
        {
            finishAction = finAction;
            MagicActionObject.SetActive(true);
            MagicActionObject.GetComponent<Animator>().SetTrigger("Opening");

            for (int i = 0; i < inputs.Length; i++)
            {
                switch (inputs[i])
                {
                    case KeyCode.UpArrow:
                        individualChain[i].sprite = lightRune;
                        break;
                    case KeyCode.DownArrow:
                        individualChain[i].sprite = darkRune;
                        break;
                    case KeyCode.RightArrow:
                        individualChain[i].sprite = fireRune;
                        break;
                    case KeyCode.LeftArrow:
                        individualChain[i].sprite = waterRune;
                        break;
                    
                    case KeyCode.X:
                        individualChain[i].sprite = windRune;
                        break;
                    case KeyCode.Z:
                        individualChain[i].sprite = natureRune;
                        break;
                }
            }

            for (int i = 0; i < individualChain.Length; i++)
            {
                individualChain[i].color = fullFaded;
            }

            individualChain[0].color = highlightFaded;
        }
        //public void 

    }

    public void HoldLeft()
    {
        GetComponent<Animator>().SetBool("DisplayAction", true);
        chargeBody.SetActive(true);
        child.SetActive(true);
        actionIcon.SetTrigger("Left Press");
        isActive = true;
    }

    public void MagicInput(Action<int, Status, int, int, int> finAction)
    {
        magicAction.ShowMagicAction(finAction);
    }

    public void LeaveBattle(Action leave)
    {
        flee = leave;
        Debug.Log("timed A");
        GetComponent<Animator>().SetBool("DisplayAction", true);
        child.SetActive(true);
        actionIcon.SetTrigger("A Press");
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
        child = transform.GetChild(0).gameObject;
        ResetObjects();
        DeactivateActionCommand();

        magicAction.Start();
    }
    void Update()
    {
        if(magicAction.MagicActionObject.activeInHierarchy)
        {
            magicAction.PressDirection();
        }
        if (!isActive || commandComplete)
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

        if(flee !=null && Input.GetKeyDown(KeyCode.Z))
        {
            flee();
        }

    }

    public void ActivateActionCommand(float xPos, float zPos)
    {
        transform.position = new Vector3(xPos, transform.position.y, zPos);
    }

    public void DeactivateActionCommand()
    {


        GetComponent<Animator>().SetBool("DisplayAction", false);
        //isActive = false;
        //chargeBody.SetActive(false);
        //child.SetActive(false);
    }

    public void TurnOffObject()
    {
        ResetObjects();
        child.SetActive(false);
    }

    // Update is called once per frame

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
        commandComplete = true;
        battleController.TriggerBattleAction(Convert.ToInt32(bonus), 1f);
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

        isActive = false;
        chargeBody.SetActive(false);
        child.SetActive(false);

        commandComplete = false;
    }
}