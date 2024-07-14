using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdaptiveMusicMixerTrigger : MonoBehaviour
{
    [SerializeField]AdaptiveMusicMixer.musicType type;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            AdaptiveMusicMixer.instance.SwapTrack(type);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AdaptiveMusicMixer.instance.ReturnToLast();
        }
    }
}
