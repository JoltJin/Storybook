using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        SceneChange,
        Both_Unused
    }

/// <summary>
/// the name of the scene's file itself
/// </summary>
public enum CutsceneSceneType
{
    Battle_Scene
    //Forest,
    //Forest_Battle
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

    [SerializeField] private CutsceneSceneChange[] cutsceneSceneChange = new CutsceneSceneChange[1];

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
                cutsceneLocations[slot].CutsceneInteract(myMonoBehaviour, action);
                break;
            case CutsceneType.SceneChange:
                cutsceneSceneChange[slot].CutsceneInteract(myMonoBehaviour, returnAction);
                break;
            case CutsceneType.Both_Unused:
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
public class CutsceneSceneChange : CutsceneTester
{
    [HideInInspector] public bool isUnfolded;
    [HideInInspector] public bool isBattleScene;
    [SerializeField] private EnemyPoolScript enemyPool;
    [SerializeField] private CutsceneSceneType SceneToLoad;

    public override void CutsceneInteract(MonoBehaviour myMonoBehaviour, Action nextSegment)
    {
        myMonoBehaviour.StartCoroutine(SceneController(nextSegment, myMonoBehaviour));
    }

    IEnumerator SceneController(Action next, MonoBehaviour myMonoBehaviour)
    {
        Time.timeScale = 0;

        BattleController.AddParticipant(PlayerData.party[0].charName.ToString(), BattleController.CurrentTurn.Main, PlayerData.party[0].maxHealth, PlayerData.party[0].currentHealth, PlayerData.party[0].baseDamage, PlayerData.party[0].defense);

        if (PlayerData.party.Count > 1)
        {
            BattleController.AddParticipant(PlayerData.party[1].charName.ToString(), BattleController.CurrentTurn.Partner, PlayerData.party[1].maxHealth, PlayerData.party[1].currentHealth, PlayerData.party[1].baseDamage, PlayerData.party[1].defense);
        }

        for (int i = 0; i <= enemyPool.enemy.Length - 1; i++)
        {
            BattleController.AddParticipant(enemyPool.enemy[i].enemyName, BattleController.CurrentTurn.Enemy1 + i, enemyPool.enemy[i].maxHealth, enemyPool.enemy[i].maxHealth, enemyPool.enemy[i].baseAttack, enemyPool.enemy[i].baseDefense);
        }

        BookTransitionController.Instance.BattleTransition(SceneToLoad.ToString());
        yield return new WaitForSeconds(1);
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
            //TextboxController.Instance.SetSize(TextboxType.Story/*GameObject.Find(dialogue[i].character.ToString()).transform*/, 1, GameObject.Find(dialogue[i].character.ToString()).GetComponentInParent<CharacterAnimator>());
            TextboxController.Instance.SetText(dialogue[i].text, dialogue[i].ender, TextboxType.Story, 1, GameObject.Find(dialogue[i].character.ToString()).GetComponentInParent<CharacterAnimator>(), GameObject.Find(dialogue[i].character.ToString()).transform);

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
    [SerializeField] public CutsceneMovementPairing[] movementCombo = new CutsceneMovementPairing[1];
    private List<CutsceneCharacter> charactersUsed = new List<CutsceneCharacter>();
    public override void CutsceneInteract(MonoBehaviour myMonoBehaviour, Action next)
    {
        charactersUsed.Clear();
        for (int i = 0; i < movementCombo.Length; i++)
        {
            if (charactersUsed.Count == 0)
            {
                myMonoBehaviour.StartCoroutine(MoveCharacter(GameObject.Find(movementCombo[i].character.ToString()), (int)movementCombo[i].speedToLocation, next, myMonoBehaviour, i));
            }
            else
            {
                for (int j = 0; j < charactersUsed.Count; j++)
                {
                    if (charactersUsed[j] == movementCombo[i].character)
                    {
                        Debug.Log("You have " + charactersUsed[j].ToString() + " moving multiple times at once!");
                        return;
                    }
                    else if(j == charactersUsed.Count -1)
                    {
                        myMonoBehaviour.StartCoroutine(MoveCharacter(GameObject.Find(movementCombo[i].character.ToString()), (int)movementCombo[i].speedToLocation, next, myMonoBehaviour, i));
                    }
                }
            }

            charactersUsed.Add(movementCombo[i].character);
        }

        //myMonoBehaviour.StartCoroutine(MoveCharacter(GameObject.Find(movementCombo[slot].character.ToString()), (int)movementCombo[slot].speedToLocation, next, myMonoBehaviour));
    }


    IEnumerator MoveCharacter(GameObject characterToMove, float walkingSpeed, Action next, MonoBehaviour myMonoBehaviour, int slotNum)
    {
        while (characterToMove.transform.position.x != movementCombo[slotNum].location.x && characterToMove.transform.position.z != movementCombo[slotNum].location.z)
        {
            Vector3 direction = (movementCombo[slotNum].location - characterToMove.transform.position);
            direction = new Vector3(direction.x, characterToMove.transform.position.y, direction.z);

            characterToMove.GetComponent<CharacterController>().Move(walkingSpeed * Time.deltaTime * direction.normalized);
            characterToMove.GetComponent<CharacterAnimator>().BasicAnimations(movementCombo[slotNum].location.x - characterToMove.transform.position.x, movementCombo[slotNum].location.z - characterToMove.transform.position.z);

            if (Mathf.Abs(movementCombo[slotNum].location.x - characterToMove.transform.position.x) < 0.01f && Mathf.Abs(movementCombo[slotNum].location.z - characterToMove.transform.position.z) < 0.01f)
            {
                characterToMove.transform.position = new Vector3(movementCombo[slotNum].location.x, characterToMove.transform.position.y, movementCombo[slotNum].location.z);
            }

            yield return null;
        }

        characterToMove.GetComponent<CharacterAnimator>().BasicAnimations(0, 0);

        for (int i = 0; i < charactersUsed.Count; i++)
        {
            if (charactersUsed[i] == movementCombo[slotNum].character)
            {
                Debug.Log("Removed " + charactersUsed[i]);
                charactersUsed.RemoveAt(i);
                break;
            }
        }
        Debug.Log(charactersUsed.Count);
        //slot++;
        if (charactersUsed.Count == 0)
        {
            Debug.Log("Walking Combo is finished");
            next();
            //slot = 0;
            yield break;
            
        }
        else
        {
            //yield return new WaitForSeconds(1);
            ////myMonoBehaviour.StartCoroutine(MoveCharacter(GameObject.Find(movementCombo[slot].character.ToString()), (int)movementCombo[slot].speedToLocation, next, myMonoBehaviour));
            //yield break;
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