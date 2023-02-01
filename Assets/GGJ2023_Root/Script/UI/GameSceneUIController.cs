using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneUIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject _pauseContainer;
    [SerializeField] Image _lifeEnergyBar;
    [SerializeField] Image _progressBar;

    List<Guid> _tokenList = new List<Guid>();

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

    private void UpdateProgress(float normalizedProgress)
    {
        _progressBar.fillAmount = normalizedProgress;
    }

    private void UpdateLifeEnergy(float normalizedLifeEnergy)
    {
        _lifeEnergyBar.fillAmount = normalizedLifeEnergy;
    }
}

