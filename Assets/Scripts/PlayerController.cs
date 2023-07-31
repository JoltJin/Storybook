using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
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

    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        cam = Camera.main.transform;
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
        
        if(horizontal != 0 || vertical != 0)
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

    private void FlipAnimation(float direction)
    {
        if (direction > 0)
        {
            if (!FacingRight)
            {
                flipAnim.SetBool("TurningLeft", false);
                flipAnim.SetTrigger("Flip");

                FacingRight = true;
            }
        }
        else if (direction < 0)
        {
            if (FacingRight)
            {
                flipAnim.SetBool("TurningLeft", true);
                flipAnim.SetTrigger("Flip");

                FacingRight = false;
            }
        }
    }


}
