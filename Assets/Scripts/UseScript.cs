using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseScript : MonoBehaviour
{
    float MapResol;
    public static float DepthGround = 0.0031f;
    public static float DefaultDepthGround = 0.0031f;

    float HeightMapDefault;

    bool NeedMove = false;

    float PointForMoveX;
    float PointForMoveZ;

    Vector3 TempPoint;

    public static Terrain ter;
    [SerializeField] GameObject terrain;

    [SerializeField] GameObject Lopata;
    [SerializeField] GameObject LopataInHand;
    [SerializeField] GameObject Hands;
    [SerializeField] GameObject Dirt;


    [SerializeField] float DistanceGive;

    float[,] StartHeights;
    public static float[,] Heights;

    public static int PointX;
    public static int PointZ;

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
        Lopata.SetActive(false);

        Ray CenterScreen = MovePlayer.Player.Povorot.GetComponent<Camera>().ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if (!MovePlayer.Player.CheckMove)
        {
            Hands.SetActive(false);
            LopataInHand.SetActive(true);
        }
        else
        {
            LopataInHand.SetActive(false);
            Hands.SetActive(true);
        }

        if (NeedMove)
            DistancePoint();

        if (Physics.Raycast(CenterScreen, out RaycastHit HitObject, DistanceGive))
        {
            if (HitObject.collider == terrain.GetComponent<TerrainCollider>())
            {
                Lopata.transform.LookAt(gameObject.transform);
                Lopata.transform.position = new Vector3(HitObject.point.x, Lopata.transform.position.y, HitObject.point.z);
                PointX = Convert.ToInt32(HitObject.point.x * MapResol);
                PointZ = Convert.ToInt32(HitObject.point.z * MapResol);

                if (Heights[PointZ, PointX] >= HeightMapDefault && Input.GetMouseButton(1))
                {
                    if (MovePlayer.Player.CheckMove)
                        Lopata.SetActive(true);
                    if (Input.GetKeyDown(KeyCode.E) && MovePlayer.Player.CheckMove)
                    {
                        MovePlayer.Player.CheckMove = false;
                        PointForMoveX = HitObject.point.x;
                        PointForMoveZ = HitObject.point.z;
                        TempPoint = HitObject.point;
                        DistancePoint();
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

    public void DistancePoint()
    {
        if (MovePlayer.Player.transform.position.x > PointForMoveX + 0.5f || MovePlayer.Player.transform.position.x < PointForMoveX - 0.5f || MovePlayer.Player.transform.position.z > PointForMoveZ + 0.5f || MovePlayer.Player.transform.position.z < PointForMoveZ - 0.5f)
        {
            NeedMove = true;
            MovePlayer.Player.transform.position = Vector3.Lerp(MovePlayer.Player.transform.position, new Vector3(PointForMoveX, MovePlayer.Player.transform.position.y, PointForMoveZ), MovePlayer.Player.speed/100);
        }
        else
        {
            MovePlayer.Player.Povorot.transform.LookAt(TempPoint);
            MovePlayer.Player.GetComponent<Animator>().Play("Digging");
            NeedMove = false;
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
