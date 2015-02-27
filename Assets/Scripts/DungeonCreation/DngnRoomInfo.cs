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

    private GameObject m_northDoor;
    private GameObject m_eastDoor;
    private GameObject m_southDoor;
    private GameObject m_westDoor;

    public GameObject m_zombiePrefab;

    //If this is a normal room, we need to keep track of how many enemies
    //the player needs to kill before they can exit this room
    public int m_targetKillCount;

    //Sound clip to play when the doors in this room are being opened
    public AudioClip m_roomCompleteAudioClip;

    public void DecrementSpawnCountRemainder()
    {
        //Open all doors when killcount has been reached
        m_targetKillCount--;
        if (m_targetKillCount <= 0)
            OpenAllDoors(false);
        else
            print("not enough kills");
    }

    public void HasDoor(GameObject door, DungeonCreator.WallSide doorSide)
    {
        switch(doorSide)
        {
            case(DungeonCreator.WallSide.North):
                m_northDoor = door;
                return;
            case(DungeonCreator.WallSide.East):
                m_eastDoor = door;
                return;
            case(DungeonCreator.WallSide.South):
                m_southDoor = door;
                return;
            case(DungeonCreator.WallSide.West):
                m_westDoor = door;
                return;
        }
    }

    public void OpenAllDoors(bool a_isStartRoom)
    {
        //Open all the doors in this room
        if (m_northDoor != null)
            m_northDoor.SendMessage("OpenDoor");
        if (m_eastDoor != null)
            m_eastDoor.SendMessage("OpenDoor");
        if (m_southDoor != null)
            m_southDoor.SendMessage("OpenDoor");
        if (m_westDoor != null)
            m_westDoor.SendMessage("OpenDoor");

        //If this isnt the starting room, play the doors open sound effect
        if (!a_isStartRoom)
            GetComponent<AudioSource>().PlayOneShot(m_roomCompleteAudioClip);
    }

    //Fills the room with enemies
    public void FillRoom()
    {
        m_targetKillCount = Random.Range(1, 10);
        for (int i = 0; i < m_targetKillCount; i++)
        {
            float l_xSpawnPos = Random.Range(-12.0f, 12.0f);
            float l_zSpawnPos = Random.Range(-12.0f, 12.0f);
            Vector3 l_spawnPos = transform.position;
            l_spawnPos.x += l_xSpawnPos;
            l_spawnPos.z += l_zSpawnPos;
            GameObject l_zombieSpawn = GameObject.Instantiate(m_zombiePrefab, l_spawnPos, Quaternion.identity) as GameObject;
            l_zombieSpawn.SendMessage("SetParentRoom", this.gameObject);
        }
    }
}