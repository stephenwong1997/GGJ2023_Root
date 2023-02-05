using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class LifeEnergyPoolSource : MonoBehaviour, IRootOnTriggerEnter
{

    [Header("Settings")]
    [SerializeField] float _depleteTime;
    [SerializeField] Ease _depleteEase;

    [Header("References")]
    [SerializeField] Image _energyBar;
    [SerializeField] Collider _collider;
    [SerializeField] float maxEnergyAmount = 1000;

    bool _isDepleting;
    Tween tween;
    Coroutine currentCoroutine;

    private void Awake()
    {
        Reset();
    }

    public void Reset()
    {
        if (tween.IsActive())
            tween.Kill();
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        _isDepleting = false;
        _energyBar.fillAmount = 1;
        _collider.enabled = true;
    }

    public void OnRootTriggerEnter()
    {
        if (_isDepleting) return;
        _isDepleting = true;
        _collider.enabled = false;

        Debug.Log("energyBar.OnRootTriggerEnter");

        TweenDeplete();

        currentCoroutine = StartCoroutine(RechargeEnergy());
    }
    private IEnumerator RechargeEnergy()
    {
        AudioManager.instance.LoopGainEnergyWaterSFX();
        for (float i = 0; i < _depleteTime; i += _depleteTime / 20)
        {
            DataManager.Instance.ChangeLifeEnergy(maxEnergyAmount / 20, true);
            yield return new WaitForSeconds(_depleteTime / 20);
        }
        AudioManager.instance.StopGainEnergyWaterSFX();
    }

    private void TweenDeplete()
    {
        tween = DOTween.To(
            getter: () => _energyBar.fillAmount,
            setter: x => _energyBar.fillAmount = x,
            endValue: 0,
            duration: _depleteTime
        ).SetEase(_depleteEase);
    }
}
