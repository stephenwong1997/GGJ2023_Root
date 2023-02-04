using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(LineRendererSmoother))]
public class RootDrawer : MonoBehaviour
{
    public List<Vector3> LinePoints => linePoints;
    public Vector3 GetLastPoint => linePoints[^1];

    //[SerializeField] LineRenderer lineRenderer;
    [SerializeField] List<Vector3> linePoints = new List<Vector3>();
    [SerializeField] List<Vector3> lineIntervalPoints = new List<Vector3>();
    [SerializeField] LineRendererSmoother smoother;
    public Collider headCollider;
    public MeshCollider meshCollider;
    public RootDrawer parentRoot;
    public List<ChildRoot> childRoots = new List<ChildRoot>();
    [SerializeField] float SmoothingLength = 2;
    [SerializeField] int SmoothingSections = 8;

    [Header("Line node interval settings")]
    [SerializeField] float intervalDistance = 1;

    [Header("Line width settings")]
    [SerializeField] float minWidth;
    [SerializeField] float maxWidth;
    [SerializeField] float LengthToWidthConstant;
    [SerializeField] float childRootLengthWeight;

    private BezierCurve[] Curves;

    private float totalSquareLength;

    private void Awake()
    {
        UpdateLineRendererWidthCurve(minWidth);
        InitTotalSquareLength();
    }

    private void UpdateLineRendererWidthCurve(float startWidth)
    {
        Keyframe[] tempKeys = new Keyframe[2];

        tempKeys[0].weightedMode = WeightedMode.Both;
        tempKeys[0].inWeight = 0;
        tempKeys[0].outWeight = 0;
        tempKeys[0].value = 1;
        tempKeys[0].time = 0;

        tempKeys[1].weightedMode = WeightedMode.Both;
        tempKeys[1].inWeight = 0;
        tempKeys[1].outWeight = 0;
        tempKeys[1].value = 0.1f;
        tempKeys[0].time = 1;

        smoother.Line.widthCurve = new AnimationCurve(tempKeys);

        smoother.Line.startWidth = startWidth;
        smoother.Line.endWidth = startWidth * 0.1f;
    }

    private void InitTotalSquareLength()
    {
        totalSquareLength = 0;
        Vector3[] positions = new Vector3[smoother.Line.positionCount];
        smoother.Line.GetPositions(positions);
        for (int i = 1; i < positions.Length; i++)
        {
            totalSquareLength += (positions[i] - positions[i - 1]).sqrMagnitude;
        }
    }

    private void OnValidate()
    {
        if (smoother == null)
        {
            smoother = GetComponent<LineRendererSmoother>();
        }
        if (meshCollider == null)
        {
            meshCollider = GetComponent<MeshCollider>();
        }
    }

    public Vector3 GetLastLineIntervalPoints(bool globalPos = false)
    {
        if (globalPos)
            return lineIntervalPoints[^1] + transform.position;
        else
            return lineIntervalPoints[^1];
    }


    public void AddChildRoot(RootDrawer newRoot, Vector3 growPos)
    {
        int indexOfGrowPos = lineIntervalPoints.FindIndex(p => p == growPos);
        ChildRoot child = new ChildRoot() { childRoot = newRoot, extendFromPointIndex = indexOfGrowPos };
        childRoots.Add(child);
    }

    public void CreateNewRootPosition(Vector3 toNewPosition)
    {
        if (toNewPosition != Vector3.zero)
        {
            Vector2 localPosition = toNewPosition - transform.position;

            float lengthUsed = (localPosition - (Vector2)GetLastPoint).magnitude;
            DataManager.Instance.ChangeLifeEnergy(-lengthUsed);

            linePoints.Add(localPosition);
            AddIntervalPoints(localPosition);

            smoother.Line.positionCount = linePoints.Count;
            smoother.Line.SetPositions(linePoints.ToArray());
            UpdateTotalSquareLength();
            UpdateRootWidth();

            EnsureCurvesMatchLineRendererPositions();
            CalculatePath();
            SmoothPath();
        }
        toNewPosition.z = 0;

        Mesh mesh = new Mesh();
        smoother.Line.BakeMesh(mesh);
        meshCollider.sharedMesh = mesh;
        headCollider.transform.position = toNewPosition;
    }

    public void UpdateTotalSquareLength()
    {
        if (linePoints.Count < 2)
            return;

        totalSquareLength += (linePoints[linePoints.Count - 1] - linePoints[linePoints.Count - 2]).sqrMagnitude;
    }

    public float GetCompleteSquareLength()
    {
        if (childRoots.Count == 0)
            return totalSquareLength;

        float completeSquareLength = 0;
        foreach (var child in childRoots)
        {
            completeSquareLength += child.childRoot.GetCompleteSquareLength();
        }
        return Mathf.Sqrt(completeSquareLength);
    }

    public void UpdateRootWidth()
    {
        float updatedWidth = Mathf.Clamp(minWidth + totalSquareLength * LengthToWidthConstant + GetCompleteSquareLength() * childRootLengthWeight, minWidth, maxWidth);
        // Debug.Log($"Total square length: {totalSquareLength}, updatedWidth: {updatedWidth}");

        UpdateLineRendererWidthCurve(updatedWidth);
        UpdateParentRootWidth();
    }


    public void UpdateParentRootWidth()
    {
        if (parentRoot != null)
        {
            parentRoot.UpdateTotalSquareLength();
            parentRoot.UpdateRootWidth();
        }
    }

    private void AddIntervalPoints(Vector3 newPosition)
    {
        if (lineIntervalPoints.Count <= 0)
        {
            lineIntervalPoints.Add(Vector3.zero);
        }

        Vector3 previousPosition = lineIntervalPoints.Last();

        int intervalCount = Mathf.FloorToInt(Vector3.Distance(previousPosition, newPosition) / intervalDistance);

        const int MIN_INTERVAL_COUNT = 2; // randomly set
        if (intervalCount > MIN_INTERVAL_COUNT)
        {
            Vector3 intervalDirection = (newPosition - previousPosition).normalized;
            Vector3 intervalVector = intervalDistance * intervalDirection;

            for (int i = 0; i < intervalCount; i++)
            {
                Vector3 intervalPoint = previousPosition + (i + 1) * intervalVector;
                lineIntervalPoints.Add(intervalPoint);
            }
        }

        lineIntervalPoints.Add(newPosition);
    }

    public Vector3 FindClosestNode(Vector3 mouseGlobalPosition, bool returnGlobalPosition = true)
    {
        Vector3 closestPoint = default;
        float closestDistance = float.PositiveInfinity;

        // foreach (Vector3 point in this.linePoints)
        foreach (Vector3 point in this.lineIntervalPoints)
        {
            Vector3 pointGlobalPosition = point + this.transform.position;

            float squareDistance = Vector3.SqrMagnitude(mouseGlobalPosition - pointGlobalPosition);
            if (squareDistance < closestDistance)
            {
                closestPoint = point;
                closestDistance = squareDistance;
            }
        }

        if (returnGlobalPosition)
            return closestPoint + this.transform.position;

        return closestPoint;
    }

    private void SmoothPath()
    {
        smoother.Line.positionCount = Curves.Length * SmoothingSections;
        int index = 0;
        for (int i = 0; i < Curves.Length; i++)
        {
            Vector3[] segments = Curves[i].GetSegments(SmoothingSections);
            for (int j = 0; j < segments.Length; j++)
            {
                smoother.Line.SetPosition(index, segments[j]);
                index++;
            }
        }

    }
    private void CalculatePath()
    {
        if (smoother.Line.positionCount < 3)
        {
            return;
        }
        EnsureCurvesMatchLineRendererPositions();

        for (int i = 0; i < Curves.Length; i++)
        {
            Vector3 position = smoother.Line.GetPosition(i);
            Vector3 lastPosition = i == 0 ? smoother.Line.GetPosition(0) : smoother.Line.GetPosition(i - 1);
            Vector3 nextPosition = smoother.Line.GetPosition(i + 1);

            Vector3 lastDirection = (position - lastPosition).normalized;
            Vector3 nextDirection = (nextPosition - position).normalized;

            Vector3 startTangent = (lastDirection + nextDirection) * SmoothingLength;
            Vector3 endTangent = (nextDirection + lastDirection) * -1 * SmoothingLength;

            Curves[i].Points[0] = position; // Start Position (P0)
            Curves[i].Points[1] = position + startTangent; // Start Tangent (P1)
            Curves[i].Points[2] = nextPosition + endTangent; // End Tangent (P2)
            Curves[i].Points[3] = nextPosition; // End Position (P3)
        }

        // Apply look-ahead for first curve and retroactively apply the end tangent
        {
            Vector3 nextDirection = (Curves[1].EndPosition - Curves[1].StartPosition).normalized;
            Vector3 lastDirection = (Curves[0].EndPosition - Curves[0].StartPosition).normalized;

            Curves[0].Points[2] = Curves[0].Points[3] +
                (nextDirection + lastDirection) * -1 * SmoothingLength;

        }
    }


    private void EnsureCurvesMatchLineRendererPositions()
    {
        if (Curves == null || Curves.Length != smoother.Line.positionCount - 1)
        {
            Curves = new BezierCurve[smoother.Line.positionCount - 1];
            for (int i = 0; i < Curves.Length; i++)
            {
                Curves[i] = new BezierCurve();
            }
        }
    }
}


[System.Serializable]
public class ChildRoot
{
    public int extendFromPointIndex;
    public RootDrawer childRoot;
}