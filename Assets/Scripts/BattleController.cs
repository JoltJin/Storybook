using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class BattleController : MonoBehaviour
{
    private enum CurrentTurn
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
        public int maxHealth, currentHealth, attack, defense;
        public Vector3 homePosition;


        public BattleParticipant(string name, int maxHp, int currentHp, int atk, int def)
        {
            charName = name;
            maxHealth = maxHp;
            currentHealth = currentHp;
            attack = atk;
            defense = def;
        }
    }

    [SerializeField] private TextMeshProUGUI mainHp;
    [SerializeField] private TextMeshProUGUI partnerHp;

    [SerializeField] private BattleIconRotation battleIcons;

    [SerializeField] private ActionCommandController actionCommand;

    //list accessable to add and reset before and after battles
    public static List<BattleParticipant> participants = new List<BattleParticipant>();

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

    public void AttackFinisher()
    {
        attackFinished = true;
    }

    private bool canDefend = false;
    [SerializeField] private float walkingRate = 4;
    /// <summary>
    /// Damage Icon stuff
    /// </summary>
    [SerializeField]int maxDamageRingSize = 12;
    int damageBuildupAmount = 0;
    public GameObject damageBarIcon;
    public GameObject damageNumberIcon;


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
            Debug.Log(focus1 + " " + focus2);
            targetGroup.m_Targets = new CinemachineTargetGroup.Target[0];
            targetGroup.AddMember(focus1.transform, 1f, 1f);
            targetGroup.AddMember(focus2.transform, 1f, 1f);

        }
        //vcam.LookAt = focus.transform;
        //vcam.Follow = focus.transform;
        
    }
    // creates the damage guage
    private void CreateDamageBar(int damage, Vector3 location)
    {
        StartCoroutine(DamageBarTimer(damage, location,1f));
    }

    IEnumerator DamageBarTimer(int damage, Vector3 location, float time)
    {
        //Vector3 center = GameObject.Find("Jackalope").transform.position;
        damageBuildupAmount += damage;

        GameObject locationHolder = new GameObject("holder");
        locationHolder.transform.position = location;
        int fullCircle = 30;
        int angleInc = 360 / fullCircle;
        //currentRingCount

        GameObject damageIcon = Instantiate(damageNumberIcon, Vector3.zero, Quaternion.identity, locationHolder.transform);
        damageIcon.GetComponentInChildren<TextMeshPro>().text = damage.ToString();

        List<Animator> icons = new List<Animator>();

        for (int i = 0; i < maxDamageRingSize; i++)
        {
            int a = i * angleInc;

            Vector3 pos = RandomCircle(locationHolder.transform.position, 0.75f, a);
            GameObject smallIcon =  Instantiate(damageBarIcon, pos, Quaternion.identity, locationHolder.transform);

            if(i < damageBuildupAmount)
            {
                icons.Add(smallIcon.GetComponent<Animator>());
            }
        }

        if (damageBuildupAmount >= maxDamageRingSize)
        {
            foreach (Animator icon in icons)
            {
                icon.SetBool("Colored", true);
                icon.SetBool("Explode", true);
            }
            damageBuildupAmount = 0;
            TakeDamage(5);
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

    // Start is called before the first frame update
    void Start()
    {
        PlayerData.party.Add(new PartyStats("Agatha", 10, 1, 0));
        PlayerData.party.Add(new PartyStats("Faylee", 10, 1, 0));
        PlayerData.enchantments.Add(new Enchantment("Big Damage", "This does the big damages", 3, EnchantmentType.Attack, 3, EnchantmentTarget.Both));


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
            AddParticipant(PlayerData.party[0], mattack, mdef);
            AddParticipant(PlayerData.party[1], pattack, pdef);
            //AddParticipant("Agatha", 10, 10, 1, 0);
            //AddParticipant("Faylee", 10, 10, 1, 0);
            AddParticipant("Jackalope", 10, 1, 0);
        }

        for (int i = 0; i < participants.Count; i++)
        {
            participants[i].homePosition = points.GetLocation(i);
            GameObject.Find(participants[i].charName).transform.position = participants[i].homePosition;
            if(participants[i].charName == "Agatha")
            {
                mainHp.text = participants[i].currentHealth.ToString() + "/" + participants[i].maxHealth.ToString();
            }
            else if(participants[i].charName == "Faylee")
            {
                partnerHp.text = participants[i].currentHealth.ToString() + "/" + participants[i].maxHealth.ToString();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Action1"))
        {
            if(battleIcons.CanSelectOption)
            {
                if(battleIcons.GetSlot() == IconType.Main_Attack)
                {
                    if (currentTurn == CurrentTurn.Main)
                    {
                        StartCoroutine(MoveParticipant("Agatha"));
                    }
                    else if (currentTurn == CurrentTurn.Partner)
                    {
                        StartCoroutine(MoveParticipant("Faylee"));
                    }
                    else
                    {
                        //StartCoroutine(MoveParticipant("Jackalope"));
                    }
                }
            }
            if(canDefend)
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
        else if(Input.GetButtonUp("Action1"))
        {
            Debug.Log("End defending");
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

    /// <param name="name">Name of Participant</param>
    IEnumerator MoveParticipant(string name)
    {
        attackFinished = false;
        damageFinished = false;
        if (currentTurn != CurrentTurn.Enemy1)
        {
            RaiseIcons();
        }

        float destination;

        Vector3 moveTo;

        GameObject character = GameObject.Find(name);

        Animator anim = character.GetComponentInChildren<SpriteRenderer>().GetComponent<Animator>();

        if (currentTurn == CurrentTurn.Enemy1)
        {
            destination = GameObject.Find("Agatha").transform.position.x + 1;
            spriteAnim = GameObject.Find("Agatha").transform.GetChild(0).GetComponent<Animator>();
        }
        else
        {
            destination = GameObject.Find("Jackalope").transform.position.x - 1;
            SetCamFocus(character, GameObject.Find("Jackalope"));
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


        if (currentTurn != CurrentTurn.Enemy1)
        {
            actionCommand.ActivateActionCommand(destination);

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

        anim.SetBool("Walking", true);

        while (character.transform.position.x != destination)
        {
            moveTo = Vector3.MoveTowards(character.transform.position, new Vector3(destination, character.transform.position.y, character.transform.position.z), walkingRate * Time.fixedDeltaTime);
            character.GetComponent<Rigidbody>().MovePosition(moveTo);
            yield return new WaitForSeconds(0.01f);
        }
        anim.SetBool("Walking", false);
        anim.SetTrigger("Attacking");

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
        if (currentTurn != CurrentTurn.Enemy1) { yield return new WaitUntil(() => damageFinished); }
        yield return new WaitForSeconds(.2f);// delay before starting to walk

        anim.SetBool("Walking", true);

        successBonusDamage = 0;

        //anim.SetBool("Attacking", false);
        character.GetComponent<Animator>().SetTrigger("Flip");
        character.GetComponent<Animator>().SetBool("TurningLeft", true);

        for (int i = 0; i < participants.Count; i++)
        {
            if (participants[i].charName == name)
            {
                destination = participants[i].homePosition.x;
                break;
            }
        }

        while (character.transform.position.x != destination)
        {
            moveTo = Vector3.MoveTowards(character.transform.position, new Vector3(destination, character.transform.position.y, character.transform.position.z), walkingRate * Time.fixedDeltaTime);
            character.GetComponent<Rigidbody>().MovePosition(moveTo);
            yield return new WaitForSeconds(0.01f);
        }

        character.GetComponent<Animator>().SetTrigger("Flip");
        character.GetComponent<Animator>().SetBool("TurningLeft", false);
        anim.SetBool("Walking", false);
        
        ChangeTurn();
    }


    public void TakeDamage(int damageAmount)
    {
        participants[2].currentHealth -= damageAmount;
        Debug.Log(participants[2].charName + " has taken " + damageAmount);

        if (participants[2].currentHealth <= 0)
        {
            FindObjectOfType<BattleStartTrigger>().EndBattle();
        }
    }
    public void TakeDamage()
    {
        if (currentTurn == CurrentTurn.Enemy1)
        {
            int damageReduction = 0;

            if (reduceDamage)
                damageReduction = 1;

            int damage = (successBonusDamage + participants[2].attack) - (participants[0].defense + damageReduction);
            
            participants[0].currentHealth -= damage;
            mainHp.text = participants[0].currentHealth.ToString() + "/" + participants[0].maxHealth.ToString();

            spriteAnim.SetTrigger("Hit");
        }
        else
        {
            int damage = (successBonusDamage + participants[0].attack) - participants[0].defense;
            participants[2].currentHealth -= damage;
            Debug.Log(participants[2].charName + " has taken " + damage);

            CreateDamageBar(damage, participants[2].homePosition);

            if (participants[2].currentHealth <= 0)
            {
                FindObjectOfType<BattleStartTrigger>().EndBattle();
            }
        }
    }
    
    /// <param name="bonus">If extra damage is applied or not</param>
    public void TriggerBattleAction(bool bonus)
    {
        if (bonus && currentTurn != CurrentTurn.Enemy1)
            successBonusDamage = 1;
        else
            successBonusDamage = 0;

        if(currentTurn == CurrentTurn.Main)
        {
            GameObject.Find("Agatha").transform.GetChild(0).GetComponent<Animator>().SetTrigger("AttackCharged");
        }

        AttackFinisher();
    }

    public void ChangeTurn()
    {
        if(currentTurn == CurrentTurn.Main)
        {
            currentTurn = CurrentTurn.Partner;

            battleIcons.transform.position = new Vector3(GameObject.Find("Faylee").transform.position.x, battleIcons.transform.position.y, battleIcons.transform.position.z);
            LowerIcons();
        }
        else if (currentTurn == CurrentTurn.Partner)
        {
            currentTurn = CurrentTurn.Enemy1;
            StartCoroutine(MoveParticipant("Jackalope"));
        }
        else
        {
            currentTurn = CurrentTurn.Main;

            battleIcons.transform.position = new Vector3(GameObject.Find("Agatha").transform.position.x, battleIcons.transform.position.y, battleIcons.transform.position.z);
            LowerIcons();
        }
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
    public static void AddParticipant(string name, int maxHp, int currentHp, int atk, int def)
    {
        participants.Add(new BattleParticipant(name, maxHp, currentHp, atk, def));
    }
    //for characters who start at max, most enemies
    public static void AddParticipant(string name, int maxHp, int atk, int def)
    {
        participants.Add(new BattleParticipant(name, maxHp, maxHp, atk, def));
    }
    //for player characters
    public static void AddParticipant(PartyStats partyMember, int enchantmentAttack, int enchantmentDefense)
    {
        participants.Add(new BattleParticipant(partyMember.charName, partyMember.maxHealth, partyMember.currentHealth, partyMember.baseDamage + enchantmentAttack, partyMember.defense + enchantmentDefense));
        //participants.Add(new BattleParticipant(name, maxHp, maxHp, atk, def));
    }
    
    public static void RemoveParticipants()
    {
        participants.Clear();
    }
}
