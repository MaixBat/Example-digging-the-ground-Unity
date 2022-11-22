using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlane : MonoBehaviour
{
    private Mesh _mesh;
    private MeshFilter _meshFilter;
    
    
    // Start is called before the first frame update
    void Start()
    {


        _meshFilter = GetComponent<MeshFilter>();
        _mesh = _meshFilter.mesh;
        TestMesh();
    }

    private void TestMesh()
    {
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i <= 100; i++)
        {
            for (int j = 0; j <= 100; j++)
            {
                points.Add(new Vector3(j * 0.1f, 0, i * 0.1f));
            }
        }

        List<int> triangles = new List<int>();

        // triangles.AddRange(new []
        // {
        //     0, 11, 1,
        //     11, 12, 1,
        //     1, 12, 2,
        //     12, 13, 2
        // });
        for (int line = 0; line < 100; line++)
        {
            for (int point = 0; point < 100; point++)
            {
                triangles.Add( line * 100 + line + point);
                triangles.Add( (line + 1) * 100 + line + 1 + point + 1);
                triangles.Add( line * 100 + line + point + 1);
                
                triangles.Add((line + 1) * 100 + line + 1 + point);
                triangles.Add( (line + 1) * 100 + line + 1 + point + 1);
                triangles.Add( line * 100 + line + point);
            }
        }

        // for (int i = 0; i < triangles.Count - 4; i += 3)
        // {
        //     Debug.Log($"{triangles[i]}, {triangles[i+1]}, {triangles[i+2]}");
        // }
        
        
        Mesh mesh = new Mesh();
        mesh.vertices = points.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        _meshFilter.mesh = mesh;

        // var points = _mesh.vertices;
        // int pointNumber = 0;
        // foreach (var p in points)
        // {
        //     var obj = new GameObject();
        //     obj.transform.position = p;
        //     obj.name = pointNumber.ToString();
        //     pointNumber++;
        // }
        //
        //
        //
        // Debug.Log(_mesh.triangles[0]);
        // Debug.Log(_mesh.triangles[1]);
        //
        // _mesh.RecalculateNormals();

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
