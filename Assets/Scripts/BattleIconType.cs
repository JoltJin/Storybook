using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public enum IconType
    {
        Main_Attack,
        Items,
        Tactics,
        Special_Attack
    }
public class BattleIconType : MonoBehaviour
{

    [SerializeField] private IconType icon;
    // Start is called before the first frame update
    void Start()
    {
        //set up to have icons change properly
    }
    

    public IconType GetIcon()
    {
        return icon;
    }
}
