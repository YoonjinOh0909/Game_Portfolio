using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
    * File : MoveObstacleInfo.cs
    * Desc : MovingPlatform에 필요한 정보를 담은 구조체.
    *
*/

public struct MoveObstacleInfo
{
    public Vector3 posNext;
    public Vector3 nowPos;
    public bool isDelay;
    public float stackTime;
    
}
