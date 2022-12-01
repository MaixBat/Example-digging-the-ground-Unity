using System;
using UnityEngine;

public class UseScript : MonoBehaviour
{
    event Action InfoObj;

    ControlButtons _CB;

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
        _CB = gameObject.GetComponent<ControlButtons>();
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
        if (!MovePlayer.Player.CheckMove)
            _lopata.SetActive(true);
        else
            _lopata.SetActive(false);

        Ray _centerScreen = new Ray(_rayDirection.transform.position, _rayDirection.transform.forward);

        if (Physics.Raycast(_centerScreen, out RaycastHit _hitObject, _distanceGive))
        {
            if (_hitObject.collider != _terrain.GetComponent<TerrainCollider>())
            {
                if (_CB.UseTarget())
                {
                    _rayDirection.SetActive(true);
                    if (_CB.Use() && MovePlayer.Player.CheckMove)
                    {
                        InfoObj += _hitObject.collider.gameObject.GetComponent<IObject>().PlaySound;
                        InfoObj += _hitObject.collider.gameObject.GetComponent<IObject>().TakeName;
                        InfoObj?.Invoke();
                        InfoObj -= _hitObject.collider.gameObject.GetComponent<IObject>().PlaySound;
                        InfoObj -= _hitObject.collider.gameObject.GetComponent<IObject>().TakeName;
                        if (_tempInHand != null)
                            Destroy(_tempInHand);
                        _hitObject.collider.gameObject.transform.parent = _hands.transform;
                        _hitObject.collider.gameObject.transform.localPosition = _localTake.transform.localPosition;
                        _tempInHand = _hitObject.collider.gameObject;
                    }
                }
            }

            if (_hitObject.collider == _terrain.GetComponent<TerrainCollider>())
            {
                _lopata.transform.LookAt(MovePlayer.Player.transform);
                _lopata.transform.position = new Vector3(_hitObject.point.x, _lopata.transform.position.y, _hitObject.point.z);
                _pointX = Convert.ToInt32(_hitObject.point.x * _mapResol);
                _pointZ = Convert.ToInt32(_hitObject.point.z * _mapResol);

                if (Heights[_pointZ, _pointX] >= _heightMapDefault && _CB.UseTarget())
                {
                    if (MovePlayer.Player.CheckMove)
                    {
                        _lopata.SetActive(true);
                        _rayDirection.SetActive(true);
                    }
                    if (_CB.Use() && MovePlayer.Player.CheckMove)
                    {
                        MovePlayer.Player.CheckMove = false;
                        _pointForMoveX = _hitObject.point.x;
                        _pointForMoveZ = _hitObject.point.z;
                        PointXStatic = Convert.ToInt32(_pointForMoveX * _mapResol);
                        PointZStatic = Convert.ToInt32(_pointForMoveZ * _mapResol);
                        _lopata.transform.position = new Vector3(_pointForMoveX, _lopata.transform.position.y, _pointForMoveZ);
                        _lopata.GetComponent<Animator>().Play("Digging");
                        /*DOTween.Sequence()
                            .AppendCallback(CheckMoveVoidF)
                            .Append(LopataInHand.transform.DOLocalMove(endValue: new Vector3(0.3784409f, -0.05f, 0.15f), duration: 0.5f))
                            .Append(LopataInHand.transform.DOLocalRotate(endValue: new Vector3(28.554f, 7.702f, 27.442f), duration: 0.3f))
                            .Append(LopataInHand.transform.DOLocalMove(endValue: new Vector3(0.2678188f, -0.3367314f, 0.830399f), duration: 0.1f))
                            .Append(LopataInHand.transform.DOLocalRotate(endValue: new Vector3(-1.71f, -2.496f, 22.479f), duration: 0.1f))
                            .AppendCallback(DirtTrue)
                            .Append(LopataInHand.transform.DOLocalMove(endValue: new Vector3(0.295495f, -0.4635951f, 0.9054654f), duration: 0.3f))
                            .Append(LopataInHand.transform.DOLocalRotate(endValue: new Vector3(-33.098f, -11.056f, 27.706f), duration: 0.4f))
                            .AppendCallback(SetHeights)
                            .Append(LopataInHand.transform.DOLocalRotate(endValue: new Vector3(-38.21f, -56.401f, 73.332f), duration: 0.2f))
                            .Append(LopataInHand.transform.DOLocalMove(endValue: new Vector3(-0.2987438f, -0.4296662f, 1.049282f), duration: 0.5f))
                            .Join(Dirt.transform.DOLocalMove(endValue: new Vector3(-3.89f, 2.71f, 1.49f), duration: 0.7f))
                            .Append(LopataInHand.transform.DOLocalRotate(endValue: new Vector3(14.877f, 6.996f, 25.548f), duration: 0.3f))
                            .Join(LopataInHand.transform.DOLocalMove(endValue: new Vector3(0.37844f, -0.502f, 0.827f), duration: 1f))
                            .AppendCallback(DirtFalse)
                            .Append(Dirt.transform.DOLocalMove(endValue: new Vector3(0.004f, 0.068f, 1.333f), duration: 0.01f))
                            .SetLoops(3)
                            .AppendCallback(CheckMoveVoidT);*/
                    }
                }
            }
        }
    }

    /*public void SetHeights()
    {
        Heights[PointZ, PointX] = DepthGround;
        Heights[PointZ - 1, PointX - 1] = DepthGround;
        Heights[PointZ + 1, PointX + 1] = DepthGround;
        Heights[PointZ + 1, PointX - 1] = DepthGround;
        Heights[PointZ - 1, PointX + 1] = DepthGround;
        Heights[PointZ + 1, PointX] = DepthGround;
        Heights[PointZ - 1, PointX] = DepthGround;
        Heights[PointZ, PointX - 1] = DepthGround;
        Heights[PointZ, PointX + 1] = DepthGround;
        ter.terrainData.SetHeights(0, 0, Heights);
    }*/
}
