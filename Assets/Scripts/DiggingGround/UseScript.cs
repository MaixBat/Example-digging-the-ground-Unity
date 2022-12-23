using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UseScript : MonoBehaviour
{
    [Range(1, 50)][SerializeField] int _countTile = 20;
    [SerializeField] int _sizeMesh = 50;
    public float _deepDigging = -0.6f;
    [HideInInspector] public float _startDeepDigging = 0;
    [Range(0,-0.21f)] public float _smooth = -0.1f;
    [Range(1,20)] public int _radius = 1;
    [SerializeField] GameObject _ground;
    private MeshFilter _meshFilter;
    [HideInInspector] public MeshCollider _meshCollider;
    [HideInInspector] public Vector3[] vertices;
    [HideInInspector] public Vector3[,] coordinate;
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

        _meshFilter = _ground.GetComponent<MeshFilter>();
        _meshCollider = _ground.GetComponent<MeshCollider>();

        //_ground.transform.localPosition = Vector3.zero;
        _ground.transform.position = new Vector3(2.5f, _ground.transform.position.y, 2.5f);

        SplittingMesh();

        //GeneretionTale();

        //CombineMesh();

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

    private void SplittingMesh()
    {
        List<Vector3> points = new List<Vector3>();
        for (int i = -_sizeMesh; i <= _sizeMesh; i++)
        {
            for (int j = -_sizeMesh; j <= _sizeMesh; j++)
            {
                points.Add(new Vector3(j * 0.1f, 0, i * 0.1f));
            }
        }

        List<int> triangles = new List<int>();

        for (int line = 0; line < _sizeMesh * 2; line++)
        {
            for (int point = 0; point < _sizeMesh * 2; point++)
            {
                triangles.Add(line * (_sizeMesh * 2) + line + point);
                triangles.Add((line + 1) * (_sizeMesh * 2) + line + 1 + point + 1);
                triangles.Add(line * (_sizeMesh * 2) + line + point + 1);

                triangles.Add((line + 1) * (_sizeMesh * 2) + line + 1 + point);
                triangles.Add((line + 1) * (_sizeMesh * 2) + line + 1 + point + 1);
                triangles.Add(line * (_sizeMesh * 2) + line + point);
            }
        }

        Mesh newmesh = new Mesh();
        newmesh.vertices = points.ToArray();
        newmesh.triangles = triangles.ToArray();
        newmesh.RecalculateNormals();
        _meshFilter.mesh = newmesh;
        _meshCollider.sharedMesh = newmesh;
    }

    void GeneretionTale()
    {
        for (float i = 0.999f; i <= _countTile * 0.999f; i += 0.999f)
        {
            for (float j = 0.999f; j <= _countTile * 0.999f; j += 0.999f)
            {
                Instantiate(_ground, new Vector3(_ground.transform.position.x + i, _ground.transform.position.y, _ground.transform.position.z + j), Quaternion.identity);
            }
        }
        for (float i = 0.999f; i <= _countTile * 0.999f; i += 0.999f)
        {
            Instantiate(_ground, new Vector3(_ground.transform.position.x + i, _ground.transform.position.y, _ground.transform.position.z), Quaternion.identity);
            Instantiate(_ground, new Vector3(_ground.transform.position.x, _ground.transform.position.y, _ground.transform.position.z + i), Quaternion.identity);
        }
    }

    void CombineMesh()
    {
        GameObject[] _gm = GameObject.FindGameObjectsWithTag("Ground");
        MeshFilter[] _meshFilters = new MeshFilter[_gm.Length];
        int k = 0;
        foreach (GameObject el in _gm)
        {
            _meshFilters[k] = el.GetComponent<MeshFilter>();
            k++;
        }
        CombineInstance[] _combines = new CombineInstance[_meshFilters.Length];

        for (int i = 0; i < _meshFilters.Length; i++)
        {
            _combines[i].mesh = _meshFilters[i].mesh;
            _combines[i].transform = _meshFilters[i].transform.localToWorldMatrix;
            if (i != 0)
                Destroy(_meshFilters[i].gameObject);
        }

        MeshFilter _meshFilter1 = _meshFilter;
        _meshFilter1.mesh = new Mesh();
        _meshFilter1.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        _meshFilter1.mesh.CombineMeshes(_combines);

        _meshFilter.mesh = _meshFilter1.mesh;
        _meshCollider.sharedMesh = _meshFilter1.mesh;

        _ground.transform.position = Vector3.zero;
        _ground.transform.localScale = Vector3.one;
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

            if (_hitObject.collider.CompareTag("Ground"))
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
                    Debug.Log(_hitObject.point);
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
