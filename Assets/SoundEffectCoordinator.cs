using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectCoordinator : MonoBehaviour
{
    PlayerController player;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponentInParent<PlayerController>();

        if(player == null)
        {
            Debug.Log("SoundEffect not connected to player countroller");
        }
    }

    public void PlayWalkingSounds()
    {
        player.PlaySounds();
    }
}
