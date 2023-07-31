using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRadiusTrigger : MonoBehaviour
{
    [SerializeField] private EnemyAIBase enemy;
    // Start is called before the first frame update

    private void OnTriggerStay(Collider other)
    {
        enemy.EnterZone(other);
    }
    private void OnTriggerExit(Collider other)
    {
        enemy.LeaveZone(other);
    }
}
