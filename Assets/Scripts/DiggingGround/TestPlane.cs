using System.Collections.Generic;
using UnityEngine;

public class TestPlane : MonoBehaviour
{
    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;

    void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshCollider = GetComponent<MeshCollider>();
        TestMesh();
    }

    private void TestMesh()
    {
        List<Vector3> points = new List<Vector3>();
        for (int i = -50; i <= 50; i++)
        {
            for (int j = -50; j <= 50; j++)
            {
                points.Add(new Vector3(j * 0.1f, 0, i * 0.1f));
            }
        }

        List<int> triangles = new List<int>();

        for (int line = 0; line < 100; line++)
        {
            for (int point = 0; point < 100; point++)
            {
                triangles.Add(line * 100 + line + point);
                triangles.Add((line + 1) * 100 + line + 1 + point + 1);
                triangles.Add(line * 100 + line + point + 1);

                triangles.Add((line + 1) * 100 + line + 1 + point);
                triangles.Add((line + 1) * 100 + line + 1 + point + 1);
                triangles.Add(line * 100 + line + point);
            }
        }


        Mesh mesh = new Mesh();
        mesh.vertices = points.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        _meshFilter.mesh = mesh;
        _meshCollider.sharedMesh = mesh;
    }

    private void Update()
    {
        Mesh mesh = _meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;
        vertices[5] = new Vector3(vertices[5].x, 2, vertices[5].z);
        mesh.vertices = vertices;
        mesh.RecalculateBounds();
    }
}