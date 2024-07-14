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
        battleControl.TakeDamage();
    }

    public void ColorRed()
    {
        sprite.color = new Color(1, .5f, .5f);
    }

    public void ColorNormal() 
    {
        sprite.color = Color.white;
    
    }
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
