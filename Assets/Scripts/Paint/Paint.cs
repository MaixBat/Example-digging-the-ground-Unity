using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading;
using System.Collections;

public class Paint : MonoBehaviour
{
    public static bool AllowPaint = false;
    public static bool StartAllowPaint = false;

    [SerializeField] Texture2D _pointPaint;

    IClick _click;

    [SerializeField] MovePlayer _player;

    [SerializeField] GameObject _endMenu;
    [SerializeField] GameObject _wall;
    [SerializeField] GameObject _pointPencyl;

    private int _textureSize = 1024;
    [SerializeField] private Texture2D _texture;
    [SerializeField] private int _brushSize = 8;
    float _correctionX, _correctionZ;
    int _correctionXInt, _correctionZInt;
    int _counter = 0;
    [SerializeField] [Range(1, 10000)] int _delay = 50;
    Vector2 _point;
    bool _checkPosition = false;

    [SerializeField] Text _countPercentEndMenu;

    Color[] _defaultPixels = new Color[1048577];
    Color[] _greenPixels = new Color[100000];
    Color[] _greenPixelsNeed = new Color[50000];
    Color[] _whitePixels = new Color[1048577];
    Color[] _tempPixels = new Color[1048577];

    int[] _greenPixelsWidth = new int[50000];
    int[] _greenPixelsHeight = new int[50000];

    float _percent = 100f;
    [SerializeField]
    [Range(0.01f, 1)] float _percentMinus = 0.1f;

    bool _checkEndStart = false;

    MeshCollider _wallCollider;


    private void OnApplicationQuit()
    {
        SetDefaultColor();
    }

    private void Awake()
    {
        _wallCollider = _wall.GetComponent<MeshCollider>();

        int k = 0;
        int kk = 0;
        int kkk = 0;
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
        //Debug.Log(k);
        //Debug.Log(kk);
        //Debug.Log(kkk);

        int kkkk = 0;
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
        //Debug.Log(kkkk);


        SetDefaultColorPoint();

        StartCoroutine(GetArea());

        /*for (int i = 25; i < _texture.width - 25; i++)
        {
            for (int j = 10; j < _texture.height - 830; j++)
            {
                _texture.SetPixel(i, j, Color.black);
            }
        }
        _texture.Apply();*/

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

    private void Update()
    {
        CheckAllowPaint();

        if (AllowPaint)
        {
            _click.ClickLeftButton(_pointPencyl);
        }
    }

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

    public void RestartScene()
    {
        StopCoroutine(GetArea());
        SetDefaultColor();
        SetDefaultColorPoint();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

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
        _countPercentEndMenu.text = "Процент совпадения " + _percent.ToString() + "%";
        _player.CheckMove = false;
        _endMenu.SetActive(true);
    }

    float x = 0, y = 0, z = 0;

    public void SetPixelColor()
    {
        Ray ray = new Ray(_pointPencyl.transform.position, _pointPencyl.transform.forward);
        RaycastHit hit;

        if (_wallCollider.Raycast(ray, out hit, 1f))
        {
            int _rayX = Convert.ToInt32(hit.textureCoord.x * _textureSize);
            int _rayZ = Convert.ToInt32(hit.textureCoord.y * _textureSize);
            if (_texture.GetPixel(_rayX, _rayZ) != Color.black)
            {
                if (_point != new Vector2(0, 0))
                {
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
                }
                else
                {
                    _point = new Vector2(_rayX, _rayZ);
                    for (int i = _rayX - _correctionZInt; i < _rayX + _correctionZInt; i++)
                    {
                        for (int j = _rayZ - _correctionXInt; j < _rayZ + _correctionXInt; j++)
                        {
                            _texture.SetPixel(i, j, Color.black);
                            _counter++;
                        }
                    }
                }
            }
            if (_counter >= _delay)
            {
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
                _pointPaint.Apply();
                _texture.Apply();
                _counter = 0;
            }
        }
        else
        {
            SetDefaultPosition();
        }
    }

    public void SetDefaultPosition()
    {
        _point = new Vector2(0, 0);
    }
}
