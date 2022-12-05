using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    [SerializeField] GameObject _controller;
    IControl _control;

    public bool CheckMove = true;

    public GameObject Camera;

    public float Speed = 5;

    public float SensX = 5, SensY = 5;
    [HideInInspector] public float MoveY, MoveX;
    public bool RootX = true, RootY = true;
    public bool TestY = true, TestX = false;
    public Vector2 MinMaxY = new Vector2(-70, 70), MinMaxX = new Vector2(-360, 360);

    private void Awake()
    {
        _control = _controller.GetComponent<IControl>();
    }

    void Update()
    {
        if (CheckMove)
        {
            _control.Rotate();
        }
    }

    private void FixedUpdate()
    {
        if (CheckMove)
        {
            _control.Move();
        }
    }
}
