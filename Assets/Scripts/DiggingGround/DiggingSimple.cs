using System.Collections.Generic;
using UnityEngine;

public class DiggingSimple : MonoBehaviour
{
    [SerializeField] float _deepDigging = -0.1f;
    [SerializeField] GameObject _camera;
    [SerializeField] GameObject _ground;
    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;

    private void Awake()
    {
        _ground.transform.position = new Vector3(_ground.transform.localScale.x * 5, _ground.transform.position.y, _ground.transform.localScale.z * 5);
        _meshFilter = _ground.GetComponent<MeshFilter>();
        _meshCollider = _ground.GetComponent<MeshCollider>();
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
        if (Input.GetMouseButton(0))
        {
            SetHeight();
        }
    }

    // Устанавливает высоту по координатам меша
    public void SetHeight()
    {
        Ray ray = _camera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 15f))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                Mesh mesh = _meshFilter.mesh;
                Vector3[] vertices = mesh.vertices;
                Vector3[,] coordinate = new Vector3[vertices.Length, vertices.Length];
                for (int i = 0; i < Mathf.Sqrt(vertices.Length); i++)
                {
                    for (int j = 0; j < Mathf.Sqrt(vertices.Length); j++)
                    {
                        coordinate[i, j] = vertices[(int)(Mathf.Sqrt(vertices.Length) * i + j)];
                    }
                }
                coordinate[(int)(hit.point.z * 10 / _ground.transform.localScale.z), (int)(hit.point.x * 10 / _ground.transform.localScale.x)] = new Vector3(coordinate[(int)(hit.point.z * 10 / _ground.transform.localScale.z), (int)(hit.point.x * 10 / _ground.transform.localScale.x)].x, _deepDigging, coordinate[(int)(hit.point.z * 10 / _ground.transform.localScale.z), (int)(hit.point.x * 10 / _ground.transform.localScale.x)].z);
                int k = 0;
                for (int i = 0; i < Mathf.Sqrt(vertices.Length); i++)
                {
                    for (int j = 0; j < Mathf.Sqrt(vertices.Length); j++)
                    {
                        vertices[k] = coordinate[i, j];
                        k++;
                    }
                }
                mesh.vertices = vertices;
                mesh.RecalculateBounds();
            }
        }
    }
    //////////////////////////////////////////////
}