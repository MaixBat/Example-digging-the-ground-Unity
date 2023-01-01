using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]
public class ControllerGame : ScriptableObject
{
    void Awake()
    {
        Application.targetFrameRate = 60;
    }
}
