using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectCoordinator : MonoBehaviour
{
    [SerializeField] private List<AudioClip> walkingSounds = new List<AudioClip>();
    [SerializeField] private AudioClip hittingLight, hittingHard, beingHitLight, beingHitHard, guarded;
    [SerializeField] AudioSource soundEffects;


    public void PlayWalkingSounds()
    {
        soundEffects.clip = walkingSounds[Random.Range(0, walkingSounds.Count)];
        soundEffects.pitch = 1f + Random.Range(-.25f, .25f);
        soundEffects.Play();
    }

    public void PlayLightHitSound()
    {
        soundEffects.clip = hittingLight;
        soundEffects.Play();
    }

    public void PlayLightDamageSound()
    {
        soundEffects.clip = beingHitLight;
        soundEffects.Play();
    }

    public void PlayGuardedSound()
    {
        soundEffects.clip = guarded;
        soundEffects.Play();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (soundEffects == null)
        {
            TryGetComponent(out soundEffects);

            if(soundEffects == null)
            {
                Debug.Log("Needed to add audio source on " + gameObject);
                gameObject.AddComponent<AudioSource>();
                soundEffects = GetComponent<AudioSource>();
            }
        }
        //player = GetComponentInParent<PlayerController>();

        //if(player == null)
        //{
        //    Debug.Log("SoundEffect not connected to player countroller");
        //}
    }

    //public void PlayWalkingSounds()
    //{
    //    player.PlaySounds();
    //}
}
