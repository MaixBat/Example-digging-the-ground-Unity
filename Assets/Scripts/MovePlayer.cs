using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    public static MovePlayer Player { get; private set; }
    [SerializeField] ControlButtons _CB;

    public bool CheckMove = true;

    public GameObject Camera;

    public float Speed = 5;

    [SerializeField] float _sensX = 5, _sensY = 5;
    float _moveY, _moveX;
    [SerializeField] bool _rootX = true, _rootY = true;
    [SerializeField] bool _testY = true, _testX = false;
    [SerializeField] Vector2 _minMaxY = new Vector2(-70, 70), _minMaxX = new Vector2(-360, 360);

    private void Awake()
    {
        Player = this;
    }

    static float ClampAngle(float _angle, float _min, float _max)
    {
        if (_angle < -360F)
            _angle += 360F;
        if (_angle > 360F)
            _angle -= 360F;
        return Mathf.Clamp(_angle, _min, _max);
    }

    void Update()
    {
        if (CheckMove)
        {
            if (_rootY)
                _moveY -= Input.GetAxis("Mouse Y") * _sensY;
            if (_testY)
                _moveY = ClampAngle(_moveY, _minMaxY.x, _minMaxY.y);
            if (_rootX)
                _moveX += Input.GetAxis("Mouse X") * _sensX;
            if (_testX)
                _moveX = ClampAngle(_moveX, _minMaxX.x, _minMaxX.y);

            Camera.transform.rotation = Quaternion.Euler(_moveY, _moveX, 0);
            gameObject.transform.rotation = Quaternion.Euler(gameObject.transform.rotation.y, _moveX, 0);
        }
    }

    private void FixedUpdate()
    {
        if (CheckMove)
        {
            _CB.MovePlayerMethod();
        }
    }
}
