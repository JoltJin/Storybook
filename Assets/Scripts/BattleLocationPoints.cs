using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleLocationPoints : MonoBehaviour
{
    [SerializeField] private BattleLocationSets[] locations = new BattleLocationSets[1];
    [SerializeField] private SpecialBattleLocationSets[] specialLocations = new SpecialBattleLocationSets[1];
    [Serializable]public class BattleLocationSets
    {
        public Vector3 location;
        public BattleController.CurrentTurn characterType;
    }

    [Serializable]
    public class SpecialBattleLocationSets
    {
        public Vector3 location;
        public BattleController.SpecialBattleLocations specialLocation;
    }


    public Vector3 GetLocation(BattleController.CurrentTurn charType)
    {
        for (int i = 0; i < locations.Length; i++)
        {
            if (locations[i].characterType == charType)
            {
                return locations[i].location;
            }
        }
        return Vector3.zero;
    }

    public Vector3 GetSpecialLocation(BattleController.SpecialBattleLocations charLocation)
    {
        for (int i = 0; i < specialLocations.Length; i++)
        {
            if (specialLocations[i].specialLocation == charLocation)
            {
                return locations[i].location;
            }
        }
        return Vector3.zero;
    }
    //public Vector2 GetLocation(int numInBattle)
    //{
    //    return locations[numInBattle].location;
    //}

    public void SwapLocations(BattleController.CurrentTurn charType1, BattleController.CurrentTurn charType2) 
    {
        int location1 = 0, location2 = 0;

        for (int i = 0; i < locations.Length; i++)
        {
            if (locations[i].characterType == charType1)
            {
                location1 = i;
            }
            if (locations[i].characterType == charType2)
            {
                location2 = i;
            }
        }

        Vector3 holder = locations[location1].location;
        locations[location1].location = locations[location2].location;
        locations[location2].location = holder;
    }
    //public void SwapLocations(int firstSwap, int secondSwap)
    //{
    //    Vector3 holder = locations[firstSwap].location;
    //    locations[firstSwap].location = locations[secondSwap].location;
    //    locations[secondSwap].location = holder;
    //}
}
