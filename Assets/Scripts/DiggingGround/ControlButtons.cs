using UnityEngine;

public class ControlButtons : MonoBehaviour, ITake, IControl
{
    MovePlayer _player;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<MovePlayer>();
    }

    public void Move()
    {
        Vector3 Move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        _player.transform.Translate(Move * _player.Speed * Time.fixedDeltaTime);
    }

    public void Rotate()
    {
        if (_player.RootY)
            _player.MoveY -= Input.GetAxis("Mouse Y") * _player.SensY;
        if (_player.TestY)
            _player.MoveY = ClampAngle(_player.MoveY, _player.MinMaxY.x, _player.MinMaxY.y);
        if (_player.RootX)
            _player.MoveX += Input.GetAxis("Mouse X") * _player.SensX;
        if (_player.TestX)
            _player.MoveX = ClampAngle(_player.MoveX, _player.MinMaxX.x, _player.MinMaxX.y);

        _player.Camera.transform.rotation = Quaternion.Euler(_player.MoveY, _player.MoveX, 0);
        _player.transform.rotation = Quaternion.Euler(_player.transform.rotation.y, _player.MoveX, 0);
    }

    static float ClampAngle(float _angle, float _min, float _max)
    {
        if (_angle < -360f)
            _angle += 360f;
        if (_angle > 360f)
            _angle -= 360f;
        return Mathf.Clamp(_angle, _min, _max);
    }

    public bool Use() => Input.GetKeyDown(KeyCode.E);

    public bool ActivateTakeItem() => Input.GetKey(KeyCode.Q);

    public bool UseTarget() => Input.GetMouseButton(1);
}
