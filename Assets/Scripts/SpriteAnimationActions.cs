using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimationActions : MonoBehaviour
{
    private EnemyAIBase enemy;
    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponentInParent<EnemyAIBase>();
    }
    

    public void ReduceLoopCount()
    {
        enemy.DecreaseAnimCounter();
    }
}
