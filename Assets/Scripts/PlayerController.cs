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

        //setting up needed variables
    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        cam = Camera.main.transform;
        dustParticles = GetComponentInChildren<ParticleSystem>().gameObject.transform;
    }

    /// <summary>
    /// Makes Hud visible or invisible based on activity of player
    /// </summary>
    /// <param name="fadeOut">If the Hud should fade away now or not</param>
    /// <returns></returns>
    IEnumerator HudDisplayTimer(bool fadeOut)
    {
        //fades in the hud
        if(!fadeOut)
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
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if(flipAnim != null)
        {
            FlipAnimation(horizontal);
        }

        if(!fader)
        {
            StopAllCoroutines();


            if (horizontal == 0 && vertical == 0)
            {
                if (!hudBar.activeInHierarchy)
                {
                    fader = true;
                    StartCoroutine(HudDisplayTimer(false));
                }
            }
            else
            {
                if (hudBar.activeInHierarchy)
                {
                    fader = true;
                    StartCoroutine(HudDisplayTimer(true));
                }
            }
        }


        BasicAnimations(horizontal, vertical);


        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * Time.deltaTime * playerSpeed);
        }




        //// Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
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
