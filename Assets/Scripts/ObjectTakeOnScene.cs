using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTakeOnScene : MonoBehaviour
{
    [SerializeField] GameObject _localTake;
    GameObject _tempObj;
    Transform[] _children;
    Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>
    {
    };

    public void Awake()
    {
        _children = gameObject.GetComponentsInChildren<Transform>();
        int i = 0;
        foreach (Transform el in _children)
        {
            _objects.Add(i, el.gameObject);
            i++;
        }
        _objects.Remove(0);
    }

    private void Update()
    {
        foreach (int el in _objects.Keys)
        {
            try
            {
                if (el == Convert.ToInt32(Input.inputString))
                {
                    if (_tempObj != null)
                        Destroy(_tempObj);
                    _objects[Convert.ToInt32(Input.inputString)].transform.parent = _localTake.transform;
                    _objects[Convert.ToInt32(Input.inputString)].transform.position = _localTake.transform.position;
                    _tempObj = _objects[Convert.ToInt32(Input.inputString)];
                }
            }
            catch (FormatException) {}
            catch (MissingReferenceException) {}
        }
    }
}
