using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnergyPoolController : SourceController
{
    public List<LifeEnergyPoolSource> energyPoolSources;
    private void OnValidate()
    {
        energyPoolSources = GetComponentsInChildren<LifeEnergyPoolSource>().ToList();
    }
    public override void Reset()
    {
        for (int i = 0; i < energyPoolSources.Count; i++)
        {
            energyPoolSources[i].Reset();
        }
    }
}
