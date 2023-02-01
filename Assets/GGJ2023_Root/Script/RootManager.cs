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
    [SerializeField] Transform rootParent;

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
            // currentRoot.CreateNewRootPosition(GameManager.instance.GetMouseClickPointOnPlane());

            // Algorithm 1: Brute force, not optimized
            // LinearSearchClosestNodeToMouseAndBuildNewRoot();

            // Algorithm 2: Fixed search area, find nearest root => nearest node within root => build new root
            OverlapSphereClosestNodeToMouseAndBuildNewRoot();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentRoot.CreateNewRootPosition(GameManager.instance.GetMouseClickPointOnPlane());
        }
    }

    private void OverlapSphereClosestNodeToMouseAndBuildNewRoot()
    {
        Vector3 mousePos = GameManager.instance.GetMouseClickPointOnPlane();

        Collider[] overlappingObjects = Physics.OverlapSphere(mousePos, GameManager.instance.MouseSphereRadius);
        if (overlappingObjects == null || overlappingObjects.Length == 0)
        {
            print("RootManager.OverlapSphere(): No overlapping objects within sphere. Look at SceneView, ensure GameManager.DebugMouseSphere is true.");
            return;
        }

        Vector3 closestNode = default;
        float closestDistance = float.PositiveInfinity;
        RootDrawer closestRoot = null;
        foreach (Collider collider in overlappingObjects)
        {
            if (!collider.TryGetComponent<RootDrawer>(out RootDrawer root)) continue;

            print($"Found '{collider.gameObject}'!");
            Vector3 node = root.FindClosestNode(mousePos);
            float squareDistance = Vector3.SqrMagnitude(mousePos - node);
            if (squareDistance < closestDistance)
            {
                closestNode = node;
                closestDistance = squareDistance;
                closestRoot = root;
            }
        }

        if (closestDistance == float.PositiveInfinity)
        {
            print("RootManager.OverlapSphere(): No overlapping objects within sphere. Look at SceneView, ensure GameManager.DebugMouseSphere is true.");
            return;
        }

        print($"closest node {closestNode}");
        print($"closestRoot.GetLastPoint  {closestRoot.GetLastLineIntervalPoints(true) }");
        if (closestRoot == currentRoot && closestNode == closestRoot.GetLastLineIntervalPoints(true))
        {
            currentRoot.CreateNewRootPosition(GameManager.instance.GetMouseClickPointOnPlane());
        }
        else
        {
            BuildNewRoot(closestNode, mousePos);
        }
    }
    private void LinearSearchClosestNodeToMouseAndBuildNewRoot()
    {
        Vector3 mousePos = GameManager.instance.GetMouseClickPointOnPlane();

        Vector3 closestNode = default;
        float closestDistance = float.PositiveInfinity;
        foreach (RootDrawer root in roots)
        {
            Vector3 node = root.FindClosestNode(mousePos);
            float squareDistance = Vector3.SqrMagnitude(mousePos - node);
            if (squareDistance < closestDistance)
            {
                closestNode = node;
                closestDistance = squareDistance;
            }
        }

        if (closestDistance == float.PositiveInfinity) return;
        // print($"RootManager.LinearSearchClosestPointToMouse(): Found position: {closestNode}");

        BuildNewRoot(closestNode, mousePos);
    }

    private void BuildNewRoot(Vector3 rootNode, Vector3 headNode)
    {
        GameObject newRoot = Instantiate(rootDrawer, rootNode, Quaternion.identity, rootParent);
        currentRoot = newRoot.GetComponent<RootDrawer>();
        roots.Add(currentRoot);
        currentRoot.CreateNewRootPosition(headNode);
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
                GameObject newRoot = Instantiate(rootDrawer, hit.point, Quaternion.identity, rootParent);
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

        float distanceWithClosestRootHead = Vector3.Distance(mousePos, roots[indexOfClosestRoot].GetLastPoint);
        if (closestDistance < distanceWithClosestRootHead) // head is not the closest, create a new root
        {
            Vector3 closetPoint = roots[indexOfClosestRoot].meshCollider.ClosestPoint(mousePos);
            Debug.LogError("Create new root at " + indexOfClosestRoot + " , position : " + closetPoint);
            GameObject newRoot = Instantiate(rootDrawer, closetPoint, Quaternion.identity, rootParent);
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
