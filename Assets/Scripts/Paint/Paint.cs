using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Paint : MonoBehaviour
{
    IClick _click;

    [SerializeField] MovePlayer _player;

    [SerializeField] GameObject _endMenu;

    [SerializeField] GameObject _wall;
    [SerializeField] GameObject _pointPencyl;

    private int _textureSize = 1024;
    [SerializeField] private Texture2D _texture;
    [SerializeField] private Color _color;
    [SerializeField] private int _brushSize = 8;
    float _correctionX, _correctionZ;
    int _correctionXInt, _correctionZInt;
    int _counter = 0;
    [SerializeField][Range(1,1000)] int _delay = 50;
    Vector2 _point;
    bool _checkPosition = false;

    [SerializeField] Text _countPercent;
    [SerializeField] Text _countPercentEndMenu;

    [SerializeField] GameObject[] _letters;
    int _countLetters;

    float _percent = 100f;
    int _delayError = 0;

    private void OnApplicationQuit()
    {
        SetDefaultColor();
    }

    private void Awake()
    {
        _countLetters = _letters.Length;
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

    void SetDefaultColor()
    {
        for (int i = 0; i < _texture.width; i++)
        {
            for (int j = 0; j < _texture.height; j++)
            {
                _texture.SetPixel(i, j, Color.white);
            }
        }
        _texture.Apply();
    }

    private void Update()
    {
        _click.ClickLeftButton();
    }

    public void RestartScene()
    {
        SetDefaultColor();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SetPixelColor()
    {
        Ray ray = new Ray(_pointPencyl.transform.position, _pointPencyl.transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1f))
        { 
            if (hit.collider.CompareTag("NeedDestroy"))
            {
                Destroy(hit.collider);
                if (hit.collider.gameObject.GetComponents<BoxCollider>().Length <= 1)
                {
                    _player.CheckMove = false;
                    _endMenu.SetActive(true);
                }
            }
            if (hit.collider.CompareTag("Letter"))
            {
                _delayError++;
                if (_delayError >= 10)
                {
                    _percent = _percent - (3 / (float)_countLetters);
                    if (_percent <= 0)
                    {
                        _percent = 0;
                        _player.CheckMove = false;
                        _endMenu.SetActive(true);
                    }
                    _countPercent.text = "Процент совпадения " + _percent.ToString() + "%";
                    _countPercentEndMenu.text = "Процент совпадения " + _percent.ToString() + "%";
                    _delayError = 0;
                }
            }
            if (hit.collider.CompareTag("Error"))
            {
                _percent = 0;
                _player.CheckMove = false;
                _endMenu.SetActive(true);
                _countPercent.text = "Процент совпадения " + _percent.ToString() + "%";
                _countPercentEndMenu.text = "Процент совпадения " + _percent.ToString() + "%";
            }
        }

        if (_wall.GetComponent<MeshCollider>().Raycast(ray, out hit, 1f))
        {
            int _rayX = Convert.ToInt32(hit.textureCoord.x * _textureSize);
            int _rayZ = Convert.ToInt32(hit.textureCoord.y * _textureSize);
            if (_texture.GetPixel(_rayX, _rayZ) != _color)
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
                                _texture.SetPixel(i, j, _color);
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
                            _texture.SetPixel(i, j, _color);
                            _counter++;
                        }
                    }
                }
            }
            if (_counter >= _delay)
            {
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
