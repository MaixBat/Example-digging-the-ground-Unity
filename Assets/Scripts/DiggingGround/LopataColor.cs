using UnityEngine;

public class LopataColor : MonoBehaviour
{
    [SerializeField] GameObject shovel;

    [SerializeField] Material _transparent;
    [SerializeField] Material _normal;

    public void ChangeOnTransparent()
    {
        shovel.GetComponent<Renderer>().material = _transparent;
    }

    public void ChangeOnNormal()
    {
        shovel.GetComponent<Renderer>().material = _normal;
    }
}
