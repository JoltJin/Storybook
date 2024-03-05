using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GridBrushBase;

public class NPCInteract : MonoBehaviour, IInterface, CharacterAnimator
{
    [SerializeField] private string text = "";
    [SerializeField] private textEnder ender;

    [SerializeField] private Animator anim;
    [SerializeField] private Animator spriteAnim;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// 
    /// 
    /// </summary>
    /// <param name="trans">Position to determine facing direction</param>
    public void Interact(Transform trans)
    {
        TextboxController.Instance.SetPosition(transform, 1, GetComponentInParent<CharacterAnimator>());
        TextboxController.Instance.SetText(text, ender);
    }

    public Transform GetTransform()
    {
        return transform;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerInteract>())
        {
            if (other.transform.position.x < gameObject.transform.position.x && !anim.GetBool("TurningLeft"))
            {
                anim.SetTrigger("Flip");
                anim.SetBool("TurningLeft", true);
            }
            else if (other.transform.position.x > gameObject.transform.position.x && anim.GetBool("TurningLeft"))
            {
                anim.SetTrigger("Flip");
                anim.SetBool("TurningLeft", false);
            }
        }
    }

    public void FlipAnimation(float direction)
    {
        //if (direction > 0)
        //{
        //    if (!FacingRight)
        //    {
        //        anim.SetBool("TurningLeft", false);
        //        anim.SetTrigger("Flip");

        //        dustParticles.transform.position = new Vector3(.15f, dustParticles.transform.position.y, dustParticles.transform.position.z);

        //        FacingRight = true;
        //    }
        //}
        //else if (direction < 0)
        //{
        //    if (FacingRight)
        //    {
        //        anim.SetBool("TurningLeft", true);
        //        anim.SetTrigger("Flip");

        //        dustParticles.transform.position = new Vector3(-.15f, dustParticles.transform.position.y, dustParticles.transform.position.z);

        //        FacingRight = false;
        //    }
        //}
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
        //if (horizontal != 0 || vertical != 0)
        //{
        //    if (vertical > 0)
        //        spriteAnim.SetBool("FacingForward", false);
        //    else if (vertical <= 0)
        //        spriteAnim.SetBool("FacingForward", true);

        //    spriteAnim.SetBool("isWalking", true);
        //    IsMoving = true;
        //}
        //else
        //{
        //    spriteAnim.SetBool("isWalking", false);
        //    IsMoving = false;
        //}
    }
}
