using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleEnemyCommunicator : MonoBehaviour
{
    private BattleController battleControl;
    public BattleController.CurrentTurn enemySlot;
    private SpriteRenderer sprite;

    [SerializeField] private int[] IdleChoices = new int[0];
    [SerializeField] private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        battleControl = GetComponentInParent<BattleController>();
        sprite = GetComponent<SpriteRenderer>();
    }

    public void SetIdleChoice()
    {
        if (IdleChoices.Length == 0)
        {
            Debug.Log("There are no idle choices");
            anim.SetInteger("IdleChoice", 0);
        }
        else
        {
            int choice = Random.Range(0, IdleChoices.Length);
            anim.SetInteger("IdleChoice", IdleChoices[choice]);
        }
    }

    public void BeginDefendableMoment() => battleControl.CanDefend();

    public void EndDefendableMoment() => battleControl.CannotDefend();

    public void EnemyAttackFinisher() => battleControl.AttackFinisher();

    public void DamagePlayer() => battleControl.TakeDamage();

    public void BeginSuperGuard()
    {

    }

    public void EndSuperGuard()
    {

    }

    public void Dead() => battleControl.RemoveEnemy(enemySlot);

    public void ColorRed() => sprite.color = new Color(1, .5f, .5f);

    public void ColorNormal() => sprite.color = Color.white;

    public void ColorTransparent()
    {
        StartCoroutine(ColorFader());
    }

    IEnumerator ColorFader()
    {
        float time = 0;
        float duration = .5f;
        Color baseColor = sprite.color;
        Color destAlpha = new Color(1, 1, 1, .5f);
        while (time < duration)
        {
            sprite.color = Color.Lerp(baseColor, destAlpha, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
    }
}
