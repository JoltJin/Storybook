using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEnemyCommunicator : MonoBehaviour
{
    private BattleController battleControl;
    public BattleController.CurrentTurn enemySlot;

    [SerializeField] private int[] IdleChoices = new int[0];
    [SerializeField] private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        battleControl = GetComponentInParent<BattleController>();
    }

    public void SetIdleChoice()
    {
        if(IdleChoices.Length == 0) 
        {
            Debug.Log("There are no idle choices");
            anim.SetInteger("IdleChoice", 0);
        }
        else
        {
            int choice = Random.Range(0,IdleChoices.Length);
            anim.SetInteger("IdleChoice", IdleChoices[choice]);
        }
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

    public void Dead()
    {
        battleControl.RemoveEnemy(enemySlot);
        //Destroy(gameObject.transform.parent);
    }
}
