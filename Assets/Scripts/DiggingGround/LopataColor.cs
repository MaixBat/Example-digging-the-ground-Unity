using UnityEngine;

public class LopataColor : MonoBehaviour
{
    [SerializeField] GameObject _shovel;
    [SerializeField] GameObject _shovelSofk;
    [SerializeField] GameObject _knife;
    [SerializeField] Material _transparent;
    [SerializeField] Material _normal;
    [SerializeField] Material _transparentSofk;
    [SerializeField] Material _normalSofk;
    [SerializeField] Material _transparentKnife;
    [SerializeField] Material _normalKnife;

    public void ChangeOnTransparent()
    {
        _shovel.GetComponent<Renderer>().material = _transparent;
        _shovelSofk.GetComponent<Renderer>().material = _transparentSofk;
        _knife.GetComponent<Renderer>().material = _transparentKnife;
    }

    public void ChangeOnNormal()
    {
        _shovel.GetComponent<Renderer>().material = _normal;
        _shovelSofk.GetComponent<Renderer>().material = _normalSofk;
        _knife.GetComponent<Renderer>().material = _normalKnife;
    }
}
