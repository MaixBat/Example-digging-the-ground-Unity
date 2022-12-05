using System;
using UnityEngine;

public class UseScript : MonoBehaviour
{
    MovePlayer _player;

    event Action InfoObj;

    ITake _take;

    float _mapResol;
    public static float DepthGround = 0.0031f;
    public static float DefaultDepthGround = 0.0031f;

    float _heightMapDefault;

    float _pointForMoveX;
    float _pointForMoveZ;

    public static Terrain Ter;
    [SerializeField] GameObject _terrain;

    [SerializeField] GameObject _localTake;
    [SerializeField] GameObject _rayDirection;
    [SerializeField] GameObject _lopata;
    [SerializeField] GameObject _hands;

    GameObject _tempInHand;


    [SerializeField] float _distanceGive;

    float[,] _startHeights;
    public static float[,] Heights;

    int _pointX;
    int _pointZ;

    public static int PointXStatic;
    public static int PointZStatic;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<MovePlayer>();
        _take = gameObject.GetComponent<ITake>();
    }

    [Obsolete]
    void Start()
    {
        Ter = _terrain.GetComponent<Terrain>();
        _mapResol = Convert.ToSingle(Ter.terrainData.heightmapResolution) / 100;
        _heightMapDefault = Ter.terrainData.GetHeights(0, 0, 1, 1)[0, 0];
        _startHeights = Ter.terrainData.GetHeights(0, 0, Ter.terrainData.heightmapWidth, Ter.terrainData.heightmapHeight);
        Heights = _startHeights;
        for (int i = 0; i < Ter.terrainData.heightmapWidth; i++)
        {
            for (int j = 0; j < Ter.terrainData.heightmapHeight; j++)
            {
                Heights[i, j] = _startHeights[i, j];
            }
        }
    }

    [Obsolete]
    private void OnApplicationQuit()
    {
        DefaultTerrain();
    }

    [Obsolete]
    void DefaultTerrain()
    {
        for (int i = 0; i < Ter.terrainData.heightmapWidth; i++)
        {
            for (int j = 0; j < Ter.terrainData.heightmapHeight; j++)
            {
                _startHeights[i, j] = _heightMapDefault;
            }
        }
        Ter.terrainData.SetHeights(0, 0, _startHeights);
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
            if (_hitObject.collider != _terrain.GetComponent<TerrainCollider>())
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

            if (_hitObject.collider == _terrain.GetComponent<TerrainCollider>())
            {
                _lopata.transform.LookAt(_player.transform);
                _lopata.transform.position = new Vector3(_hitObject.point.x, _lopata.transform.position.y, _hitObject.point.z);
                _pointX = Convert.ToInt32(_hitObject.point.x * _mapResol);
                _pointZ = Convert.ToInt32(_hitObject.point.z * _mapResol);

                if (Heights[_pointZ, _pointX] >= _heightMapDefault)
                {
                    if (_player.CheckMove)
                    {
                        _lopata.SetActive(true);
                        _rayDirection.SetActive(true);
                    }
                    if (_take.Use() && _player.CheckMove)
                    {
                        _player.CheckMove = false;
                        _pointForMoveX = _hitObject.point.x;
                        _pointForMoveZ = _hitObject.point.z;
                        PointXStatic = Convert.ToInt32(_pointForMoveX * _mapResol);
                        PointZStatic = Convert.ToInt32(_pointForMoveZ * _mapResol);
                        _lopata.transform.position = new Vector3(_pointForMoveX, _lopata.transform.position.y, _pointForMoveZ);
                        _lopata.GetComponent<Animator>().Play("Digging");
                    }
                }
            }
        }
    }
}
