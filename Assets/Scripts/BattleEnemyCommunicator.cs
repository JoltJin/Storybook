using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEnemyCommunicator : MonoBehaviour
{
    private BattleController battleControl;
    // Start is called before the first frame update
    void Start()
    {
        battleControl = GetComponentInParent<BattleController>();
    }

    public void BeginDefendableMoment()
    {
        battleControl.CanDefend();
    }

    public void EndDefendableMoment()
    {
        battleControl.CannotDefend();
    }

    public void EnemyAttackFinisher()
    {
        battleControl.AttackFinisher();
    }

    public void DamagePlayer()
    {
        battleControl.TakeDamage();
    }

    public void BeginSuperGuard()
    {

    }

    public void EndSuperGuard()
    {

    }
}
