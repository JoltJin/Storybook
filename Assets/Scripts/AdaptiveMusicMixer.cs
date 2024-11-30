using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdaptiveMusicMixer : MonoBehaviour
{
    public enum musicType
    {
        Normal,
        Scary,
        Peaceful,
    }

    public static AdaptiveMusicMixer instance;

    private musicType lastType = musicType.Normal;
    private musicType currentType = musicType.Normal;

    [SerializeField] private MusicCombo[] musicCombo = new MusicCombo[1];
    [Serializable]public class MusicCombo
    {
        public musicType musicType;
        public AudioClip clip;
        public AudioSource audioSource;
    }

    private void OnEnable()
    {
        if(instance == null)
        {
            instance = this;
        }

        if(!GetComponent<AudioSource>())
        {
            for (int i = 0; i < musicCombo.Length; i++)
            {
                musicCombo[i].audioSource = gameObject.AddComponent<AudioSource>();
                musicCombo[i].audioSource.clip = musicCombo[i].clip;
                musicCombo[i].audioSource.volume = 0;
                musicCombo[i].audioSource.Play();
            }
        }
        else
        {
            for (int i = 0; i < musicCombo.Length; i++)
            {
                musicCombo[i].audioSource.volume = 0;
                musicCombo[i].audioSource.Stop();
                musicCombo[i].audioSource.Play();
            }
        }

        SwapTrack(musicType.Normal);
    }

    // Start is called before the first frame update
    void Start()
    {
            
    }

    public void SwapTrack(musicType type)
    {
        lastType = currentType;
        currentType = type;

        StopAllCoroutines();

        StartCoroutine(FadeTrack());
    }

    public void ReturnToLast()
    {
        musicType holder = currentType;
        currentType = lastType;
        lastType = holder;

        StopAllCoroutines();

        StartCoroutine(FadeTrack());
    }

    private IEnumerator FadeTrack()
    {
        float timeToFade = 2f;
        float timeElapsed = 0;

        int oldTrack = 0, newTrack = 0;

        for (int i = 0; i < musicCombo.Length; i++)
        {
            if (musicCombo[i].musicType == lastType)
            {
                oldTrack = i;
            }
            if (musicCombo[i].musicType == currentType)
            {
                newTrack = i;
            }
        }

        float oldTrackVol = musicCombo[oldTrack].audioSource.volume;
        float newTrackVol = musicCombo[newTrack].audioSource.volume;

        while(timeElapsed < timeToFade)
        {
            musicCombo[newTrack].audioSource.volume = Mathf.Lerp(newTrackVol, 1, timeElapsed / timeToFade);
            musicCombo[oldTrack].audioSource.volume = Mathf.Lerp(oldTrackVol, 0, timeElapsed / timeToFade);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        musicCombo[newTrack].audioSource.volume = 1;
        musicCombo[oldTrack].audioSource.volume = 0;
    }
}
