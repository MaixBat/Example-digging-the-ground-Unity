using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UseScript : MonoBehaviour
{
    UnityEvent InfoObj = new UnityEvent();

    ControlButtons CB;

    float MapResol;
    public static float DepthGround = 0.0031f;
    public static float DefaultDepthGround = 0.0031f;

    float HeightMapDefault;

    float PointForMoveX;
    float PointForMoveZ;

    public static Terrain ter;
    [SerializeField] GameObject terrain;

    [SerializeField] GameObject LocalTake;
    [SerializeField] GameObject RayDirection;
    [SerializeField] GameObject Lopata;
    [SerializeField] GameObject Hands;

    GameObject TempInHand;


    [SerializeField] float DistanceGive;

    float[,] StartHeights;
    public static float[,] Heights;

    int PointX;
    int PointZ;

    public static int PointXStatic;
    public static int PointZStatic;

    private void Awake()
    {
        CB = gameObject.GetComponent<ControlButtons>();
    }

    [Obsolete]
    void Start()
    {
        ter = terrain.GetComponent<Terrain>();
        MapResol = Convert.ToSingle(ter.terrainData.heightmapResolution) / 100;
        HeightMapDefault = ter.terrainData.GetHeights(0, 0, 1, 1)[0, 0];
        StartHeights = ter.terrainData.GetHeights(0, 0, ter.terrainData.heightmapWidth, ter.terrainData.heightmapHeight);
        Heights = StartHeights;
        for (int i = 0; i < ter.terrainData.heightmapWidth; i++)
        {
            for (int j = 0; j < ter.terrainData.heightmapHeight; j++)
            {
                Heights[i, j] = StartHeights[i, j];
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
        for (int i = 0; i < ter.terrainData.heightmapWidth; i++)
        {
            for (int j = 0; j < ter.terrainData.heightmapHeight; j++)
            {
                StartHeights[i, j] = HeightMapDefault;
            }
        }
        ter.terrainData.SetHeights(0, 0, StartHeights);
    }

    void Update()
    {
        RayDirection.SetActive(false);
        if (!MovePlayer.Player.CheckMove)
            Lopata.SetActive(true);
        else
            Lopata.SetActive(false);

        Ray CenterScreen = new Ray(RayDirection.transform.position, RayDirection.transform.forward);
        //Debug.DrawRay(RayDirection.transform.position, RayDirection.transform.forward);
        //Ray CenterScreen = MovePlayer.Player.Povorot.GetComponent<Camera>().ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if (Physics.Raycast(CenterScreen, out RaycastHit HitObject, DistanceGive))
        {
            if (HitObject.collider != terrain.GetComponent<TerrainCollider>())
            {
                if (CB.UseTarget())
                {
                    RayDirection.SetActive(true);
                    if (CB.Use(HitObject.collider.gameObject) != null && MovePlayer.Player.CheckMove)
                    {
                        //Debug.Log(CB.Use(HitObject.collider.gameObject));
                        InfoObj.AddListener(CB.Use(HitObject.collider.gameObject));
                        InfoObj.Invoke();
                        if (TempInHand != null)
                            Destroy(TempInHand);
                        HitObject.collider.gameObject.transform.parent = Hands.transform;
                        HitObject.collider.gameObject.transform.localPosition = LocalTake.transform.localPosition;
                        TempInHand = HitObject.collider.gameObject;
                    }
                }
            }

            if (HitObject.collider == terrain.GetComponent<TerrainCollider>())
            {
                Lopata.transform.LookAt(MovePlayer.Player.transform);
                Lopata.transform.position = new Vector3(HitObject.point.x, Lopata.transform.position.y, HitObject.point.z);
                PointX = Convert.ToInt32(HitObject.point.x * MapResol);
                PointZ = Convert.ToInt32(HitObject.point.z * MapResol);

                if (Heights[PointZ, PointX] >= HeightMapDefault && CB.UseTarget())
                {
                    if (MovePlayer.Player.CheckMove)
                    {
                        Lopata.SetActive(true);
                        RayDirection.SetActive(true);
                    }
                    if (CB.Use() && MovePlayer.Player.CheckMove)
                    {
                        MovePlayer.Player.CheckMove = false;
                        PointForMoveX = HitObject.point.x;
                        PointForMoveZ = HitObject.point.z;
                        PointXStatic = Convert.ToInt32(PointForMoveX * MapResol);
                        PointZStatic = Convert.ToInt32(PointForMoveZ * MapResol);
                        Lopata.transform.position = new Vector3(PointForMoveX, Lopata.transform.position.y, PointForMoveZ);
                        Lopata.GetComponent<Animator>().Play("Digging");
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
