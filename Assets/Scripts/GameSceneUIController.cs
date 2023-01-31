using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneUIController : MonoBehaviour
{
    [SerializeField] GameObject _pauseContainer;

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
}
