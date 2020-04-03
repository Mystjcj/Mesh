using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MeshManager : MonoBehaviour
{

    private void Start()
    {
        Test();
    }
    void Test()
    {
        List<Vector3> pointList = new List<Vector3>();
        for (float i = 0; i < 10; i += 0.1f)
        {
            for (float j = 0; j < 10; j += 0.1f)
            {
                for (float k = 0; k < 10; k += 0.1f)
                {
                    Vector3 point = new Vector3(i, i, k);
                    pointList.Add(point);
                }
            }
        }
        Vector3[] points = pointList.ToArray();
        Mesh mesh = CreatMesh(points);
        CreatObj(mesh);
    }
    Mesh CreatMesh(Vector3[] points)
    {
        Mesh mesh = new Mesh();
        mesh.indexFormat = IndexFormat.UInt32;
        mesh.vertices = points;
        Color[] colors = new Color[points.Length];

        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.green;
        }
        mesh.colors = colors;
        int[] indecies = new int[points.Length];
        for (int i = 0; i < indecies.Length; i++)
        {
            indecies[i] = i;
        }
        mesh.SetIndices(indecies, MeshTopology.LineStrip, 0);
        return mesh;
    }
    void CreatObj(Mesh mesh)
    {
        GameObject obj = new GameObject();
        MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
        Material mat = new Material(Shader.Find("Custom/VertexColor"));
        meshFilter.mesh = mesh;
        meshRenderer.material = mat;
    }
}
