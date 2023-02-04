using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class WaterSource : MonoBehaviour, IRootOnTriggerEnter
{
    [Header("Settings")]
    [SerializeField] float _depleteTime;
    [SerializeField] Ease _depleteEase;

    [Header("References")]
    [SerializeField] Image _waterProgressBar;
    [SerializeField] Collider _collider;

    bool _isDepleting;
    Tween tween;

    private void Awake()
    {
        Reset();
    }

    public void Reset()
    {
        if (tween.IsActive())
            tween.Kill();

        _isDepleting = false;
        _waterProgressBar.fillAmount = 1;
        _collider.enabled = true;
    }

    public void OnRootTriggerEnter()
    {
        if (_isDepleting) return;
        _isDepleting = true;
        _collider.enabled = false;

        Debug.Log("WaterSource.OnRootTriggerEnter");
        DataManager.Instance.AddProgress(1, _depleteTime);
        TweenDeplete();
    }

    private void TweenDeplete()
    {
        tween = DOTween.To(
            getter: () => _waterProgressBar.fillAmount,
            setter: x => _waterProgressBar.fillAmount = x,
            endValue: 0,
            duration: _depleteTime
        ).SetEase(_depleteEase);
    }
}