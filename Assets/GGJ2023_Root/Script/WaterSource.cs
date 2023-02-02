using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class WaterSource : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float _depleteTime;
    [SerializeField] Ease _depleteEase;

    [Header("References")]
    [SerializeField] Image _waterProgressBar;
    [SerializeField] Collider _collider;

    bool _isDepleting;

    private void Awake()
    {
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
        DataManager.Instance.AddProgress(1);
        TweenDeplete();
    }

    private void TweenDeplete()
    {
        DOTween.To(
            getter: () => _waterProgressBar.fillAmount,
            setter: x => _waterProgressBar.fillAmount = x,
            endValue: 0,
            duration: _depleteTime
        ).SetEase(_depleteEase);
    }
}