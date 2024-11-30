using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CutsceneLocationsScript : CutsceneInterface
{
    [SerializeField] private CutsceneMovementPair[] locations = new CutsceneMovementPair[1];

    

    public override void CutsceneInteract(MonoBehaviour myMonoBehaviour, Action next)
    {
        myMonoBehaviour.StartCoroutine(MoveCharacter(GameObject.Find(locations[slot].character.ToString()), locations[slot].speedToLocation, next, myMonoBehaviour));
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
            myMonoBehaviour.StartCoroutine(MoveCharacter(GameObject.Find(locations[slot].character.ToString()), locations[slot].speedToLocation, next, myMonoBehaviour));
            yield break;
        }
        else
        {
            Debug.Log("Walking Finished");
            next();
            slot = 0;
            yield break;
        }


    }
}
[Serializable]
public class CutsceneMovementPair
{
    public CutsceneCharacter character;
    public Vector3 location;
    public float speedToLocation = 2f;
}