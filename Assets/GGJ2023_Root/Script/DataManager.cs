using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    int _currentProgress
    {
        get => m_currentProgress;
        set
        {
            m_currentProgress = value;
            OnProgressChanged();
        }
    }
    int m_currentProgress;

    int _totalProgress;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }

        Instance = this;
    }

    public void SetTotalProgress(int totalProgress)
        => _totalProgress = totalProgress;

    public void AddProgress(int progressToAdd)
    {
        _currentProgress += progressToAdd;
    }

    public void ResetProgress()
    {
        _currentProgress = 0;
    }

    private void OnProgressChanged()
    {
        float normalizedProgress = (float)m_currentProgress / (float)_totalProgress;
        Debug.Log($"DataManager: Current progress updated ({m_currentProgress}/{_totalProgress} = {normalizedProgress})");
        MessageHubSingleton.Instance.Publish(new OnProgressChangedEvent(m_currentProgress, normalizedProgress));
    }
}

public class OnProgressChangedEvent
{
    public int ChangedValue;
    public float NormalizedValue;

    public OnProgressChangedEvent(int changedValue, float normalizedValue)
    {
        ChangedValue = changedValue;
        NormalizedValue = normalizedValue;
    }
}

