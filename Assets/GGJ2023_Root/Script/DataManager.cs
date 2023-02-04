using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    public int currentLevel = 1;

    /// <summary>
    /// Progress => The top bar. Increases when the root drinks water.
    /// </summary>
    /// <value></value>
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
    float m_lastWaterDepleteTime;

    /// <summary>
    /// Total Progress => The required total water to drink to complete the level.
    /// </summary>
    int _totalProgress;

    /// <summary>
    /// LifeEnergy => The left bar. Increases when the root eats energy. Decreases when the root grows.
    /// </summary>
    /// <value></value>
    float _currentLifeEnergy
    {
        get => m_currentLifeEnergy;
        set
        {
            m_currentLifeEnergy = value;
            OnLifeEnergyChanged();
        }
    }
    [SerializeField] float m_currentLifeEnergy;

    /// <summary>
    /// Total LifeEnergy => The total capacity of life energy to grow root.
    /// </summary>
    float _totalLifeEnergy;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }

        Instance = this;
    }

    #region Progress
    public void SetTotalProgress(int totalProgress)
    {
        _totalProgress = totalProgress;
        Debug.Log($"DataManager: Total Progress set as {_totalProgress}");
    }

    public void AddProgress(int progressToAdd, float waterDepleteTime)
    {
        m_lastWaterDepleteTime = waterDepleteTime;
        _currentProgress += progressToAdd;
        AudioManager.instance.TurnOnTrackVolume(_currentProgress);
    }

    public void ResetProgress()
    {
        _currentProgress = 0;
    }

    private void OnProgressChanged()
    {
        float normalizedProgress = (float)m_currentProgress / (float)_totalProgress;
        Debug.Log($"DataManager: Current progress updated ({m_currentProgress}/{_totalProgress} = {normalizedProgress})");
        MessageHubSingleton.Instance.Publish(new OnProgressChangedEvent(m_currentProgress, normalizedProgress, m_lastWaterDepleteTime));
    }
    #endregion

    #region LifeEnergy
    public float GetLifeEnergyLeft()
    {
        return _currentLifeEnergy;
    }
    public void SetTotalLifeEnergy(int totalLifeEnergy)
    {
        _totalLifeEnergy = totalLifeEnergy;
        Debug.Log($"DataManager: Total Life Energy set as {_totalLifeEnergy}");
    }

    public void ChangeLifeEnergy(float LifeEnergyToChange)
    {
        _currentLifeEnergy += LifeEnergyToChange;
    }

    public void ResetLifeEnergy()
    {
        _currentLifeEnergy = 0;
    }

    private void OnLifeEnergyChanged()
    {
        float normalizedLifeEnergy = (float)m_currentLifeEnergy / (float)_totalLifeEnergy;
        Debug.Log($"DataManager: Current LifeEnergy updated ({m_currentLifeEnergy}/{_totalLifeEnergy} = {normalizedLifeEnergy})");
        MessageHubSingleton.Instance.Publish(new OnLifeEnergyChangedEvent(m_currentLifeEnergy, normalizedLifeEnergy));
    }
    #endregion
}

public class OnProgressChangedEvent
{
    public int ChangedValue;
    public float NormalizedValue;
    public float TweenDuration;

    public OnProgressChangedEvent(int changedValue, float normalizedValue, float tweenDuration)
    {
        ChangedValue = changedValue;
        NormalizedValue = normalizedValue;
        TweenDuration = tweenDuration;
    }
}

public class OnLifeEnergyChangedEvent
{
    public float ChangedValue;
    public float NormalizedValue;

    public OnLifeEnergyChangedEvent(float changedValue, float normalizedValue)
    {
        ChangedValue = changedValue;
        NormalizedValue = normalizedValue;
    }
}

