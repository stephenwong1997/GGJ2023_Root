using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using MoreMountains.Feedbacks;

public class GameSceneUIController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float _progressBarTweenDuration = 1f;
    [SerializeField] float _lifeEnergyBarTweenDuration = 0.3f;

    [Header("References")]
    [SerializeField] GameObject _pauseContainer;
    [SerializeField] Image _lifeEnergyBar;
    [SerializeField] Image _lifeEnergyBarBackground;
    [SerializeField] Image _progressBar;
    [SerializeField] TextMeshProUGUI _subtitle;
    [SerializeField] Image _muteSlash;
    [SerializeField] GameObject _onScreenRestart;
    [SerializeField] GameObject _onScreenPause;
    [SerializeField] Button muteButton;

    [Header("Feedbacks")]
    [SerializeField] MMF_Player _returnToStartSceneFeedback;

    List<Guid> _tokenList = new List<Guid>();

    bool _muteButtonState
    {
        get => m_muteButtonState;
        set => SetMuteButtonState(value); // Updates mute slash automatically
    }
    bool m_muteButtonState;

    Tween _progressTween;
    Tween _lifeEnergyTween;
    Sequence _lifeEnergyRedSequence;

    private void Awake()
    {
        UpdateProgress(0, tween: false);
        UpdateLifeEnergy(1, tween: false);
    }

    private void Start()
    {
        _tokenList.Add(MessageHubSingleton.Instance.Subscribe<OnProgressChangedEvent>((e) => UpdateProgress(e.NormalizedValue, e.TweenDuration)));
        _tokenList.Add(MessageHubSingleton.Instance.Subscribe<OnLifeEnergyChangedEvent>((e) => UpdateLifeEnergy(e.NormalizedValue, e.Tween)));
        _tokenList.Add(MessageHubSingleton.Instance.Subscribe<ToggleOnScreenButtonsEvent>((e) =>
        {
            _onScreenRestart.SetActive(e.isToggle);
            _onScreenPause.SetActive(e.isToggle);
        }));
        _tokenList.Add(MessageHubSingleton.Instance.Subscribe<RestartEvent>((e) =>
        {
            if (_lifeEnergyRedSequence.IsActive())
                _lifeEnergyRedSequence.Kill();
        }));
        if (CentralSetting.Instance)
        {
            _muteButtonState = CentralSetting.Instance.muteAudio;
            CentralSetting.Instance.UpdateAudioMute();
        }
    }

    private void OnDestroy()
    {
        MessageHubSingleton.Instance.Unsubscribe(_tokenList);
    }

    public void OnPauseButtonClicked()
    {
        Debug.Log("Pause clicked!");

        _subtitle.text = "";
        // TODO : set _muteButtonState
        UpdateMuteButton();
        _pauseContainer.SetActive(true);
    }

    public void OnResumeButtonClicked()
    {
        Debug.Log("Resume clicked!");

        _pauseContainer.SetActive(false);
    }

    public void OnResumeButtonHoverEnter()
    {
        Debug.Log("Resume hover enter!");
        _subtitle.text = "Resume";
    }

    public void OnResumeButtonHoverExit()
    {
        Debug.Log("Resume hover exit!");
        // _subtitle.text = "";
    }

    public void OnMuteButtonClicked()
    {
        Debug.Log("Mute clicked!");
        _muteButtonState = !_muteButtonState;
        if (CentralSetting.Instance)
        {
            CentralSetting.Instance.muteAudio = _muteButtonState;
            CentralSetting.Instance.UpdateAudioMute();
        }
    }

    public void OnMuteButtonHoverEnter()
    {
        Debug.Log("Mute hover enter!");
        _subtitle.text = "Music";
    }

    public void OnMuteButtonHoverExit()
    {
        Debug.Log("Mute hover exit!");
        // _subtitle.text = "";
    }

    public void OnLevelButtonClicked()
    {
        Debug.Log("Level clicked!");
    }

    public void OnLevelButtonHoverEnter()
    {
        Debug.Log("Level hover enter!");
        _subtitle.text = "Level Selection";
    }

    public void OnLevelButtonHoverExit()
    {
        Debug.Log("Level hover exit!");
        // _subtitle.text = "";
    }

    public void OnRestartButtonClicked()
    {
        OnResumeButtonClicked();
        GameManager.instance.RestartLevel(DataManager.Instance.currentLevel);
        Debug.Log("Restart clicked!");
    }

    public void OnRestartButtonHoverEnter()
    {
        Debug.Log("Restart hover enter!");
        _subtitle.text = "Restart";
    }

    public void OnRestartButtonHoverExit()
    {
        Debug.Log("Restart hover exit!");
        // _subtitle.text = "";
    }

    public void OnQuitButtonClicked()
    {
        Debug.Log("Quit clicked!");
        _returnToStartSceneFeedback.PlayFeedbacks();
    }

    public void OnQuitButtonHoverEnter()
    {
        Debug.Log("Quit hover enter!");
        _subtitle.text = "Exit";
    }

    public void OnQuitButtonHoverExit()
    {
        Debug.Log("Quit hover exit!");
        // _subtitle.text = "";
    }

    private void SetMuteButtonState(bool state)
    {
        m_muteButtonState = state;
        UpdateMuteButton();
    }

    private void UpdateMuteButton()
    {
        _muteSlash.enabled = _muteButtonState;
        _muteSlash.gameObject.SetActive(_muteButtonState);
    }

    private void UpdateProgress(float normalizedProgress, bool tween = true)
    {
        UpdateProgress(normalizedProgress, tween ? _progressBarTweenDuration : 0);
    }

    private void UpdateProgress(float normalizedProgress, float tweenDuration)
    {
        if (_progressTween.IsActive())
        {
            _progressTween.Kill();
        }

        // Force reset and dont tween
        if (normalizedProgress == 0)
        {
            _progressBar.fillAmount = 0;
            return;
        }

        _progressTween = DOTween.To(
            getter: () => _progressBar.fillAmount,
            setter: x => _progressBar.fillAmount = x,
            endValue: normalizedProgress,
            duration: tweenDuration
        ).SetEase(Ease.Linear);
    }

    private void UpdateLifeEnergy(float normalizedLifeEnergy, bool tween)
    {
        if (_lifeEnergyTween.IsActive())
        {
            _lifeEnergyTween.Kill();
        }

        if (!tween)
            _lifeEnergyBar.fillAmount = normalizedLifeEnergy;
        else
        {
            _lifeEnergyTween = DOTween.To(
                getter: () => _lifeEnergyBar.fillAmount,
                setter: x => _lifeEnergyBar.fillAmount = x,
                endValue: normalizedLifeEnergy,
                duration: _lifeEnergyBarTweenDuration
            ).SetEase(Ease.Linear);
        }

        if (normalizedLifeEnergy <= 0)
        {
            Debug.Log("Playing life energy bar red flash");

            const float duration = 0.6f;

            if (_lifeEnergyRedSequence.IsActive())
                _lifeEnergyRedSequence.Kill();

            _lifeEnergyRedSequence = DOTween.Sequence();
            _lifeEnergyRedSequence
            .Append(
                _lifeEnergyBarBackground.DOColor(Color.red, duration)
            )
            .Append(
                _lifeEnergyBarBackground.DOColor(Color.white, duration)
            )
            .SetLoops(-1);
        }
    }
}

public class ToggleOnScreenButtonsEvent
{
    public bool isToggle;

    public ToggleOnScreenButtonsEvent(bool isToggle)
    {
        this.isToggle = isToggle;
    }
}

