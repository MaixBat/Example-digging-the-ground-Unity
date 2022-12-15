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
    [SerializeField] private Texture2D _texturePaint;
    [SerializeField] private Texture2D _textureStart;
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
    // ���������� ��������� ����������� ��������
    int _countRight = 0;
    // ���������� ����������� ����������� ��������
    int _countWrong = 0;


    private void OnApplicationQuit()
    {
        SetDefaultColor();
    }

    private void Awake()
    {
        _textureWidth = _texturePaint.width;
        _textureHeight = _texturePaint.height;

        _defaultPixels = new Color[_textureWidth * _textureHeight + 1];

        // ������� ��� ������� ��������� �������� � �������� ��������
        int k = 0;
        // ��������� ��� ������� �� �����
        for (int i = 0; i < _texturePaint.width; i++)
        {
            for (int j = 0; j < _texturePaint.height; j++)
            {
                _defaultPixels[k] = _texturePaint.GetPixel(i, j);
                k++;
            }
        }
        ///////////////////////////////////
    }

    // ����� ��������� ������������ ���������
    void EndEquals()
    {
        for (int i = 0; i < _texturePaint.width; i++)
        {
            for (int j = 0; j < _texturePaint.height; j++)
            {
                if (_texturePaint.GetPixel(i, j) == Color.black)
                {
                    if (_textureStart.GetPixel(i, j) != Color.white)
                        _countRight++;
                    else
                        _countWrong++;
                }
            }
        }
    }
    ///////////////////////////////////

    // ���������� �������� ����� ����� �� ���������
    void SetDefaultColor()
    {
        int k = 0;
        for (int i = 0; i < _texturePaint.width; i++)
        {
            for (int j = 0; j < _texturePaint.height; j++)
            {
                _texturePaint.SetPixel(i, j, _defaultPixels[k]);
                k++;
            }
        }
        _texturePaint.Apply();
    }
    //////////////////////////////////////////////

    private void Update()
    {
        // ��� �������� ������ ���������
        if (Input.GetMouseButton(1))
        {
            EndEquals();
            Debug.Log("Right " + _countRight);
            Debug.Log("Wrong " + _countWrong);
            _countRight = 0;
            _countWrong = 0;
        }
        ///////////////////////////////
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
            for (int k = 0; k < 100; k++)
            {
                _point = Vector2.Lerp(_point, _tempPoint, 0.02f);
                int _tempX = (int)_point.x;
                int _tempZ = (int)_point.y;
                for (int i = 0; i < _brushSize; i++)
                {
                    for (int j = 0; j < _brushSize; j++)
                    {
                        float x2 = Mathf.Pow(i - _brushSize / 2, 2);
                        float y2 = Mathf.Pow(j - _brushSize / 2, 2);
                        float r2 = Mathf.Pow(_brushSize / 2 - 0.5f, 2);

                        if (x2 + y2 < r2)
                        {
                            int pixelX = _tempX + i - _brushSize / 2;
                            int pixelY = _tempZ + j - _brushSize / 2;

                            Color oldColor = _texturePaint.GetPixel(pixelX, pixelY);
                            Color resultColor = Color.Lerp(oldColor, Color.black, Color.black.a);
                            _texturePaint.SetPixel(pixelX, pixelY, resultColor);
                        }
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
            for (int i = 0; i < _brushSize; i++)
            {
                for (int j = 0; j < _brushSize; j++)
                {
                    float x2 = Mathf.Pow(i - _brushSize / 2, 2);
                    float y2 = Mathf.Pow(j - _brushSize / 2, 2);
                    float r2 = Mathf.Pow(_brushSize / 2 - 0.5f, 2);

                    if (x2 + y2 < r2)
                    {
                        int pixelX = _rayX + i - _brushSize / 2;
                        int pixelY = _rayZ + j - _brushSize / 2;

                        Color oldColor = _texturePaint.GetPixel(pixelX, pixelY);
                        Color resultColor = Color.Lerp(oldColor, Color.black, Color.black.a);
                        _texturePaint.SetPixel(pixelX, pixelY, resultColor);
                    }
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
                    _texturePaint.Apply();
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