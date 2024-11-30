using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public abstract class CutsceneInterface: ScriptableObject
{
    public abstract void CutsceneInteract(MonoBehaviour myMonoBehaviour, Action next);
    public int slot = 0;
    public bool isTalking;
}