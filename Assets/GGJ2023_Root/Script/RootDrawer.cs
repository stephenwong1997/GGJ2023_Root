using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(LineRendererSmoother))]
public class RootDrawer : MonoBehaviour
{
    public List<Vector3> LinePoints => linePoints;

    //[SerializeField] LineRenderer lineRenderer;
    [SerializeField] List<Vector3> linePoints = new List<Vector3>();
    [SerializeField] List<Vector3> lineIntervalPoints = new List<Vector3>();
    [SerializeField] LineRendererSmoother smoother;
    public MeshCollider meshCollider;
    [SerializeField] float SmoothingLength = 2;
    [SerializeField] int SmoothingSections = 8;

    [Header("Line node interval settings")]
    [SerializeField] float intervalDistance = 1;

    private BezierCurve[] Curves;

    public Vector3 GetLastPoint
    {
        get { return linePoints[^1]; }
    }
    public Vector3 GetLastLineIntervalPoints(bool globalPos = false)
    {
        if (globalPos)
            return lineIntervalPoints[^1] + transform.position;
        else
            return lineIntervalPoints[^1];
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


    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Mouse0))
        //{
        //    CreateNewRootPosition(GameManager.instance.GetMouseClickPointOnPlane());
        //}
    }

    public void CreateNewRootPosition(Vector3 toNewPosition)
    {
        if (toNewPosition != Vector3.zero)
        {
            Vector2 localPosition = toNewPosition - transform.position;

            //smoother.Line.positionCount++;
            linePoints.Add(localPosition);
            AddIntervalPoints(localPosition);
            //smoother.Line.SetPosition(smoother.Line.positionCount - 1, hitObject.point);
            smoother.Line.positionCount = linePoints.Count;

            smoother.Line.SetPositions(linePoints.ToArray());
            EnsureCurvesMatchLineRendererPositions();
            CalculatePath();
            SmoothPath();
        }
        Mesh mesh = new Mesh();
        smoother.Line.BakeMesh(mesh);
        meshCollider.sharedMesh = mesh;
    }

    private void AddIntervalPoints(Vector3 newPosition)
    {
        if (lineIntervalPoints.Count <= 0)
        {
            lineIntervalPoints.Add(Vector3.zero);
        }

        Vector3 previousPosition = lineIntervalPoints.Last();

        int intervalCount = Mathf.FloorToInt(Vector3.Distance(previousPosition, newPosition) / intervalDistance);
        Vector3 intervalDirection = (newPosition - previousPosition).normalized;
        Vector3 intervalVector = intervalDistance * intervalDirection;

        for (int i = 0; i < intervalCount; i++)
        {
            Vector3 intervalPoint = previousPosition + (i + 1) * intervalVector;
            lineIntervalPoints.Add(intervalPoint);
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

        // Reset values so inspector doesn't freeze if you use lots of smoothing sections
        //SmoothingSections = 1;
        //SmoothingLength = 0;

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

            //Handles.color = Color.green;
            //Handles.DotHandleCap(EditorGUIUtility.GetControlID(FocusType.Passive), position + startTangent, Quaternion.identity, 0.25f, EventType.Repaint);

            //if (i != 0)
            //{
            //    Handles.color = Color.blue;
            //    Handles.DotHandleCap(EditorGUIUtility.GetControlID(FocusType.Passive), nextPosition + endTangent, Quaternion.identity, 0.25f, EventType.Repaint);
            //}

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

            //Handles.color = Color.blue;
            //Handles.DotHandleCap(EditorGUIUtility.GetControlID(FocusType.Passive), Curves[0].Points[2], Quaternion.identity, 0.25f, EventType.Repaint);
        }

        //DrawSegments();
    }

    //private void DrawSegments()
    //{
    //    for (int i = 0; i < Curves.Length; i++)
    //    {
    //        Vector3[] segments = Curves[i].GetSegments(SmoothingSections.intValue);
    //        for (int j = 0; j < segments.Length - 1; j++)
    //        {
    //            Handles.color = Color.white;
    //            Handles.DrawLine(segments[j], segments[j + 1]);

    //            float color = (float)j / segments.Length;
    //            Handles.color = new Color(color, color, color);
    //            Handles.Label(segments[j], $"C{i} S{j}");
    //            Handles.DotHandleCap(EditorGUIUtility.GetControlID(FocusType.Passive), segments[j], Quaternion.identity, 0.05f, EventType.Repaint);
    //        }

    //        Handles.color = Color.white;
    //        Handles.Label(segments[segments.Length - 1], $"C{i} S{segments.Length - 1}");
    //        Handles.DotHandleCap(EditorGUIUtility.GetControlID(FocusType.Passive), segments[segments.Length - 1], Quaternion.identity, 0.05f, EventType.Repaint);

    //        Handles.DrawLine(segments[segments.Length - 1], Curves[i].EndPosition);
    //    }
    //}

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
