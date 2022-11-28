using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorControl : MonoBehaviour
{
    public LopataColor lc;
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
        UseScript.ter.terrainData.SetHeights(0, 0, UseScript.Heights);
        UseScript.DepthGround -= 0.0001f;
    }

    public void ChangeColorOnT()
    {
        lc.ChangeT();
    }

    public void ChangeColorOnN()
    {
        lc.ChangeN();
    }

    public void DefaultDepth()
    {
        UseScript.DepthGround = UseScript.DefaultDepthGround;
    }
}
