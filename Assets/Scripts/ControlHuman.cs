using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlHuman : MonoBehaviour, IControl
{
    public void Move()
    {
        Vector3 Move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        MovePlayer.Player.transform.Translate(Move * MovePlayer.Player.speed * Time.fixedDeltaTime);
    }
}
