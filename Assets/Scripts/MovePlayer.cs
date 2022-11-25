﻿using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    public static MovePlayer Player { get; private set; }

    public bool CheckMove = true;

    public GameObject Povorot;

    public float speed = 5;

    public float SenX = 5, SensY = 5;
    float moveY, moveX;
    public bool RootX = true, RootY = true;
    public bool TestY = true, TestX = false;
    public Vector2 MinMaxY = new Vector2(-70, 70), MinMaxX = new Vector2(-360, 360);

    private void Awake()
    {
        Player = this;
    }

    static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F) 
            angle += 360F;
        if (angle > 360F) 
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }

    void Update()
    {
        if (CheckMove)
        {
            if (RootY)
                moveY -= Input.GetAxis("Mouse Y") * SensY;
            if (TestY)
                moveY = ClampAngle(moveY, MinMaxY.x, MinMaxY.y);
            if (RootX)
                moveX += Input.GetAxis("Mouse X") * SenX;
            if (TestX)
                moveX = ClampAngle(moveX, MinMaxX.x, MinMaxX.y);

            Povorot.transform.rotation = Quaternion.Euler(moveY, moveX, 0);
            gameObject.transform.rotation = Quaternion.Euler(gameObject.transform.rotation.y, moveX, 0);
        }
    }

    public void CheckMoveTrue()
    {
        CheckMove = true;
    }

    public void CheckMoveFalse()
    {
        CheckMove = false;
    }

    public void SetHeights()
    {
        UseScript.Heights[UseScript.PointZStatic, UseScript.PointXStatic] = UseScript.DepthGround;
        UseScript.ter.terrainData.SetHeights(0, 0, UseScript.Heights);
        UseScript.DepthGround -= 0.0001f;
    }

    public void DefaultDepth()
    {
        UseScript.DepthGround = UseScript.DefaultDepthGround;
    }

    private void FixedUpdate()
    {
        if (CheckMove)
        {
            ControlButtons.MovePlayerMethod();
        }
    }
}
