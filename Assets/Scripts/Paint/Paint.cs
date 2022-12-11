using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading;
using System.Collections;

public class Paint : MonoBehaviour
{
    // Проверка на то можно ли рисовать
    public static bool AllowPaint = false;
    // Проверка на то нужно ли перекрасить кисточку
    public static bool StartAllowPaint = false;
    // Текстура на кончике кисточки
    [SerializeField] Texture2D _pointPaint;
    // Интерфейс управления кликами мышки
    IClick _click;
    // Скрипт на игроке
    [SerializeField] MovePlayer _player;
    // Стена для рисования
    [SerializeField] GameObject _wall;
    // Кончик кисточки
    [SerializeField] GameObject _pointPencyl;
    // Размер текстуры стены
    private int _textureSize = 1024;
    // Текстура стены
    [SerializeField] private Texture2D _texture;
    // Размер кисти
    [SerializeField] private int _brushSize = 8;
    // Коррекция размера кисти на растянутой текстуре
    float _correctionX, _correctionZ;
    int _correctionXInt, _correctionZInt;
    // Счётчик для срабатывания применения текстуры
    int _counter = 0;
    // Задержка в применении текстуры
    [SerializeField] [Range(1, 10000)] int _delay = 50;
    // Точка в пространстве для плавного следования кисти
    Vector2 _point;
    // Проверка изменения позиции перемещения кисти по стене
    bool _checkPosition = false;
    // Начальные значения цветов пикселей на текстуре
    Color[] _defaultPixels = new Color[1048577];
    // Пиксели не равные белому цвету
    Color[] _greenPixels = new Color[100000];
    // Пиксели которые нужно закрасить
    Color[] _greenPixelsNeed = new Color[50000];
    // Пиксели которые не нужно закрашивать
    Color[] _whitePixels = new Color[1048577];
    // Все пиксели на стене для просчёта изменений от начальных значений
    Color[] _tempPixels = new Color[1048577];
    // Координаты зелёных пикселей
    int[] _greenPixelsWidth = new int[50000];
    int[] _greenPixelsHeight = new int[50000];
    // Процент совпадения
    float _percent = 100f;
    // Процент который отнимается в случае если закрашен белый пиксель
    [SerializeField] [Range(0.01f, 1)] float _percentMinus = 0.1f;
    // Проверка закрашены ли все зелёные пиксели
    bool _checkEndStart = false;
    // Коллайдер стены
    MeshCollider _wallCollider;


    private void OnApplicationQuit()
    {
        SetDefaultColor();
    }

    private void Awake()
    {
        _wallCollider = _wall.GetComponent<MeshCollider>();

        // Счётчик для массива начальных пикселей и конечных пикселей
        int k = 0;
        // Счётчик для массива зелёных пикселей
        int kk = 0;
        // Счётчик для массива белых пикселей
        int kkk = 0;
        // Считываем все пиксели со сцены
        for (int i = 0; i < _texture.width; i++)
        {
            for (int j = 0; j < _texture.height; j++)
            {
                _defaultPixels[k] = _texture.GetPixel(i, j);
                _tempPixels[k] = _texture.GetPixel(i, j);
                if (_texture.GetPixel(i, j) != Color.white)
                {
                    _greenPixels[kk] = _texture.GetPixel(i, j);
                    kk++;
                }
                else
                {
                    _whitePixels[kkk] = _texture.GetPixel(i, j);
                    kkk++;
                }
                k++;
            }
        }
        ///////////////////////////////////

        // Считываем для массива пикселей, которые нужно закрасить
        int kkkk = 0;
        // Считываем пиксели, которые нужно закрасить
        for (int i = 0; i < _texture.width; i++)
        {
            for (int j = 0; j < _texture.height; j++)
            {
                if (_texture.GetPixel(i, j) == _greenPixels[5])
                {
                    _greenPixelsNeed[kkkk] = _texture.GetPixel(i, j);
                    _greenPixelsWidth[kkkk] = i;
                    _greenPixelsHeight[kkkk] = j;
                    kkkk++;
                }
            }
        }
        ////////////////////////////////////


        SetDefaultColorPoint();

        StartCoroutine(GetArea());

        // Для проверки работы процента совпадения
        /*for (int i = 25; i < _texture.width - 25; i++)
        {
            for (int j = 10; j < _texture.height - 830; j++)
            {
                _texture.SetPixel(i, j, Color.black);
            }
        }
        _texture.Apply();*/
        ////////////////////////////////////

        _click = GetComponent<IClick>();
        _correctionX = 1;
        _correctionZ = 1;
        if (_wall.transform.localScale.x > _wall.transform.localScale.z)
            _correctionX = _wall.transform.localScale.x / _wall.transform.localScale.z;
        if (_wall.transform.localScale.z > _wall.transform.localScale.x)
            _correctionZ = _wall.transform.localScale.z / _wall.transform.localScale.x;
        _correctionXInt = (int)(_brushSize * _correctionX);
        _correctionZInt = (int)(_brushSize * _correctionZ);
    }

    // Установить значение цвета кисти по умолчанию
    void SetDefaultColorPoint()
    {
        for (int i = 0; i < _pointPaint.width; i++)
        {
            for (int j = 0; j < _pointPaint.height; j++)
            {
                _pointPaint.SetPixel(i, j, Color.white);
            }
        }
        _pointPaint.Apply();
    }
    //////////////////////////////////////////////

    // Установить значение цвета стены по умолчанию
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

    // Корутина для проверки завершения закрашивания
    IEnumerator GetArea()
    {
        while (!_checkEndStart)
        {
            bool _checkEnd = true;

            for (int i = 0; i < _greenPixelsNeed.Length; i++)
            {
                if (_texture.GetPixel(_greenPixelsWidth[i], _greenPixelsHeight[i]) == _greenPixels[5])
                    _checkEnd = false;
            }
            if (_checkEnd)
            {
                EndMenuStart();
                _checkEndStart = true;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
    //////////////////////////////////////////////

    private void Update()
    {
        CheckAllowPaint();

        if (AllowPaint)
        {
            _click.ClickLeftButton(_pointPencyl);
        }
    }

    // Проверка разрешено ли перекрасить кисть
    void CheckAllowPaint()
    {
        if (AllowPaint && StartAllowPaint)
        {
            for (int i = 0; i < _pointPaint.width; i++)
            {
                for (int j = 0; j < _pointPaint.height; j++)
                {
                    _pointPaint.SetPixel(i, j, Color.black);
                }
            }
            StartAllowPaint = false;
            _pointPaint.Apply();
        }
    }
    //////////////////////////////////////////////

    // Метод окончания уровня
    void EndMenuStart()
    {
        int k = 0;
        for (int i = 0; i < _texture.width; i++)
        {
            for (int j = 0; j < _texture.height; j++)
            {
                _tempPixels[k] = _texture.GetPixel(i, j);
                k++;
            }
        }
        for (int i = 0; i < _defaultPixels.Length; i++)
        {
            if (_defaultPixels[i] == Color.white && _tempPixels[i] == Color.black)
            {
                _percent -= _percentMinus;
            }
        }
        if (_percent < 0)
            _percent = 0;
        _player.CheckMove = false;
    }
    //////////////////////////////////////////////


    // Метод рисования
    void OnPaint(int _rayX, int _rayZ)
    {
        if (_texture.GetPixel(_rayX, _rayZ) != Color.black)
        {
            if (_point != new Vector2(0, 0))
            {
                // Следование за кистью для плавного рисования
                // Точка к которой будет закрашена стена
                Vector2 _tempPoint = new Vector2(_rayX, _rayZ);
                for (int k = 0; k < 10; k++)
                {
                    _point = Vector2.Lerp(_point, _tempPoint, 0.03f);
                    int _tempX = (int)_point.x;
                    int _tempZ = (int)_point.y;
                    for (int i = _tempX - _correctionZInt; i < _tempX + _correctionZInt; i++)
                    {
                        for (int j = _tempZ - _correctionXInt; j < _tempZ + _correctionXInt; j++)
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
                // Если нет следующей точки к которой надо закрасить
                _point = new Vector2(_rayX, _rayZ);
                for (int i = _rayX - _correctionZInt; i < _rayX + _correctionZInt; i++)
                {
                    for (int j = _rayZ - _correctionXInt; j < _rayZ + _correctionXInt; j++)
                    {
                        _texture.SetPixel(i, j, Color.black);
                        _counter++;
                    }
                }
                //////////////////////////////////////////////
            }
        }
    }
    //////////////////////////////////////////////

    // Красный, зелёный и синий канал соответственно
    float x = 0, y = 0, z = 0;

    // Применение закрашенных пикселей на текстуре
    public void SetPixelColor()
    {
        Ray ray = new Ray(_pointPencyl.transform.position, _pointPencyl.transform.forward);
        RaycastHit hit;

        if (_wallCollider.Raycast(ray, out hit, 1f))
        {
            OnPaint(Convert.ToInt32(hit.textureCoord.x * _textureSize), Convert.ToInt32(hit.textureCoord.y * _textureSize));

            if (_counter >= _delay)
            {
                // Плавное изменение цвета текстуры кончика кисти
                x += 0.001f;
                y += 0.001f;
                z += 0.001f;
                Color _newColor = new Color(x, y, z, 0);
                if (x >= 1)
                {
                    AllowPaint = false;
                    x = 0;
                    y = 0;
                    z = 0;
                }
                for (int i = 0; i < _pointPaint.width; i++)
                {
                    for (int j = 0; j < _pointPaint.height; j++)
                    {
                        _pointPaint.SetPixel(i, j, _newColor);
                    }
                }
                //////////////////////////////////////

                // Изменить текстуру кончика кисти
                _pointPaint.Apply();
                // Изменить текстуру стены для рисования
                _texture.Apply();
                _counter = 0;
            }
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
        _point = new Vector2(0, 0);
    }
    //////////////////////////////////////////////
}
