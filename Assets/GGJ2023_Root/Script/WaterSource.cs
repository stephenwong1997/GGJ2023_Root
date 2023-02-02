using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class WaterSource : MonoBehaviour
{
    [SerializeField] float _depleteTime;
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
        DepleteAsync();
    }

    private async void DepleteAsync()
    {
        const int DEPLET_INTERVAL = 1;

        _waterProgressBar.fillAmount = 1;

        float elapsedTime = _depleteTime;
        while (elapsedTime > 0)
        {
            await Task.Delay(DEPLET_INTERVAL);
            elapsedTime -= DEPLET_INTERVAL / 1000f;

            float normalizedValue = elapsedTime / _depleteTime;
            _waterProgressBar.fillAmount = Mathf.Clamp(normalizedValue, 0, 1);
        }

        _waterProgressBar.fillAmount = 0;
    }
}