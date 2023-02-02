using UnityEngine;

public class TestGameManager : MonoBehaviour
{
    [SerializeField] int _totalWaterSource = 5;
    [SerializeField] int _totalLifeEnergy;

    private void Start()
    {
        DataManager.Instance.SetTotalProgress(_totalWaterSource);
        DataManager.Instance.SetTotalLifeEnergy(_totalLifeEnergy);
    }
}