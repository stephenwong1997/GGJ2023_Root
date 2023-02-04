using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeEnergySource : MonoBehaviour, IRootOnTriggerEnter
{
    [SerializeField] Collider _collider;
    [SerializeField] float energyAmount = 1000;
    public void OnRootTriggerEnter()
    {
        DataManager.Instance.ChangeLifeEnergy(energyAmount);
        gameObject.transform.parent.gameObject.SetActive(false);
    }
}
