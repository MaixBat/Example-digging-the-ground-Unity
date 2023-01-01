using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

    [SerializeField] private LopataColor _colorChange;
    [SerializeField] private GameObject _stone;
    [SerializeField] private MeshCollider[] _collidersDigging;
    [SerializeField] private MeshRenderer[] _meshRendererDigging;
    [SerializeField] private GameObject _ground; // Меш для копания (принадлежит к системе копания)
    [SerializeField] private GameObject _localTake;
    [SerializeField] private GameObject _rayDirection;
    [SerializeField] private GameObject _lopata;
    [SerializeField] private GameObject _lopataSofk;
    [SerializeField] private GameObject _knife;
    [SerializeField] private GameObject _hands;
    [SerializeField] private float _distanceGive;
    [Range(0, 20)][SerializeField] private int _countAnim = 2;
    [Range(0, 20)][SerializeField] private int _countAnimSofk = 2;
    [Range(0, 20)][SerializeField] private int _countAnimKnife = 2;
    [Range(10, 127)][SerializeField] private int _sizeMesh = 60; // Размер меша (принадлежит к системе копания)

    private GameObject _tempDestroy;
    private GameObject _tempDestroy2;
    private GameObject _tempClearObject;
    private GameObject[] _tempDestroy3;
    private int _countAnimStart = 0;
    private int _countAnimSofkStart = 0;
    private int _countAnimKnifeStart = 0;
    private MovePlayer _player;
    private MeshFilter _meshFilter; // Принадлежит к системе копания
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

    IEnumerator PlayAnimationShovel()
    {
        while (_countAnimStart < _countAnim)
        {
            _lopata.transform.position = new Vector3(_ground.transform.position.x + UnityEngine.Random.Range(-0.3f, 0.3f), _lopata.transform.position.y, _ground.transform.position.z + UnityEngine.Random.Range(-0.3f, 0.3f));
            PointZ = _lopata.transform.position.z * 10 / _ground.transform.localScale.z - _correctorZ; // Для системы копания
            PointX = _lopata.transform.position.x * 10 / _ground.transform.localScale.x - _correctorX; // Для системы копания
            _lopata.GetComponent<Animator>().Play("Digging");
            _countAnimStart++;
            yield return new WaitForSeconds(1.55f);
        }
    }

    IEnumerator PlayAnimationSofk()
    {
        while (_countAnimSofkStart < _countAnimSofk)
        {
            _lopataSofk.GetComponent<Animator>().Play("Clearing");
            _countAnimSofkStart++;
            yield return new WaitForSeconds(1.55f);
        }
    }

    IEnumerator PlayAnimationBrush()
    {
        while (_countAnimKnifeStart < _countAnimKnife)
        {
            _knife.GetComponent<Animator>().Play("Clear");
            _countAnimKnifeStart++;
            yield return new WaitForSeconds(0.55f);
        }
    }

    private void FixedUpdate()
    {
        if (_countAnimStart >= _countAnim)
        {
            StopAllCoroutines();
            _countAnimStart = 0;
            _colorChange.ChangeOnTransparent();
            _lopata.SetActive(false);
            _player.CheckMove = true;
            Instantiate(_stone, new Vector3(_ground.transform.position.x - 0.5f, _ground.transform.position.y, _ground.transform.position.z - 0.5f), Quaternion.identity);
            _ground.SetActive(false);
            _tempDestroy.SetActive(false);
        }
        if (_countAnimSofkStart >= _countAnimSofk)
        {
            StopAllCoroutines();
            Destroy(_tempDestroy2);
            _countAnimSofkStart = 0;
            _colorChange.ChangeOnTransparent();
            _lopataSofk.SetActive(false);
            _player.CheckMove = true;
        }
        if (_countAnimKnifeStart >= _countAnimKnife)
        {
            StopAllCoroutines();
            for (int i = 0; i < _tempDestroy3.Length; i++)
            {
                _tempDestroy3[i].SetActive(false);
            }
            _tempClearObject.GetComponent<TakeObject>().isClear = true;
            _countAnimKnifeStart = 0;
            _colorChange.ChangeOnTransparent();
            _knife.SetActive(false);
            _player.CheckMove = true;
        }
    }

    void Update()
    {
        foreach (MeshCollider el in _collidersDigging)
        {
            if (el.gameObject.transform.position != _ground.transform.position)
                el.enabled = true;
        }
        foreach (MeshRenderer el in _meshRendererDigging)
        {
            if (el.gameObject.transform.position != _ground.transform.position)
                el.enabled = true;
        }

        _rayDirection.SetActive(false);

        if (!_take.UseTarget() && _countAnimStart == 0)
            _lopata.SetActive(false);
        if (!_take.UseTarget() && _countAnimSofkStart == 0)
            _lopataSofk.SetActive(false);
        if (!_take.UseTarget() && _countAnimKnifeStart == 0)
            _knife.SetActive(false);

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
            if (!_player.CheckMove)
                return;
            if (_hitObject.collider.CompareTag("TakeObject"))
            {
                if (_hitObject.collider.gameObject.GetComponent<TakeObject>().canClear)
                {
                    _knife.transform.position = new Vector3(_hitObject.point.x, _hitObject.point.y + 0.2f, _hitObject.point.z);
                    _lopata.SetActive(false);
                    _lopataSofk.SetActive(false);
                    if (!_hitObject.collider.gameObject.GetComponent<TakeObject>().isClear)
                        _knife.SetActive(true);
                    _rayDirection.SetActive(true);
                    if (_take.Use())
                    {
                        if (_hitObject.collider.gameObject.GetComponent<TakeObject>().isClear)
                        {
                            if (_tempInHand != null)
                                Destroy(_tempInHand);
                            _hitObject.collider.gameObject.transform.parent = _localTake.transform;
                            _hitObject.collider.gameObject.transform.position = _localTake.transform.position;
                            _tempInHand = _hitObject.collider.gameObject;
                        }
                        else
                        {
                            Transform[] tempDirt = _hitObject.collider.gameObject.GetComponentsInChildren<Transform>();
                            _tempDestroy3 = new GameObject[tempDirt.Length - 1];
                            int k = 0;
                            for (int i = 1; i < tempDirt.Length; i++)
                            {
                                _tempDestroy3[k] = tempDirt[i].gameObject;
                                k++;
                            }
                            k = 0;
                            _tempClearObject = _hitObject.collider.gameObject;
                            _colorChange.ChangeOnNormal();
                            _player.CheckMove = false;
                            StartCoroutine(PlayAnimationBrush());
                        }
                    }
                }
            }

            if (_hitObject.collider.CompareTag("Stone"))
            {
                _lopataSofk.transform.position = new Vector3(_hitObject.point.x, _lopataSofk.transform.position.y, _hitObject.point.z);
                _lopata.SetActive(false);
                _knife.SetActive(false);
                _lopataSofk.SetActive(true);
                _rayDirection.SetActive(true);
                if (_take.Use())
                {
                    _colorChange.ChangeOnNormal();
                    _player.CheckMove = false;
                    Transform[] _tempStoneDestroy = _hitObject.collider.gameObject.GetComponentsInParent<Transform>();
                    _tempDestroy2 = _tempStoneDestroy[1].gameObject;
                    StartCoroutine(PlayAnimationSofk());
                }
            }

            if (_hitObject.collider.CompareTag("Ground"))
            {
                _lopata.transform.LookAt(_player.transform);
                _lopata.transform.position = new Vector3(_hitObject.point.x, _lopata.transform.position.y, _hitObject.point.z);
                _lopataSofk.SetActive(false);
                _knife.SetActive(false);
                _lopata.SetActive(true);
                _rayDirection.SetActive(true);
                if (_take.Use())
                {
                    _colorChange.ChangeOnNormal();
                    _player.CheckMove = false;
                    StartCoroutine(PlayAnimationShovel());
                }
            }

            if (_hitObject.collider.CompareTag("PlaceDigging"))
            {
                SetDefaultMesh();
                _ground.SetActive(true);
                _tempDestroy = _hitObject.collider.gameObject;
                _hitObject.collider.gameObject.GetComponent<MeshCollider>().enabled = false;
                _hitObject.collider.gameObject.GetComponent<MeshRenderer>().enabled = false;
                _ground.transform.position = _hitObject.collider.gameObject.transform.position;
                _correctorZ = 0;
                _correctorX = 0;
                for (int i = 1; i <= _ground.transform.position.z; i++)
                {
                    _correctorZ += 20;
                }
                for (int i = 1; i <= _ground.transform.position.x; i++)
                {
                    _correctorX += 20;
                }
            }
        }
    }
}
