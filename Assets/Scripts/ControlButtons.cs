using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlButtons : MonoBehaviour
{
    public static void MovePlayerMethod()
    {
        Vector3 Move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        MovePlayer.Player.transform.Translate(Move * MovePlayer.Player.speed * Time.fixedDeltaTime);
    }

    public static bool Use()
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
