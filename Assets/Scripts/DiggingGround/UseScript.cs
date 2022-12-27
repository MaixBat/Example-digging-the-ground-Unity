using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UseScript : MonoBehaviour
{
    // Переменные для системы копания (всё что находится в этом блоке принадлежит к системе копания)
    [Range(0f, 200f)] public float _correctorZ = 0f; // Корректор координат
    [Range(0f, 200f)] public float _correctorX = 0f; // Корректор координат
    [Range(-1f, 0f)] public float _smooth = -0.1f; // Сглаживание по краям
    [Range(1, 20)] public int _radius = 1; // Размер ямы (не трогать, уже настроено)
    [HideInInspector] public float _startDeepDigging = 0;
    [HideInInspector] public MeshCollider _meshCollider;
    [HideInInspector] public Vector3[] vertices;
    [HideInInspector] public Vector3[,] coordinate; // Двумерный массив координат меша
    [HideInInspector] public Mesh mesh;

    public float _deepDigging = -0.6f;
    public float PointZ;
    public float PointX;
    ///////////////////////////////////////////////////////

    [SerializeField] private GameObject _ground; // Меш для копания (принадлежит к системе копания)
    [SerializeField] private GameObject _localTake;
    [SerializeField] private GameObject _rayDirection;
    [SerializeField] private GameObject _lopata;
    [SerializeField] private GameObject _hands;
    [SerializeField] private float _distanceGive;
    [Range(20, 127)][SerializeField] private int _sizeMesh = 60; // Размер меша (принадлежит к системе копания)

    private MovePlayer _player;
    private MeshFilter _meshFilter; // Принадлежит к системе копания
    private event Action InfoObj;
    private GameObject _tempInHand;
    private ITake _take;

    void Start()
    {
        _startDeepDigging = _deepDigging; // Для системы копания
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<MovePlayer>();
        _take = gameObject.GetComponent<ITake>();
        // Для системы копания
        _meshFilter = _ground.GetComponent<MeshFilter>();
        _meshCollider = _ground.GetComponent<MeshCollider>();
        _ground.transform.position = new Vector3(_ground.transform.localScale.x * (_sizeMesh / 10), _ground.transform.position.y, _ground.transform.localScale.z * (_sizeMesh / 10)); // Коррекция размеров меша и координат
        SplittingMesh();
        CreateCoordinates();
        //////////////////////
    }

    // Вернуть меш в исходное состояние
    public void SetDefaultMesh()
    {
        SplittingMesh();
        CreateCoordinates();
    }
    /////////////////////////////////

    // Всё что находится в этом блоке принадлежит к системе копания
    private void CreateCoordinates()
    {
        // Переводим индексы вершин меша в координаты
        mesh = _meshFilter.mesh;
        vertices = mesh.vertices;
        coordinate = new Vector3[(int)Mathf.Sqrt(vertices.Length), (int)Mathf.Sqrt(vertices.Length)];
        for (int i = 0; i < (int)Mathf.Sqrt(vertices.Length); i++)
        {
            for (int j = 0; j < (int)Mathf.Sqrt(vertices.Length); j++)
            {
                coordinate[i, j] = vertices[(int)(Mathf.Sqrt(vertices.Length) * i + j)];
            }
        }
        ////////////////////////////////////////////
    }
    ////////////////////////////////////////////

    // Всё что находится в этом блоке принадлежит к системе копания
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
    ////////////////////////////////////////////

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
                    PointZ = _hitObject.point.z * 10 / _ground.transform.localScale.z - _correctorZ; // Для системы копания
                    PointX = _hitObject.point.x * 10 / _ground.transform.localScale.x - _correctorX; // Для системы копания
                    _lopata.transform.position = new Vector3(PointX, _lopata.transform.position.y, PointZ);
                    _lopata.GetComponent<Animator>().Play("Digging");
                    _player.CheckMove = false;
                }
            }
        }
    }
}
