using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneUIController : MonoBehaviour
{
    [SerializeField] GameObject _pauseContainer;

    List<Guid> _tokenList = new List<Guid>();

    private void Start()
    {
        _tokenList.Add(MessageHubSingleton.Instance.Subscribe<OnProgressChangedEvent>((e) => UpdateProgress(e.NormalizedValue)));
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

    private void UpdateProgress(float normalizedValue)
    {

    }
}

