using System;
using System.Collections.Generic;
using UnityEngine;

public class UseScript : MonoBehaviour
{
    public float _deepDigging = -0.6f;
    [HideInInspector] public float _startDeepDigging = 0;
    [Range(0,-0.21f)] public float _smooth = -0.1f;
    [Range(1,20)] public int _radius = 1;
    [SerializeField] GameObject _ground;
    private MeshFilter _meshFilter;
    [HideInInspector] public MeshCollider _meshCollider;
    public Vector3[] vertices;
    public Vector3[,] coordinate;
    [HideInInspector] public Mesh mesh;

    private MovePlayer _player;

    private event Action InfoObj;

    private ITake _take;

    [SerializeField] GameObject _localTake;
    [SerializeField] GameObject _rayDirection;
    [SerializeField] GameObject _lopata;
    [SerializeField] GameObject _hands;

    private GameObject _tempInHand;

    public float PointZ;
    public float PointX;

    [SerializeField] float _distanceGive;

    void Start()
    {
        _startDeepDigging = _deepDigging;
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<MovePlayer>();
        _take = gameObject.GetComponent<ITake>();

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

    void Update()
    {
        _rayDirection.SetActive(false);

        _lopata.SetActive(!_player.CheckMove);
        
        if (_take.UseTarget())
        {
            RaycastOnObject();
        }
    }

    void RaycastOnObject()
    {
        Ray _centerScreen = new Ray(_rayDirection.transform.position, _rayDirection.transform.forward);

        if (Physics.Raycast(_centerScreen, out RaycastHit _hitObject, _distanceGive))
        {
            if (_hitObject.collider.CompareTag("Take"))
            {
                _rayDirection.SetActive(true);
                if (_take.Use() && _player.CheckMove)
                {
                    InfoObj += _hitObject.collider.gameObject.GetComponent<IObject>().PlaySound;
                    InfoObj += _hitObject.collider.gameObject.GetComponent<IObject>().TakeName;
                    InfoObj?.Invoke();
                    InfoObj -= _hitObject.collider.gameObject.GetComponent<IObject>().PlaySound;
                    InfoObj -= _hitObject.collider.gameObject.GetComponent<IObject>().TakeName;
                    if (_tempInHand != null)
                        Destroy(_tempInHand);
                    _hitObject.collider.gameObject.transform.parent = _localTake.transform;
                    _hitObject.collider.gameObject.transform.position = _localTake.transform.position;
                    _tempInHand = _hitObject.collider.gameObject;
                }
            }

            if (_hitObject.collider.CompareTag("Ground") && coordinate[(int)(_hitObject.point.z * 10 / _ground.transform.localScale.z), (int)(_hitObject.point.x * 10 / _ground.transform.localScale.x)].y > _deepDigging)
            {
                _lopata.transform.LookAt(_player.transform);
                _lopata.transform.position = new Vector3(_hitObject.point.x, _lopata.transform.position.y, _hitObject.point.z);

                if (_player.CheckMove)
                {
                    _lopata.SetActive(true);
                    _rayDirection.SetActive(true);
                }
                if (_take.Use() && _player.CheckMove)
                {
                    PointZ = _hitObject.point.z * 10 / _ground.transform.localScale.z;
                    PointX = _hitObject.point.x * 10 / _ground.transform.localScale.x;
                    _lopata.transform.position = new Vector3(PointX, _lopata.transform.position.y, PointZ);
                    _lopata.GetComponent<Animator>().Play("Digging");
                    _player.CheckMove = false;
                }
            }
        }
    }
}
