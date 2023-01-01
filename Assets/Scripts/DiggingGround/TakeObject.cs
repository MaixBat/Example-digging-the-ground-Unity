using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeObject : MonoBehaviour
{
    [HideInInspector] public bool canClear = true;
    [HideInInspector] public bool isClear;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Stone") || other.CompareTag("UndergroundWall"))
            canClear = false;
        else
            canClear = true;
    }

    private void FixedUpdate()
    {
        canClear = true;
    }
}
