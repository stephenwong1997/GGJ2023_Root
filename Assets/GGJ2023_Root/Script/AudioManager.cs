using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] AudioSource[] BGMAudioSources;
    [SerializeField] AudioClip[] Level1BGM;
    [SerializeField] AudioClip[] Level2BGM;
    [SerializeField] AudioClip[] Level3BGM;
    [SerializeField] AudioClip[] Level4BGM;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void ResetBGM(int level)
    {
        ClearAllBGM();
        TurnOffVolumeInAllBGM();
        PlayAllTrack(level);

    }


    public void ClearAllBGM()
    {
        for (int i = 0; i < BGMAudioSources.Length; i++)
        {
            BGMAudioSources[i].Stop();
        }
    }
    public void TurnOffVolumeInAllBGM()
    {
        for (int i = 0; i < BGMAudioSources.Length; i++)
        {
            BGMAudioSources[i].volume = 0;
        }
    }

    public void PlayAllTrack(int level)
    {
        if (level == 1)
        {
            for (int i = 0; i < Level1BGM.Length; i++)
            {
                BGMAudioSources[i].clip = Level1BGM[i];
                BGMAudioSources[i].Play();
            }
        }
    }

    public void TurnOnTrackVolume(int trackNum)
    {
        BGMAudioSources[trackNum].volume = 1;
    }

    //public void PlayBGM(int level, int trackNum)
    //{
    //    if (level == 1)
    //    {
    //        BGMAudioSources[trackNum].clip = Level1BGM[trackNum];
    //        BGMAudioSources[trackNum].Play();
    //    }

    //}


}
