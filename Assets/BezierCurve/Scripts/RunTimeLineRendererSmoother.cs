using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunTimeLineRendererSmoother : MonoBehaviour
{
    private LineRendererSmoother Smoother;

    private void OnEnable()
    {
        if (Smoother.Line == null)
        {
            Smoother.Line = Smoother.GetComponent<LineRenderer>();
        }
    }
}
