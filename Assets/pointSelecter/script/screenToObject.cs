using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class screenToObject : MonoBehaviour {
	
	void Update () {
        if (Input.GetMouseButton(0))
        {
            Vector3 wordPoint = CountObjectPoint(Input.mousePosition);
            GetComponent<Renderer>().material.SetVector("_Point", wordPoint);
        }
    }

    Vector3 CountObjectPoint(Vector2 screenPoint)
    {
        Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;

        List<Vector3> vertices = new List<Vector3>();
        mesh.GetVertices(vertices);
        Vector3[] worldVertices = new Vector3[vertices.Count];
        Vector3[] screenVertices = new Vector3[vertices.Count];
        
        int[] triangles = mesh.triangles;

        Vector3 worldPoint = Vector3.zero;
        float minDistance = float.MaxValue;
        float currentDistance = 0f;
        Vector3 currentPoint = Vector3.zero;

        for(int i = 0;i < vertices.Count; i++)
        {
            worldVertices[i] = transform.TransformPoint(vertices[i]);
            screenVertices[i] = Camera.main.WorldToScreenPoint(worldVertices[i]);
        }

        for (int i = 2; i < triangles.Length; i += 3)
        {
            if (JudgeAndCountPoint(screenPoint,

                            screenVertices[triangles[i - 2]],
                            screenVertices[triangles[i - 1]],
                            screenVertices[triangles[i]],

                            worldVertices[triangles[i - 2]],
                            worldVertices[triangles[i - 1]],
                            worldVertices[triangles[i]],
                            ref currentPoint))
            {
                currentDistance = Vector3.Distance(Camera.main.gameObject.transform.position, currentPoint);
                if(currentDistance < minDistance)
                {
                    minDistance = currentDistance;
                    worldPoint = currentPoint;
                }
            }
        }
        return worldPoint;
    }

    bool JudgeAndCountPoint(Vector2 point,
        Vector2 tri1, Vector2 tri2, Vector2 tri3,
        Vector3 wor1, Vector3 wor2, Vector3 wor3,
        ref Vector3 worldPoint)
    {
        worldPoint = Vector3.zero;
        float triArea = CountArea(tri1, tri2, tri3);

        float pointArea1 = CountArea(point, tri2, tri3);
        float pointArea2 = CountArea(tri1, point, tri3);
        float pointArea3 = CountArea(tri1, tri2, point);
        float pointArea = pointArea1 + pointArea2 + pointArea3;

        if (triArea < pointArea)
            return false;

        float weight1 = pointArea1 / triArea;
        float weight2 = pointArea2 / triArea;
        float weight3 = pointArea3 / triArea;

        worldPoint = wor1 * weight1 + wor2 * weight2 + wor3 * weight3;
        return true;
    }

    //vector cross
    float CountArea(Vector2 v1, Vector2 v2, Vector2 v3)
    {
        Vector2 a = v2 - v1;
        Vector2 b = v3 - v1;
        float area = a.x * b.y - a.y * b.x;
        return Mathf.Abs(area);
    }
}
