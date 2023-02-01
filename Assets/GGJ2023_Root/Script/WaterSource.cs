using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WaterSource : MonoBehaviour
{
    [SerializeField] float _depleteTime;
    [SerializeField] Image _waterProgressBar;

    bool _isDepleting;

    private void Awake()
    {
        _isDepleting = false;
    }

    public void OnRootTriggerEnter()
    {
        if (_isDepleting) return;
        _isDepleting = true;

        Debug.Log("WaterSource.OnRootTriggerEnter");
        StartCoroutine(DepleteCoroutine());
    }

    private IEnumerator DepleteCoroutine()
    {
        const float DEPLET_INTERVAL = 0.001f;
        WaitForSeconds waitForSeconds = new WaitForSeconds(DEPLET_INTERVAL);

        _waterProgressBar.fillAmount = 1;

        float elapsedTime = _depleteTime;
        while (elapsedTime > 0)
        {
            yield return waitForSeconds;
            elapsedTime -= DEPLET_INTERVAL;

            float normalizedValue = elapsedTime / _depleteTime;
            _waterProgressBar.fillAmount = Mathf.Clamp(normalizedValue, 0, 1);
        }

        _waterProgressBar.fillAmount = 0;
    }
}