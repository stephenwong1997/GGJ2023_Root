using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnergyController : SourceController
{
    public List<LifeEnergySource> energySources;
    private void OnValidate()
    {
        energySources = GetComponentsInChildren<LifeEnergySource>().ToList();
    }
    public override void Reset()
    {
        for (int i = 0; i < energySources.Count; i++)
        {
            energySources[i].Reset();
        }
    }
}
