using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
    * File : GimmickPush.cs
    * Desc : 플레이어 외 움직이는 객체의 Undo 기능을 위한 인터페이스
    *
*/
interface GimmickPush
{
    public void PushState();
    public void PopState();
}
