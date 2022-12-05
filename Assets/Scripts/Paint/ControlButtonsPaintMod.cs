using UnityEngine;

public class ControlButtonsPaintMod : MonoBehaviour, IControl, IClick
{
    MovePlayer _player;
    Paint _paint;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<MovePlayer>();
        _paint = GetComponent<Paint>();
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

    public void ClickLeftButton()
    {
        if (Input.GetMouseButton(0))
        {
            _paint.SetPixelColor();
        }
        if (Input.GetMouseButtonUp(0))
        {
            _paint.SetDefaultPosition();
        }
    }
}
