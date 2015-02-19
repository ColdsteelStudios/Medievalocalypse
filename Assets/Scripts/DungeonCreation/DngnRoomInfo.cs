// ---------------------------------------------------------------------------
// DngnRoomInfo.cs
// 
// Class description goes here
// 
// Original Author: Harley Laurie
// ---------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class DngnRoomInfo : MonoBehaviour
{
    public enum RoomType
    {
        Void = 0,
        Start = 1,
        Boss = 2,
        Normal = 3
    }

    public RoomType m_roomType = RoomType.Void;
    public Vector2 m_roomPosition;

    public bool m_hasNorthWall = false;
    public bool m_hasSouthWall = false;
    public bool m_hasEastWall = false;
    public bool m_hasWestWall = false;
}