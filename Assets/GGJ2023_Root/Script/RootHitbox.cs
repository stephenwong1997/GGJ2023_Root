using UnityEngine;

public class RootHitbox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("RootHitbox.OnTriggerEnter");

        if (!other.TryGetComponent(out IRootOnTriggerEnter enteredObject)) return;

        enteredObject.OnRootTriggerEnter();
    }

}