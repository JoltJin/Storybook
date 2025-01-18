using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Mathematics;
using System;
using UnityEngine.UI;
using UnityEngine.UIElements;

public enum textEnder
{
    Generic,
    Agatha,
    Faylee,
    Willow,
    Kael,
}
public enum TextboxType
{
    Small,
    Story,
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

    [SerializeField] private TextMeshProUGUI textbox;
    //[SerializeField] private Animator anim;
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

    private TextboxType textboxType;

    [SerializeField] private RectTransform m_parent;
    [SerializeField] private Camera m_camera;

    private float originalTextboxHeight,
                currentTextboxHeight;
    
    bool isChangingSize = false;
    private int lineCounter;
    private UnityEngine.UI.Image textboxImage;

    [SerializeField] private GameObject textboxArrow, textboxBounds;
    [SerializeField] private Sprite talkingLeftOfBox, talkingRightOfBox;
    private Transform characterTalking;
    [SerializeField] Camera uiCamera;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        textboxImage = GetComponent< UnityEngine.UI.Image >();
        textbox.text = "";
        textboxImage.color = new Color(textboxImage.color.r, textboxImage.color.g, textboxImage.color.b, 0);
        originalTextboxHeight = currentTextboxHeight = GetComponent<RectTransform>().sizeDelta.y;
        textboxArrow.SetActive(false);
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

    private void CheckVisibleLineCount(int lineCount)
    {
        //string holder = textbox.text;
        //string secondHolder = "";
        //string[] newText = holder.Split(" ");
        //bool endOfText = false;
        //for (int i = 0; i < newText.Length; i++)
        //{
        //    if (newText[i].Contains("<color=#00000000>"))
        //    {
        //        for (int j = 0; j <= i; j++)
        //        {
        //            if (i == j)
        //            {
        //                secondHolder += newText[j] + "</color>";
        //                break;

        //            }
        //            else
        //            {
        //                secondHolder += newText[j] + " ";
        //            }
        //        }
        //        break;
        //    }
        //    //else if (!newText[i].Contains("<color=#00000000>") && i == newText.Length - 2)
        //    //{
        //    //    endOfText = true;
        //    //}
        //}
        //textbox.text = secondHolder;
        //textbox.ForceMeshUpdate();
        //int lineCount = 0;
        //lineCount = textbox.textInfo.lineCount;
        //Debug.Log(lineCount);
        //textbox.text = holder;
        int increaseAmount = 0;
        lineCounter = lineCount;
        if (lineCount > 3)
        {
            increaseAmount = 20 * (lineCount - 3);
            currentTextboxHeight = originalTextboxHeight + increaseAmount;

            if (!isChangingSize)
            {
                StartCoroutine(ChangeTextboxSize(increaseAmount, .45f));
            }
        }
        else if (lineCount == 1)
        {
            if (currentTextboxHeight != originalTextboxHeight)
            {
                StartCoroutine(ChangeTextboxSize(0, .45f));
            }
        }
        //GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, currentTextboxHeight);
        //transform.localPosition = new Vector3(transform.localPosition.x, 157.553f - increaseAmount/2f, transform.localPosition.z);
    }

    // Update is called once per frame
    IEnumerator ChangeTextboxSize(int height, float time)
    {
        isChangingSize = true;
        currentTextboxHeight = originalTextboxHeight + height;
        Vector2 originalSize = GetComponent<RectTransform>().sizeDelta;
        //Vector3 originalPosition = transform.localPosition;
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(originalSize, new Vector2(GetComponent<RectTransform>().sizeDelta.x, currentTextboxHeight), elapsedTime / time);
            //transform.localPosition = Vector3.Lerp(originalPosition, new Vector3(transform.localPosition.x, originalPosition.y - height / 2f, transform.localPosition.z), elapsedTime / time);

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, currentTextboxHeight);
        //transform.localPosition = new Vector3(transform.localPosition.x, originalPosition.y - height / 2f, transform.localPosition.z);

        isChangingSize = false;
    }
    void KeepFullyOnScreen()
    {
        float arrowY = textboxArrow.transform.position.y;
        RectTransform textboxArrowRectTransform = textboxArrow.GetComponent<RectTransform>();
        Vector3 targetPositionScreenPoint = Camera.main.WorldToScreenPoint(characterTalking.position + (Vector3.right/3));

        if (targetPositionScreenPoint.x <= (Screen.width / 2))//checks where character is on the screen then adjusts placement of arrow to the side and flips if needed
        {
            textboxArrow.GetComponent<UnityEngine.UI.Image>().sprite = talkingLeftOfBox;
            targetPositionScreenPoint = Camera.main.WorldToScreenPoint(characterTalking.position + (Vector3.right / 3));
        }else
        {
            textboxArrow.GetComponent<UnityEngine.UI.Image>().sprite = talkingRightOfBox;
            targetPositionScreenPoint = Camera.main.WorldToScreenPoint(characterTalking.position + (Vector3.left / 3));
        }
            Vector3 cappedTargetScreenPosition = targetPositionScreenPoint;
        if (cappedTargetScreenPosition.x <= (Screen.width/2) -textboxBounds.GetComponent<RectTransform>().sizeDelta.x/2)
            cappedTargetScreenPosition.x = (Screen.width / 2) - textboxBounds.GetComponent<RectTransform>().sizeDelta.x / 2;
        if (cappedTargetScreenPosition.x >= (Screen.width / 2) + textboxBounds.GetComponent<RectTransform>().sizeDelta.x / 2)
            cappedTargetScreenPosition.x = (Screen.width / 2) + textboxBounds.GetComponent<RectTransform>().sizeDelta.x / 2;

        Vector3 pointerWorldPosition = m_camera.ScreenToWorldPoint(cappedTargetScreenPosition);
        textboxArrowRectTransform.position = pointerWorldPosition;
        textboxArrowRectTransform.localPosition = new Vector3 (textboxArrowRectTransform.localPosition.x, 0, textboxArrowRectTransform.position.z);
    }

    IEnumerator MoveTextboxArrow()
    {
        Vector2 anchoredPos;
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(textboxArrow.GetComponent<RectTransform>(), characterTalking.position, m_camera, out anchoredPos);
        //textboxArrow.GetComponent<RectTransform>().anchoredPosition = 
            KeepFullyOnScreen();
        yield return new WaitForEndOfFrame();
        StartCoroutine(MoveTextboxArrow());
    }
    IEnumerator OpenTextboxAnimation(float time)
    {
        textbox.text = "";
        Vector2 originalSize = GetComponent<RectTransform>().sizeDelta;
        Color originalColor = textboxImage.color;
        float elapsedTime = 0f;

        GetComponent<RectTransform>().sizeDelta = new Vector2(75, 75);
        while (elapsedTime < time)
        {
            GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(new Vector2(75, 75),originalSize, elapsedTime / time);
            textboxImage.color = Color.Lerp(originalColor, new Color(textboxImage.color.r, textboxImage.color.g, textboxImage.color.b, 1), elapsedTime / time);
            Debug.Log(GetComponent<RectTransform>().sizeDelta);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        textboxImage.color = new Color(textboxImage.color.r, textboxImage.color.g, textboxImage.color.b, 1);
        GetComponent<RectTransform>().sizeDelta = originalSize;

        isOpen = true;
        textWriterSingle = TextWriter.AddWriter_Static(textbox, message[messageNum], 0.05f, true, true, StopTalkingSound, EndSentenceSound, StartTalkingSound, EndTextboxSound, CheckVisibleLineCount);

        StartCoroutine(MoveTextboxArrow());
    }

    IEnumerator CloseTextboxAnimation(float time)
    {
        textbox.text = "";
        Vector2 originalSize = GetComponent<RectTransform>().sizeDelta;
        Color originalColor = textboxImage.color;
        float elapsedTime = 0f;

        while (elapsedTime < time)
        {
            GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(originalSize, new Vector2(75, 75), elapsedTime / time);
            textboxImage.color = Color.Lerp(originalColor, new Color(textboxImage.color.r, textboxImage.color.g, textboxImage.color.b, 0), elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        textboxImage.color = new Color(textboxImage.color.r, textboxImage.color.g, textboxImage.color.b, 0);
        GetComponent<RectTransform>().sizeDelta = originalSize;

        isOpen = false;

        PlayerController.isBusy = false;
    }
    void Update()
    {
        if (isOpen)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                if (textWriterSingle != null && textWriterSingle.IsActive())
                {
                    //textWriterSingle.WriteAllAndDestroy();

                    //textWriterSingle = null;
                }
                else if (textWriterSingle == null)
                {

                    textWriterSingle = TextWriter.AddWriter_Static(textbox, message[messageNum], 0.05f, true, true, StopTalkingSound, EndSentenceSound, StartTalkingSound, EndTextboxSound, CheckVisibleLineCount);
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
                        textWriterSingle = TextWriter.AddWriter_Static(textbox, message[messageNum], 0.05f, true, true, StopTalkingSound, EndSentenceSound, StartTalkingSound, EndTextboxSound, CheckVisibleLineCount);
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
        string indenter = "";
        for (int i = lineCounter; i < 3; i++)
        {
            indenter += "\n";
        }
        textbox.text = message[messageNum] + indenter + endingMark + visibleEnder;
        yield return new WaitForSeconds(.5f);
        textbox.text = message[messageNum] + indenter + endingMark + invisibleEnder;
        yield return new WaitForSeconds(.5f);
        StartCoroutine(EndingMarkFlash());
    }

    public void SetSize(TextboxType boxType, float height, CharacterAnimator characterAnimator)
    {
        textboxType = boxType;
        PlayerController.isBusy = true;

        //transform.SetParent(pos.transform);
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

        //transform.position = pos.position + Vector3.up * height;

        //anim.SetTrigger("Open");

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

    private void ResetTextbox()
    {
        textboxArrow.SetActive(true);
        currentTextboxHeight = originalTextboxHeight;
        textbox.text = "";
        Vector2 anchoredPos;
        RectTransform boxPos = GetComponent<RectTransform>();
        boxPos.sizeDelta = new Vector2(boxPos.sizeDelta.x, originalTextboxHeight);
        Vector3 textboxOffset = new Vector3(Screen.width/2, Screen.height * (4.5f/10f), 1);// distance of middle of text box from the top
        RectTransformUtility.ScreenPointToLocalPointInRectangle(m_parent, textboxOffset, m_camera, out anchoredPos);
        boxPos.anchoredPosition = anchoredPos;

        StartCoroutine(OpenTextboxAnimation(.4f));
    }

    public void SetText(List<string> txt, textEnder ender, TextboxType boxType, float height, CharacterAnimator characterAnimator, Transform character)
    {
        textboxType = boxType;
        PlayerController.isBusy = true;

        //transform.SetParent(pos.transform);
        this.characterAnimator = characterAnimator;
        characterTalking = character.transform;

        if (isOpen)
        {
            //CloseTextbox();
            return;
        }

        if (height == 0)
        {
            height = 1.25f;
            Debug.Log("Using default height");
        }

        //transform.position = pos.position + Vector3.up * height;

        //anim.SetTrigger("Open");


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

        
        ResetTextbox();
    }
    
    private void CloseTextbox()
    {
        //anim.SetTrigger("Close");
        StopAllCoroutines();
        textboxArrow.SetActive(false);
        StartCoroutine(CloseTextboxAnimation(.4f));
        //StartCoroutine(TextboxDelay());
    }

    //IEnumerator TextboxDelay()
    //{
    //    yield return new WaitForEndOfFrame();
    //    PlayerController.isBusy = false;
    //}
}
