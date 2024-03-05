using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePlayerCommunicator : MonoBehaviour
{
    private BattleController battleControl;

    private SpriteRenderer sprite;
    // Start is called before the first frame update
    void Start()
    {
        battleControl = GetComponentInParent<BattleController>();
        sprite = GetComponent<SpriteRenderer>();
    }

    public void BeginAttackBonusMoment()
    {
        battleControl.StartBonusAttackWindow(GetComponent<Animator>());
    }
    public void EndAttackBonusMoment()
    {
        battleControl.EndBonusAttackWindow();
    }

    public void ClearSpriteRef()
    {
        battleControl.ClearSpriteRef();
    }


    /// <summary>
    /// Types of commands to show
    /// </summary>


    public void HoldLeft()
    {
        battleControl.HoldLeft();
    }

    public void TimedA()
    {
        battleControl.TimedA();
    }

    public void DamageEnemy()
    {
        Debug.Log(battleControl);
        battleControl.TakeDamage();
    }
}
