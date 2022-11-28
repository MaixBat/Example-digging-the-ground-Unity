using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlButtons : MonoBehaviour
{
    IControl control;

    private void Awake()
    {
        control = gameObject.GetComponent<IControl>();
    }

    public void MovePlayerMethod()
    {
        control.Move();
    }

    public bool Use()
    {
        if (Input.GetKeyDown(KeyCode.E))
            return true;
        return false;
    }

    public static bool UseTarget()
    {
        if (Input.GetMouseButton(1))
            return true;
        return false;
    }
}
