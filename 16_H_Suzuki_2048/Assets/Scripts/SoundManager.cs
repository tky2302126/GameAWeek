using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum BGM 
{
    Main,
    ALL
}

public enum SE 
{
    cellMove,
    cellMerge,
    GameStart,
    GameOver,
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

        for(int i = 0; i < SEclips.Count; i++) 
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
        SEsources[(int)se].clip = SEclips[(int)se];
        SEsources[(int)se].Play();
    }

    private void Update()
    {
        if (BGMsource.isPlaying) 
        {
            var time = BGMsource.time;

            if (time>100.0f)
            {
                BGMsource.time = 0f;
            }
        }
    }
}
