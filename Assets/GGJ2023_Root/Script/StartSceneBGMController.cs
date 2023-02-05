using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSceneBGMController : MonoBehaviour
{
    public static StartSceneBGMController Instance;
    [SerializeField] AudioSource[] tracks;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    private void Start()
    {
        UpdateAudioMute();
    }
    public void UpdateAudioMute()
    {
        for (int i = 0; i < tracks.Length; i++)
        {
            tracks[i].mute = CentralSetting.Instance.muteAudio;
        }
    }
}
