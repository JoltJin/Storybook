using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu]
public class CutsceneDialogueScript : CutsceneInterface
{
    [SerializeField] private CutsceneDialoguePair[] dialogue = new CutsceneDialoguePair[1];

    [SerializeField] private Vector3 lookAtPos;
    [SerializeField] dialogueEmotes dialogueEmote;

    public override void CutsceneInteract(MonoBehaviour myMonoBehaviour, Action next)
    {
        myMonoBehaviour.StartCoroutine(DialogueController(next, myMonoBehaviour));
    }

    IEnumerator DialogueController(Action next, MonoBehaviour myMonoBehaviour)
    {
        GameObject talkingCharacter = GameObject.Find(dialogue[slot].character.ToString());

        talkingCharacter.GetComponentInParent<CharacterAnimator>().Emote(dialogueEmote);
        if(dialogueEmote == dialogueEmotes.Turning)
        {
            yield return new WaitForSeconds(1);
        }
        else
        {
            yield return new WaitForSeconds(.5f);
        }

        talkingCharacter.GetComponentInParent<CharacterAnimator>().FlipAnimation(lookAtPos.x - talkingCharacter.transform.position.x);

        yield return new WaitForSeconds(0.4f);

        TextboxController.Instance.SetPosition(GameObject.Find(dialogue[slot].character.ToString()).transform, 1, GameObject.Find(dialogue[slot].character.ToString()).GetComponentInParent<CharacterAnimator>());
        TextboxController.Instance.SetText(dialogue[slot].text, dialogue[slot].ender);

        while (!TextboxController.Instance.isFinished())
        {
            yield return new WaitForSeconds(0.1f);
        }
            slot = 0;
            next();
            yield break;
        //else
        //{
        //    yield return new WaitForSeconds(0.1f);
        //    myMonoBehaviour.StopAllCoroutines();
        //    myMonoBehaviour.StartCoroutine(DialogueController(next, myMonoBehaviour));
        //}
        
    }
}
[Serializable]
public class CutsceneDialoguePair
{
    public CutsceneCharacter character;
    public List<string> text = new List<string>();
    public textEnder ender;
}