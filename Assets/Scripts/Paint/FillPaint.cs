using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FillPaint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PaintPoint"))
        {
            Paint.AllowPaint = true;
            Paint.StartAllowPaint = true;
        }
    }
}
