// ---------------------------------------------------------------------------
// DungeonCreator.cs
// 
// Creates a random dungeon for gameplay
// 
// Original Author: Harley Laurie
// ---------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class DungeonCreator : MonoBehaviour 
{
    public GameObject m_floorPrefab;
    public GameObject m_startCubePrefab;
    public GameObject m_bossCubePrefab;
    public GameObject m_normalCubePrefab;

    private float m_floorSize;

    public int m_gridSize = 5; //Originally we want to create 5x5 grid

    private GameObject[,] m_levelGrid;

    void Start()
    {
        //Get the dimentions of the rooms
        m_floorSize = m_floorPrefab.GetComponent<MeshFilter>().mesh.bounds.size.x;
        //Generate the base grid
        Generate();
        //Create the starting room
        PlaceStart();
    }

    private void Generate()
    {
        //Initialise our array
        m_levelGrid = new GameObject[m_gridSize, m_gridSize];

        //Loop through the entire grid
        for ( int x = 0; x < m_gridSize; x++ )
        {
            for ( int y = 0; y < m_gridSize; y++ )
            {
                //Figure out the position to place this grid piece
                Vector3 l_roomPos = new Vector3(x * m_floorSize, 0, y * m_floorSize);
                //Spawn and add it
                m_levelGrid[x, y] = GameObject.Instantiate(m_floorPrefab, l_roomPos, Quaternion.identity) as GameObject;
            }
        }
    }

    private void PlaceStart()
    {
        //Select a random position for the start room
        int l_xPos = Random.Range(0, m_gridSize);
        int l_yPos = Random.Range(0, m_gridSize);
        //Get the room we have randomly selected
        GameObject l_selectedRoom = m_levelGrid[l_xPos, l_yPos];
        //Set its room type
        l_selectedRoom.GetComponent<DngnRoomInfo>().m_roomType = DngnRoomInfo.RoomType.Start;
        //We are going to place a cube on the start room to visually mark where it is
        GameObject l_startMarker = GameObject.Instantiate(m_startCubePrefab, l_selectedRoom.transform.position, Quaternion.identity) as GameObject;
    }
}