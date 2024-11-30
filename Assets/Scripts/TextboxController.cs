using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Mathematics;
using System;

public enum textEnder
{
    Generic,
    Agatha,
    Faylee,
    Willow,
    Kael,
}
public class TextboxController : MonoBehaviour
{
    private string
        alignment = "\n<align=right>",
        agathaEnder = " <sprite name=\"Agatha\"",
        fayleeEnder = " <sprite name=\"Faylee\"",
        willowEnder = " <sprite name=\"Willow\"",
        genericEnder = " <sprite name=\"Generic\"",
        kaelEnder = " <sprite name=\"Kael\"",
        invisibleEnder = " color=#FFFFFF00><color=#00000000>.",
        visibleEnder = "><color=#00000000>.";

    public static TextboxController Instance;

    [SerializeField] private TextMeshPro text;
    [SerializeField] private Animator anim;
    [SerializeField] private List<AudioClip> normalTalking = new List<AudioClip>();
    [SerializeField] private List<AudioClip> endSentence = new List<AudioClip>();
    [SerializeField] private List<AudioClip> endTalking = new List<AudioClip>();
    [SerializeField] private AudioSource audioSource;
    private int audioClipNum = 0;

    private TextWriter.TextWriterSingle textWriterSingle;
    private List<string> message = new List<string>();
    private int messageNum = 0;
    private string endingMark = "";

    private bool isOpen;
    private bool addedEnding = false;

    private CharacterAnimator characterAnimator = null;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    private void StartTalkingSound()
    {
        audioSource.loop = true;

        int rand = UnityEngine.Random.Range(0, normalTalking.Count);
        audioSource.clip = normalTalking[rand];
        audioSource.Play();
        characterAnimator.Talking();
    }

    private void StopTalkingSound()
    {
        audioSource.loop = false;
        audioSource?.Stop();
        characterAnimator.StopTalking();
    }

    private void EndSentenceSound()
    {
        audioSource.loop = false;
        int rand = UnityEngine.Random.Range(0, endSentence.Count);
        audioSource.clip = endSentence[rand];
        audioSource.Play();
        characterAnimator.StopTalking();
    }

    private void EndTextboxSound()
    {
        audioSource.loop = false;
        int rand = UnityEngine.Random.Range(0, endTalking.Count);
        audioSource.clip = endTalking[rand];
        audioSource.Play();
        characterAnimator.StopTalking();
    }

    // Update is called once per frame
    void Update()
    {
        if (isOpen)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                if (textWriterSingle != null && textWriterSingle.IsActive())
                {
                    textWriterSingle.WriteAllAndDestroy();

                    //textWriterSingle = null;
                }
                else if (textWriterSingle == null)
                {

                    textWriterSingle = TextWriter.AddWriter_Static(text, message[messageNum], 0.05f, true, true, StopTalkingSound, EndSentenceSound, StartTalkingSound, EndTextboxSound);
                }
                else if (!textWriterSingle.IsActive())
                {
                    StopAllCoroutines();
                    textWriterSingle = null;
                    if (messageNum == message.Count - 1)
                    {
                        messageNum = -1;
                        CloseTextbox();
                    }
                    else
                    {
                        messageNum++;
                        addedEnding = false;
                        textWriterSingle = TextWriter.AddWriter_Static(text, message[messageNum], 0.05f, true, true, StopTalkingSound, EndSentenceSound, StartTalkingSound, EndTextboxSound);
                    }
                }
            }
        }

        if (textWriterSingle != null && !textWriterSingle.IsActive() && !addedEnding)
        {
            StartCoroutine(EndingMarkFlash());
            addedEnding = true;
        }
        /*
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StopAllCoroutines();
            SetPosition(FindObjectOfType<PlayerController>().gameObject.transform, 1, transform.GetComponentInParent<CharacterAnimator>());
            SetText("Hmm... The last thing I remember is opening the book and getting pulled in.", textEnder.Agatha);
            audioClipNum = 0;
            textWriterSingle = TextWriter.AddWriter_Static(text, message, 0.05f, true, true, StopTalkingSound, EndSentenceSound, StartTalkingSound, EndTextboxSound);
            StartTalkingSound();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StopAllCoroutines();
            SetPosition(FindObjectOfType<PlayerController>().gameObject.transform, 1, transform.GetComponentInParent<CharacterAnimator>());
            SetText("Am I really inside the book?", textEnder.Agatha);
            audioClipNum = 10;
            textWriterSingle = TextWriter.AddWriter_Static(text, message, 0.05f, true, true, StopTalkingSound, EndSentenceSound, StartTalkingSound, EndTextboxSound);
            StartTalkingSound();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StopAllCoroutines();
            SetPosition(FindObjectOfType<PlayerController>().gameObject.transform, 1, transform.GetComponentInParent<CharacterAnimator>());
            SetText("Everything feels so flat... I feel strangely light too..", textEnder.Agatha);
            audioClipNum = 1;
            textWriterSingle = TextWriter.AddWriter_Static(text, message, 0.05f, true, true, StopTalkingSound, EndSentenceSound, StartTalkingSound, EndTextboxSound);
            StartTalkingSound();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            StopAllCoroutines();
            SetPosition(FindObjectOfType<PlayerController>().gameObject.transform, 1, transform.GetComponentInParent<CharacterAnimator>());
            SetText("W-wait are all my thoughts being written out?!\r\nEr.. Just what is up with this world??", textEnder.Agatha);
            audioClipNum = 2;
            textWriterSingle = TextWriter.AddWriter_Static(text, message, 0.05f, true, true, StopTalkingSound, EndSentenceSound, StartTalkingSound, EndTextboxSound);
            StartTalkingSound();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            StopAllCoroutines();
            SetPosition(FindObjectOfType<PlayerController>().gameObject.transform, 1, transform.GetComponentInParent<CharacterAnimator>());
            SetText("I can see the table I set the book on, maybe I can get out of...", textEnder.Agatha);
            audioClipNum = 3;
            textWriterSingle = TextWriter.AddWriter_Static(text, message, 0.05f, true, true, StopTalkingSound, EndSentenceSound, StartTalkingSound, EndTextboxSound);
            StartTalkingSound();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            StopAllCoroutines();
            SetPosition(FindObjectOfType<PlayerController>().gameObject.transform, 1, transform.GetComponentInParent<CharacterAnimator>());
            SetText("There's some kind of barrier... Of course it couldn't be that easy...", textEnder.Agatha);
            audioClipNum = 4;
            textWriterSingle = TextWriter.AddWriter_Static(text, message, 0.05f, true, true, StopTalkingSound, EndSentenceSound, StartTalkingSound, EndTextboxSound);
            StartTalkingSound();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            StopAllCoroutines();
            SetPosition(FindObjectOfType<PlayerController>().gameObject.transform, 1, transform.GetComponentInParent<CharacterAnimator>());
            SetText("Aww its such a cute rabbi-!", textEnder.Agatha);
            audioClipNum = 5;
            textWriterSingle = TextWriter.AddWriter_Static(text, message, 0.05f, true, true, StopTalkingSound, EndSentenceSound, StartTalkingSound, EndTextboxSound);
            StartTalkingSound();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            StopAllCoroutines();
            SetPosition(transform.parent.gameObject.transform, 1, transform.GetComponentInParent<CharacterAnimator>());
            SetText("I didn't expect to be fighting you, but I guess I have no choice!", textEnder.Agatha);
            audioClipNum = 6;
            textWriterSingle = TextWriter.AddWriter_Static(text, message, 0.05f, true, true, StopTalkingSound, EndSentenceSound, StartTalkingSound, EndTextboxSound);
            StartTalkingSound();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            StopAllCoroutines();
            SetPosition(transform.parent.gameObject.transform, 1, transform.GetComponentInParent<CharacterAnimator>());
            SetText("Let's see what I should do..", textEnder.Agatha);
            audioClipNum = 7;
            textWriterSingle = TextWriter.AddWriter_Static(text, message, 0.05f, true, true, StopTalkingSound, EndSentenceSound, StartTalkingSound, EndTextboxSound);
            StartTalkingSound();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            StopAllCoroutines();
            SetPosition(transform.parent.gameObject.transform, 1, transform.GetComponentInParent<CharacterAnimator>());
            SetText("Woah that felt stronger than normal! I guess the gauge has something to do with it?", textEnder.Agatha);
            audioClipNum = 8;
            textWriterSingle = TextWriter.AddWriter_Static(text, message, 0.05f, true, true, StopTalkingSound, EndSentenceSound, StartTalkingSound, EndTextboxSound);
            StartTalkingSound();
        }
        else if (Input.GetKeyDown(KeyCode.Minus))
        {
            StopAllCoroutines();
            SetPosition(transform.parent.gameObject.transform, 1, transform.GetComponentInParent<CharacterAnimator>());
            SetText("Hey that was so close, that could have really hurt!", textEnder.Agatha);
            audioClipNum = 9;
            textWriterSingle = TextWriter.AddWriter_Static(text, message, 0.05f, true, true, StopTalkingSound, EndSentenceSound, StartTalkingSound, EndTextboxSound);
            StartTalkingSound();
        }

        */
    }

    IEnumerator EndingMarkFlash()
    {
        text.text = message[messageNum] + endingMark + visibleEnder;
        yield return new WaitForSeconds(.5f);
        text.text = message[messageNum] + endingMark + invisibleEnder;
        yield return new WaitForSeconds(.5f);
        StartCoroutine(EndingMarkFlash());
    }

    public void SetPosition(Transform pos, float height, CharacterAnimator characterAnimator)
    {

        PlayerController.isBusy = true;

        transform.SetParent(pos.transform);
        this.characterAnimator = characterAnimator;

        if(isOpen)
        {
            //CloseTextbox();
            return;
        }

        if (height == 0)
        {
            height = 1.25f;
            Debug.Log("Using default height");
        }

        transform.position = pos.position + Vector3.up * height;

        anim.SetTrigger("Open");

        isOpen = true;
    }

    public bool isFinished()
    {
        if(messageNum == -1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void SetText(List<string> txt, textEnder ender)
    {
        message = txt;
        
        messageNum = 0;
        addedEnding = false;
        string finisher = alignment;

        switch(ender)
        {
            case textEnder.Agatha:
                finisher += agathaEnder;
                break;
                case textEnder.Faylee:
                finisher += fayleeEnder;
                break;
            case textEnder.Willow:
                finisher += willowEnder;
                break;
            case textEnder.Kael:
                finisher += kaelEnder;
                break;
            case textEnder.Generic:
                finisher += genericEnder;
                //finisher = "";
                break;
        }

        //Hi there <sprite name="Agatha" color=#FFFFFF00> set it to transparent
        endingMark =  finisher;

        textWriterSingle = TextWriter.AddWriter_Static(text, message[messageNum], 0.05f, true, true, StopTalkingSound, EndSentenceSound, StartTalkingSound, EndTextboxSound);
    }
    
    private void CloseTextbox()
    {
        anim.SetTrigger("Close");
        StopAllCoroutines();
        isOpen = false;
        StartCoroutine(TextboxDelay());
    }

    IEnumerator TextboxDelay()
    {
        yield return new WaitForEndOfFrame();
        PlayerController.isBusy = false;
    }
}
