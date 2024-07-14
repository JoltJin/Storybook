using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public int maxHealth, baseDefense, baseAttack;

}