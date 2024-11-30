using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneController : MonoBehaviour
{
    //[SerializeField] private CutsceneInterface[] cutsceneData = new CutsceneInterface[1];
    [SerializeField] private CutsceneFullController[] cutsceneData = new CutsceneFullController[1];

    private int slot = 0;
    // Start is called before the first frame update
    void Start()
    {
        slot = 0;
        if (cutsceneData == null || cutsceneData[0] == null)
        {
            Debug.Log("Need Location Data for " + gameObject);
            return;
        }
        PlayerData.inCutscene = true;

        cutsceneData[slot].slot = 0;
        cutsceneData[slot].CutsceneInteract(this, NextAction);

        slot++;
        //StartCoroutine(MoveCharacter(GameObject.Find(locationData.character.ToString()), 0, locationData.speedToLocation));

        


        //PlayerData.inCutscene = false;
    }

    private void NextAction()
    {
        if (slot <= cutsceneData.Length - 1)
        {
            Debug.Log(cutsceneData[slot]);
            cutsceneData[slot].CutsceneInteract(this, NextAction);
            slot++;
        }
        else
        {
            slot = 0;
            PlayerData.inCutscene = false;
        }
    }
}