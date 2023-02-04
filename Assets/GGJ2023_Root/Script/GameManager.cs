using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public float MouseSphereRadius => mouseSphereRadius;

    [Header("Settings")]
    [SerializeField] bool debugMouseSphere;
    [SerializeField] float mouseSphereRadius;
    [SerializeField] float plantViewY;
    [SerializeField] float upperBoundaryY;
    [SerializeField] float lowerBoundaryY;

    [Header("Object reference")]
    public Camera mainCamera;

    [Header("Level Setting")]
    [SerializeField] int _totalWaterSource = 5;
    [SerializeField] int _totalLifeEnergy;
    [SerializeField] List<LevelController> levelControllers;
    //[SerializeField] List<SourceController> level1Controllers;
    //[SerializeField] List<SourceController> level2Controllers;
    //[SerializeField] List<SourceController> level3Controllers;
    //[SerializeField] List<SourceController> level4Controllers;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    private void Start()
    {
        // RestartLevel(1);

        DataManager.Instance.currentLevel = 0;
        RestartLevel(DataManager.Instance.currentLevel, skipStartFade: true);
    }

    private void Update()
    {
        CameraControl();
    }

    public void ToNextLevel()
    {
        int currentLevel = ++DataManager.Instance.currentLevel;
        if (currentLevel <= 3)
        {
            StartCoroutine(ToNextLevel(currentLevel));
        }
        else
        {

        }
    }
    private IEnumerator ToNextLevel(int level)
    {
        if (level > 0)
            levelControllers[level - 1].SetSeedToCrop();

        MessageHubSingleton.Instance.Publish(new ToggleOnScreenButtonsEvent(false));

        mainCamera.transform.DOMoveY(plantViewY, 3f);
        yield return new WaitForSeconds(5);
        RestartLevel(level);
    }

    public void RestartLevel(int level, bool skipStartFade = false)
    {
        Debug.Log($"GameManager.RestartLevel(): level: {level}");

        StartCoroutine(RestartCoroutine(level, skipStartFade));
    }
    private IEnumerator RestartCoroutine(int level, bool skipStartFade)
    {
        if (!skipStartFade)
        {
            TransitionUIController.Instance.FadeOut();
            yield return new WaitForSeconds(1.2f);
        }
        else
        {
            TransitionUIController.Instance.ForceFadeOut();
        }

        MessageHubSingleton.Instance.Publish(new ToggleOnScreenButtonsEvent(true));

        AudioManager.instance.ResetBGM(level);
        AudioManager.instance.TurnOnTrackVolume(0);

        DataManager.Instance.SetTotalProgress(_totalWaterSource);
        DataManager.Instance.SetTotalLifeEnergy(_totalLifeEnergy);
        DataManager.Instance.ResetLifeEnergy();
        DataManager.Instance.ResetProgress();

        // ResetLevelController(DataManager.Instance.currentLevel);
        ResetLevelController(level);

        MessageHubSingleton.Instance.Publish(new RestartEvent());

        TransitionUIController.Instance.FadeIn();
    }

    private void ResetLevelController(int currentLevel)
    {
        mainCamera.transform.position = levelControllers[currentLevel].cameraDefaultPos.position;
        RootManager.instance.ResetRoots(levelControllers[currentLevel]);
        levelControllers[currentLevel].ResetSources();
    }

    public IEnumerator FinishGameAnimation()
    {
        yield return null;
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