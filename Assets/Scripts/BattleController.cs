using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class BattleController : MonoBehaviour
{
    public enum CharacterStatus
    {
        Normal,
        Asleep,
        Poisoned,
        Cursed,
        Dead,
        Frozen
    }
    public enum CurrentTurn
    {
        Main,
        Partner,
        Enemy1,
        Enemy2,
        Enemy3,
        Enemy4
    }

    

    private CurrentTurn currentTurn;

    //class to hold all information for each character in the battle
    public class BattleParticipant
    {
        public string charName;
        public CurrentTurn charRole;
        public int maxHealth, currentHealth, attack, defense;
        public Vector3 homePosition;
        public GameObject character;
        public GameObject arrowLocation;
        public CharacterStatus status;
        public int weakenedTimer = 0;
        public Animator participantAnim;

        public int maxDamageRingSize;
        public int damageBuildupAmount = 0;

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
            participantAnim = character.transform.GetChild(0).GetComponent<Animator>();

            if(character.transform.Find("Arrow Position"))
            {
                arrowLocation = character.transform.Find("Arrow Position").gameObject;
            }
            else
            {
                Debug.Log(character + " doesn't have an arrow position added originally");
                GameObject arrowSpawn = new GameObject("Temporary arrow");
                arrowSpawn.transform.SetParent(character.transform);
                arrowSpawn.transform.position = Vector3.up;
                arrowLocation = arrowSpawn;
            }
        }

        public void TakeDamage(int damage)
        {
            currentHealth = currentHealth - damage;
        }

        public void TurnTracker()
        {

        }

        public void ReduceWeakenedTimer()
        {
            weakenedTimer--;
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
        private bool canRotateIcons = true;

    //for retriggering animation loops
    private Animator spriteAnim;
    private bool chainAttack = false;

    [SerializeField] private GameObject zoomCam;
    [SerializeField] private CinemachineTargetGroup targetGroup;

    [SerializeField] GameObject battleObjectHolder;
    [SerializeField] private Animator bookAnim;


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

    // Start is called before the first frame update
    void Start()
    {
        selectArrow.SetActive(false);
        partnerUI.SetActive(false);

        //if(battleObjectHolder != null) { battleObjectHolder.SetActive(false); }

        PlayerData.party.Add(new PartyStats("Agatha", 10, 1, 0));
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

        BattleLocationPoints points = FindObjectOfType<BattleLocationPoints>();

        //default if nothing is currently set
        if(participants.Count < 1)
        {
            AddParticipant(PlayerData.party[0], CurrentTurn.Main, mattack, mdef);
            //AddParticipant(PlayerData.party[1], CurrentTurn.Partner, pattack, pdef);
            //AddParticipant("Agatha", 10, 10, 1, 0);
            //AddParticipant("Faylee", 10, 10, 1, 0);
            AddParticipant("Jackalope", CurrentTurn.Enemy1, 5, 1, 0);
            AddParticipant("Jackalope", CurrentTurn.Enemy2, 5, 1, 0);
        }

        for (int i = 0; i < participants.Count; i++)
        {
            participants[i].homePosition = points.GetLocation(participants[i].charRole);


            for (int j = 0; j < characterPrefabs.Length; j++)
            {
                if (characterPrefabs[j].name == participants[i].charName)
                {
                    GameObject body = Instantiate(characterPrefabs[j], new Vector3(0, 0, 0), Quaternion.identity, battleObjectHolder.transform);
                    body.name = characterPrefabs[j].name;
                    participants[i].SetParticipantObject(body);
                    body.SetActive(false);

                    StartCoroutine(DelayEntryTimer(UnityEngine.Random.Range(0, .75f), body));


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


            //participants[i].SetParticipantObject(GameObject.Find(participants[i].charName));
            participants[i].character.transform.position = participants[i].homePosition;
            Debug.Log(participants[i].character.transform.position = participants[i].homePosition);
            //GameObject.Find(participants[i].charName).transform.position = participants[i].homePosition;

            if(participants[i].charRole == CurrentTurn.Main)
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
            if (battleIcons.CanSelectOption)
            {
                if (battleIcons.GetSlot() == IconType.Main_Attack && !selectArrow.activeInHierarchy)
                {
                    selectArrow.SetActive(true);

                    SetBaseSelection(true);

                    canRotateIcons = false;

                    //if (currentTurn == CurrentTurn.Main)
                    //{
                    //    StartCoroutine(MoveParticipant(participants[0]));
                    //}
                    //else if (currentTurn == CurrentTurn.Partner)
                    //{
                    //    StartCoroutine(MoveParticipant(participants[1]));
                    //}
                    //else
                    //{
                    //    //StartCoroutine(MoveParticipant("Jackalope"));
                    //}
                }
                else if (battleIcons.GetSlot() == IconType.Main_Attack && selectArrow.activeInHierarchy)
                {
                    selectArrow.SetActive(false);

                    FindTarget(currentTurn).participantAnim.SetBool("Deciding", false);

                    StartCoroutine(MoveParticipant(FindTarget(currentTurn)));
                }
            }
            if (canDefend)
            {
                Debug.Log("Set up defend animation junk");
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
            if (battleIcons.GetSlot() == IconType.Main_Attack && selectArrow.activeInHierarchy)
            {
                selectArrow.SetActive(false);
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

    public void StartBattle()
    {
        if (battleObjectHolder == null || bookAnim == null)
        {
            Debug.Log("Not assigned");
        }
        else
        {
            battleObjectHolder.SetActive(true);
            bookAnim.SetTrigger("BattleOpen");
        }
    }

    public void AttackFinisher()
    {
        attackFinished = true;
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
        if(focus1 == null)
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
        StartCoroutine(DamageBarTimer(damage, location,1f, target));
    }

    IEnumerator DamageBarTimer(int damage, Vector3 location, float time, BattleParticipant target)
    {
        //Vector3 center = GameObject.Find("Jackalope").transform.position;
        target.damageBuildupAmount += damage;

        GameObject locationHolder = new GameObject("holder");
        locationHolder.transform.position = location;

        int targetMaxCount = target.maxDamageRingSize;
        int maxCircleSize = 0;

        if(targetMaxCount <= 5)
        {
            maxCircleSize = 15;
        }
        else if(targetMaxCount <= 15)
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

        GameObject damageIcon = Instantiate(damageNumberIcon, Vector3.zero, Quaternion.identity, locationHolder.transform);
        damageIcon.GetComponentInChildren<TextMeshPro>().text = damage.ToString();

        List<Animator> icons = new List<Animator>();

        for (int i = 0; i < target.maxDamageRingSize; i++)
        {
            int a = i * angleInc;

            Vector3 pos = RandomCircle(locationHolder.transform.position, 0.75f, a);
            GameObject smallIcon =  Instantiate(damageBarIcon, pos, Quaternion.identity, locationHolder.transform);

            if(i < target.damageBuildupAmount)
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
        }
        else
        {
            for (int i = 0; i < icons.Count; i++)
            {
                yield return new WaitForSeconds(.15f);
                icons[i].SetBool("Colored", true);
            }
        }

        yield return new WaitForSeconds(time);

        damageFinished = true;

        Destroy(locationHolder);
    }
    IEnumerator SpawnEXP(int numberOfExp, CurrentTurn target)
    {
        int currentExp = 0;
        while (currentExp < numberOfExp)
        {
            currentExp++;
            GameObject exp = Instantiate(expPrefab, FindTarget(target).character.transform.GetChild(1).transform.position, Quaternion.identity);

            float randX = UnityEngine.Random.Range(0f, 4f);
            float randY = UnityEngine.Random.Range(0f, 4f);
            StartCoroutine(EXPCurve(exp, 1f, exp.transform.position, new Vector3(randX, randY, exp.transform.position.z), expGoalLocation.transform.position));
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

    /// <param name="charToMove">Participant moving</param>
    IEnumerator MoveParticipant(BattleParticipant charToMove)
    {
        attackFinished = false;
        damageFinished = false;
        if (currentTurn < CurrentTurn.Enemy1)
        {
            RaiseIcons();
        }

        Vector3 destination;

        Vector3 moveTo;

        BattleParticipant charTarget =  FindTarget(selection);

        if (currentTurn >= CurrentTurn.Enemy1)
        {
            destination.x = charTarget.character.transform.position.x + 1; //GameObject.Find("Agatha").transform.position.x + 1;
            destination.z = charTarget.character.transform.position.z;
            spriteAnim = charTarget.participantAnim;
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


        ///////asdasd
        if (currentTurn < CurrentTurn.Enemy1) { yield return new WaitUntil(() => damageFinished); }
        yield return new WaitForSeconds(.2f);// delay before starting to walk

        charToMove.participantAnim.SetBool("Walking", true);

        successBonusDamage = 0;

        //anim.SetBool("Attacking", false);
        charToMove.character.GetComponent<Animator>().SetTrigger("Flip");
        charToMove.character.GetComponent<Animator>().SetBool("TurningLeft", true);


        destination = charToMove.homePosition;
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
        
        ChangeTurn();
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
    public void TakeDamage(int damageAmount)
    {
        FindTarget(selection).TakeDamage(damageAmount);
        Debug.Log(FindTarget(selection).charName + " has taken " + damageAmount);

        if (FindTarget(selection).currentHealth <= 0 && !FindTarget(selection).character.GetComponentInChildren<SpriteRenderer>().GetComponent<Animator>().GetBool("isDead"))
        {
            StartCoroutine(SpawnEXP(5, selection));
            Debug.Log("here 1");
            FindTarget(selection).character.GetComponent<Animator>().SetTrigger("isDead");
            FindTarget(selection).character.GetComponentInChildren<SpriteRenderer>().GetComponent<Animator>().SetBool("isDead", true);
            //FindObjectOfType<BattleStartTrigger>().EndBattle();
        }
    }
    public void TakeDamage()
    {
        if (currentTurn >= CurrentTurn.Enemy1)
        {
            int damageReduction = 0;

            if (reduceDamage)
                damageReduction = 1;

            int damage = (successBonusDamage + FindTarget(currentTurn).attack) - (FindTarget(selection).defense + damageReduction);

            FindTarget(CurrentTurn.Main).TakeDamage(damage);
            mainHp[0].text = FindTarget(CurrentTurn.Main).currentHealth.ToString();
            mainHp[1].text = "/";
            mainHp[2].text = FindTarget(CurrentTurn.Main).maxHealth.ToString();

            spriteAnim.SetTrigger("Hit");
        }
        else
        {
            int damage = (successBonusDamage + FindTarget(currentTurn).attack) - FindTarget(selection).defense;
            FindTarget(selection).currentHealth -= damage;
            Debug.Log(FindTarget(selection).charName + " has taken " + damage);

            CreateDamageBar(damage, FindTarget(selection).homePosition, FindTarget(selection));

            if (FindTarget(selection).currentHealth <= 0 && !FindTarget(selection).character.GetComponentInChildren<SpriteRenderer>().GetComponent<Animator>().GetBool("isDead"))
            {
                StartCoroutine(SpawnEXP(5, selection));
                Debug.Log("here 2");
                FindTarget(selection).character.GetComponent<Animator>().SetTrigger("isDead");
                FindTarget(selection).character.GetComponentInChildren<SpriteRenderer>().GetComponent<Animator>().SetBool("isDead", true);
                //FindObjectOfType<BattleStartTrigger>().EndBattle();
            }
        }
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
    public void TriggerBattleAction(bool bonus)
    {
        if (bonus && currentTurn < CurrentTurn.Enemy1)
            successBonusDamage = 1;
        else
            successBonusDamage = 0;

        if(currentTurn == CurrentTurn.Main)
        {
            FindTarget(currentTurn).character.transform.GetChild(0).GetComponent<Animator>().SetTrigger("AttackCharged");
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

        if (FindTarget(selection) == null || FindTarget(selection).status == CharacterStatus.Dead)
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

        if (FindTarget(selection) == null || FindTarget(selection).status == CharacterStatus.Dead)
        {
            SelectEnemy(goRight);
            return;
        }

        PlaceArrow();
    }

    private void PlaceArrow()
    {
        selectArrow.transform.position = FindTarget(selection).arrowLocation.transform.position;
    }

    private void SelectTeammate()
    {

    }
    public void ChangeTurn()
    {
        currentTurn++;
        
        if(currentTurn > CurrentTurn.Enemy4)
        {
            currentTurn = CurrentTurn.Main;
        }

        if (FindTarget(currentTurn) == null || FindTarget(currentTurn).status == CharacterStatus.Dead)
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
            if(FindTarget(currentTurn).weakenedTimer > 0)
            {
                FindTarget(currentTurn).ReduceWeakenedTimer();
                ChangeTurn();
                return;
            }

            // enemy moving
            selection = CurrentTurn.Main;
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
    }

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
        participants.Add(new BattleParticipant(partyMember.charName, role, partyMember.maxHealth, partyMember.currentHealth, partyMember.baseDamage + enchantmentAttack, partyMember.defense + enchantmentDefense));
        //participants.Add(new BattleParticipant(name, maxHp, maxHp, atk, def));
    }
    
    public static void RemoveParticipants()
    {
        participants.Clear();
    }
}