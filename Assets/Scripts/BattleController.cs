using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class BattleController : MonoBehaviour
{
    //public enum CharacterStatus
    //{
    //    Normal,
    //    Asleep,
    //    Poisoned,
    //    Cursed,
    //    Dead,
    //    Frozen
    //}
    public enum CurrentTurn
    {
        Main,
        Partner,
        Enemy1,
        Enemy2,
        Enemy3,
        Enemy4
    }

    public enum SpecialBattleLocations
    {
        PlayerCast,
        EnemyCast,
        PlayerFlee
    }
    

    private CurrentTurn currentTurn;

    public class InflictedStatus
    {
        public Status inflictedStatus = Status.Normal;
        public int duration = 0;
        public int damageApplied = 0;
        public int neutralizeReduction = 0;

        public InflictedStatus(Status inflictedStatus, int duration, int damageApplied)
        {
            this.inflictedStatus = inflictedStatus;
            this.duration = duration;
            this.damageApplied = damageApplied;

            if(inflictedStatus == Status.Poisoned)
            {
                neutralizeReduction = 1;
            }
        }
    }

    //class to hold all information for each character in the battle
    public class BattleParticipant
    {
        public string charName;
        public CurrentTurn charRole;
        public int maxHealth, currentHealth, attack, defense;
        public Vector3 homePosition;
        public GameObject character;
        public GameObject arrowLocation;
        public List<InflictedStatus> statuses = new List<InflictedStatus>();
        public int weakenedTimer = 0;
        public Animator participantAnim;


        public int maxDamageRingSize;
        public int damageBuildupAmount = 0;
        internal bool isDead = false;

        private Action<int, int, CurrentTurn, BattleParticipant> statusDamageNotif;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Character name</param>
        /// <param name="role">Slot character takes up</param>
        /// <param name="maxHp">Max health</param>
        /// <param name="currentHp">Current health</param>
        /// <param name="atk">Base attack value</param>
        /// <param name="def">Base defense value</param>

        public BattleParticipant(string name,CurrentTurn role, int maxHp, int currentHp, int atk, int def)
        {
            charName = name;
            charRole = role;
            maxHealth = maxHp;
            currentHealth = currentHp;
            attack = atk;
            defense = def;

            maxDamageRingSize = maxHp / 2;
        }


        public void SetParticipantObject(GameObject character)
        {
            this.character = character;
            participantAnim = character.transform.GetChild(0).GetComponentInChildren<Animator>();

            if(participantAnim.transform.Find("Arrow Position"))
            {
                arrowLocation = participantAnim.transform.Find("Arrow Position").gameObject;
            }
            else
            {
                Debug.Log(character + " doesn't have an arrow position added originally");
                GameObject arrowSpawn = new GameObject("Temporary arrow");
                arrowSpawn.transform.SetParent(participantAnim.transform);
                arrowSpawn.transform.position = Vector3.up;
                arrowLocation = arrowSpawn;
            }
        }

        public void TakeDamage(int damage, InflictedStatus status, Action<int, int, CurrentTurn, BattleParticipant> statusAction)
        {
            currentHealth = currentHealth - damage;
            statusDamageNotif = statusAction;
            if (status != null)
                statuses.Add(status);
        }

        public void TakeDamage(int damage)
        {
            currentHealth = currentHealth - damage;

            if (currentHealth <= 0)
            {
                isDead = true;


                participantAnim.SetTrigger("PhysicallyHit");
                FindObjectOfType<BattleController>().DefeatEnemy(this);
            }
        }

        public void TurnTracker()
        {

        }

        public void StatusTimers()
        {
            if(weakenedTimer >0)
                weakenedTimer--;


            for (int i = 0; i < statuses.Count; i++)
            {
                statusDamageNotif(statuses[i].damageApplied, statuses[i].neutralizeReduction, charRole, this);

                statuses[i].duration--;

                if (statuses[i].duration < 0)
                {
                    statuses.RemoveAt(i);
                    i--;
                }
            }
        }

        public void SetWeakenedTimer()
        {
            Debug.Log(charName + " has been weakened for 2 turns");
            weakenedTimer = 2;
        }
    }

    public static List<BattleParticipant> participants = new List<BattleParticipant>();
    
    [SerializeField] private TextMeshProUGUI[] mainHp = new TextMeshProUGUI[3];
    [SerializeField] private TextMeshProUGUI[] partnerHp = new TextMeshProUGUI[3];
    [SerializeField] private TextMeshProUGUI[] MpBar = new TextMeshProUGUI[3];
    [SerializeField] private TextMeshProUGUI expText;
    [SerializeField] private GameObject expPrefab;
    [SerializeField] private BattleIconRotation battleIcons;
    [SerializeField] private GameObject partnerUI;
    [SerializeField]private GameObject expGoalLocation;


    [SerializeField] private ActionCommandController actionCommand;

    //list accessable to add and reset before and after battles

    private bool attackFinished = false;
    private bool damageFinished = false;
    private bool reduceDamage = false;

    private int successBonusDamage = 0;
    private float neutralizeMultiplier = 0f;
    private InflictedStatus statusToInflict;
        private bool canRotateIcons = true;

    //for retriggering animation loops
    private Animator spriteAnim;
    private bool chainAttack = false;

    [SerializeField] private GameObject zoomCam;
    [SerializeField] private CinemachineTargetGroup targetGroup;
    [SerializeField] private GameObject defaultFocus;

    [SerializeField] GameObject battleObjectHolder;
    [SerializeField] private GameObject book;


    private bool canDefend = false;
    [SerializeField] private float walkingRate = 4;

    /// <summary>
    /// Damage Icon stuff
    /// </summary>
    public GameObject damageBarIcon;
    public GameObject damageNumberIcon;


    [SerializeField] GameObject[] characterPrefabs = new GameObject[3];

    /// <summary>
    /// For selecting between targets
    /// </summary>
    public GameObject selectArrow;
    private CurrentTurn selection;

    private List<BattleParticipant> targetList = new List<BattleParticipant>();

    [SerializeField] private GameObject locationPointsHolder;

    private bool testing = false;
    // Start is called before the first frame update
    void Start()
    {
        selectArrow.SetActive(false);
        partnerUI.SetActive(false);

        //if(battleObjectHolder != null) { battleObjectHolder.SetActive(false); }

        if (PlayerData.party.Count == 0)
        {
            PlayerData.party.Add(new PartyStats(TeammateNames.Agatha, 10, 1, 0));

        }

        //PlayerData.party.Add(new PartyStats("Faylee", 10, 1, 0));
        //PlayerData.enchantments.Add(new Enchantment("Big Damage", "This does the big damages", 3, EnchantmentType.Attack, 3, EnchantmentTarget.Both));
        expText.text = "x" + PlayerData.party[0].currentExp.ToString();


        int pattack = 0, pdef = 0, mattack = 0, mdef = 0;

        for (int i = 0; i < PlayerData.enchantments.Count; i++)
        {
            if (PlayerData.enchantments[i].type == EnchantmentType.Attack)
            {
                if (PlayerData.enchantments[i].target == EnchantmentTarget.Main)
                {
                    mattack += PlayerData.enchantments[i].effectAmount;
                }
                else if(PlayerData.enchantments[i].target == EnchantmentTarget.Partner)
                {
                    pattack += PlayerData.enchantments[i].effectAmount;
                }
                else
                {
                    mattack += PlayerData.enchantments[i].effectAmount;
                    pattack += PlayerData.enchantments[i].effectAmount;
                }
            }
            else if (PlayerData.enchantments[i].type == EnchantmentType.Defense)
            {
                if (PlayerData.enchantments[i].target == EnchantmentTarget.Main)
                {
                    mdef += PlayerData.enchantments[i].effectAmount;
                }
                else if (PlayerData.enchantments[i].target == EnchantmentTarget.Partner)
                {
                    pdef += PlayerData.enchantments[i].effectAmount;
                }
                else
                {
                    mdef += PlayerData.enchantments[i].effectAmount;
                    pdef += PlayerData.enchantments[i].effectAmount;
                }
            }


        }

        //defaultFocus = zoomCam.LookAt.gameObject;

        //default if nothing is currently set
        if(participants.Count < 1)
        {
            testing = true;
            AddParticipant(PlayerData.party[0], CurrentTurn.Main, mattack, mdef);
            //AddParticipant(PlayerData.party[1], CurrentTurn.Partner, pattack, pdef);
            //AddParticipant("Agatha", 10, 10, 1, 0);
            //AddParticipant("Faylee", 10, 10, 1, 0);
            AddParticipant("Jackalope", CurrentTurn.Enemy1, 5, 1, 0);
            AddParticipant("Jackalope", CurrentTurn.Enemy2, 5, 1, 0);
        }

        for (int i = 0; i < participants.Count; i++)
        {
            
            for (int j = 0; j < characterPrefabs.Length; j++)
            {
                if (characterPrefabs[j].name == participants[i].charName)
                {
                    GameObject body = Instantiate(characterPrefabs[j], new Vector3(0, 0, 0), Quaternion.identity, battleObjectHolder.transform);
                    body.name = characterPrefabs[j].name;
                    participants[i].SetParticipantObject(body);
                    body.SetActive(false);

                    //StartCoroutine(DelayEntryTimer(UnityEngine.Random.Range(0, .75f), body));


                    break;
                }

                if (j == characterPrefabs.Length - 1) Debug.Log("Character not found");
            }

            if (participants[i].charRole > CurrentTurn.Partner)
            {
                if (participants[i].character.GetComponentInChildren<BattleEnemyCommunicator>())
                {
                    participants[i].character.GetComponentInChildren<BattleEnemyCommunicator>().enemySlot = participants[i].charRole;
                }
                else Debug.Log("Failed to find");
            }

            participants[i].homePosition = locationPointsHolder.GetComponent<BattleLocationPoints>().GetLocation(participants[i].charRole);
            participants[i].character.transform.position = participants[i].homePosition;
            //participants[i].SetParticipantObject(GameObject.Find(participants[i].charName));

            //Debug.Log(participants[i].character.transform.position = participants[i].homePosition);
            //GameObject.Find(participants[i].charName).transform.position = participants[i].homePosition;

            if (participants[i].charRole == CurrentTurn.Main)
            {
                mainHp[0].text = participants[i].currentHealth.ToString();
                mainHp[1].text = "/";
                mainHp[2].text = participants[i].maxHealth.ToString();
            }
            else if(participants[i].charRole == CurrentTurn.Partner)
            {
                partnerUI.SetActive(true);
                partnerHp[0].text = participants[i].currentHealth.ToString();
                partnerHp[1].text = "/";
                partnerHp[2].text = participants[i].maxHealth.ToString();
            }
        }
        if (!FindTarget(currentTurn).character.transform.Find("Idea Rotator Location"))
        {
            Debug.Log("Idea Rotator Location is missing on " + FindTarget(currentTurn).charName);
            return;
        }
        Vector3 spawnLocation = FindTarget(currentTurn).character.transform.Find("Idea Rotator Location").position;
        battleIcons.transform.position = spawnLocation;

        if(testing)
        {
            UnfoldBattle();
        }

        //battleIcons.transform.position = new Vector3(FindTarget(currentTurn).character.transform.position.x, battleIcons.transform.position.y + FindTarget(currentTurn).character.transform.position.y, battleIcons.transform.position.z);
    }

    private IEnumerator DelayEntryTimer(float delayAmount, GameObject target)
    {
        yield return new WaitForSeconds(delayAmount);
        target.SetActive(true);

        if (target = FindTarget(currentTurn).character)
        {

            FindTarget(currentTurn).participantAnim.SetBool("Deciding", true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Action1"))
        {
            if (battleIcons.CanSelectOption && currentTurn <= CurrentTurn.Partner)
            {
                if (battleIcons.GetSlot() == IconType.Main_Attack)// if main attack
                {
                    if (!selectArrow.activeInHierarchy)// if nothing has been selected yet put out an arrow to allow selection
                    {
                        selectArrow.SetActive(true);

                        SetBaseSelection(true);

                        canRotateIcons = false;
                    }
                    else if (selectArrow.activeInHierarchy)// if something has been selected already then identify who was selected and turn off the arrow
                    {
                        selectArrow.SetActive(false);
                        selectArrow.transform.parent = null;

                        FindTarget(currentTurn).participantAnim.SetBool("Deciding", false);

                        targetList.Clear();
                        targetList.Add(FindTarget(selection));
                        Debug.Log("reset targets");

                        foreach (BattleParticipant target in targetList)
                        {
                            target.participantAnim.SetBool("Targeted", true);
                        }

                        StartCoroutine(MoveParticipant(FindTarget(currentTurn)));// start the movement of the action for the attack
                    }
                }
                else if (battleIcons.GetSlot() == IconType.Items)// if the spells option is selected
                {
                    selectArrow.SetActive(false);
                    selectArrow.transform.parent = null;

                    FindTarget(currentTurn).participantAnim.SetBool("Deciding", false);

                    targetList.Clear();

                    foreach (BattleParticipant target in participants)
                    {
                        if (target.charRole > CurrentTurn.Partner)
                        {
                            targetList.Add(target);
                        }
                    }

                    foreach (BattleParticipant target in targetList)
                    {
                        target.participantAnim.SetBool("Targeted", true);
                    }

                    PlayerData.currentMP -= 2;

                    StartCoroutine(MoveParticipant(FindTarget(currentTurn), SpecialBattleLocations.PlayerCast));
                    // start a different movement using a special location as the target to move towards
                }
                else if (battleIcons.GetSlot() == IconType.Tactics)// if the tactics option is selected
                {
                    selectArrow.SetActive(false);
                    selectArrow.transform.parent = null;

                    FindTarget(currentTurn).participantAnim.SetBool("Deciding", false);

                    StartCoroutine(MoveParticipant(FindTarget(currentTurn), SpecialBattleLocations.PlayerFlee));
                    // start a different movement using a special location as the target to move towards
                }
            }
            else if (canDefend)
            {
                spriteAnim.SetTrigger("Guarding");
                reduceDamage = true;
            }
            //else if(chainAttack)
            //{
            //    Debug.Log("Chained");
            //    spriteAnim.SetTrigger("Attacking");
            //}
        }
        else if (selectArrow.activeInHierarchy && Input.GetAxisRaw("Horizontal") != 0)
        {
            if ((Input.GetAxisRaw("Horizontal") < 0))
            {
                if (Input.GetButtonDown("Horizontal"))
                {
                    SelectEnemy(false);
                }
            }
            else
            {
                if (Input.GetButtonDown("Horizontal"))
                {
                    SelectEnemy(true);
                }
            }
        }
        else if (Input.GetButtonDown("Action2"))
        {
            if (/*battleIcons.GetSlot() == IconType.Main_Attack &&*/ selectArrow.activeInHierarchy)
            {
                selectArrow.SetActive(false);
                selectArrow.transform.parent = null;

                canRotateIcons = true;
            }
        }
        else if (Input.GetButtonUp("Action1"))
        {
            //Debug.Log("End defending");
            reduceDamage = false;
        }
        else if (!battleIcons.moving && canRotateIcons)
        {
            if (battleIcons.CanSelectOption)
            {
                if (Input.GetAxisRaw("Horizontal") < 0 || Input.GetAxisRaw("Vertical") < 0)
                {
                    battleIcons.RotateCounterclockwise();
                }
                else if (Input.GetAxisRaw("Horizontal") > 0 || Input.GetAxisRaw("Vertical") > 0)
                {
                    battleIcons.RotateClockwise();
                }
            }

        }
    }

    public void HideScene(Vector3 positionToLoad)
    {
        battleObjectHolder.SetActive(false);
        transform.position = positionToLoad;
        
    }

    public void UnfoldBattle()
    {
        book.SetActive(true);
        FindObjectOfType<FolderCommunicator>().StartAnimation(StartBattle);
    }
    public void StartBattle()
    {
        if (battleObjectHolder == null)
        {
            Debug.Log("Not assigned");
        }
        else
        {
            battleObjectHolder.SetActive(true);

            for (int i = 0; i < participants.Count; i++)
            {
                StartCoroutine(DelayEntryTimer(UnityEngine.Random.Range(0, .75f), participants[i].character));
            }

            foreach (BaseFlippingAnimator item in FindObjectsOfType<BaseFlippingAnimator>())
            {
                item.SceneTransitionStart();
            }
            //bookAnim.SetTrigger("BattleOpen");
        }
    }



    Vector3 RandomCircle(Vector3 center, float radius, int a)
    {
        //Debug.Log(a);
        float ang = a;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        pos.z = center.z;
        return pos;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="focus1">First Focus</param>
    /// <param name="focus2">Second Focus</param>

    private void SetCamFocus(GameObject focus1, GameObject focus2)
    {
        if(focus1 == null || focus2 == null)
        {
            zoomCam.SetActive(false);
        }
        else
        {
            zoomCam.SetActive(true);
            //CinemachineTargetGroup newGroup = GetComponent<CinemachineTargetGroup>();
            //Debug.Log(focus1 + " " + focus2);
            targetGroup.m_Targets = new CinemachineTargetGroup.Target[0];
            targetGroup.AddMember(focus1.transform, 1f, 1f);
            targetGroup.AddMember(focus2.transform, 1f, 1f);

        }
        //vcam.LookAt = focus.transform;
        //vcam.Follow = focus.transform;
        
    }
    // creates the damage guage
    private void CreateDamageBar(int damage, Vector3 location, BattleParticipant target)
    {
        StartCoroutine(DamageBarTimer(damage, location, 1f, target));
    }

    IEnumerator DamageBarTimer(int damage, Vector3 location, float time, BattleParticipant target)
    {
        float fullyShowBarTime = time; // how long the bar will be displayed after all pieces get colored
        //Vector3 center = GameObject.Find("Jackalope").transform.position;
        GameObject locationHolder = new GameObject("holder");
        locationHolder.transform.position = location;
        GameObject damageIcon = Instantiate(damageNumberIcon, Vector3.zero, Quaternion.identity, locationHolder.transform);
        damageIcon.GetComponentInChildren<TextMeshPro>().text = damage.ToString();

        if (target.weakenedTimer <=0)
        {
            target.damageBuildupAmount += (int)(damage * neutralizeMultiplier);

            int targetMaxCount = target.maxDamageRingSize;

            time /= targetMaxCount; // makes the counter take the same time to fully complete regardless of size

            int maxCircleSize = 0;

            if (targetMaxCount <= 5)
            {
                maxCircleSize = 15;
            }
            else if (targetMaxCount <= 15)
            {
                maxCircleSize = 30;
            }
            else
            {
                maxCircleSize = 50;
            }

            //int fullCircle = 30;
            int angleInc = 360 / maxCircleSize;
            //currentRingCount


            List<Animator> icons = new List<Animator>();

            for (int i = 0; i < target.maxDamageRingSize; i++)
            {
                int a = i * angleInc;

                Vector3 pos = RandomCircle(locationHolder.transform.position, 0.75f, a);
                GameObject smallIcon = Instantiate(damageBarIcon, pos, Quaternion.identity, locationHolder.transform);

                if (i < target.damageBuildupAmount)
                {
                    icons.Add(smallIcon.GetComponent<Animator>());
                }
            }

            if (target.damageBuildupAmount >= target.maxDamageRingSize)
            {
                foreach (Animator icon in icons)
                {
                    icon.SetBool("Colored", true);
                    icon.SetBool("Explode", true);
                }
                target.damageBuildupAmount = 0;
                target.SetWeakenedTimer();

                fullyShowBarTime *= 1.5f; // holds the delay a bit longer to show it is filled
            }
            else
            {
                for (int i = 0; i < icons.Count; i++)
                {
                    icons[i].SetBool("Colored", true);
                    yield return new WaitForSeconds(time);
                }
            }
        }
        else
        {
            time /= 2;
        }
        

        yield return new WaitForSeconds(fullyShowBarTime);

        damageFinished = true;

        Destroy(locationHolder);
    }
    IEnumerator SpawnEXP(int numberOfExp, BattleParticipant target)
    {
        int currentExp = 0;
        while (currentExp < numberOfExp)
        {
            currentExp++;
            GameObject exp = Instantiate(expPrefab, target.character.transform.GetChild(1).transform.position, Quaternion.identity);

            float randX = UnityEngine.Random.Range(-2f, 4f);
            float randY = UnityEngine.Random.Range(-2f, 2.5f);
            StartCoroutine(EXPCurve(exp, 1f, exp.transform.position, new Vector3(exp.transform.position.x + randX, exp.transform.position.y + randY, exp.transform.position.z), expGoalLocation.transform.position));
            //new Vector3(3f, 4f, 1)
            yield return new WaitForSeconds(0.1f);
        }
        //RemoveEnemy(target);
    }

    IEnumerator EXPCurve(GameObject expIcon, float time, Vector3 pos0, Vector3 pos1, Vector3 pos2)
    {
        float timer = 0f;

        while(timer < time)
        {
            timer += Time.deltaTime * 2;
            Vector3 location = Mathf.Pow(1 - timer, 2) * pos0 + 2 * (1 - timer) * timer * pos1 + Mathf.Pow(timer, 2) * pos2;
                
            expIcon.transform.position = location;

            yield return new WaitForEndOfFrame();
        }

        PlayerData.party[0].currentExp++;

        expText.text = "x" + PlayerData.party[0].currentExp;

        Destroy(expIcon);
    }

    //IEnumerator EXPCurve(GameObject expIcon)
    //{
    //    float centerOffset = 1f; //Random.Range(-3.5f, 3.5f);

    //    Vector3 startPos = expIcon.transform.position;
    //    Vector3 endPos = new Vector3(3f, 4f, 1);//expEndLocation.transform.position;

    //    Vector3 centerPivot = (startPos + endPos) * 0.5f;
    //    centerPivot -= new Vector3(0, -centerOffset);

    //    Vector3 startRelativeCenter = startPos - centerPivot;
    //    Vector3 endRelativeCenter = endPos - centerPivot;

    //    float time = 0;
    //    float duration = 1;
    //    while (time < duration / 2)
    //    {//expIcon.transform.localPosition, positions[i], time / duration
    //        expIcon.transform.localPosition = Vector3.Slerp(expIcon.transform.position, endPos, time / duration);
    //        time += Time.deltaTime;

    //        yield return null;
    //    }
    //    //yield return new WaitForSeconds(0.01f);
    //}

    private IEnumerator CharacterFader(Color colorDestination, List<SpriteRenderer> spritesToChange, float timeToRun)
    {
        float time = 0;
        if(spritesToChange.Count == 0)
        {
            yield break;
        }
        Color baseColor = spritesToChange[0].color;
        while (time < timeToRun)
        {
            for (int i = 0; i < spritesToChange.Count; i++)
            {
                spritesToChange[i].color = Color.Lerp(baseColor, colorDestination, time / timeToRun);
            }
                time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    /// <param name="charToMove">Participant moving</param>
    IEnumerator MoveParticipant(BattleParticipant charToMove)// single target
    {
        attackFinished = false;
        damageFinished = false;
        if (currentTurn < CurrentTurn.Enemy1)
        {
            RaiseIcons();
        }

        Vector3 destination;

        Vector3 moveTo;

        BattleParticipant charTarget = targetList[0];

            spriteAnim = charTarget.participantAnim;

        List<SpriteRenderer> spritesToChange = new List<SpriteRenderer>();
        for (int i = 0; i < participants.Count; i++)
        {
            if (participants[i] != charToMove && participants[i] != charTarget)
            {
                spritesToChange.Add(participants[i].participantAnim.GetComponent<SpriteRenderer>());
            }
        }
        StartCoroutine(CharacterFader(new Color(.75f, .75f, .75f, .75f), spritesToChange, .5f));

        if (currentTurn >= CurrentTurn.Enemy1)
        {
            destination.x = charTarget.character.transform.position.x + 1; //GameObject.Find("Agatha").transform.position.x + 1;
            destination.z = charTarget.character.transform.position.z;
        }
        else
        {
            destination.x = charTarget.character.transform.position.x - 1; //GameObject.Find("Jackalope").transform.position.x - 1;
            destination.z = charTarget.character.transform.position.z;
            SetCamFocus(charToMove.character, charTarget.character);
        }
        //if(name != "Agatha" && name != "Faylee")
        //{
        //    GameObject.Find(name).transform.position = GameObject.Find("Agatha").transform.position + Vector3.right;
        //    Debug.Log("Agatha was hit");
        //}
        //else
        //{
        //    GameObject.Find(name).transform.position = GameObject.Find("Jackalope").transform.position + Vector3.left;
        //    Debug.Log(name + " attacked");
        //}

        if (currentTurn < CurrentTurn.Enemy1)
        {
            actionCommand.ActivateActionCommand(destination.x, destination.z);

            if(currentTurn == CurrentTurn.Main)
            {
                actionCommand.HoldLeft();
            }
            else
            {
                actionCommand.TimedA();
            }
        }

        yield return new WaitForSeconds(0.5f);// delay before starting to walk to target

        charToMove.participantAnim.SetBool("Walking", true);

        while (charToMove.character.transform.position.x != destination.x || charToMove.character.transform.position.z != destination.z)
        {
            moveTo = Vector3.MoveTowards(charToMove.character.transform.position, new Vector3(destination.x, charToMove.character.transform.position.y, destination.z), walkingRate * Time.fixedDeltaTime);
            charToMove.character.GetComponent<Rigidbody>().MovePosition(moveTo);
            yield return new WaitForSeconds(0.01f);
        }
        charToMove.participantAnim.SetBool("Walking", false);
        charToMove.participantAnim.SetTrigger("Attacking");

        //if(currentTurn != CurrentTurn.Enemy1)
        //{
        //    actionCommand.ActivateActionCommand(character.transform.position.x);
        //}
        if (attackFinished) Debug.Log("Finished Early somehow");
        yield return new WaitUntil(() => attackFinished);
        //while (!attackFinished)
        //{
        //    yield return new WaitForSeconds(0.01f);
        //}

        SetCamFocus(null, null);// resets the camera

        StartCoroutine(CharacterFader(Color.white, spritesToChange, .5f)); // reset color of non target characters

        charTarget.participantAnim.SetBool("Targeted", false);

        ///////asdasd
        if (currentTurn < CurrentTurn.Enemy1) { yield return new WaitUntil(() => damageFinished); }
        yield return new WaitForSeconds(.2f);// delay before starting to walk

        charToMove.participantAnim.SetBool("Walking", true);

        successBonusDamage = 0;

        //anim.SetBool("Attacking", false);
        charToMove.character.GetComponent<Animator>().SetTrigger("Flip");
        charToMove.character.GetComponent<Animator>().SetBool("TurningLeft", true);


        destination = charToMove.homePosition + transform.position;
        //for (int i = 0; i < participants.Count; i++)
        //{
        //    if (participants[i].charName == name)
        //    {
        //        destination = participants[i].homePosition.x;
        //        break;
        //    }
        //}

        while (charToMove.character.transform.position != destination)
        {
            moveTo = Vector3.MoveTowards(charToMove.character.transform.position, new Vector3(destination.x, charToMove.character.transform.position.y, destination.z), walkingRate * Time.fixedDeltaTime);
            charToMove.character.GetComponent<Rigidbody>().MovePosition(moveTo);
            yield return new WaitForSeconds(0.01f);
        }

        charToMove.character.GetComponent<Animator>().SetTrigger("Flip");
        charToMove.character.GetComponent<Animator>().SetBool("TurningLeft", false);
        charToMove.participantAnim.SetBool("Walking", false);
        
        StartCoroutine(ChangeTurn());   
    }
    IEnumerator MoveParticipant(BattleParticipant charToMove, SpecialBattleLocations specialLoc)// multiple targets
    {
        Vector3 destination = FindObjectOfType<BattleLocationPoints>().GetSpecialLocation(specialLoc) + transform.position;
        attackFinished = false;
        damageFinished = false;
        if (currentTurn < CurrentTurn.Enemy1)
        {
            RaiseIcons();
        }

        //Vector3 destination = standingLocation;

        Vector3 moveTo;

        List<SpriteRenderer> spritesToChange = new List<SpriteRenderer>();
        for (int i = 0; i < participants.Count; i++)
        {
            if (participants[i] != charToMove)
            {
                for (int j = 0; j < targetList.Count; j++)
                {
                    if (participants[i] != targetList[j] && j == targetList.Count -1)
                    {
                        spritesToChange.Add(participants[i].participantAnim.GetComponent<SpriteRenderer>());
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        StartCoroutine(CharacterFader(new Color(.75f, .75f, .75f, .75f), spritesToChange, .5f));


        //spriteAnim = charTarget.participantAnim;
        if (specialLoc == SpecialBattleLocations.PlayerFlee)
        {
            SetCamFocus(charToMove.character, defaultFocus);
            actionCommand.LeaveBattle(FindObjectOfType<BattleStartTrigger>().FleeBattle);
        }


        if (specialLoc == SpecialBattleLocations.PlayerCast)
        {
            actionCommand.ActivateActionCommand(destination.x, destination.z);

            if (currentTurn == CurrentTurn.Main)
            {
                actionCommand.MagicInput(TriggerBattleAction, TeammateNames.Agatha);
            }
            else
            {
                actionCommand.TimedA();
            }
        }
        else if(specialLoc == SpecialBattleLocations.PlayerFlee)
        {
            
        }

        yield return new WaitForSeconds(0.5f);// delay before starting to walk to target

        charToMove.participantAnim.SetBool("Walking", true);

        if(specialLoc == SpecialBattleLocations.PlayerFlee)
        {
            charToMove.character.GetComponent<Animator>().SetTrigger("Flip");
            charToMove.character.GetComponent<Animator>().SetBool("TurningLeft", true);
        }

        while (charToMove.character.transform.position.x != destination.x || charToMove.character.transform.position.z != destination.z)
        {
            moveTo = Vector3.MoveTowards(charToMove.character.transform.position, new Vector3(destination.x, charToMove.character.transform.position.y, destination.z), walkingRate * Time.fixedDeltaTime);
            charToMove.character.GetComponent<Rigidbody>().MovePosition(moveTo);
            yield return new WaitForSeconds(0.01f);
        }
        charToMove.participantAnim.SetBool("Walking", false);
        charToMove.participantAnim.SetTrigger("Attacking");

        //if(currentTurn != CurrentTurn.Enemy1)
        //{
        //    actionCommand.ActivateActionCommand(character.transform.position.x);
        //}
        if (attackFinished) Debug.Log("Finished Early somehow");
        yield return new WaitUntil(() => attackFinished);
        //while (!attackFinished)
        //{
        //    yield return new WaitForSeconds(0.01f);
        //}

        SetCamFocus(null, null);// resets the camera

        StartCoroutine(CharacterFader(Color.white, spritesToChange, .5f));// change color of non target characters

        foreach (BattleParticipant target in targetList)
        {
            target.participantAnim.SetBool("Targeted", false);
        }

        ///////asdasd
        if (currentTurn < CurrentTurn.Enemy1) { yield return new WaitUntil(() => damageFinished); }
        yield return new WaitForSeconds(.2f);// delay before starting to walk

        charToMove.participantAnim.SetBool("Walking", true);

        successBonusDamage = 0;

        //anim.SetBool("Attacking", false);

        if (specialLoc == SpecialBattleLocations.PlayerFlee)
        {
            charToMove.character.GetComponent<Animator>().SetTrigger("Flip");
            charToMove.character.GetComponent<Animator>().SetBool("TurningLeft", false);
        }

        ////charToMove.character.GetComponent<Animator>().SetTrigger("Flip");
        ////charToMove.character.GetComponent<Animator>().SetBool("TurningLeft", true);


        destination = charToMove.homePosition + transform.position;
        //for (int i = 0; i < participants.Count; i++)
        //{
        //    if (participants[i].charName == name)
        //    {
        //        destination = participants[i].homePosition.x;
        //        break;
        //    }
        //}

        while (charToMove.character.transform.position != destination)
        {
            moveTo = Vector3.MoveTowards(charToMove.character.transform.position, new Vector3(destination.x, charToMove.character.transform.position.y, destination.z), walkingRate * Time.fixedDeltaTime);
            charToMove.character.GetComponent<Rigidbody>().MovePosition(moveTo);
            yield return new WaitForSeconds(0.01f);
        }

        //charToMove.character.GetComponent<Animator>().SetTrigger("Flip");
        //charToMove.character.GetComponent<Animator>().SetBool("TurningLeft", false);
        charToMove.participantAnim.SetBool("Walking", false);

        StartCoroutine(ChangeTurn());
    }

    private BattleParticipant FindTarget(CurrentTurn target)
    {
        for (int i = 0; i < participants.Count; i++)
        {
            if (participants[i].charRole == target)
            {
                return participants[i];
            }
        }
        return null;
    }
    //public void TakeDamage(int damageAmount)
    //{
    //    FindTarget(selection).TakeDamage(damageAmount);
    //    Debug.Log(FindTarget(selection).charName + " has taken " + damageAmount);

    //    if (FindTarget(selection).currentHealth <= 0 && !FindTarget(selection).character.GetComponentInChildren<SpriteRenderer>().GetComponent<Animator>().GetBool("isDead"))
    //    {
    //        StartCoroutine(SpawnEXP(5, selection));
    //        Debug.Log("here 1");
    //        FindTarget(selection).character.GetComponent<Animator>().SetTrigger("isDead");
    //        FindTarget(selection).character.GetComponentInChildren<SpriteRenderer>().GetComponent<Animator>().SetBool("isDead", true);
    //        //FindObjectOfType<BattleStartTrigger>().EndBattle();
    //    }
    //}
    public void TakeDamage()
    {
       //neutralizeMultiplier = neutralizeMul;
        if (currentTurn >= CurrentTurn.Enemy1)
        {
            int damageReduction = 0;

            if (reduceDamage)
                damageReduction = 1;

            

            foreach (BattleParticipant target in targetList)
            {
                int damage = (successBonusDamage + FindTarget(currentTurn).attack) - (target.defense + damageReduction);
                target.TakeDamage(damage,statusToInflict, TakeDamageStatus);
                target.participantAnim.SetTrigger("Hit");
            }
            statusToInflict = null;


            //FindTarget(CurrentTurn.Main).TakeDamage(damage);
            mainHp[0].text = FindTarget(CurrentTurn.Main).currentHealth.ToString();
            mainHp[1].text = "/";
            mainHp[2].text = FindTarget(CurrentTurn.Main).maxHealth.ToString();

            //spriteAnim.SetTrigger("Hit");
        }
        else
        {

            foreach (BattleParticipant target in targetList)
            {
                int damage = (successBonusDamage + FindTarget(currentTurn).attack) - target.defense;
                target.TakeDamage(damage,statusToInflict, TakeDamageStatus);

                target.participantAnim.SetTrigger("PhysicallyHit");
                CreateDamageBar(damage, target.homePosition + transform.position, target);

                if (target.currentHealth <= 0 && !target.character.GetComponentInChildren<SpriteRenderer>().GetComponent<Animator>().GetBool("isDead"))
                {
                    DefeatEnemy(target);
                    Debug.Log(target.charName + " has taken " + damage);
                }
            }

            statusToInflict = null;
        }
    }
    private void TakeDamageStatus(int damage, int neutralizeRed, CurrentTurn charSlot, BattleParticipant charBattle)
    {
        neutralizeMultiplier = neutralizeRed;
        if (charSlot >= CurrentTurn.Enemy1)
        {
            charBattle.TakeDamage(damage);

            charBattle.participantAnim.SetTrigger("PhysicallyHit");
            CreateDamageBar(damage, charBattle.homePosition + transform.position, charBattle);

            if (charBattle.currentHealth <= 0 && !charBattle.character.GetComponentInChildren<SpriteRenderer>().GetComponent<Animator>().GetBool("isDead"))
            {
                DefeatEnemy(charBattle);
            }
        }
        else
        {

            charBattle.TakeDamage(damage);
            charBattle.participantAnim.SetTrigger("Hit");

            mainHp[0].text = FindTarget(charSlot).currentHealth.ToString();
            mainHp[1].text = "/";
            mainHp[2].text = FindTarget(charSlot).maxHealth.ToString();
        }
    }

    public void DefeatEnemy(BattleParticipant target)
    {
        StartCoroutine(SpawnEXP(5, target));
        target.character.GetComponent<Animator>().SetTrigger("isDead");
        target.character.GetComponentInChildren<SpriteRenderer>().GetComponent<Animator>().SetBool("isDead", true);

    }

    IEnumerator PostBattle()
    {

        yield return new WaitForSeconds(1.5f);
        foreach (BattleParticipant character in participants)
        {
            character.participantAnim.SetTrigger("Victory");
        }
        yield return new WaitForSeconds(1f);

        foreach (BattleParticipant character in participants)
        {
            character.character.GetComponent<Animator>().SetTrigger("Flip");
            character.participantAnim.SetTrigger("Leave");
        }



        yield return new WaitForSeconds(.8f);
        FindObjectOfType<BattleStartTrigger>().EndBattle();
    }

    public void RemoveEnemy(CurrentTurn deadEnemy)
    {
        for (int i = 0; i < participants.Count; i++)
        {
            if (participants[i].charRole == deadEnemy)
            {
                Destroy(participants[i].character);
                participants.RemoveAt(i);
                return;
            }
        }
    }

    /// <param name="bonus">If extra damage is applied or not</param>
    public void TriggerBattleAction(int bonus, float neutralizeMult)
    {
        //if (bonus && currentTurn < CurrentTurn.Enemy1)
        //    successBonusDamage = 1;
        //else
        //    successBonusDamage = 0;
        neutralizeMultiplier = neutralizeMult;
        successBonusDamage = bonus;

        if (currentTurn == CurrentTurn.Main)
        {
            FindTarget(currentTurn).character.transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetTrigger("AttackCharged");
            Debug.Log("triggered");
            //GameObject.Find("Agatha").transform
        }

        AttackFinisher();
    }

    public void TriggerBattleAction(int bonus, Status inflictedStatus, int statusDir, int statusDamage, int neutralizeMult)
    {
        neutralizeMultiplier = neutralizeMult;
        successBonusDamage = bonus;

        if (inflictedStatus != Status.Normal)
        {
            statusToInflict = new InflictedStatus(inflictedStatus, statusDir, statusDamage);
        }
        if (currentTurn == CurrentTurn.Main)
        {
            FindTarget(currentTurn).character.transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetTrigger("AttackCharged");
            //GameObject.Find("Agatha").transform
        }

        AttackFinisher();
    }

    private void SetBaseSelection(bool enemy)
    {
        if (enemy)
        {
            selection = CurrentTurn.Partner;

            do
                selection++;
            while (FindTarget(selection) == null && (int)selection < Enum.GetNames(typeof(CurrentTurn)).Length);
        }
        else
        {
            selection = currentTurn;
        }

        if (FindTarget(selection) == null || FindTarget(selection).isDead)
        {
            SetBaseSelection(enemy);
            return;
        }

        PlaceArrow();
    }
    private void SelectEnemy(bool goRight)
    {
        if (goRight)
            selection++;
        else
            selection--;

        if(selection < CurrentTurn.Enemy1)
        {
            selection = (CurrentTurn)Enum.GetNames(typeof(CurrentTurn)).Length;
        }
        else if((int)selection > Enum.GetNames(typeof(CurrentTurn)).Length)
        {
            selection = CurrentTurn.Enemy1;
        }

        if (FindTarget(selection) == null || FindTarget(selection).statuses.Count > 0 && FindTarget(selection).statuses[0].inflictedStatus == Status.Dead)
        {
            SelectEnemy(goRight);
            return;
        }

        PlaceArrow();
    }

    private void PlaceArrow()
    {
        selectArrow.transform.parent = FindTarget(selection).arrowLocation.transform;
        selectArrow.transform.localPosition = Vector3.zero;
    }

    private void SelectTeammate()
    {

    }
    public IEnumerator ChangeTurn()
    {
        currentTurn++;

        if (currentTurn > CurrentTurn.Enemy4)
        {
            currentTurn = CurrentTurn.Main;

            int enemiesAlive = 0;

            for (int i = 0; i < participants.Count; i++)
            {

                if (participants[i].charRole > CurrentTurn.Partner && !participants[i].isDead)
                {
                    enemiesAlive++;
                }
            }

            if (enemiesAlive < 1)
            {
                StartCoroutine(PostBattle());
                yield break;
            }
        }

        

        if(FindTarget(currentTurn) != null)
        {
            yield return new WaitForSeconds(.5f);
            FindTarget(currentTurn).StatusTimers();

            if (FindTarget(currentTurn).weakenedTimer > 0)
            {
                StartCoroutine(ChangeTurn());
                yield break;
            }
        }

        

        if (FindTarget(currentTurn) == null || FindTarget(currentTurn).isDead)
        {
            if (FindTarget(currentTurn) == null)
            {
            }
            else
            {
                FindTarget(currentTurn).participantAnim.SetBool("Deciding", false);
            }

            StartCoroutine(ChangeTurn());
            yield break;
        }

        if (currentTurn > CurrentTurn.Partner)
        {


            // enemy moving
            selection = CurrentTurn.Main;
            targetList.Clear();
            targetList.Add(FindTarget(selection));
            StartCoroutine(MoveParticipant(FindTarget(currentTurn)));
        }
        else// if its player turn and they can move
        {
            if (!FindTarget(currentTurn).character.transform.Find("Idea Rotator Location"))
            {
                Debug.Log("Idea Rotator Location is missing on " + FindTarget(currentTurn).charName);
                yield break; ;
            }
            Vector3 spawnLocation = FindTarget(currentTurn).character.transform.Find("Idea Rotator Location").position;
            battleIcons.transform.position = spawnLocation;

            FindTarget(currentTurn).participantAnim.SetBool("Deciding", true);
            LowerIcons();
        }
    }
    /*public void ChangeTurn()
    {
        currentTurn++;
        
        if(currentTurn > CurrentTurn.Enemy4)
        {
            currentTurn = CurrentTurn.Main;
        }

        FindTarget(currentTurn).StatusTimers();

        if (FindTarget(currentTurn).weakenedTimer > 0)
        {
            ChangeTurn();
            return;
        }

        if (FindTarget(currentTurn) == null || FindTarget(currentTurn).isDead)
        {
            if (FindTarget(currentTurn) == null)
            {
            }
            else
            {
                FindTarget(currentTurn).participantAnim.SetBool("Deciding", false);
            }

            ChangeTurn();
            return;
        }

        if(currentTurn > CurrentTurn.Partner)
        {
            

            // enemy moving
            selection = CurrentTurn.Main;
            targetList.Clear();
            targetList.Add(FindTarget(selection));
            StartCoroutine(MoveParticipant(FindTarget(currentTurn)));
        }
        else// if its player turn and they can move
        {
            if(!FindTarget(currentTurn).character.transform.Find("Idea Rotator Location"))
            {
                Debug.Log("Idea Rotator Location is missing on " + FindTarget(currentTurn).charName);
                return;
            }
            Vector3 spawnLocation = FindTarget(currentTurn).character.transform.Find("Idea Rotator Location").position;
            battleIcons.transform.position = spawnLocation;

            FindTarget(currentTurn).participantAnim.SetBool("Deciding", true);

            //battleIcons.transform.position = new Vector3(FindTarget(currentTurn).character.transform.Find("Idea Rotator Location").position.x, battleIcons.transform.position.y + .5f + FindTarget(currentTurn).character.transform.position.y, battleIcons.transform.position.z);
            LowerIcons();
        }

        //if (currentTurn == CurrentTurn.Main)
        //{
        //    currentTurn = CurrentTurn.Partner;
            

            
        //}
        //else if (currentTurn == CurrentTurn.Partner)
        //{
        //    currentTurn = CurrentTurn.Enemy1;
        //}
        //else
        //{
        //    currentTurn = CurrentTurn.Main;

        //    battleIcons.transform.position = new Vector3(FindTarget(currentTurn).character.transform.position.x, battleIcons.transform.position.y, battleIcons.transform.position.z);
        //    LowerIcons();
        //}
    }*/

    /// <summary>
    /// Battle Action Commands
    /// </summary>
    public void CanDefend()
    {
        canDefend = true;
    }

    public void CannotDefend()
    {
        canDefend = false;
    }

    public void PlayChainAttack()
    {
        Debug.Log("Chained");
        spriteAnim.SetTrigger("Attacking");
    }

    //Brings in sprite ref for chain attack replaying and opens window to chain
    public void StartBonusAttackWindow(Animator sprite)
    {
        chainAttack = true;
        spriteAnim = sprite;

        actionCommand.chainAttack = chainAttack;
        actionCommand.ActivatePressed();
    }
    //closes window to chain attacks together
    public void EndBonusAttackWindow()
    {
        chainAttack = false;

        actionCommand.chainAttack = chainAttack;
        actionCommand.DeactivatePressed();
    }
    //clears the battle controller sprite ref after attack animation is complete
    public void ClearSpriteRef()
    {
        spriteAnim = null;
        AttackFinisher();
        actionCommand.DeactivateActionCommand();
    }

    /// <summary>
    /// Battle Icon controls
    /// </summary>

    private void RaiseIcons()
    {
        canRotateIcons = false;
        battleIcons.SetAnimTrigger("Raise");
    }

    private void LowerIcons()
    {
        canRotateIcons = true;
        battleIcons.SetAnimTrigger("Lower");
    }

    /// <summary>
    /// Start of types of commands
    /// </summary>
    public void AttackFinisher()
    {
        attackFinished = true;
    }

    public void HoldLeft()
    {
        actionCommand.HoldLeft();
    }

    public void TimedA()
    {
        actionCommand.TimedA();
    }

    /// <summary>
    /// Start of participant loading
    /// </summary>

    public static void ClearParticipants()
    {
        participants.Clear();
    }

    //for characters with hp not at max
    public static void AddParticipant(string name, CurrentTurn role, int maxHp, int currentHp, int atk, int def)
    {
        participants.Add(new BattleParticipant(name,role, maxHp, currentHp, atk, def));
    }
    //for characters who start at max, most enemies
    public static void AddParticipant(string name, CurrentTurn role, int maxHp, int atk, int def)
    {
        participants.Add(new BattleParticipant(name, role, maxHp, maxHp, atk, def));
    }
    //for player characters
    public static void AddParticipant(PartyStats partyMember, CurrentTurn role, int enchantmentAttack, int enchantmentDefense)
    {
        participants.Add(new BattleParticipant(partyMember.charName.ToString(), role, partyMember.maxHealth, partyMember.currentHealth, partyMember.baseDamage + enchantmentAttack, partyMember.defense + enchantmentDefense));
        //participants.Add(new BattleParticipant(name, maxHp, maxHp, atk, def));
    }
    
    public static void RemoveParticipants()
    {
        participants.Clear();
    }
}