using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LopataColor : MonoBehaviour
{
    public GameObject LopataPart1;
    public GameObject LopataPart2;

    [SerializeField] Material Transparent;
    [SerializeField] Material Normal;

    public void ChangeT()
    {
        LopataPart1.GetComponent<Renderer>().material = Transparent;
        LopataPart2.GetComponent<Renderer>().material = Transparent;
    }

    public void ChangeN()
    {
        LopataPart1.GetComponent<Renderer>().material = Normal;
        LopataPart2.GetComponent<Renderer>().material = Normal;
    }
}
