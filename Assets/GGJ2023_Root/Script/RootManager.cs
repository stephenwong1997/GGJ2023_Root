using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootManager : MonoBehaviour
{
    public static RootManager instance;

    [Header("Root expand value reference")]
    [SerializeField] bool canGrow = false;
    [Range(0, 180f)] [SerializeField] float angle = 40;
    [Range(0, 180f)] [SerializeField] float range = 20;
    [Range(0, 180f)] [SerializeField] float stopGrowDistance = 3;

    [Header("Run time reference")]
    [SerializeField] RootDrawer currentRoot;
    [SerializeField] List<RootDrawer> roots;

    [Header("Object reference")]
    [SerializeField] Transform rootParent;

    [Header("Prefab reference")]
    [SerializeField] GameObject rootDrawer;
    [SerializeField] GameObject sphereMark;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        StartCoroutine(GrowTowardsMouse());

    }
    private void Update()
    {
        //if (Input.GetKey(KeyCode.Mouse0))
        //{
        //    // BuildRoot();
        //    // DetectClosestPointToMouse();
        //    // currentRoot.CreateNewRootPosition(GameManager.instance.GetMouseClickPointOnPlane());

        //    // Algorithm 1: Brute force, not optimized
        //    // LinearSearchClosestNodeToMouseAndBuildNewRoot();

        //    // Algorithm 2: Fixed search area, find nearest root => nearest node within root => build new root
        //    OverlapSphereClosestNodeToMouseAndBuildNewRoot();
        //}

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    RandomArcCirclePoint(currentRoot.transform.position, GameManager.instance.GetMouseClickPointOnPlane());
        //}
    }

    private IEnumerator GrowTowardsMouse()
    {
        while (true)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                OverlapSphereClosestNodeToMouseAndBuildNewRoot();
            }
            yield return new WaitForSeconds(0.3f);
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

            //print($"Found '{collider.gameObject}'!");
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

        if (Vector3.Distance(closestNode, mousePos) < stopGrowDistance)
        {
            print("Close enough, stop growing!");
            return;
        }

        if (roots.Contains(closestRoot) && closestNode == closestRoot.GetLastLineIntervalPoints(true))
        {
            ExtendClosestRoot(closestRoot, closestNode, mousePos);
        }
        else
        {
            BuildNewRoot(closestRoot, closestNode, mousePos);
        }
    }
    //private void LinearSearchClosestNodeToMouseAndBuildNewRoot()
    //{
    //    Vector3 mousePos = GameManager.instance.GetMouseClickPointOnPlane();

    //    Vector3 closestNode = default;
    //    float closestDistance = float.PositiveInfinity;
    //    foreach (RootDrawer root in roots)
    //    {
    //        Vector3 node = root.FindClosestNode(mousePos);
    //        float squareDistance = Vector3.SqrMagnitude(mousePos - node);
    //        if (squareDistance < closestDistance)
    //        {
    //            closestNode = node;
    //            closestDistance = squareDistance;
    //        }
    //    }

    //    if (closestDistance == float.PositiveInfinity) return;
    //    // print($"RootManager.LinearSearchClosestPointToMouse(): Found position: {closestNode}");

    //    BuildNewRoot(closestNode, mousePos);
    //}

    private void BuildNewRoot(RootDrawer extendFromRoot, Vector3 rootNode, Vector3 headNode)
    {
        GameObject newRoot = Instantiate(rootDrawer, rootNode, Quaternion.identity, rootParent);
        currentRoot = newRoot.GetComponent<RootDrawer>();
        roots.Add(currentRoot);
        Vector3 localPosOfFromRoot = rootNode - extendFromRoot.transform.position;
        extendFromRoot.AddChildRoot(currentRoot, localPosOfFromRoot);
        currentRoot.CreateNewRootPosition(RandomArcCirclePoint(rootNode, headNode));
    }
    private void ExtendClosestRoot(RootDrawer closestRoot, Vector3 rootPos, Vector3 headNode)
    {
        closestRoot.CreateNewRootPosition(RandomArcCirclePoint(rootPos, headNode));
    }

    Vector3 RandomArcCirclePoint(Vector3 growPoint, Vector3 mousePos)
    {

        //if range > root to mouse distance, return mouse Pos as exact growth target point
        float rootMouseDistance = (mousePos - growPoint).magnitude;
        if (rootMouseDistance < range)
        {
            return mousePos;
        }

        //if mouse is far away, grow "range" length with random arc
        Vector3 direction = (mousePos - growPoint).normalized * range;
        float randomPos = UnityEngine.Random.Range(-angle / 2, angle / 2);
        Vector3 target = growPoint + Quaternion.Euler(Vector3.forward * randomPos) * direction;
        return target;
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
