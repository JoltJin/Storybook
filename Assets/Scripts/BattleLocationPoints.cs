using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleLocationPoints : MonoBehaviour
{
    [SerializeField] private Vector3[] locations = new Vector3[1];
    
    public Vector3 GetLocation(int numInBattle)
    {
        return locations[numInBattle];
    }

    public void SwapLocations(int firstSwap, int secondSwap)
    {
        Vector3 holder = locations[firstSwap];
        locations[firstSwap] = locations[secondSwap];
        locations[secondSwap] = holder;
    }
}
