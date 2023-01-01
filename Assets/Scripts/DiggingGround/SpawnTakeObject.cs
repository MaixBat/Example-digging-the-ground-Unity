using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTakeObject : MonoBehaviour
{
    [SerializeField] GameObject[] _spawnObjects;

    private void Awake()
    {
        int _countObj = Random.Range(5,30);
        for (int i = 0; i <= _countObj; i++)
        {
            Instantiate(_spawnObjects[Random.Range(0, _spawnObjects.Length)], new Vector3(Random.Range(0.2625f, 5.733f), Random.Range(-0.1813f, -0.1657f), Random.Range(0.2697f, 5.762f)), Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f)));
        }
    }
}
