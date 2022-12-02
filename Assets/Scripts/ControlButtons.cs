using UnityEngine;

public class ControlButtons : MonoBehaviour, ITake, IControl
{
    public void Move()
    {
        Vector3 Move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        MovePlayer.Player.transform.Translate(Move * MovePlayer.Player.Speed * Time.fixedDeltaTime);
    }

    public bool Use()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            return true;
        }
        return false;
    }

    public bool ActivateTakeItem()
    {
        if (Input.GetKey(KeyCode.Q))
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
