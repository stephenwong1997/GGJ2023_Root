using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public float MouseSphereRadius => mouseSphereRadius;

    [Header("Settings")]
    [SerializeField] bool debugMouseSphere;
    [SerializeField] float mouseSphereRadius;
    [SerializeField] float upperBoundaryY;
    [SerializeField] float lowerBoundaryY;

    [Header("Object reference")]
    public Camera mainCamera;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Update()
    {
        CameraControl();
    }

    private void CameraControl()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            if (mainCamera.transform.position.y <= upperBoundaryY && mainCamera.transform.position.y >= lowerBoundaryY)
            {
                Vector3 moveTo = mainCamera.transform.position + Vector3.up * Input.mouseScrollDelta.y * Time.deltaTime * 1900;
                moveTo.z = -10;
                mainCamera.transform.position = moveTo;
                if (mainCamera.transform.position.y > upperBoundaryY)
                {
                    moveTo = mainCamera.transform.position;
                    moveTo.y = upperBoundaryY;
                }
                if (mainCamera.transform.position.y < lowerBoundaryY)
                {
                    moveTo = mainCamera.transform.position;
                    moveTo.y = lowerBoundaryY;
                }
                moveTo.z = -10;
                mainCamera.transform.position = moveTo;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (debugMouseSphere && Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(GetMouseClickPointOnPlane(), mouseSphereRadius);
        }
    }

    public Vector3 GetMouseClickPointOnPlane()
    {
        Ray ray = GameManager.instance.mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitObject;
        int layerMask = 1 << 9;
        if (Physics.Raycast(ray, out hitObject, 500, layerMask))
        {
            return hitObject.point;
        }
        return Vector3.zero;
    }
}
