using UnityEngine;

public class AnimatorControl : MonoBehaviour
{
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
        UseScript.Heights[UseScript.PointZStatic, UseScript.PointXStatic] = UseScript.DepthGround;
        UseScript.Ter.terrainData.SetHeights(0, 0, UseScript.Heights);
        UseScript.DepthGround -= 0.0001f;
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
        UseScript.DepthGround = UseScript.DefaultDepthGround;
    }
}
