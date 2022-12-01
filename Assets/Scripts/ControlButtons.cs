using UnityEngine;

public class ControlButtons : MonoBehaviour
{
    IControl _control;

    private void Awake()
    {
        _control = gameObject.GetComponent<IControl>();
    }

    public void MovePlayerMethod()
    {
        _control.Move();
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
