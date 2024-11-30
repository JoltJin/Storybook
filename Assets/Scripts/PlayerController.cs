using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerController : MonoBehaviour, CharacterAnimator
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;

    private Transform cam;

    public float turnSmoothTime = 0.0f;
    private float turnSmoothVelocity;
    [HideInInspector] public bool FacingRight { get; private set; } = true;
    [HideInInspector] public bool IsMoving { get; private set; } = true;
    [SerializeField] Animator flipAnim;
    [SerializeField] Animator spriteAnim;

    private Transform dustParticles;

    [SerializeField] private GameObject hudBar;
    private bool fader;

    public static bool isBusy = false;
    public static bool canBattle = true;

    private bool isAirborne = false;

    [SerializeField] private Material screenshotMat;
    [SerializeField] private LayerMask basicMask;
        RenderTexture screenshotRend;

    float lastFrameY;
    // Setting up needed variables
    private void Awake()
    {
        if (PlayerData.party.Count == 0)
        {
            PlayerData.party.Add(new PartyStats(TeammateNames.Agatha, 10, 1, 0));
            PlayerData.party.Add(new PartyStats(TeammateNames.Faylee, 10, 1, 0));
        }

        ///
        if (screenshotMat != null)
        {
            screenshotMat.SetTexture("_BaseMap", screenshotRend);
        }

        screenshotMat.color = new Color(1, 1, 1, 1);

        ///



        controller = gameObject.GetComponent<CharacterController>();
        cam = Camera.main.transform;
        dustParticles = GetComponentInChildren<ParticleSystem>().gameObject.transform;
        StartCoroutine(PreviousYLocation());
    }

    private void Start()
    {
        ///
        Camera.main.targetTexture = screenshotRend;
        Camera.main.cullingMask = basicMask;
        Camera.main.Render();
        Camera.main.targetTexture = null;

        ///
    }


    /// <summary>
    /// Makes Hud visible or invisible based on activity of player
    /// </summary>
    /// <param name="fadeOut">If the Hud should fade away now or not</param>
    /// <returns></returns>
    IEnumerator HudDisplayTimer(bool fadeOut)
    {
        //fades in the hud
        if (!fadeOut)
        {
            hudBar.SetActive(true);

            //delay so it doesn't come on every time you stop
            yield return new WaitForSeconds(2.5f);

            while (hudBar.GetComponent<CanvasGroup>().alpha < 1)
            {
                hudBar.GetComponent<CanvasGroup>().alpha += 3 * Time.fixedDeltaTime;
                yield return new WaitForSecondsRealtime(0.02f);
            }
        }
        else
        {
            while (hudBar.GetComponent<CanvasGroup>().alpha > 0)
            {
                hudBar.GetComponent<CanvasGroup>().alpha -= 3 * Time.fixedDeltaTime;
                yield return new WaitForSecondsRealtime(0.01f);//set to half the delay for a faster clean screen
            }

            hudBar.SetActive(false);
        }

        fader = false;
    }

    void Update()
    {
        //if(Input.GetButtonDown("Jump"))
        //{
        //    Camera.main.targetTexture = screenshotRend;
        //    Camera.main.cullingMask = basicMask;
        //    Camera.main.Render();
        //    Camera.main.targetTexture = null;
        //    return;
        //}


        Vector3 moveVector = Vector3.zero;

        if (!isBusy && !PlayerData.inCutscene)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

            //if (!fader)
            //{
            //    StopAllCoroutines();


            //    if (horizontal == 0 && vertical == 0)
            //    {
            //        if (!hudBar.activeInHierarchy)
            //        {
            //            fader = true;
            //            StartCoroutine(HudDisplayTimer(false));
            //        }
            //    }
            //    else
            //    {
            //        if (hudBar.activeInHierarchy)
            //        {
            //            fader = true;
            //            StartCoroutine(HudDisplayTimer(true));
            //        }
            //    }
            //}

            BasicAnimations(horizontal, vertical);


            if (direction.magnitude >= 0.01f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                //controller.Move(playerSpeed * Time.deltaTime * moveDir.normalized);

                moveVector = playerSpeed * Time.deltaTime * moveDir.normalized;
            }

            //// Changes the height position of the player..
            if (Input.GetButtonDown("Jump") && groundedPlayer)
            {
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);

                spriteAnim.SetTrigger("Jumping");
                groundedPlayer = false;
                isAirborne = true;
            }
        }


        //Debug.Log(lastFrameY - transform.position.y);
        if (lastFrameY > transform.position.y + .1f)
        {
            groundedPlayer = false;
            spriteAnim.SetTrigger("Airborne");
            isAirborne = true;
        }
        //groundedPlayer = controller.isGrounded;
        if (controller.isGrounded && playerVelocity.y < 0.01f)
        {
            groundedPlayer = true;
            playerVelocity.y = 0.01f;
        }
        
        if (isAirborne && groundedPlayer && playerVelocity.y < 0.011f /*|| playerVelocity.y == 0 && spriteAnim.GetBool("Airborne")*/)
        {

            //Debug.Log("Landing");
            spriteAnim.SetTrigger("Landing");
            isAirborne = false;
        }
        //if (playerVelocity.y < -1.59f)
        //{
        //    groundedPlayer = false;
        //    Debug.Log("here");
        //    spriteAnim.SetTrigger("Airborne");
        //    isAirborne = true;
        //}

        playerVelocity.y += gravityValue * Time.deltaTime;

        moveVector += (playerVelocity * Time.deltaTime);

        controller.Move(moveVector);

    }

    IEnumerator PreviousYLocation()
    {
        lastFrameY = transform.position.y;
        yield return new WaitForSeconds(.1f);
        StartCoroutine(PreviousYLocation());
    }


    public void FlipAnimation(float direction)
    {
        if (direction > 0)
        {
            if (!FacingRight)
            {
                flipAnim.SetBool("TurningLeft", false);
                flipAnim.SetTrigger("Flip");

                dustParticles.transform.position = new Vector3(.15f, dustParticles.transform.position.y, dustParticles.transform.position.z);

                FacingRight = true;
            }
        }
        else if (direction < 0)
        {
            if (FacingRight)
            {
                flipAnim.SetBool("TurningLeft", true);
                flipAnim.SetTrigger("Flip");

                dustParticles.transform.position = new Vector3(-.15f, dustParticles.transform.position.y, dustParticles.transform.position.z);

                FacingRight = false;
            }
        }
    }

    public void Talking()
    {
        spriteAnim.SetBool("isTalking", true);
    }

    public void StopTalking()
    {
        spriteAnim.SetBool("isTalking", false);
    }


    public void BasicAnimations(float horizontal, float vertical)
    {
        if (flipAnim != null)
        {
            FlipAnimation(horizontal);
        }
        if (!isAirborne)
        {
            if (horizontal != 0 || vertical != 0)
            {
                if (vertical > 0)
                    spriteAnim.SetBool("FacingForward", false);
                else if (vertical <= 0)
                    spriteAnim.SetBool("FacingForward", true);

                spriteAnim.SetBool("isWalking", true);
                IsMoving = true;
            }
            else
            {
                spriteAnim.SetBool("isWalking", false);
                IsMoving = false;
            }
        }
        
    }

    public void Emote(dialogueEmotes emote)
    {
        switch (emote)
        {
            case dialogueEmotes.Normal:
                spriteAnim.SetBool("Angry", false);
                spriteAnim.SetBool("Shocked", false);
                break;
            case dialogueEmotes.Angry:
                spriteAnim.SetBool("Angry", true);
                break;
            case dialogueEmotes.Turning:
                StartCoroutine(LookingAround());
                break;
            case dialogueEmotes.Shocked:
                spriteAnim.SetBool("Shocked", false);
                break;
        }
    }

    IEnumerator LookingAround()
    {
        spriteAnim.speed = .25f;
        //FlipAnimation(-1);
        BasicAnimations(-1, -1);
        BasicAnimations(0, 0);
        yield return new WaitForSeconds(.4f);
        BasicAnimations(1, -1);
        BasicAnimations(0, 0);
        yield return new WaitForSeconds(.4f);
        spriteAnim.speed = 1;
    }
}
