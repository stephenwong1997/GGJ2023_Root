using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeEnergySource : MonoBehaviour, IRootOnTriggerEnter
{
    [SerializeField] Collider _collider;
    [SerializeField] float energyAmount = 1000;
    [SerializeField] float maxEnergyAmount = 1000;
    public void Reset()
    {
        energyAmount = maxEnergyAmount;
        gameObject.transform.parent.gameObject.SetActive(true);
    }
    public void OnRootTriggerEnter()
    {
        DataManager.Instance.ChangeLifeEnergy(energyAmount);
        gameObject.transform.parent.gameObject.SetActive(false);
    }
}
