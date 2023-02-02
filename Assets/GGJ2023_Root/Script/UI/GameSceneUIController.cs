using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameSceneUIController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float _tweenDuration = 1f;

    [Header("References")]
    [SerializeField] GameObject _pauseContainer;
    [SerializeField] Image _lifeEnergyBar;
    [SerializeField] Image _progressBar;

    List<Guid> _tokenList = new List<Guid>();

    private void Awake()
    {
        UpdateProgress(0, tween: false);
        UpdateLifeEnergy(1, tween: false);
    }

    private void Start()
    {
        _tokenList.Add(MessageHubSingleton.Instance.Subscribe<OnProgressChangedEvent>((e) => UpdateProgress(e.NormalizedValue)));
        _tokenList.Add(MessageHubSingleton.Instance.Subscribe<OnLifeEnergyChangedEvent>((e) => UpdateLifeEnergy(e.NormalizedValue)));
    }

    private void OnDestroy()
    {
        MessageHubSingleton.Instance.Unsubscribe(_tokenList);
    }

    public void OnPauseButtonClicked()
    {
        Debug.Log("Pause clicked!");

        _pauseContainer.SetActive(true);
    }

    public void OnResumeButtonClicked()
    {
        Debug.Log("Resume clicked!");

        _pauseContainer.SetActive(false);
    }

    public void OnRestartButtonClicked()
    {
        Debug.Log("Restart clicked!");
    }

    public void OnQuitButtonClicked()
    {
        Debug.Log("Quit clicked!");
    }

    private void UpdateProgress(float normalizedProgress, bool tween = true)
    {
        if (!tween)
            _progressBar.fillAmount = normalizedProgress;
        else
            DOTween.To(
                getter: () => _progressBar.fillAmount,
                setter: x => _progressBar.fillAmount = x,
                endValue: normalizedProgress,
                duration: _tweenDuration
            );
    }

    private void UpdateLifeEnergy(float normalizedLifeEnergy, bool tween = true)
    {
        if (!tween)
            _lifeEnergyBar.fillAmount = normalizedLifeEnergy;
        else
            DOTween.To(
                getter: () => _lifeEnergyBar.fillAmount,
                setter: x => _lifeEnergyBar.fillAmount = x,
                endValue: normalizedLifeEnergy,
                duration: _tweenDuration
            );
    }
}

