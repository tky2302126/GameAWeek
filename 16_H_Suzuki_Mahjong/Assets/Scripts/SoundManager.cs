using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum BGM 
{
    Title,
    ShisenSho,
    ALL
}

public enum SE 
{
    select,
    failed,
    succeed,
    over,
    ALL
}

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField]
    private AudioSource BGMsource;

    [SerializeField]
    private List<AudioSource> SEsources;

    [SerializeField]
    private List<AudioClip> BGMclips;

    [SerializeField]
    private List<AudioClip> SEclips;


     protected override void Awake() 
    {
        base.Awake();
        BGMsource.loop = true;
        BGMsource.playOnAwake = false;

        for(int i = 0; i < SEsources.Count; i++) 
        {
            SEsources[i].loop = false;
            SEsources[i].playOnAwake = false;
        }
    }

    public void PlayBGM(BGM bgm) 
    {
        BGMsource.clip = BGMclips[(int)bgm];
        BGMsource.Play();
    }

    public void StopBGM() 
    {
        BGMsource?.Stop();
    }

    public void PlaySE(SE se) 
    {
        int sourceIndex=0;
        for(int i = 0;i < SEsources.Count;i++) 
        {
            if (!SEsources[i].isPlaying) { sourceIndex = i; break; }
        }

        SEsources[sourceIndex].clip = SEclips[(int)se];
        SEsources[sourceIndex].Play();
    }
}
