using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentralSetting : MonoBehaviour
{
    public static CentralSetting Instance;
    public bool muteAudio = false;
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public void UpdateAudioMute()
    {
        if (StartSceneBGMController.Instance)
        {
            StartSceneBGMController.Instance.UpdateAudioMute();
        }
        if (AudioManager.instance)
        {
            AudioManager.instance.UpdateAudioMute();
        }
    }
}
