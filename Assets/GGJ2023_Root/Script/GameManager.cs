using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("Object reference")]
    public Camera mainCamera;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public Vector3 GetMouseClickPointOnPlane()
    {
        Ray ray = GameManager.instance.mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitObject;
        if (Physics.Raycast(ray, out hitObject, 500))
            return hitObject.point;
        return Vector3.zero;
    }

}
