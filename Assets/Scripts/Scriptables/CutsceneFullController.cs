using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum dialogueEmotes
{
    Normal,
    Turning,
    Shocked,
    Angry,
}
public enum CutsceneCharacter
{
    Agatha,
    Faylee,
    Kael,
    Willow,
    Tavernkeep,
}
    public enum CutsceneType
    {
        Talking,
        Walking,
        Both
    }

public enum CutsceneWalkSpeed
{
    Slow = 2,
    Normal = 4,
    Fast = 6
}
[Serializable]
public abstract class CutsceneTester
{
    
    public abstract void CutsceneInteract(MonoBehaviour myMonoBehaviour, Action nextSegment);
    [HideInInspector]public int slot = 0;
    //[SerializeField]
}
[Serializable]
public class CutsceneFullController : CutsceneTester
{
    [HideInInspector] public bool isUnfolded;
    public string animationComment = "";
    [SerializeField] protected CutsceneType cutsceneType;
    //[SerializeField] public  CutsceneInterface[] cutsceneData = new CutsceneInterface[1];
    [SerializeField] private CutsceneDialogue[] cutsceneDialogue = new CutsceneDialogue[1];

    [SerializeField] private CutsceneLocations[] cutsceneLocations = new CutsceneLocations[1];

    private Action returnAction;
    private MonoBehaviour mono;
    public override void CutsceneInteract(MonoBehaviour myMonoBehaviour, Action next)
    {
        returnAction = next;
        mono = myMonoBehaviour;
        Action action;

        switch (cutsceneType)
        {
            case CutsceneType.Talking:
                if (slot >= cutsceneDialogue.Length - 1)
                {
                    action = returnAction;
                }
                else
                {
                    action = NextCutscene;
                }
                cutsceneDialogue[slot].CutsceneInteract(myMonoBehaviour, action);
                break;
            case CutsceneType.Walking:
                if (slot >= cutsceneLocations.Length - 1)
                {
                    action = returnAction;
                }
                else
                {
                    action = NextCutscene;
                }
                Debug.Log(slot);
                cutsceneLocations[slot].CutsceneInteract(myMonoBehaviour, action);
                break;
            case CutsceneType.Both:
                break;
        }
        slot = 0;
    }

    private void NextCutscene()
    {
        slot++;
        CutsceneInteract(mono, returnAction);
    }
}
[Serializable]
public class CutsceneDialogue : CutsceneTester
{
    [HideInInspector] public bool isUnfolded;
    [SerializeField] public CutsceneDialoguePairing[] dialogue = new CutsceneDialoguePairing[1];

    [SerializeField] private Vector3 lookAtPos;
    [SerializeField] dialogueEmotes dialogueEmote;

    public override void CutsceneInteract(MonoBehaviour myMonoBehaviour, Action nextSegment)
    {
        Debug.Log(slot);
        myMonoBehaviour.StartCoroutine(DialogueController(nextSegment, myMonoBehaviour));
    }

    IEnumerator DialogueController(Action next, MonoBehaviour myMonoBehaviour)
    {
        GameObject talkingCharacter = GameObject.Find(dialogue[slot].character.ToString());

        talkingCharacter.GetComponentInParent<CharacterAnimator>().Emote(dialogueEmote);
        if (dialogueEmote == dialogueEmotes.Turning)
        {
            yield return new WaitForSeconds(1);
        }
        else
        {
            yield return new WaitForSeconds(.5f);
        }

        talkingCharacter.GetComponentInParent<CharacterAnimator>().FlipAnimation(lookAtPos.x - talkingCharacter.transform.position.x);

        yield return new WaitForSeconds(0.4f);

        CutsceneCharacter lastChar = dialogue[0].character;
        for (int i = 0; i < dialogue.Length; i++)
        {
            if (lastChar != dialogue[i].character)
            {
                yield return new WaitForSeconds(0.5f);
            }
            TextboxController.Instance.SetPosition(GameObject.Find(dialogue[i].character.ToString()).transform, 1, GameObject.Find(dialogue[i].character.ToString()).GetComponentInParent<CharacterAnimator>());
            TextboxController.Instance.SetText(dialogue[i].text, dialogue[i].ender);

            while (!TextboxController.Instance.isFinished())
            {
                yield return new WaitForSeconds(0.1f);
            }

            
            lastChar = dialogue[i].character;
        }

        
        slot = 0;
        next();
        yield break;
    }
}
[Serializable]
public class CutsceneDialoguePairing
{
    [HideInInspector] public bool isUnfolded;
    public CutsceneCharacter character;
    public List<string> text = new List<string>();
    public textEnder ender;
}

[Serializable]
public class CutsceneLocations : CutsceneTester
{
    [HideInInspector] public bool isUnfolded;
    [SerializeField] public CutsceneMovementPairing[] locations = new CutsceneMovementPairing[1];

    public override void CutsceneInteract(MonoBehaviour myMonoBehaviour, Action next)
    {
        Debug.Log(slot);
        myMonoBehaviour.StartCoroutine(MoveCharacter(GameObject.Find(locations[slot].character.ToString()), (int)locations[slot].speedToLocation, next, myMonoBehaviour));
    }


    IEnumerator MoveCharacter(GameObject characterToMove, float walkingSpeed, Action next, MonoBehaviour myMonoBehaviour)
    {
        while (characterToMove.transform.position.x != locations[slot].location.x && characterToMove.transform.position.z != locations[slot].location.z)
        {
            Vector3 direction = (locations[slot].location - characterToMove.transform.position);
            direction = new Vector3(direction.x, characterToMove.transform.position.y, direction.z);

            characterToMove.GetComponent<CharacterController>().Move(walkingSpeed * Time.deltaTime * direction.normalized);
            characterToMove.GetComponent<PlayerController>().BasicAnimations(locations[slot].location.x - characterToMove.transform.position.x, locations[slot].location.z - characterToMove.transform.position.z);

            if (Mathf.Abs(locations[slot].location.x - characterToMove.transform.position.x) < 0.01f && Mathf.Abs(locations[slot].location.z - characterToMove.transform.position.z) < 0.01f)
            {
                characterToMove.transform.position = new Vector3(locations[slot].location.x, characterToMove.transform.position.y, locations[slot].location.z);
            }

            yield return null;
        }

        characterToMove.GetComponent<PlayerController>().BasicAnimations(0, 0);

        slot++;
        if (slot <= locations.Length - 1)
        {
            yield return new WaitForSeconds(1);
            myMonoBehaviour.StartCoroutine(MoveCharacter(GameObject.Find(locations[slot].character.ToString()), (int)locations[slot].speedToLocation, next, myMonoBehaviour));
            yield break;
        }
        else
        {
            Debug.Log("Walking Finished with slot " + slot);
            next();
            slot = 0;
            yield break;
        }


    }
}
[Serializable]
public class CutsceneMovementPairing
{
    [HideInInspector] public bool isUnfolded;
    public CutsceneCharacter character;
    public Vector3 location;
    public CutsceneWalkSpeed speedToLocation = CutsceneWalkSpeed.Slow;
}