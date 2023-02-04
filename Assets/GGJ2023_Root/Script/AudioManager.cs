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
        Debug.Log($"AudioManager.ResetBGM(): level: {level}");

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
        if (level == 0)
        {
            for (int i = 0; i < Level1BGM.Length; i++)
            {
                BGMAudioSources[i].clip = Level1BGM[i];
                BGMAudioSources[i].Play();
            }
        }

        else if (level == 1)
        {
            for (int i = 0; i < Level2BGM.Length; i++)
            {
                BGMAudioSources[i].clip = Level2BGM[i];
                BGMAudioSources[i].Play();
            }
        }

        else if (level == 2)
        {
            for (int i = 0; i < Level3BGM.Length; i++)
            {
                BGMAudioSources[i].clip = Level3BGM[i];
                BGMAudioSources[i].Play();
            }
        }

        else if (level == 3)
        {
            for (int i = 0; i < Level4BGM.Length; i++)
            {
                BGMAudioSources[i].clip = Level4BGM[i];
                BGMAudioSources[i].Play();
            }
        }
        else
        {
            Debug.LogError($"AudioManager.PlayAllTrack(): Did not handle level index {level}");
        }
    }

    public void TurnOnTrackVolume(int trackNum)
    {
        BGMAudioSources[trackNum].volume = 1;

        if (BGMAudioSources[trackNum].clip != null)
            Debug.Log($"AudioManager.TurnOnTrackVolume(): Song name: {BGMAudioSources[trackNum].clip.name}");
        else
            Debug.LogError($"AudioManager.TurnOnTrackVolume(): BGMAudioSources[trackNum].clip is null! Not playing any additional track... trackNum: {trackNum}");
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
