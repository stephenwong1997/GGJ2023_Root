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

    [Header("Level Setting")]
    [SerializeField] int _totalWaterSource = 5;
    [SerializeField] int _totalLifeEnergy;
    [SerializeField] List<SourceController> level1Controllers;
    [SerializeField] List<SourceController> level2Controllers;
    [SerializeField] List<SourceController> level3Controllers;
    [SerializeField] List<SourceController> level4Controllers;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    private void Start()
    {
        RestartLevel(1);
    }

    private void Update()
    {
        CameraControl();
    }

    public void RestartLevel(int level)
    {
        AudioManager.instance.ResetBGM(level);
        AudioManager.instance.TurnOnTrackVolume(0);
        DataManager.Instance.SetTotalProgress(_totalWaterSource);
        DataManager.Instance.SetTotalLifeEnergy(_totalLifeEnergy);
        DataManager.Instance.ChangeLifeEnergy(_totalLifeEnergy);
        DataManager.Instance.ResetProgress();
        RootManager.instance.ResetRoots();

        MessageHubSingleton.Instance.Publish(new RestartEvent());

        switch (level)
        {
            case 1:
                for (int i = 0; i < level1Controllers.Count; i++)
                {
                    level1Controllers[i].Reset();
                }
                break;
            case 2:
                for (int i = 0; i < level1Controllers.Count; i++)
                {
                    level2Controllers[i].Reset();
                }
                break;
            case 3:
                for (int i = 0; i < level1Controllers.Count; i++)
                {
                    level3Controllers[i].Reset();
                }
                break;
            case 4:
                for (int i = 0; i < level1Controllers.Count; i++)
                {
                    level4Controllers[i].Reset();
                }
                break;

        }

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

    public RaycastHit GetMouseRaycastHit()
    {
        Ray ray = GameManager.instance.mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitObject;
        Physics.Raycast(ray, out hitObject);
        return hitObject;
    }
}

public class RestartEvent { }