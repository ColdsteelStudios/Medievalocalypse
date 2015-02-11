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

    //Only spawn zombies if the player is in this testing area
    private GameObject PC; //Reference to player character

    void Start()
    {
        PC = GameObject.FindGameObjectWithTag("Player");
    }
	
	void Update () 
	{
        //Calculate distance to player character
        float D = Vector3.Distance(transform.position, PC.transform.position);

        //Spawn zombies if the player is close enough
        if(D<=10.0f)
        {
            nextSpawn -= Time.deltaTime;
            if (nextSpawn <= 0.0f)
            {
                GameObject.Instantiate(ZombiePrefab, transform.position, Quaternion.identity);
                nextSpawn = spawnCooldown;
            }
        }
	}
}