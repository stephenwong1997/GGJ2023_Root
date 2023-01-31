using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootManager : MonoBehaviour
{
    public static RootManager instance;
    [Header("Run time reference")]
    [SerializeField] RootDrawer currentRoot;
    [SerializeField] List<RootDrawer> roots;
    [Header("Object reference")]
    [SerializeField] MouseSphereDetecter mouseDetectionSphere;
    [Header("Prefab reference")]
    [SerializeField] GameObject rootDrawer;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        //currentRoot.CreateNewRootPosition(Vector3.forward);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //BuildRoot();
            //   DetectClosestPointToMouse();
            currentRoot.CreateNewRootPosition(GameManager.instance.GetMouseClickPointOnPlane());
        }
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    currentRoot.CreateNewRootPosition(GameManager.instance.GetMouseClickPointOnPlane());
        //}
    }

    private void DetectClosestPointToMouse()
    {
        Vector3 mousePos = GameManager.instance.GetMouseClickPointOnPlane();
        RaycastHit hit;
        int layerMask = 1 << 7;
        for (float radius = 0.5f; radius < 100; radius += 0.5f)
        {
            if (Physics.SphereCast(mousePos, radius, Vector3.one, out hit, 10, layerMask))
            {
                GameObject newRoot = Instantiate(rootDrawer, hit.point, Quaternion.identity);
                currentRoot = newRoot.GetComponent<RootDrawer>();
                roots.Add(currentRoot);
                currentRoot.CreateNewRootPosition(mousePos);
                print("Found position :  " + hit.point);
                break;
            }
            print("Expend sphere , current radius " + radius + " mouse pos :  " + mousePos);
        }
    }

    private void BuildRoot()
    {
        float closestDistance = int.MaxValue;
        int indexOfClosestRoot = 0;
        Vector3 mousePos = GameManager.instance.GetMouseClickPointOnPlane();
        for (int i = 0; i < roots.Count; i++)
        {
            Vector3 closestPoint = roots[i].meshCollider.ClosestPoint(mousePos);
            float newDistance = Vector3.Distance(mousePos, closestPoint);
            if (newDistance < closestDistance)
            {
                closestDistance = newDistance;
                indexOfClosestRoot = i;
            }
        }

        float distanceWithClosestRootHead = Vector3.Distance(mousePos, roots[indexOfClosestRoot].GetHeadPoint);
        if (closestDistance < distanceWithClosestRootHead) // head is not the closest, create a new root
        {
            Vector3 closetPoint = roots[indexOfClosestRoot].meshCollider.ClosestPoint(mousePos);
            Debug.LogError("Create new root at " + indexOfClosestRoot + " , position : " + closetPoint);
            GameObject newRoot = Instantiate(rootDrawer, closetPoint, Quaternion.identity);
            currentRoot = newRoot.GetComponent<RootDrawer>();
            roots.Add(currentRoot);
        }
        else// head is near, extend the root of head
        {
            currentRoot = roots[indexOfClosestRoot];
        }


        currentRoot.CreateNewRootPosition(mousePos);

    }
}
