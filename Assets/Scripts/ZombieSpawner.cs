// ---------------------------------------------------------------------------
// ZombieSpawner.cs
// 
// Class description goes here
// 
// Original Author: Harley Laurie
// ---------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class ZombieSpawner : MonoBehaviour 
{
    public GameObject ZombiePrefab;
    public float spawnCooldown = 3.0f;
    private float nextSpawn = 3.0f;
    public int spawnCount;
    private int spawnCountRemaining;

    //Only spawn zombies if the player is in this testing area
    private GameObject PC; //Reference to player character

    //The room this spawner is inside
    private GameObject m_parentRoom;

    void Start()
    {
        PC = GameObject.FindGameObjectWithTag("Player");
    }
	
	void Update () 
	{
        if (spawnCount > 0)
        {
            //Calculate distance to player character
            float D = Vector3.Distance(transform.position, PC.transform.position);

            //Spawn zombies if the player is close enough
            if (D <= 10.0f)
            {
                nextSpawn -= Time.deltaTime;
                if (nextSpawn <= 0.0f)
                {
                    GameObject Zombie = GameObject.Instantiate(ZombiePrefab, transform.position, Quaternion.identity) as GameObject;
                    Zombie.SendMessage("SetParentRoom", this.gameObject);
                    nextSpawn = spawnCooldown;
                    spawnCount--;
                }
            }
        }
	}

    public void SetSpawnCount(int targetSpawnCount)
    {
        spawnCount = targetSpawnCount;
        spawnCountRemaining = spawnCount;
    }

    public void SetParentRoom(GameObject parentRoom)
    {
        m_parentRoom = parentRoom;
    }

    //Open doors in this room once all the enemies have been killed
    public void DecrementSpawnCountRemainder()
    {
        spawnCountRemaining--;
        if (spawnCountRemaining == 0)
            m_parentRoom.SendMessage("OpenAllDoors", false);
    }
}