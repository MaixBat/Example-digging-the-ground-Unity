using System;
using UnityEngine;

public class Paint : MonoBehaviour
{
    [SerializeField] GameObject _camera;
    // Стена для рисования
    [SerializeField] GameObject _wall;
    // Ширина текстуры стены
    private int _textureWidth;
    // Высота текстуры стены
    private int _textureHeight;
    // Текстура стены
    [SerializeField] private Texture2D _texturePaint;
    [SerializeField] private Texture2D _textureStart;
    // Размер кисти
    [SerializeField] private int _brushSize = 8;
    // Счётчик для срабатывания применения текстуры
    int _counter = 0;
    // Задержка в применении текстуры
    [SerializeField] [Range(1, 10000)] int _delay = 50;
    // Точка в пространстве для плавного следования кисти
    Vector2 _point;
    // Проверка изменения позиции перемещения кисти по стене
    bool _checkPosition = false;
    // Начальные значения цветов пикселей на текстуре
    Color[] _defaultPixels;
    // Процент совпадения
    float _percent = 100f;
    // Процент который отнимается в случае если закрашен белый пиксель
    [SerializeField] [Range(0.01f, 1)] float _percentMinus = 0.1f;
    // Количество правильно закрашенных пикселей
    int _countRight = 0;
    // Количество неправильно закрашенных пикселей
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

        // Счётчик для массива начальных пикселей и конечных пикселей
        int k = 0;
        // Считываем все пиксели со сцены
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

    // Метод сравнения правильности рисования
    public void EndEquals()
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

    // Установить значение цвета стены по умолчанию
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
        // Для проверки работы сравнения
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

    // Метод рисования
    void OnPaint(int _rayX, int _rayZ)
    {
        if (_point != new Vector2())
        {
            // Следование за кистью для плавного рисования
            // Точка к которой будет закрашена стена
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
            // Если нет следующей точки к которой надо закрасить
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

    // Применение закрашенных пикселей на текстуре
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
                    // Изменить текстуру стены для рисования
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

    // Значение позиции рисования по умолчанию (для сброса пути рисования)
    public void SetDefaultPosition()
    {
        _point = new Vector2();
    }
    //////////////////////////////////////////////
}