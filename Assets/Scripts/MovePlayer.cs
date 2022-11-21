using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    public static MovePlayer Player { get; private set; }

    public GameObject Povorot;

    [SerializeField] float speed = 5;

    public float SenX = 5, SensY = 5;
    float moveY, moveX;
    public bool RootX = true, RootY = true;
    public bool TestY = true, TestX = false;
    public Vector2 MinMaxY = new Vector2(-70, 70), MinMaxX = new Vector2(-360, 360);

    private void Start()
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
        if (UseScript.Use.CheckMove)
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


    private void FixedUpdate()
    {
        if (UseScript.Use.CheckMove)
        {
            if (gameObject.GetComponent<Rigidbody>().velocity.magnitude > speed)
                gameObject.GetComponent<Rigidbody>().velocity = gameObject.GetComponent<Rigidbody>().velocity.normalized * speed;
            if (Input.GetKey(KeyCode.W))
                gameObject.GetComponent<Rigidbody>().AddForce(gameObject.transform.forward.normalized * speed, ForceMode.Impulse);
            if (Input.GetKey(KeyCode.S))
                gameObject.GetComponent<Rigidbody>().AddForce(-gameObject.transform.forward.normalized * speed, ForceMode.Impulse);
            if (Input.GetKey(KeyCode.A))
                gameObject.GetComponent<Rigidbody>().AddForce(-gameObject.transform.right.normalized * speed, ForceMode.Impulse);
            if (Input.GetKey(KeyCode.D))
                gameObject.GetComponent<Rigidbody>().AddForce(gameObject.transform.right.normalized * speed, ForceMode.Impulse);
        }
    }
}
