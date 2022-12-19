using System.Collections.Generic;
using UnityEngine;

public class DiggingSimple : MonoBehaviour
{
    [SerializeField] float _deepDigging = -0.1f;
    [SerializeField] GameObject _camera;
    [SerializeField] GameObject _ground;
    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;
    private Vector3[] vertices;
    private Vector3[,] coordinate;
    private Mesh mesh; 


    private void Awake()
    {
        _ground.transform.position = new Vector3(_ground.transform.localScale.x * 5, _ground.transform.position.y, _ground.transform.localScale.z * 5);
        _meshFilter = _ground.GetComponent<MeshFilter>();
        _meshCollider = _ground.GetComponent<MeshCollider>();
        TestMesh();

        mesh = _meshFilter.mesh;
        vertices = mesh.vertices;
        coordinate = new Vector3[(int)Mathf.Sqrt(vertices.Length), (int)Mathf.Sqrt(vertices.Length)];
        for (int i = 0; i < Mathf.Sqrt(vertices.Length); i++)
        {
            for (int j = 0; j < Mathf.Sqrt(vertices.Length); j++)
            {
                coordinate[i, j] = vertices[(int)(Mathf.Sqrt(vertices.Length) * i + j)];
            }
        }
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

        Mesh newmesh = new Mesh();
        newmesh.vertices = points.ToArray();
        newmesh.triangles = triangles.ToArray();
        newmesh.RecalculateNormals();
        _meshFilter.mesh = newmesh;
        _meshCollider.sharedMesh = newmesh;
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
                int intPointZ = (int)(hit.point.z * 10 / _ground.transform.localScale.z);
                int intPointX = (int)(hit.point.x * 10 / _ground.transform.localScale.x);
                coordinate[intPointZ, intPointX - 1] = new Vector3(coordinate[intPointZ, intPointX - 1].x, _deepDigging, coordinate[intPointZ, intPointX - 1].z);
                coordinate[intPointZ, intPointX + 1] = new Vector3(coordinate[intPointZ, intPointX + 1].x, _deepDigging, coordinate[intPointZ, intPointX + 1].z);
                coordinate[intPointZ - 1, intPointX] = new Vector3(coordinate[intPointZ - 1, intPointX].x, _deepDigging, coordinate[intPointZ - 1, intPointX].z);
                coordinate[intPointZ + 1, intPointX] = new Vector3(coordinate[intPointZ + 1, intPointX].x, _deepDigging, coordinate[intPointZ + 1, intPointX].z);
                coordinate[intPointZ + 1, intPointX + 1] = new Vector3(coordinate[intPointZ + 1, intPointX + 1].x, _deepDigging, coordinate[intPointZ + 1, intPointX + 1].z);
                coordinate[intPointZ - 1, intPointX - 1] = new Vector3(coordinate[intPointZ - 1, intPointX - 1].x, _deepDigging, coordinate[intPointZ - 1, intPointX - 1].z);
                coordinate[intPointZ, intPointX] = new Vector3(coordinate[intPointZ, intPointX].x, _deepDigging, coordinate[intPointZ, intPointX].z);
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