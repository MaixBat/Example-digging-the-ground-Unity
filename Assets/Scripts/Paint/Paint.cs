using System;
using UnityEngine;

public class Paint : MonoBehaviour
{
    [SerializeField] GameObject _camera;
    // ����� ��� ���������
    [SerializeField] GameObject _wall;
    // ������ �������� �����
    private int _textureWidth;
    // ������ �������� �����
    private int _textureHeight;
    // �������� �����
    [SerializeField] private Texture2D _texture;
    // ������ �����
    [SerializeField] private int _brushSize = 8;
    // ������� ��� ������������ ���������� ��������
    int _counter = 0;
    // �������� � ���������� ��������
    [SerializeField] [Range(1, 10000)] int _delay = 50;
    // ����� � ������������ ��� �������� ���������� �����
    Vector2 _point;
    // �������� ��������� ������� ����������� ����� �� �����
    bool _checkPosition = false;
    // ��������� �������� ������ �������� �� ��������
    Color[] _defaultPixels;
    // ������� ����������
    float _percent = 100f;
    // ������� ������� ���������� � ������ ���� �������� ����� �������
    [SerializeField] [Range(0.01f, 1)] float _percentMinus = 0.1f;


    private void OnApplicationQuit()
    {
        SetDefaultColor();
    }

    private void Awake()
    {
        _textureWidth = _texture.width;
        _textureHeight = _texture.height;

        _defaultPixels = new Color[_textureWidth * _textureHeight + 1];

        // ������� ��� ������� ��������� �������� � �������� ��������
        int k = 0;
        // ��������� ��� ������� �� �����
        for (int i = 0; i < _texture.width; i++)
        {
            for (int j = 0; j < _texture.height; j++)
            {
                _defaultPixels[k] = _texture.GetPixel(i, j);
                k++;
            }
        }
        ///////////////////////////////////
    }

    // ���������� �������� ����� ����� �� ���������
    void SetDefaultColor()
    {
        int k = 0;
        for (int i = 0; i < _texture.width; i++)
        {
            for (int j = 0; j < _texture.height; j++)
            {
                _texture.SetPixel(i, j, _defaultPixels[k]);
                k++;
            }
        }
        _texture.Apply();
    }
    //////////////////////////////////////////////

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            SetPixelColor();
        }
        if (Input.GetMouseButtonUp(0))
        {
            SetDefaultPosition();
        }
    }

    // ����� ���������
    void OnPaint(int _rayX, int _rayZ)
    {
        if (_point != new Vector2())
        {
            // ���������� �� ������ ��� �������� ���������
            // ����� � ������� ����� ��������� �����
            Vector2 _tempPoint = new Vector2(_rayX, _rayZ);
            for (int k = 0; k < 10; k++)
            {
                _point = Vector2.Lerp(_point, _tempPoint, 0.03f);
                int _tempX = (int)_point.x;
                int _tempZ = (int)_point.y;
                for (int i = _tempX - _brushSize; i < _tempX + _brushSize; i++)
                {
                    for (int j = _tempZ - _brushSize; j < _tempZ + _brushSize; j++)
                    {
                        _texture.SetPixel(i, j, Color.black);
                        _counter++;
                    }
                }
                if (_point == _tempPoint)
                    _checkPosition = true;
            }

            if (_checkPosition)
            {
                _point = new Vector2(_rayX, _rayZ);
                _checkPosition = false;
            }
            //////////////////////////////////////////////
        }
        else
        {
            // ���� ��� ��������� ����� � ������� ���� ���������
            _point = new Vector2(_rayX, _rayZ);
            for (int i = _rayX - _brushSize; i < _rayX + _brushSize; i++)
            {
                for (int j = _rayZ - _brushSize; j < _rayZ + _brushSize; j++)
                {
                    _texture.SetPixel(i, j, Color.black);
                    _counter++;
                }
            }
            //////////////////////////////////////////////
        }
    }
    //////////////////////////////////////////////

    // ���������� ����������� �������� �� ��������
    public void SetPixelColor()
    {
        Ray ray = _camera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 5f))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                OnPaint(Convert.ToInt32(hit.textureCoord.x * _textureWidth), Convert.ToInt32(hit.textureCoord.y * _textureHeight));

                if (_counter >= _delay)
                {
                    // �������� �������� ����� ��� ���������
                    _texture.Apply();
                    _counter = 0;
                }
            }
            else
                SetDefaultPosition();
        }
        else
        {
            SetDefaultPosition();
        }
    }
    //////////////////////////////////////////////

    // �������� ������� ��������� �� ��������� (��� ������ ���� ���������)
    public void SetDefaultPosition()
    {
        _point = new Vector2();
    }
    //////////////////////////////////////////////
}
