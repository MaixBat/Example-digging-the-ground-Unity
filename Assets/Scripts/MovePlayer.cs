using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    [SerializeField] UseScript _useScript;

    [SerializeField] GameObject _ground;

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlaceDigging"))
        {
            Transform[] obj = other.GetComponentsInChildren<Transform>();
            _ground.transform.position = obj[1].gameObject.transform.position;
            obj[1].gameObject.GetComponent<MeshRenderer>().enabled = false;
            obj[1].gameObject.GetComponent<MeshCollider>().enabled = false;
            for (int i = 2; i <= _ground.transform.position.z; i++)
            {
                _useScript._correctorZ += 20;
            }
            for (int i = 2; i <= _ground.transform.position.x; i++)
            {
                _useScript._correctorX += 20;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlaceDigging"))
        {
            Transform[] obj = other.GetComponentsInChildren<Transform>();
            obj[1].gameObject.GetComponent<MeshRenderer>().enabled = true;
            obj[1].gameObject.GetComponent<MeshCollider>().enabled = true;
            _useScript.SetDefaultMesh();
            _useScript._correctorZ = 0;
            _useScript._correctorX = 0;
        }
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
