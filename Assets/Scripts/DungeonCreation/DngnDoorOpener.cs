// ---------------------------------------------------------------------------
// DngnDoorOpener.cs
// 
// Used to open doors in a dungeon room when its cleared
// 
// Original Author: Harley Laurie
// ---------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class DngnDoorOpener : MonoBehaviour
{
    public GameObject m_roomDoor;

    public void OpenDoor()
    {
        m_roomDoor.active = false;
    }
}