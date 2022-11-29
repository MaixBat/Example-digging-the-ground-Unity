using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    public UnityAction Use(GameObject gameobj)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            return gameobj.GetComponent<IObject>().Info();
        }
        return null;
    }

    public bool Use()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            return true;
        }
        return false;
    }

    public bool UseTarget()
    {
        if (Input.GetMouseButton(1))
            return true;
        return false;
    }
}
