using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaterController : SourceController
{
    public List<WaterSource> waterSources;

    private void OnValidate()
    {
        waterSources = GetComponentsInChildren<WaterSource>().ToList();
    }

    public override void Reset()
    {
        for (int i = 0; i < waterSources.Count; i++)
        {
            waterSources[i].Reset();
        }
    }
}
