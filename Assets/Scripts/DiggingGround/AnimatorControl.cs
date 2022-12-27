using System;
using UnityEngine;

public class AnimatorControl : MonoBehaviour
{
    [SerializeField] UseScript _useScript;

    MovePlayer _player;

    public LopataColor _colorChangeLopata;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<MovePlayer>();
    }

    public void CheckMoveTrue()
    {
        _player.CheckMove = true;
    }

    public void CheckMoveFalse()
    {
        _player.CheckMove = false;
    }

    public void SetHeights()
    {
        int intPointZ = (int)_useScript.PointZ;
        int intPointX = (int)_useScript.PointX;
        for (int i = 1; i <= _useScript._radius; i++)
        {
            if (i == _useScript._radius)
            {
                _useScript._deepDigging = -0.1f;
            }
            for (int j = 0; j <= _useScript._radius - 1; j++)
            {
                try
                {
                    if (_useScript.coordinate[intPointZ - j, intPointX - i].y >= _useScript._deepDigging)
                        _useScript.coordinate[intPointZ - j, intPointX - i] = new Vector3(_useScript.coordinate[intPointZ - j, intPointX - i].x, _useScript._deepDigging, _useScript.coordinate[intPointZ - j, intPointX - i].z);
                    if (_useScript.coordinate[intPointZ + j, intPointX + i].y >= _useScript._deepDigging)
                        _useScript.coordinate[intPointZ + j, intPointX + i] = new Vector3(_useScript.coordinate[intPointZ + j, intPointX + i].x, _useScript._deepDigging, _useScript.coordinate[intPointZ + j, intPointX + i].z);
                    if (_useScript.coordinate[intPointZ - i, intPointX - j].y >= _useScript._deepDigging)
                        _useScript.coordinate[intPointZ - i, intPointX - j] = new Vector3(_useScript.coordinate[intPointZ - i, intPointX - j].x, _useScript._deepDigging, _useScript.coordinate[intPointZ - i, intPointX - j].z);
                    if (_useScript.coordinate[intPointZ + i, intPointX + j].y >= _useScript._deepDigging)
                        _useScript.coordinate[intPointZ + i, intPointX + j] = new Vector3(_useScript.coordinate[intPointZ + i, intPointX + j].x, _useScript._deepDigging, _useScript.coordinate[intPointZ + i, intPointX + j].z);
                    if (_useScript.coordinate[intPointZ - i, intPointX + j].y >= _useScript._deepDigging)
                        _useScript.coordinate[intPointZ - i, intPointX + j] = new Vector3(_useScript.coordinate[intPointZ - i, intPointX + j].x, _useScript._deepDigging, _useScript.coordinate[intPointZ - i, intPointX + j].z);
                    if (_useScript.coordinate[intPointZ + i, intPointX - j].y >= _useScript._deepDigging)
                        _useScript.coordinate[intPointZ + i, intPointX - j] = new Vector3(_useScript.coordinate[intPointZ + i, intPointX - j].x, _useScript._deepDigging, _useScript.coordinate[intPointZ + i, intPointX - j].z);
                    if (_useScript.coordinate[intPointZ - j, intPointX + i].y >= _useScript._deepDigging)
                        _useScript.coordinate[intPointZ - j, intPointX + i] = new Vector3(_useScript.coordinate[intPointZ - j, intPointX + i].x, _useScript._deepDigging, _useScript.coordinate[intPointZ - j, intPointX + i].z);
                    if (_useScript.coordinate[intPointZ + j, intPointX - i].y >= _useScript._deepDigging)
                        _useScript.coordinate[intPointZ + j, intPointX - i] = new Vector3(_useScript.coordinate[intPointZ + j, intPointX - i].x, _useScript._deepDigging, _useScript.coordinate[intPointZ + j, intPointX - i].z);
                    _useScript._deepDigging -= _useScript._smooth;
                }
                catch (IndexOutOfRangeException)
                {
                    Debug.Log("OutOfRange");
                }
            }
        }
        SetDefaultDepth();
        if (_useScript.coordinate[intPointZ, intPointX].y > _useScript._deepDigging)
            _useScript.coordinate[intPointZ, intPointX] = new Vector3(_useScript.coordinate[intPointZ, intPointX].x, _useScript._deepDigging, _useScript.coordinate[intPointZ, intPointX].z);
        
        int k = 0;
        for (int i = 0; i < Mathf.Sqrt(_useScript.vertices.Length); i++)
        {
            for (int j = 0; j < Mathf.Sqrt(_useScript.vertices.Length); j++)
            {
                _useScript.vertices[k] = _useScript.coordinate[i, j];
                k++;
            }
        }
        _useScript.mesh.vertices = _useScript.vertices;
        _useScript.mesh.RecalculateBounds();
        _useScript._meshCollider.sharedMesh = _useScript.mesh;
    }

    public void ChangeColorOnTransparent()
    {
        _colorChangeLopata.ChangeOnTransparent();
    }

    public void ChangeColorOnNormal()
    {
        _colorChangeLopata.ChangeOnNormal();
    }

    public void SetDefaultDepth()
    {
        _useScript._deepDigging = _useScript._startDeepDigging;
    }
}
