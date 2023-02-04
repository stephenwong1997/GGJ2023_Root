using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class StartManager : MonoBehaviour
{
    [Header("Transition Settings")]
    [SerializeField] float _tweenDuration = 1f;

    [Header("Video References")]
    [SerializeField] RenderTexture _videoRenderTexture;
    [SerializeField] VideoPlayer _startingVideo;
    [SerializeField] VideoPlayer _loopVideo;

    [Header("Canvas References")]
    [SerializeField] Image _startButton;
    [SerializeField] Image _creditsButton;
    [SerializeField] Image _exitButton;
    [SerializeField] TextMeshProUGUI _title;

    private IEnumerator Start()
    {
        Color startButtonInitialColor = _startButton.color;
        Color creditsButtonInitialColor = _creditsButton.color;
        Color exitButtonInitialColor = _exitButton.color;
        Color titleInitialColor = _title.color;

        _startButton.color = Color.clear;
        _creditsButton.color = Color.clear;
        _exitButton.color = Color.clear;
        _title.color = Color.clear;

        _videoRenderTexture.Release(); // Set render texture to black

        _startingVideo.Prepare();
        while (!_startingVideo.isPrepared)
        {
            yield return null;
        }

        _startingVideo.Play();

        _loopVideo.Prepare();
        while (!_loopVideo.isPrepared)
        {
            yield return null;
        }

        while (_startingVideo.isPlaying)
        {
            yield return null;
        }

        _loopVideo.Play();

        _startButton.DOColor(startButtonInitialColor, _tweenDuration);
        _creditsButton.DOColor(creditsButtonInitialColor, _tweenDuration);
        _exitButton.DOColor(exitButtonInitialColor, _tweenDuration);
        _title.DOColor(titleInitialColor, _tweenDuration);
    }

    public void OnExitButtonClicked()
    {
        Debug.Log("StartManager.OnExitButtonClicked()");
        Application.Quit();
    }
}