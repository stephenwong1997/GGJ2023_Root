using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneUIController : MonoBehaviour
{
    public void OnPauseButtonClicked()
    {
        Debug.Log("Pause clicked!");
    }

    public void OnResumeButtonClicked()
    {
        Debug.Log("Resume clicked!");
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
