using UnityEngine;

public class LopataColor : MonoBehaviour
{
    public GameObject LopataPart1;
    public GameObject LopataPart2;

    [SerializeField] Material _transparent;
    [SerializeField] Material _normal;

    public void ChangeOnTransparent()
    {
        LopataPart1.GetComponent<Renderer>().material = _transparent;
        LopataPart2.GetComponent<Renderer>().material = _transparent;
    }

    public void ChangeOnNormal()
    {
        LopataPart1.GetComponent<Renderer>().material = _normal;
        LopataPart2.GetComponent<Renderer>().material = _normal;
    }
}
