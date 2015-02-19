// ---------------------------------------------------------------------------
// DungeonCreator.cs
// 
// Creates a random dungeon for gameplay
// 
// Original Author: Harley Laurie
// ---------------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DungeonCreator : MonoBehaviour 
{
    //Prefabs
    public GameObject m_dungeonRootPrefab;
    public GameObject m_floorPrefab;
    public GameObject m_doorPrefab;
    public GameObject m_wallPrefab;

    public GameObject m_startCubePrefab;
    public GameObject m_bossCubePrefab;
    public GameObject m_normalCubePrefab;

    private float m_floorSize;

    //Amount of rooms in the level
    public int m_gridSize = 5; //Originally we want to create 5x5 grid
    private int m_gridSizeIter; //Used for accessing grids through array

    //2D array which contains all the rooms
    private GameObject[,] m_levelGrid;

    private GameObject m_dungeonRoot;
    private GameObject m_startRoomMarker;
    private GameObject m_startRoom;
    private GameObject m_endRoomMarker;
    private GameObject m_endRoom;
    private GameObject m_closestRoom; //Used for building path from start to end

    //Player stuff
    public GameObject m_playerPrefab;
    public GameObject m_cameraPrefab;

    void Start()
    {
        //Create dungeon root
        m_dungeonRoot = GameObject.Instantiate(m_dungeonRootPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        m_gridSizeIter = m_gridSize - 1;
        //Get the dimentions of the rooms
        m_floorSize = m_floorPrefab.GetComponent<MeshFilter>().sharedMesh.bounds.size.x;
        //Generate the base grid
        Generate();
        //Create the starting room
        PlaceStart();
        //Boss room
        PlaceBoss();
        //Make sure end and start arent next to each other
        SpaceRooms();
        //Add doors to connect adjacent rooms
        AddWalls();
        //Spawn player in the starter room
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        Vector3 SpawnPos = m_startRoomMarker.transform.position;
        GameObject Player = GameObject.Instantiate(m_playerPrefab, SpawnPos, Quaternion.identity) as GameObject;
        GameObject Cam = GameObject.Instantiate(m_cameraPrefab, SpawnPos, Quaternion.identity) as GameObject;
        Cam.GetComponent<HarleyMouseOrbit>().Target = Player.transform.FindChild("CamTarget").transform;
    }

    private void AddWalls()
    {
        for ( int x = 0; x < m_gridSize; x++ )
        {
            for ( int y = 0; y < m_gridSize; y++ )
            {
                PlaceNorthWall(m_levelGrid[x, y]);
                PlaceEastWall(m_levelGrid[x, y]);
                PlaceSouthWall(m_levelGrid[x, y]);
                PlaceWestWall(m_levelGrid[x, y]);
            }
        }
    }

    private void PlaceNorthWall(GameObject target)
    {
        DngnRoomInfo RoomInfo = target.GetComponent<DngnRoomInfo>();

        //Dont bother placing walls around empty rooms
        if (RoomInfo.m_roomType == DngnRoomInfo.RoomType.Void)
            return;

        if (!RoomInfo.m_hasNorthWall)
        {
            //Get the room to the north
            GameObject NorthRoom = GetNorth(target);

            //If there is void to the north, place a wall
            if (NorthRoom == null)
            {
                //Find the position for it
                Vector3 RoomPos = target.transform.position;
                RoomPos += Vector3.forward * (m_floorSize * 0.5f);
                //Spawn it
                GameObject Wall = GameObject.Instantiate(m_wallPrefab, RoomPos, m_wallPrefab.transform.rotation) as GameObject;
                Wall.transform.parent = m_dungeonRoot.transform;
                //Rotate it
                Wall.transform.LookAt(target.transform.position);
                RoomInfo.m_hasNorthWall = true;
                return;
            }

            //If there is a room to the north, only place a wall if the north room is empty
            DngnRoomInfo NorthInfo = NorthRoom.GetComponent<DngnRoomInfo>();
            if (NorthInfo.m_roomType == DngnRoomInfo.RoomType.Void)
            {
                //Find the position for it
                Vector3 RoomPos = target.transform.position;
                RoomPos += Vector3.forward * (m_floorSize * 0.5f);
                //Spawn it
                GameObject Wall = GameObject.Instantiate(m_wallPrefab, RoomPos, m_wallPrefab.transform.rotation) as GameObject;
                Wall.transform.parent = m_dungeonRoot.transform;
                //Rotate it
                Wall.transform.LookAt(target.transform.position);
                RoomInfo.m_hasNorthWall = true;
            }
        }
    }

    private void PlaceEastWall(GameObject target)
    {
        DngnRoomInfo RoomInfo = target.GetComponent<DngnRoomInfo>();
        if (RoomInfo.m_roomType == DngnRoomInfo.RoomType.Void)
            return;
        if (!RoomInfo.m_hasEastWall)
        {
            GameObject EastRoom = GetEast(target);
            if (EastRoom == null)
            {
                Vector3 RoomPos = target.transform.position;
                RoomPos += Vector3.right * (m_floorSize * 0.5f);
                GameObject Wall = GameObject.Instantiate(m_wallPrefab, RoomPos, m_wallPrefab.transform.rotation) as GameObject;
                Wall.transform.parent = m_dungeonRoot.transform;
                Wall.transform.LookAt(target.transform.position);
                RoomInfo.m_hasEastWall = true;
                return;
            }
            DngnRoomInfo EastInfo = EastRoom.GetComponent<DngnRoomInfo>();
            if (EastInfo.m_roomType == DngnRoomInfo.RoomType.Void)
            {
                Vector3 RoomPos = target.transform.position;
                RoomPos += Vector3.right * (m_floorSize * 0.5f);
                GameObject Wall = GameObject.Instantiate(m_wallPrefab, RoomPos, m_wallPrefab.transform.rotation) as GameObject;
                Wall.transform.parent = m_dungeonRoot.transform;
                Wall.transform.LookAt(target.transform.position);
                RoomInfo.m_hasEastWall = true;
            }
        }
    }

    private void PlaceSouthWall(GameObject target)
    {
        DngnRoomInfo RoomInfo = target.GetComponent<DngnRoomInfo>();
        if (RoomInfo.m_roomType == DngnRoomInfo.RoomType.Void)
            return;
        if(!RoomInfo.m_hasSouthWall)
        {
            GameObject SouthRoom = GetSouth(target);
            if(SouthRoom == null)
            {
                Vector3 RoomPos = target.transform.position;
                RoomPos += (-Vector3.forward) * (m_floorSize * 0.5f);
                GameObject Wall = GameObject.Instantiate(m_wallPrefab, RoomPos, m_wallPrefab.transform.rotation) as GameObject;
                Wall.transform.parent = m_dungeonRoot.transform;
                Wall.transform.LookAt(target.transform.position);
                RoomInfo.m_hasSouthWall = true;
                return;
            }
            DngnRoomInfo SouthInfo = SouthRoom.GetComponent<DngnRoomInfo>();
            if (SouthInfo.m_roomType == DngnRoomInfo.RoomType.Void)
            {
                Vector3 RoomPos = target.transform.position;
                RoomPos += (-Vector3.forward) * (m_floorSize * 0.5f);
                GameObject Wall = GameObject.Instantiate(m_wallPrefab, RoomPos, m_wallPrefab.transform.rotation) as GameObject;
                Wall.transform.parent = m_dungeonRoot.transform;
                Wall.transform.LookAt(target.transform.position);
                RoomInfo.m_hasSouthWall = true;
            }
        }
    }

    private void PlaceWestWall(GameObject target)
    {
        DngnRoomInfo RoomInfo = target.GetComponent<DngnRoomInfo>();
        if (RoomInfo.m_roomType == DngnRoomInfo.RoomType.Void)
            return;
        if(!RoomInfo.m_hasWestWall)
        {
            GameObject WestRoom = GetWest(target);
            if(WestRoom == null)
            {
                Vector3 RoomPos = target.transform.position;
                RoomPos += (-Vector3.right) * (m_floorSize * 0.5f);
                GameObject Wall = GameObject.Instantiate(m_wallPrefab, RoomPos, m_wallPrefab.transform.rotation) as GameObject;
                Wall.transform.parent = m_dungeonRoot.transform;
                Wall.transform.LookAt(target.transform.position);
                RoomInfo.m_hasWestWall = true;
                return;
            }
            DngnRoomInfo WestInfo = WestRoom.GetComponent<DngnRoomInfo>();
            if (WestInfo.m_roomType == DngnRoomInfo.RoomType.Void)
            {
                Vector3 RoomPos = target.transform.position;
                RoomPos += (-Vector3.right) * (m_floorSize * 0.5f);
                GameObject Wall = GameObject.Instantiate(m_wallPrefab, RoomPos, m_wallPrefab.transform.rotation) as GameObject;
                Wall.transform.parent = m_dungeonRoot.transform;
                Wall.transform.LookAt(target.transform.position);
                RoomInfo.m_hasWestWall = true;
            }
        }
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
                m_levelGrid[x, y].transform.parent = m_dungeonRoot.transform;
                //Tell it its position in the grid
                m_levelGrid[x, y].GetComponent<DngnRoomInfo>().m_roomPosition = new Vector2(x, y);
            }
        }
    }

    private void PlaceStart()
    {
        //Select a random position for the start room
        int l_xPos = Random.Range(0, m_gridSize);
        int l_yPos = Random.Range(0, m_gridSize);
        //Get the room we have randomly selected
        m_startRoom = m_levelGrid[l_xPos, l_yPos];
        //Set its room type
        m_startRoom.GetComponent<DngnRoomInfo>().m_roomType = DngnRoomInfo.RoomType.Start;
        //We are going to place a cube on the start room to visually mark where it is
        m_startRoomMarker = GameObject.Instantiate(m_startCubePrefab, m_startRoom.transform.position, Quaternion.identity) as GameObject;
        m_startRoomMarker.transform.parent = m_dungeonRoot.transform;
    }

    private void PlaceBoss()
    {
        //Select random room
        int l_xPos = Random.Range(0, m_gridSize);
        int l_yPos = Random.Range(0, m_gridSize);
        m_endRoom = m_levelGrid[l_xPos, l_yPos];
        //Choose a new one if this room is already taken
        if (m_levelGrid[l_xPos, l_yPos].GetComponent<DngnRoomInfo>().m_roomType != DngnRoomInfo.RoomType.Void)
            PlaceBoss();
        //Set its type
        m_levelGrid[l_xPos, l_yPos].GetComponent<DngnRoomInfo>().m_roomType = DngnRoomInfo.RoomType.Boss;
        //Place marker
        m_endRoomMarker = GameObject.Instantiate(m_bossCubePrefab, m_levelGrid[l_xPos, l_yPos].transform.position, Quaternion.identity) as GameObject;
        m_endRoomMarker.transform.parent = m_dungeonRoot.transform;
    }

    private void SpaceRooms()
    {
        //If the start and end rooms are too close destroy them and replace them
        float startEndDistance = Vector3.Distance(m_startRoom.transform.position, m_endRoom.transform.position);
        if(startEndDistance<= 14.14214f)
        {
            GameObject.Destroy(m_startRoomMarker);
            GameObject.Destroy(m_endRoomMarker);
            PlaceStart();
            PlaceBoss();
            SpaceRooms();
        }
        else
        {
            //Create a path from start to end
            if (StartPath())
                //If the starting rooms connected to the boss room, then were done
                return;
            else
                //otherwise we continue making the pathway
                ContinuePath();
        }
    }

    //If we happen to connect to the boss room by placing rooms around the start
    //we return true, otherwise return false
    private bool StartPath()
    {
        //Place 1-4 rooms that branch off from the starting room
        int rand = Random.Range(1, 4);
        for (int i = 0; i < rand; i++)
        {
            GameObject SelectedRoom = GetRandomAdjacentVoid(m_startRoom);
            //Selected Room may come back as void if there were none to be found
            if (SelectedRoom == null)
                continue;
            SelectedRoom.GetComponent<DngnRoomInfo>().m_roomType = DngnRoomInfo.RoomType.Normal;
            GameObject l_normalCube = GameObject.Instantiate(m_normalCubePrefab, SelectedRoom.transform.position, Quaternion.identity) as GameObject;
            l_normalCube.transform.parent = m_dungeonRoot.transform;
            //Check if the new room we placed connected to the boss room
            if (IsBossAdjacent(SelectedRoom))
                return true;
        }
        return false;
    }

    private void ContinuePath()
    {
        //Place 1-3 random rooms, connected to any normal rooms that already exist
        int rand = Random.Range(1, 3);
        for (int i = 0; i < rand; i++ )
        {
            //Grab random normal room
            GameObject SelectedRoom = GetRandomNormalRoom();
            //Get one of its adjacent void rooms
            SelectedRoom = GetRandomAdjacentVoid(SelectedRoom);
            if (SelectedRoom == null)
                continue;
            //Turn it into a normal room
            SelectedRoom.GetComponent<DngnRoomInfo>().m_roomType = DngnRoomInfo.RoomType.Normal;
            //Place an indicator here
            GameObject l_normalCubeB = GameObject.Instantiate(m_normalCubePrefab, SelectedRoom.transform.position, Quaternion.identity) as GameObject;
            l_normalCubeB.transform.parent = m_dungeonRoot.transform;
            //Check if it connects to the boss room
            if (IsBossAdjacent(SelectedRoom))
                return;
        }
        //Find the closest room to the boss room and add another that takes it one room closer
        GameObject ClosestNormal = GetClosestNormalRoom();
        ClosestNormal = GetClosestAdjacentVoid(ClosestNormal);
        //Make this into a new room
        ClosestNormal.GetComponent<DngnRoomInfo>().m_roomType = DngnRoomInfo.RoomType.Normal;
        GameObject l_normalCube = GameObject.Instantiate(m_normalCubePrefab, ClosestNormal.transform.position, Quaternion.identity) as GameObject;
        l_normalCube.transform.parent = m_dungeonRoot.transform;
        //Check if we are adjacent to the boss room
        if (IsBossAdjacent(ClosestNormal))
            return;
        else
            ContinuePath();
    }

    //Returns the room to the north of the room passed in
    //or void if there isnt one
    private GameObject GetNorth(GameObject target)
    {
        if (target == null)
            return null;
        //Get args room position
        Vector2 l_roomPos = target.GetComponent<DngnRoomInfo>().m_roomPosition;
        //If there is a room to the north return that
        if (l_roomPos.y + 1 <= m_gridSizeIter)
            return m_levelGrid[(int)l_roomPos.x, (int)l_roomPos.y + 1];
        //Otherwise just return null
        return null;
    }

    private GameObject GetEast(GameObject target)
    {
        if (target == null)
            return null;
        //Get args room position
        Vector2 l_roomPos = target.GetComponent<DngnRoomInfo>().m_roomPosition;
        //If there is a room to the north return that
        if (l_roomPos.x + 1 <= m_gridSizeIter)
            return m_levelGrid[(int)l_roomPos.x + 1, (int)l_roomPos.y];
        //Otherwise just return null
        return null;
    }

    private GameObject GetSouth(GameObject target)
    {
        if (target == null)
            return null;
        //Get args room position
        Vector2 l_roomPos = target.GetComponent<DngnRoomInfo>().m_roomPosition;
        //If there is a room to the north return that
        if ((l_roomPos.y - 1 <= m_gridSizeIter) && (l_roomPos.y - 1 >= 0))
            return m_levelGrid[(int)l_roomPos.x, (int)l_roomPos.y - 1];
        //Otherwise just return null
        return null;
    }

    private GameObject GetWest(GameObject target)
    {
        if (target == null)
            return null;
        //Get args room position
        Vector2 l_roomPos = target.GetComponent<DngnRoomInfo>().m_roomPosition;
        //If there is a room to the north return that
        if ((l_roomPos.x - 1 <= m_gridSizeIter) && (l_roomPos.x - 1 >= 0))
            return m_levelGrid[(int)l_roomPos.x - 1, (int)l_roomPos.y];
        //Otherwise just return null
        return null;
    }

    //Returns closest active room adjacent to the passed in room
    private GameObject GetClosestAdjacent(GameObject adjacentTarget)
    {
        GameObject l_closestRoom = adjacentTarget;
        float l_closestRoomDistance = Vector3.Distance(l_closestRoom.transform.position, m_endRoom.transform.position);

        GameObject northAdjacent = GetNorth(adjacentTarget);
        if(northAdjacent!=null)
        {
            float northDistance = Vector3.Distance(northAdjacent.transform.position, m_endRoom.transform.position);
            if(northDistance < l_closestRoomDistance)
            {
                l_closestRoomDistance = northDistance;
                l_closestRoom = northAdjacent;
            }
        }
        GameObject eastAdjacent = GetEast(adjacentTarget);
        if(eastAdjacent!=null)
        {
            float eastDistance = Vector3.Distance(eastAdjacent.transform.position, m_endRoom.transform.position);
            if(eastDistance < l_closestRoomDistance)
            {
                l_closestRoomDistance = eastDistance;
                l_closestRoom = eastAdjacent;
            }
        }
        GameObject southAdjacent = GetSouth(adjacentTarget);
        if (southAdjacent != null)
        {
            float southDistance = Vector3.Distance(southAdjacent.transform.position, m_endRoom.transform.position);
            if (southDistance < l_closestRoomDistance)
            {
                l_closestRoomDistance = southDistance;
                l_closestRoom = southAdjacent;
            }
        }
        GameObject westAdjacent = GetWest(adjacentTarget);
        if (westAdjacent != null)
        {
            float westDistance = Vector3.Distance(westAdjacent.transform.position, m_endRoom.transform.position);
            if (westDistance < l_closestRoomDistance)
            {
                l_closestRoomDistance = westDistance;
                l_closestRoom = westAdjacent;
            }
        }

        return l_closestRoom;
    }

    //Returns the adjacent void room which is closest to the boss room
    private GameObject GetClosestAdjacentVoid(GameObject target)
    {
        GameObject ClosestRoom = target;
        float ClosestRoomDistance = Vector3.Distance(ClosestRoom.transform.position, m_endRoom.transform.position);
        //Check each adjacent room and figure out which one is the closest
        GameObject North = GetNorth(target);
        //Make sure there is a room here, and its void
        if(North!=null && North.GetComponent<DngnRoomInfo>().m_roomType == DngnRoomInfo.RoomType.Void)
        {
            //Check how far away it is
            float D = Vector3.Distance(North.transform.position, m_endRoom.transform.position);
            //See if its the closest
            if(D<ClosestRoomDistance)
            {
                ClosestRoom = North;
                ClosestRoomDistance = D;
            }
        }
        GameObject East = GetEast(target);
        if (East != null && East.GetComponent<DngnRoomInfo>().m_roomType == DngnRoomInfo.RoomType.Void)
        {
            float D = Vector3.Distance(East.transform.position, m_endRoom.transform.position);
            if (D < ClosestRoomDistance)
            {
                ClosestRoom = East;
                ClosestRoomDistance = D;
            }
        }
        GameObject South = GetSouth(target);
        if (South != null && South.GetComponent<DngnRoomInfo>().m_roomType == DngnRoomInfo.RoomType.Void)
        {
            float D = Vector3.Distance(South.transform.position, m_endRoom.transform.position);
            if (D < ClosestRoomDistance)
            {
                ClosestRoom = South;
                ClosestRoomDistance = D;
            }
        }
        GameObject West = GetWest(target);
        if (West != null && West.GetComponent<DngnRoomInfo>().m_roomType == DngnRoomInfo.RoomType.Void)
        {
            float D = Vector3.Distance(West.transform.position, m_endRoom.transform.position);
            if (D < ClosestRoomDistance)
            {
                ClosestRoom = West;
                ClosestRoomDistance = D;
            }
        }

        return ClosestRoom;
    }

    //Returns a random adjacent, void room
    private GameObject GetRandomAdjacentVoid(GameObject original)
    {
        //Get any adjacent void rooms and add them to a list
        List<GameObject> VoidRooms = new List<GameObject>();
        if (GetNorth(original) != null)
        {
            if (GetNorth(original).GetComponent<DngnRoomInfo>().m_roomType == DngnRoomInfo.RoomType.Void)
                VoidRooms.Add(GetNorth(original));
        }
        if (GetEast(original) != null)
        {
            if (GetEast(original).GetComponent<DngnRoomInfo>().m_roomType == DngnRoomInfo.RoomType.Void)
                VoidRooms.Add(GetEast(original));
        }
        if (GetSouth(original) != null)
        {
            if (GetSouth(original).GetComponent<DngnRoomInfo>().m_roomType == DngnRoomInfo.RoomType.Void)
                VoidRooms.Add(GetSouth(original));
        }
        if (GetWest(original) != null)
        {
            if (GetWest(original).GetComponent<DngnRoomInfo>().m_roomType == DngnRoomInfo.RoomType.Void)
                VoidRooms.Add(GetWest(original));
        }
        //If there are no items in the list then there are no adjacent rooms
        if(VoidRooms.Count==0)
            return null;
        //Otherwise we select a random room from this list and return that
        int rand = Random.Range(0, VoidRooms.Count - 1);
        return VoidRooms[rand];
    }

    //Returns a random normal room on the map
    private GameObject GetRandomNormalRoom()
    {
        //Find all the normal rooms and put them into a list
        List<GameObject> NormalRooms = new List<GameObject>();

        for ( int x = 0; x < m_gridSize; x++ )
        {
            for ( int y = 0; y < m_gridSize; y++ )
            {
                if (m_levelGrid[x, y].GetComponent<DngnRoomInfo>().m_roomType == DngnRoomInfo.RoomType.Normal)
                    NormalRooms.Add(m_levelGrid[x, y]);
            }
        }

        //Grab a random room from the list of normals and return that
        int randIter = Random.Range(0, NormalRooms.Count);
        return NormalRooms[randIter];
    }

    //Checks if the passed room has any adjacent void rooms
    private bool HasAdjacentVoid(GameObject target)
    {
        if (GetNorth(target).GetComponent<DngnRoomInfo>().m_roomType == DngnRoomInfo.RoomType.Void)
            return true;
        if (GetEast(target).GetComponent<DngnRoomInfo>().m_roomType == DngnRoomInfo.RoomType.Void)
            return true;
        if (GetSouth(target).GetComponent<DngnRoomInfo>().m_roomType == DngnRoomInfo.RoomType.Void)
            return true;
        if (GetWest(target).GetComponent<DngnRoomInfo>().m_roomType == DngnRoomInfo.RoomType.Void)
            return true;
        return false;
    }

    //Checks if the boss room is adjacent to the room that was passed in
    private bool IsBossAdjacent(GameObject target)
    {
        if(GetNorth(target)!=null)
        {
            if (GetNorth(target).GetComponent<DngnRoomInfo>().m_roomType == DngnRoomInfo.RoomType.Boss)
                return true;
        }
        if(GetEast(target)!=null)
        {
            if (GetEast(target).GetComponent<DngnRoomInfo>().m_roomType == DngnRoomInfo.RoomType.Boss)
                return true;
        }
        if(GetSouth(target)!=null)
        {
            if (GetSouth(target).GetComponent<DngnRoomInfo>().m_roomType == DngnRoomInfo.RoomType.Boss)
                return true;
        }
        if(GetWest(target)!=null)
        {
            if (GetWest(target).GetComponent<DngnRoomInfo>().m_roomType == DngnRoomInfo.RoomType.Boss)
                return true;
        }
        return false;
    }

    //Returns to closest Normal room to the boss room
    private GameObject GetClosestNormalRoom()
    {
        GameObject ClosestRoom = m_startRoom;
        float ClosestRoomDistance = Vector3.Distance(m_startRoom.transform.position, m_endRoom.transform.position);

        //Find all the normal rooms and put them into a list
        List<GameObject> NormalRooms = new List<GameObject>();

        for (int x = 0; x < m_gridSize; x++)
        {
            for (int y = 0; y < m_gridSize; y++)
            {
                if (m_levelGrid[x, y].GetComponent<DngnRoomInfo>().m_roomType == DngnRoomInfo.RoomType.Normal)
                    NormalRooms.Add(m_levelGrid[x, y]);
            }
        }

        //Figure out which one is closest to the boss room
        foreach (GameObject NormalRoom in NormalRooms)
        {
            float D = Vector3.Distance(NormalRoom.transform.position, m_endRoom.transform.position);
            if(D<ClosestRoomDistance)
            {
                ClosestRoom = NormalRoom;
                ClosestRoomDistance = D;
            }
        }

        return ClosestRoom;
    }
}