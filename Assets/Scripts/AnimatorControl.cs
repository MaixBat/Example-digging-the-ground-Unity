using UnityEngine;

public class AnimatorControl : MonoBehaviour
{
    public LopataColor _colorChangeLopata;
    public void CheckMoveTrue()
    {
        MovePlayer.Player.CheckMove = true;
    }

    public void CheckMoveFalse()
    {
        MovePlayer.Player.CheckMove = false;
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
